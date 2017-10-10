using System;
using System.Collections.Generic;
using System.Text;
using QD.ORM.Common;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :ManyToOneAttribute 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/23 18:42:04 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.MappingAttribute.Relation
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ManyToOneAttribute : Attribute
    {
        /// <summary>
        /// 当前字段名，即外键
        /// </summary>
        public string FKColumn
        {
            set;
            get;
        }

        /// <summary>
        /// 关联表
        /// </summary>
        public string JoinTable
        {
            set;
            get;
        }

        /// <summary>
        /// 关联表的主键，一般是ID，但也有可能不是ID
        /// </summary>
        public string RefTablePK
        {
            set;
            get;
        }

        public ManyToOneAttribute()
        {
            this.RefTablePK = Constant.ID;
        }

        public ManyToOneAttribute(string _FKColumn)
            : this()
        {
            this.FKColumn = _FKColumn;
            //this.JoinColumn = Constant.ID;
        }

        /// <summary>
        /// 推荐使用这个
        /// </summary>
        /// <param name="_FKColumn"></param>
        /// <param name="joinTable"></param>
        public ManyToOneAttribute(string _FKColumn, string joinTable)
            : this()
        {
            this.FKColumn = _FKColumn;
            this.JoinTable = joinTable;
        }

        public ManyToOneAttribute(string _FKColumn, string joinTable, string refPK)
            : this(_FKColumn, joinTable)
        {
            this.RefTablePK = refPK;
        }
    }
}