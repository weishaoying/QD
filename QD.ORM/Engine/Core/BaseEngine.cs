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
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine.Core
{
    public class BaseEngine
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
            Type t1 = typeof(List<>);//���ﲻ����IList�ӿ�����
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
            //����ҪField
            //FieldInfo[] fields = targetType.GetFields(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            if (CommonHelper.IsCollectionEmpty(propInfos))
            {
                return targetObject;
            }
            foreach (PropertyInfo prop in propInfos)
            {
                Attribute[] attrs = Attribute.GetCustomAttributes(prop, true);
                //���Զ�������
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
                        //TODO:����
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
                        //�ж����Ƿ���ڣ���������ڻᱨ��
                        throw new ORMException(TipConst.TableDontHaveColumn + columnName, ex);
                    }

                    try
                    {
                        //����Σ�գ���������Ϊ int?, �ҿ���û��ֵ
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
                }
                else
                {
                    //����ӳ��
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
                    //ע�����ݿ��ֵʱ�����봦��
                    return string.Empty;
                }
                return value;
            }
            //������ݿ��е�ֵΪ�գ����⴦��
            if (value == DBNull.Value)
            {
                //�����ֵ����
                if (type.IsValueType)
                {
                    Type t = Nullable.GetUnderlyingType(type);
                    if (null != t)
                    {
                        //˵�������ǿ�Ϊ�����ͣ�public double?
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
                    return null;
                }
            }

            //���ݿ�����ֵ���������ǿ�Ϊ�������磺int?��double?
            Type t1 = Nullable.GetUnderlyingType(type);
            if (null != t1)
            {
                //return GetTargetTypeValue(value, t1);//�ݹ�Ҳ���ԣ�ֱ�������������ͣ�����ִ��Ч�ʸ���
                type = t1;
            }

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