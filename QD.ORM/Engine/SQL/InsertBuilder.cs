using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)Œ∫…ÿ”± 2014-2015, All rights reserved. 
// 
// File Name :InsertBuilder 
// Function Description: 
// Version: 1.0.1 
// 
// Author£∫WeiShaoying 
// Mail:   15910708769@163.com
// Create Time£∫2015/2/26 8:38:56 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine.SQL
{
    class InsertBuilder : SQLBuilder
    {
        private static InsertBuilder _Instance = null;

        private static object obj = new object();

        private InsertBuilder() { }

        public static InsertBuilder GetInstance()
        {
            lock (obj)
            {
                if (null == _Instance)
                {
                    _Instance = new InsertBuilder();
                }
            }
            return _Instance;
        }

        public override string BuildSQL<T>(string oql)
        {
            throw new NotImplementedException();
        }

        public override string BuildSQL(object obj)
        {
            throw new NotImplementedException();
        }
    }
}