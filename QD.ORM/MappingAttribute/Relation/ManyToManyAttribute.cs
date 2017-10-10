using System;
using System.Collections.Generic;
using System.Text;
using QD.ORM.Common;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :ManyToManyAttribute 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/23 21:29:49 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.MappingAttribute.Relation
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class ManyToManyAttribute : Attribute
    {
        /**
         * 最经典的就是用户和角色，使用中间表 T_USER_ROLE来存储映射
         * T_USER (ID)
         * T_ROLE (ID)
         * T_USER_ROLE(ID, USERID, ROLEID)
         */

        public string CurtPK
        {
            set;
            get;
        }

        /// <summary>
        /// 中间表名
        /// </summary>
        public string MiddleTable
        {
            set;
            get;
        }

        /// <summary>
        /// 中间表中指向当前表的外键字段
        /// </summary>
        public string MiddleRefThis
        {
            set;
            get;
        }

        public string MiddleRefThat
        {
            set;
            get;
        }

        public string ThatPK
        {
            set;
            get;
        }

        public ManyToManyAttribute()
        {
            this.CurtPK = this.ThatPK = Constant.ID;
        }

        /// <summary>
        /// 推荐使用这个
        /// </summary>
        /// <param name="middleTable"></param>
        /// <param name="midRefThis"></param>
        /// <param name="midRefThat"></param>
        public ManyToManyAttribute(string middleTable, string midRefThis, string midRefThat)
            : this()
        {
            this.MiddleTable = middleTable;
            this.MiddleRefThis = midRefThis;
            this.MiddleRefThat = midRefThat;
        }

        public ManyToManyAttribute(string thisPK, string middleTable, string midRefThis,
            string midRefThat, string thatPK)
        {
            this.CurtPK = thisPK;
            this.MiddleTable = middleTable;
            this.MiddleRefThis = midRefThis;
            this.MiddleRefThat = midRefThat;
            this.ThatPK = thatPK;
        }

    }
}