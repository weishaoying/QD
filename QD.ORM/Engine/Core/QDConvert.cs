using System;
using System.Data;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :QDConvert 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/25 21:38:44 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine.Core
{
    public class QDConvert
    {
        /// <summary>
        /// ������ת��Ϊ���ݿ���֧�ֵ�����
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DbType ConvertToDBType(object obj)
        {
            if (null == obj || DBNull.Value == obj)
            {
                //������ݿ�����DateTime���ͣ�������������ɣ�
                return DbType.String;
            }
            Type type = obj.GetType();
            return ConvertToDBType(type);
        }

        /// <summary>
        /// ������typeת��Ϊ���ݿ���֧�ֵ�����
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DbType ConvertToDBType(Type type)
        {
            if (null == type)
            {
                throw new ArgumentNullException("type");
            }
            if (!type.IsValueType && !type.IsArray)/*����ֵ�����Ҳ�������*/
            {
                return DbType.String;
            }
            //���������DateTime? ��Ϊ������Ҫע��
            Type t = Nullable.GetUnderlyingType(type);
            if (null != t)
            {
                type = t;
            }

            if (type == typeof(int))
            {
                return DbType.Int32;
            }
            else if (type == typeof(DateTime))
            {
                return DbType.DateTime;
            }
            else if (type == typeof(float))
            {
                return DbType.Single;
            }
            else if (type == typeof(long))
            {
                return DbType.Int64;
            }
            else if (type == typeof(double))
            {
                return DbType.Double;
            }
            else if (type == typeof(char))
            {
                throw new ORMException("��ʱ��֧��char����ӳ��,����string����char");
                //return DbType.Byte;
                //return DbType.SByte;
                //return DbType.Object;
            }
            else if (type == typeof(bool))
            {
                return DbType.Boolean;
            }
            else if (type.IsEnum)
            {
                return DbType.Int32;
            }
            else if (/*type == typeof(byte) &&*/ type.IsArray)
            {
                return DbType.Binary;
            }
            else
            {
                return DbType.String;
            }
        }


        /// <summary>
        /// �����������ֵת��Ϊ���ݿ���֧�ֵ�����
        /// </summary>
        /// <param name="value">����ֵ</param>
        /// <param name="type">��������</param>
        /// <returns></returns>
        public static object ToDBValue(object value, Type type)
        {
            if (type == typeof(string))
            {
                //���Դ�������
                return value;
            }

            Type t1 = Nullable.GetUnderlyingType(type);

            if (null == value || DBNull.Value == value)
            {
                //�����ֵ����
                if (type.IsValueType)
                {
                    if (null != t1)
                    {
                        //˵�������ǿ�Ϊ������
                        return null;
                    }
                    else
                    {
                        //����Ϊ��
                        return Activator.CreateInstance(type);//ʵ����ʱ����Ĭ��ֵ
                    }
                }
                else
                {
                    return null;
                }
            }

            //���ݿ�����ֵ
            if (null != t1)
            {
                type = t1;
            }
            //ö�����ͺͲ�������
            if (type == typeof(bool))
            {
                if (null == value)
                {
                    return 0;
                }
                bool b;
                if (bool.TryParse(value.ToString(), out b))
                {
                    return b ? 1 : 0;
                }
                throw new ORMException(value + "�޷�ת��Ϊbool����");
            }
            else if (type.IsEnum)
            {
                return Convert.ToInt32(value);
            }
            else if (type == typeof(DateTime))
            {
                //return DateTime
                return DateTime.Parse(value.ToString());
            }
            else
            {
                return value;
            }

            //if (value == null)
            //{
            //    //�����ֵ����
            //    if (type.IsValueType)
            //    {
            //        Type t = Nullable.GetUnderlyingType(type);
            //        if (null != t)
            //        {
            //            //˵�������ǿ�Ϊ�����ͣ�public double?
            //            //ֱ�ӷ��ؿռ���
            //            return null;
            //        }
            //        else
            //        {
            //            //��ͨ�����磺public double Money{set;get;}
            //            return Activator.CreateInstance(type);//ʵ����ʱ����Ĭ��ֵ
            //        }
            //    }
            //    else
            //    {
            //        return null;
            //    }
            //}

            //Type t1 = Nullable.GetUnderlyingType(type);
            //if (null != t1)
            //{
            //    type = t1;
            //}

            ////������ͱ����ͷ���ֱ�ӷ���
            //if (type == value.GetType())
            //{
            //    return value;
            //}

            //if (type == typeof(int))
            //{
            //    return Convert.ToInt32(value);
            //}
            //else if (type == typeof(bool))
            //{
            //    return ToBool(value);
            //}
            //else if (type == typeof(char))
            //{
            //    return Convert.ToChar(value);
            //}
            //else if (type == typeof(byte))
            //{
            //    return Convert.ToByte(value);
            //}
            //else if (type == typeof(short))
            //{
            //    return Convert.ToInt16(value);
            //}
            //else if (type == typeof(float))
            //{
            //    return Convert.ToSingle(value);
            //}
            //else if (type == typeof(long))
            //{
            //    return Convert.ToInt64(value);
            //}
            //else if (type == typeof(double))
            //{
            //    return Convert.ToDouble(value);
            //}
            //else if (type == typeof(uint))
            //{
            //    return Convert.ToUInt32(value);
            //}
            //else if (type.IsEnum)
            //{
            //    return Enum.ToObject(type, Convert.ToInt32(value));

            //}
            //else
            //{
            //    //double obj = (double)Activator.CreateInstance(typeof(double));
            //    return Convert.ChangeType(value, type);
            //}
        }



    }
}