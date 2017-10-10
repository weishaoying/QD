using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :OneToOne 
// Function Description: 可以认为是ManyToOne的特殊情况，
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/23 21:15:51 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.MappingAttribute.Relation
{
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Field, AllowMultiple = false)]
    public class OneToOneAttribute : Attribute
    {
        /**
         * 要想实现OneToOne映射，可以有两种方式：
         * 比如Person和IdCard，在Person中有IdCard成员属性
         * 1.在Person表中加外键指向IdCard的主键，并且对外键增加唯一约束
         * 2.先创建IdCard再创建Person，并把IdCard的主键复制给Person的主键
         * 
         * 下面按照第一种方式
         */

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
        /// 关联表的主键
        /// </summary>
        public string RefTablePK
        {
            set;
            get;
        }

        public OneToOneAttribute()
        {
            this.RefTablePK = QD.ORM.Common.Constant.ID;
        }

        public OneToOneAttribute(string fk, string joinTable, string refPk)
        {
            this.FKColumn = fk;
            this.JoinTable = joinTable;
            this.RefTablePK = refPk;
        }
    }
}