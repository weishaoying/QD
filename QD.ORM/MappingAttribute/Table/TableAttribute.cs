using System;
using System.Collections.Generic;
using System.Text;
using QD.ORM.Model;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :TableAttribute 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/23 14:19:42 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.MappingAttribute.Table
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class TableAttribute : Attribute
    {

        public string TableName
        {
            set;
            get;
        }

        //Ĭ����UnKnown
        private PKGenerateStrategy _PKStaregy = PKGenerateStrategy.UnKnown;

        /// <summary>
        /// ���������ɲ���
        /// </summary>
        public PKGenerateStrategy PKStaregy
        {
            set
            {
                this._PKStaregy = value;
            }
            get
            {
                return this._PKStaregy;
            }
        }

        public String SeqName
        {
            set;
            get;
        }

        public TableAttribute()
        {
            //���Ϊ����ȡ����
        }

        public TableAttribute(string tableName)
        {
            this.TableName = tableName;
            //��ΪҪ����Oracle��SQLServer�ȶ������ݿ⣬Ĭ��ֵ��ҪGUID 2016-7-22
            this.PKStaregy = PKGenerateStrategy.UnKnown;
        }

        public TableAttribute(string tableName, string seqName)
        {
            this.TableName = tableName;
            this.PKStaregy = PKGenerateStrategy.SEQ;
            this.SeqName = seqName;
        }

        public TableAttribute(string tableName, PKGenerateStrategy strategy, string seqName)
        {
            this.TableName = tableName;
            this.PKStaregy = strategy;
            this.SeqName = seqName;
        }
    }
}