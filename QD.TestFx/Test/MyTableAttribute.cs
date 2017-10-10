using System;
using System.Collections.Generic;
using System.Text;
using QD.ORM.Model;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :TableAttribute 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/23 14:19:42 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.TestFx.Test
{
    [AttributeUsage(AttributeTargets.All, AllowMultiple = false)]
    public class MyTableAttribute : Attribute
    {

        public string TableName
        {
            set;
            get;
        }

        //默认是GUID方式
        private PKGenerateStrategy _PKStaregy = PKGenerateStrategy.GUID;

        /// <summary>
        /// 主键的生成策略
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

        public MyTableAttribute(string tableName)
        {
            this.TableName = tableName;
            this.PKStaregy = PKGenerateStrategy.GUID;
        }

        public MyTableAttribute(string tableName, string seqName)
        {
            this.TableName = tableName;
            this.PKStaregy = PKGenerateStrategy.SEQ;
            this.SeqName = seqName;
        }

        public MyTableAttribute(string tableName, PKGenerateStrategy strategy, string seqName)
        {
            this.TableName = tableName;
            this.PKStaregy = strategy;
            this.SeqName = seqName;
        }
    }
}