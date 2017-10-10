using System;
using System.Collections.Generic;
using System.Text;
using QD.ORM.MappingAttribute.Table;
using System.Reflection;
using QD.ORM.MappingAttribute.Col;
using QD.ORM.Common;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :Util 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/26 8:57:31 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine.Core
{
    public class ORMUtil
    {

        public static string GetTableName(Type type)
        {
            TableAttribute tableAttr = Attribute.GetCustomAttribute(type, typeof(TableAttribute)) as TableAttribute;
            if (null != tableAttr)
            {
                if (string.IsNullOrEmpty(tableAttr.TableName))
                {
                    return type.Name.ToUpper();
                }
                return tableAttr.TableName;
            }
            throw new ORMException(TipConst.TableNameIsEmpty);
        }

        /// <summary>
        /// �����
        /// </summary>
        /// <param name="type"></param>
        /// <param name="propName"></param>
        /// <returns></returns>
        public static string GetColumnName(Type type, string propName)
        {
            PropertyInfo pi = type.GetProperty(propName);
            if (null == pi)
            {
                return null;
            }
            ColumnAttribute colAttr = Attribute.GetCustomAttribute(pi, typeof(ColumnAttribute)) as ColumnAttribute;
            if (null == colAttr)
            {
                return null;
            }
            return string.IsNullOrEmpty(colAttr.ColumnName) ? propName.ToUpper() : colAttr.ColumnName;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GenerateSelectAllSQL(Type type)
        {
            #region MyRegion
            //�޷�������Ϊ��System.Object[]���Ķ���ǿ��ת��Ϊ���͡�System.Attribute[]
            //Attribute[] attrs = (Attribute[])type.GetCustomAttributes(true);
            //if (null != attrs)
            //{
            //    TableAttribute ta = attrs[0] as TableAttribute;
            //    string tableName = ta.TableName;
            //    return "SELECT * FROM " + tableName;
            //} 
            #endregion

            string tableName = GetTableName(type);
            if (!String.IsNullOrEmpty(tableName))
            {
                return "SELECT * FROM " + tableName;
            }
            return null;
        }

        /// <summary>
        /// ֻ����Update +����������û��
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string GenerateUpdateSQL(Type type)
        {
            string tableName = GetTableName(type);
            if (!String.IsNullOrEmpty(tableName))
            {
                return "UPDATE " + tableName;
            }
            return null;
        }

        ///// <summary>
        ///// ����LoadById() SQL
        ///// </summary>
        ///// <typeparam name="T"></typeparam>
        ///// <param name="Id"></param>
        ///// <returns></returns>
        //public static string GenerateLoadUniqueSQL<T>(string Id)
        //{
        //    string tableName = GetTableName(typeof(T));
        //    return string.Format("SELECT * FROM {0} WHERE ID = :{1}", tableName, Constant.ID);
        //}

        /// <summary>
        /// ����LoadById() SQL
        /// Oracle SQLServer��ͬ
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="prefix"></param>
        /// <returns></returns>
        public static string GenerateLoadUniqueSQL<T>(char prefix)
        {
            string tableName = GetTableName(typeof(T));
            return string.Format("SELECT * FROM {0} WHERE ID = {1}{2}", tableName, prefix, Constant.ID);
        }
    }
}