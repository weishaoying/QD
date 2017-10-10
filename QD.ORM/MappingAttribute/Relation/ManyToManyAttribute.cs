using System;
using System.Collections.Generic;
using System.Text;
using QD.ORM.Common;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :ManyToManyAttribute 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/23 21:29:49 
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
         * ���ľ����û��ͽ�ɫ��ʹ���м�� T_USER_ROLE���洢ӳ��
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
        /// �м����
        /// </summary>
        public string MiddleTable
        {
            set;
            get;
        }

        /// <summary>
        /// �м����ָ��ǰ�������ֶ�
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
        /// �Ƽ�ʹ�����
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