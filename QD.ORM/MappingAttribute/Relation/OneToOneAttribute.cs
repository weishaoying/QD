using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :OneToOne 
// Function Description: ������Ϊ��ManyToOne�����������
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/23 21:15:51 
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
         * Ҫ��ʵ��OneToOneӳ�䣬���������ַ�ʽ��
         * ����Person��IdCard����Person����IdCard��Ա����
         * 1.��Person���м����ָ��IdCard�����������Ҷ��������ΨһԼ��
         * 2.�ȴ���IdCard�ٴ���Person������IdCard���������Ƹ�Person������
         * 
         * ���水�յ�һ�ַ�ʽ
         */

        /// <summary>
        /// ��ǰ�ֶ����������
        /// </summary>
        public string FKColumn
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
        /// �����������
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