using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :ColumnAttribute 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/23 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.MappingAttribute.Col
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class ColumnAttribute : Attribute
    {
        /// <summary>
        /// 可以为空，如果为空在映射时通过属性名获取
        /// </summary>
        public string ColumnName
        {
            set;
            get;
        }

        public ColumnAttribute()
        {
        }

        public ColumnAttribute(string columnName)
        {
            this.ColumnName = columnName;
        }

    }
}