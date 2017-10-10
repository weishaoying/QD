using System;
using System.Collections.Generic;
using System.Text;
using QD.Found;
using System.Data.Common;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :QDDBException 
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

namespace QD.ORM
{
    public class ORMException : QDException
    {

        public ErrorType ExceptionType
        {
            set;
            get;
        }

        //出错的SQL语句
        public string SQLText
        {
            set;
            get;
        }

        public IList<DbParameter> Params
        {
            set;
            get;
        }

        public ORMException()
            : base("An exception occurred in the QD ORM Layer")
        {
        }

        public ORMException(string message)
            : base(message)
        {
        }

        public ORMException(string message, Exception ex)
            : base(message, ex)
        {
        }

        public ORMException(ErrorType errorType, String sql, IList<DbParameter> paramList)
        {
            this.ExceptionType = errorType;
            this.SQLText = sql;
            this.Params = paramList;
        }
    }


    /// <summary>
    /// 产生错误的原因
    /// </summary>
    public enum ErrorType
    {
        UnKnown = 0,

        QueryDataSet = 1,

        QuerySingle = 2,

        //包括Insert, Update, Delete, 函数，存储过程
        ExcuteUpdate = 3,
    }
}