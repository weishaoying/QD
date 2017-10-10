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
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :BaseMappingEngine 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/23 22:24
// 
// Modify Time: 
// Modify Description: ���������п�Ϊ�գ�����Ϊ��(int?)��˵����
// �ڱ���ʱ��������Զ�û�и�ֵ����Ϊ�յ��ֶλ�Ϊ��,
// ����Ϊ�յ��ֶζ����и�Ĭ��ֵ����int 0 , DateTime 0001/1/1��ö��Ĭ��0
//
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine.Core
{
    public class BaseORMEngine
    {
        //��Ϊ�����࣬����û���õ���ģʽ���������˾�̬����

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
            Type t1 = typeof(List<>);//���ﲻ����IList�ӿ�����
            Type t2 = t1.MakeGenericType(targetType);

            IList list = Activator.CreateInstance(t2) as IList;//���򱨴�: �޷������ӿڵ�ʵ����
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
            //����ҪField
            //FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (CommonHelper.IsCollectionEmpty(propInfos))
            {
                return targetObject;
            }

            //�����ļ��п��Եõ�ӳ����Ϣ�����Ƿ���������ֱ�Ӵ�DataSet���ݼ���֪����ӳ���ĸ��ֶ�
            //������Ҫ������������
            foreach (PropertyInfo prop in propInfos)
            {
                #region �Ż������Զ������Ի���͸�����ԣ����������²�����������ͽ���ж�Ч���Ե�Щ������ע�͵����� 2015-8-21
                //Attribute[] attrs = Attribute.GetCustomAttributes(prop, true);
                ////���Զ������ԣ���һ��
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

                #region ��do while��ʽ���Խ�����ע������⣬����Ч��û�������
                //Attribute[] attrs = Attribute.GetCustomAttributes(prop, true);
                //int z = -1;
                //Attribute attr = null;
                //do
                //{
                //    z++;
                //    attr = attrs[z];
                //} while (z < attrs.Length && !(attr is ColumnAttribute)); 
                #endregion

                //������ʾ�������Ͽ����ж��ע�⣬��������˳��һ��
                //[Column]
                //[Required]
                //[Display(Name = "����")]
                //public string RealName { }

                //����ֱ�ӻ�ȡ���0�������⣬��������ڶ���������Type������true
                //attr = attrs[0];

                //if (attr is ColumnAttribute)
                //{
                //if (attr is PKAttribute)
                //{
                //    //���������
                //}

                /*
                 * �����[ManyToOne("CLASS_ID", "T_CLASS")]����ֶΣ�
                 * �������Ӧ����ͨ���������(�ٷ�һ��SQL)��ѯ����¼Ȼ�����õ�������
                 * 
                 * ��������Ƕ���ѯ�����������²������ö������ԣ�����ͨ������2�ַ�ʽ
                 * 1.������ͼ����Ӧ�½�һ���ࣨ����Ҫhbmʽ��ӳ���ļ�����������ĸ����������д�������У���ͨ��ӳ���ѯ
                 * ����
                 * 2.������ͼ��������������Ҫ������(��Student���Ӳ���Class����)��д������SQL����List<object[]>�ڴ�����ƴ
                */
                ColumnAttribute attr = Attribute.GetCustomAttribute(prop, typeof(ColumnAttribute)) as ColumnAttribute;
                if (null == attr)
                {
                    //������ʱ����ManyToOne��ManyToMany������
                    continue;
                }

                ColumnAttribute columnAttr = attr as ColumnAttribute;
                string columnName = columnAttr.ColumnName;
                //����ֶ�������������ͬ����Լ�дΪ��[Column]
                if (String.IsNullOrEmpty(columnName))
                {
                    columnName = prop.Name.ToUpper();
                }

                object value = null;
                try
                {
                    //�ж��Ƿ��и��ֶΣ�SQL����п���ֻ��ѯ�˲����ֶ�,���������������
                    if (!dataRow.Table.Columns.Contains(columnName))
                    {
                        continue;
                    }
                    value = dataRow[columnName];//�����Ǵ�д��Сд�����϶�����
                }
                catch (Exception ex)
                {
                    //�ж����Ƿ���ڣ���������ڻᱨ��,������Ҫ�Ƿ�ֹ������д��ӳ������
                    throw new ORMException(TipConst.TableDontHaveColumn + columnName, ex);
                }

                try
                {
                    //�������һ����ֱ�Ӹ�ֵ��2016��3��9�շſ������ע��
                    if (value.GetType() == prop.PropertyType)//����Σ�գ���������Ϊ int?, �ҿ���û��ֵ������Ϊ����DBNull
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
                    //��������Ϊstring Id, �����ݿ�����Decimal����
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
                //    //����ӳ�䲻��
                //}
            }
            return targetObject;
        }

        /// <summary>
        /// ���ݿ��class������ת�����磺�����ݿ���varchar����ת��ΪString
        /// ע�����ݿ��ֵ�������ݿ���dataRow[NAME]û��ֵ
        /// ��Ҫ��������3������
        /// 1.���ݿ��Ƿ���ֵ?
        /// 2.�Ƿ�Ϊֵ����?
        /// 3.�Ƿ�Ϊ��Ϊ������?
        /// </summary>
        /// <param name="value"></param>
        /// <param name="type"></param>
        /// <returns></returns>        
        private static object GetTargetTypeValue(object value, Type type)
        {
            #region ����߼���д��ȫû���⣬���������ŵ����������Ϊ�˼����ж�
            //if (type == typeof(string))
            //{
            //    if (DBNull.Value != value)
            //    {
            //        return value;
            //    }
            //    //ע��,���ݿ��ֵʱ���봦��
            //    return string.Empty;
            //}
            #endregion

            Type underType = Nullable.GetUnderlyingType(type);
            bool bCanBeNull = (null != underType);

            //������ݿ��е�ֵΪ�գ����⴦��
            if (value == DBNull.Value)
            {
                if (type.IsValueType)//�����ֵ����
                {
                    if (bCanBeNull)
                    {
                        //˵�������ǿ�Ϊ������public double?
                        //ֱ�ӷ��ؿռ���
                        return null;
                    }
                    else
                    {
                        //��ͨ�����磺public double Money{set;get;}
                        return Activator.CreateInstance(type);//ʵ����ʱ����Ĭ��ֵ
                    }
                }
                else
                {
                    //��ֵ����
                    return null;
                }
            }
            else
            {
                //���ݿ�����ֵ�����������ǿ�Ϊ�ջ�����ͨ����Ϊ�����Ͷ����ղ���Ϊ��������伴��

                //�����ǿ�Ϊ�����ͣ�int?, double?
                if (bCanBeNull)
                {
                    //return GetTargetTypeValue(value, underType);//�ݹ�Ҳ���ԣ���ֱ�������������ͣ�����ִ��Ч�ʸ���
                    type = underType;
                }
                //��������Ϊ�ղ����ж��ˣ���������ֵ��ֻ��Ҫ�ж��Ƿ���ֵ���ͣ�������Ŀ�ж���ֵ�������Ծͼ��ˡ�

                //������ͱ����ͷ���ֱ�ӷ���
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
        /// true, Y, 1����true����������false
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
            //���ȳ����ܷ���ȷ�����������ֱ�ӷ��ؽ������ֵ
            if (Boolean.TryParse(obj.ToString(), out b))
            {
                return b;
            }
            string val = obj.ToString().ToUpper();
            return val.Equals("Y") | val.Equals("1");
        }

    }
}