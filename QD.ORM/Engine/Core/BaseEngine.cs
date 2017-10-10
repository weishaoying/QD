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
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine.Core
{
    public class BaseEngine
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
            Type t1 = typeof(List<>);//这里不能是IList接口类型
            Type t2 = t1.MakeGenericType(targetType);

            IList list = Activator.CreateInstance(t2) as IList;
            foreach (DataRow item in dataTable.Rows)
            {
                object obj = GetObjectFromRow(item, targetType);
                list.Add(obj);
            }
            return list;
        }

        private static object GetObjectFromRow(DataRow dataRow, Type targetType)
        {
            Object targetObject = Activator.CreateInstance(targetType);
            PropertyInfo[] propInfos = targetType.GetProperties(BindingFlags.Public | BindingFlags.Instance);// |BindingFlags.NonPublic
            //不需要Field
            //FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (CommonHelper.IsCollectionEmpty(propInfos))
            {
                return targetObject;
            }
            foreach (PropertyInfo prop in propInfos)
            {
                Attribute[] attrs = Attribute.GetCustomAttributes(prop, true);
                //无自定义属性
                if (CommonHelper.IsCollectionEmpty(attrs))
                {
                    continue;
                }

                Attribute attr = Attribute.GetCustomAttribute(prop, typeof(TransparentAttribute));
                if (null != attr)
                {
                    continue;
                }
                attr = attrs[0];
                if (attr is ColumnAttribute)
                {
                    if (attr is PKAttribute)
                    {
                        //TODO:主键
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
                        //判断列是否存在，如果不存在会报错
                        throw new ORMException(TipConst.TableDontHaveColumn + columnName, ex);
                    }

                    try
                    {
                        //这样危险，比如属性为 int?, 且库中没有值
                        //if (value.GetType() == prop.PropertyType)
                        //{
                        //    prop.SetValue(targetObject, value, null);
                        //}
                        //else
                        //{
                        object newValue = GetTargetTypeValue(value, prop.PropertyType);
                        prop.SetValue(targetObject, newValue, null);
                        //}
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
                }
                else
                {
                    //复杂映射
                }
            }
            return targetObject;
        }

        private static object GetTargetTypeValue(object value, Type type)
        {
            if (type == typeof(string))
            {
                if (DBNull.Value == value)
                {
                    //注意数据库空值时，必须处理
                    return string.Empty;
                }
                return value;
            }
            //如果数据库中的值为空，特殊处理
            if (value == DBNull.Value)
            {
                //如果是值类型
                if (type.IsValueType)
                {
                    Type t = Nullable.GetUnderlyingType(type);
                    if (null != t)
                    {
                        //说明属性是可为空类型，public double?
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
                    return null;
                }
            }

            //数据库中有值，而属性是可为空类型如：int?，double?
            Type t1 = Nullable.GetUnderlyingType(type);
            if (null != t1)
            {
                //return GetTargetTypeValue(value, t1);//递归也可以，直接重新设置类型，往下执行效率更好
                type = t1;
            }

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