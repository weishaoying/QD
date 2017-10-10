using System;
using System.Data;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :QDConvert 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/25 21:38:44 
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
        /// 将对象转换为数据库中支持的类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DbType ConvertToDBType(object obj)
        {
            if (null == obj || DBNull.Value == obj)
            {
                //如果数据库中是DateTime类型，这样会有问题吧？
                return DbType.String;
            }
            Type type = obj.GetType();
            return ConvertToDBType(type);
        }

        /// <summary>
        /// 将类型type转换为数据库中支持的类型
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public static DbType ConvertToDBType(Type type)
        {
            if (null == type)
            {
                throw new ArgumentNullException("type");
            }
            if (!type.IsValueType && !type.IsArray)/*不是值类型且不是数组*/
            {
                return DbType.String;
            }
            //属性如果是DateTime? 可为空类型要注意
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
                throw new ORMException("暂时不支持char类型映射,请用string代替char");
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
        /// 将对象的属性值转换为数据库中支持的类型
        /// </summary>
        /// <param name="value">属性值</param>
        /// <param name="type">属性类型</param>
        /// <returns></returns>
        public static object ToDBValue(object value, Type type)
        {
            if (type == typeof(string))
            {
                //绝对大多数情况
                return value;
            }

            Type t1 = Nullable.GetUnderlyingType(type);

            if (null == value || DBNull.Value == value)
            {
                //如果是值类型
                if (type.IsValueType)
                {
                    if (null != t1)
                    {
                        //说明属性是可为空类型
                        return null;
                    }
                    else
                    {
                        //不可为空
                        return Activator.CreateInstance(type);//实例化时会有默认值
                    }
                }
                else
                {
                    return null;
                }
            }

            //数据库中有值
            if (null != t1)
            {
                type = t1;
            }
            //枚举类型和布尔类型
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
                throw new ORMException(value + "无法转换为bool类型");
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
            //    //如果是值类型
            //    if (type.IsValueType)
            //    {
            //        Type t = Nullable.GetUnderlyingType(type);
            //        if (null != t)
            //        {
            //            //说明属性是可为空类型，public double?
            //            //直接返回空即可
            //            return null;
            //        }
            //        else
            //        {
            //            //普通属性如：public double Money{set;get;}
            //            return Activator.CreateInstance(type);//实例化时会有默认值
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

            ////如果类型本来就符合直接返回
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