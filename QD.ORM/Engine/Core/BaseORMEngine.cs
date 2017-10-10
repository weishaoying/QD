using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
using System.Data;
using System.Reflection;
using QD.ORM.MappingAttribute.Col;
using QD.Found.Util;
using QD.ORM.Common;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :BaseMappingEngine 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/23 22:24
// 
// Modify Time: 
// Modify Description: 关于属性中可为空，不可为空(int?)的说明。
// 在保存时，如果属性都没有赋值，可为空的字段会为空,
// 不可为空的字段都会有个默认值，如int 0 , DateTime 0001/1/1，枚举默认0
//
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine.Core
{
    public class BaseORMEngine
    {
        //因为有子类，这里没有用单例模式，而都用了静态方法

        public static IList Mapping(DataSet dataSet, Type targetType)
        {
            if (null == dataSet || null == dataSet.Tables)
            {
                return null;
            }
            return Mapping(dataSet.Tables[0], targetType);
        }

        public static IList Mapping(DataTable dataTable, Type targetType)
        {
            if (null == dataTable)
            {
                return null;
            }
            //{Name = "List`1" FullName = "System.Collections.Generic.List`1"}
            Type t1 = typeof(List<>);//这里不能是IList接口类型
            Type t2 = t1.MakeGenericType(targetType);

            IList list = Activator.CreateInstance(t2) as IList;//否则报错: 无法创建接口的实例。
            foreach (DataRow item in dataTable.Rows)
            {
                object obj = GetObjectFromDataRow(item, targetType);
                list.Add(obj);
            }
            return list;
        }


        private static object GetObjectFromDataRow(DataRow dataRow, Type targetType)
        {
            Object targetObject = Activator.CreateInstance(targetType);
            PropertyInfo[] propInfos = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);// |BindingFlags.NonPublic
            //不需要Field
            //FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (CommonHelper.IsCollectionEmpty(propInfos))
            {
                return targetObject;
            }

            //从类文件中可以得到映射信息，但是反过来不能直接从DataSet数据集中知道该映射哪个字段
            //所以需要遍历所有属性
            foreach (PropertyInfo prop in propInfos)
            {
                #region 优化：无自定义属性或者透明属性，大多数情况下不会是这样，徒增判断效率稍低些，所以注释掉它们 2015-8-21
                //Attribute[] attrs = Attribute.GetCustomAttributes(prop, true);
                ////无自定义属性，下一个
                //if (CommonHelper.IsCollectionEmpty(attrs))
                //{
                //    continue;
                //}

                //Attribute attr = Attribute.GetCustomAttribute(prop, typeof(TransparentAttribute));
                //if (null != attr)
                //{
                //    continue;
                //}
                #endregion

                #region 用do while方式可以解决多个注解的问题，但是效率没有下面高
                //Attribute[] attrs = Attribute.GetCustomAttributes(prop, true);
                //int z = -1;
                //Attribute attr = null;
                //do
                //{
                //    z++;
                //    attr = attrs[z];
                //} while (z < attrs.Length && !(attr is ColumnAttribute)); 
                #endregion

                //如下所示：属性上可能有多个注解，而且上下顺序不一定
                //[Column]
                //[Required]
                //[Display(Name = "姓名")]
                //public string RealName { }

                //所以直接获取会第0个有问题，所以下面第二个参数是Type，而非true
                //attr = attrs[0];

                //if (attr is ColumnAttribute)
                //{
                //if (attr is PKAttribute)
                //{
                //    //如果是主键
                //}

                /*
                 * 如果是[ManyToOne("CLASS_ID", "T_CLASS")]外键字段，
                 * 正规情况应该先通过外键关联(再发一条SQL)查询出记录然后设置到对象上
                 * 
                 * 但是如果是多表查询，大多数情况下不会设置对象属性，而是通过下面2种方式
                 * 1.建个视图，对应新建一个类（不需要hbm式的映射文件），将所需的各个表的属性写到新类中，再通过映射查询
                 * 或者
                 * 2.不建视图，在类中新增需要的属性(如Student增加部分Class属性)，写个复杂SQL返回List<object[]>在代码中拼
                */
                ColumnAttribute attr = Attribute.GetCustomAttribute(prop, typeof(ColumnAttribute)) as ColumnAttribute;
                if (null == attr)
                {
                    //所以暂时不管ManyToOne，ManyToMany等属性
                    continue;
                }

                ColumnAttribute columnAttr = attr as ColumnAttribute;
                string columnName = columnAttr.ColumnName;
                //如果字段名和属性名相同则可以简写为：[Column]
                if (String.IsNullOrEmpty(columnName))
                {
                    columnName = prop.Name.ToUpper();
                }

                object value = null;
                try
                {
                    //判断是否含有该字段，SQL语句中可能只查询了部分字段,如果不包含则跳过
                    if (!dataRow.Table.Columns.Contains(columnName))
                    {
                        continue;
                    }
                    value = dataRow[columnName];//列名是大写，小写，或混合都可以
                }
                catch (Exception ex)
                {
                    //判断列是否存在，如果不存在会报错,这里主要是防止代码中写错映射列名
                    throw new ORMException(TipConst.TableDontHaveColumn + columnName, ex);
                }

                try
                {
                    //如果类型一致则直接赋值，2016年3月9日放开下面的注释
                    if (value.GetType() == prop.PropertyType)//这样危险？比如属性为 int?, 且库中没有值，库中为空则DBNull
                    {
                        prop.SetValue(targetObject, value, null);
                    }
                    else
                    {
                        object newValue = GetTargetTypeValue(value, prop.PropertyType);
                        prop.SetValue(targetObject, newValue, null);
                    }
                }
                catch (Exception ex)
                {
                    //类中属性为string Id, 但数据库中是Decimal类型
                    if (typeof(String) == prop.PropertyType && typeof(Decimal) == dataRow.Table.Columns[columnName].DataType)
                    {
                        prop.SetValue(targetObject, Convert.ToInt32(value).ToString(), null);
                    }
                    else
                    {
                        throw new ORMException(prop.Name + TipConst.ChangeTypeError + prop.PropertyType.Name, ex);
                    }
                }
                //}
                //else
                //{
                //    //复杂映射不做
                //}
            }
            return targetObject;
        }

        /// <summary>
        /// 数据库和class中类型转换，如：将数据库中varchar类型转换为String
        /// 注意数据库空值：如数据库中dataRow[NAME]没有值
        /// 需要考虑下面3个问题
        /// 1.数据库是否有值?
        /// 2.是否为值类型?
        /// 3.是否为可为空类型?
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>        
        private static object GetTargetTypeValue(object value, Type type)
        {
            #region 这段逻辑不写完全没问题，增加它并放到最上面就是为了减少判断
            //if (type == typeof(string))
            //{
            //    if (DBNull.Value != value)
            //    {
            //        return value;
            //    }
            //    //注意,数据库空值时必须处理
            //    return string.Empty;
            //}
            #endregion

            Type underType = Nullable.GetUnderlyingType(type);
            bool bCanBeNull = (null != underType);

            //如果数据库中的值为空，特殊处理
            if (value == DBNull.Value)
            {
                if (type.IsValueType)//如果是值类型
                {
                    if (bCanBeNull)
                    {
                        //说明属性是可为空类型public double?
                        //直接返回空即可
                        return null;
                    }
                    else
                    {
                        //普通属性如：public double Money{set;get;}
                        return Activator.CreateInstance(type);//实例化时会有默认值
                    }
                }
                else
                {
                    //非值类型
                    return null;
                }
            }
            else
            {
                //数据库中有值，不论属性是可为空还是普通不可为空类型都按照不可为空类型填充即可

                //属性是可为空类型：int?, double?
                if (bCanBeNull)
                {
                    //return GetTargetTypeValue(value, underType);//递归也可以，但直接重新设置类型，往下执行效率更好
                    type = underType;
                }
                //所以类型为空不用判断了，数据又有值，只需要判断是否是值类型，现在项目中都是值类型所以就简单了。

                //如果类型本来就符合直接返回
                if (type == value.GetType())
                {
                    return value;
                }

                if (type == typeof(int))
                {
                    return Convert.ToInt32(value);
                }
                else if (type == typeof(bool))
                {
                    return ToBool(value);
                }
                else if (type == typeof(char))
                {
                    return Convert.ToChar(value);
                }
                else if (type == typeof(byte))
                {
                    return Convert.ToByte(value);
                }
                else if (type == typeof(short))
                {
                    return Convert.ToInt16(value);
                }
                else if (type == typeof(float))
                {
                    return Convert.ToSingle(value);
                }
                else if (type == typeof(long))
                {
                    return Convert.ToInt64(value);
                }
                else if (type == typeof(double))
                {
                    return Convert.ToDouble(value);
                }
                else if (type == typeof(uint))
                {
                    return Convert.ToUInt32(value);
                }
                else if (type.IsEnum)
                {
                    return Enum.ToObject(type, Convert.ToInt32(value));
                }
                else
                {
                    //double obj = (double)Activator.CreateInstance(typeof(double));
                    return Convert.ChangeType(value, type);
                }
            }
        }

        /// <summary>
        /// true, Y, 1都是true，其它都是false
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        private static bool ToBool(object obj)
        {
            if (null == obj)
            {
                return false;
            }
            bool b;
            //首先尝试能否正确解析，如果能直接返回解析后的值
            if (Boolean.TryParse(obj.ToString(), out b))
            {
                return b;
            }
            string val = obj.ToString().ToUpper();
            return val.Equals("Y") | val.Equals("1");
        }

    }
}