using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :OneToManyAttribute 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/23 20:22:03 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.MappingAttribute.Relation
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class OneToManyAttribute : Attribute
    {
        public string CurtPK
        {
            set;
            get;
        }

        /// <summary>
        /// ������
        /// </summary>
        public string JoinTable
        {
            set;
            get;
        }

        /// <summary>
        /// ����������
        /// </summary>
        public string FK
        {
            set;
            get;
        }

        public OneToManyAttribute()
        {
            this.CurtPK = QD.ORM.Common.Constant.ID;
        }

        public OneToManyAttribute(string fk)
        {
            this.FK = fk;
        }

        /// <summary>
        /// �Ƽ�ʹ��������췽��
        /// </summary>
        /// <param name="joinTable"></param>
        /// <param name="fk"></param>
        public OneToManyAttribute(string joinTable, string fk)
        {
            this.JoinTable = joinTable;
            this.FK = fk;
        }
    }
}