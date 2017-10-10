using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)Œ∫…ÿ”± 2014-2015, All rights reserved. 
// 
// File Name :SQLBuilder 
// Function Description: 
// Version: 1.0.1 
// 
// Author£∫WeiShaoying 
// Mail:   15910708769@163.com
// Create Time£∫2015/2/26 8:25:06 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine.SQL
{
    public abstract class SQLBuilder//<T>
    {
        //protected StringBuilder sb = new StringBuilder();

        //public abstract void BuildSQL(Type type, string oql);

        //public abstract void BuildSQL(Type type);

        //public string GetSQL()
        //{
        //    return sb.ToString();
        //}

        public abstract string BuildSQL<T>(string oql);

        public abstract string BuildSQL(object obj);

    }
}