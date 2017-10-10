using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)Œ∫…ÿ”± 2014-2015, All rights reserved. 
// 
// File Name :UpdateBuilder 
// Function Description: 
// Version: 1.0.1 
// 
// Author£∫WeiShaoying 
// Mail:   15910708769@163.com
// Create Time£∫2015/2/26 8:38:20 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine.SQL
{
    class UpdateBuilder : SQLBuilder
    {
        private static UpdateBuilder _Instance = null;

        private static object obj = new object();

        private UpdateBuilder() { }

        public static UpdateBuilder GetInstance()
        {
            lock (obj)
            {
                if (null == _Instance)
                {
                    _Instance = new UpdateBuilder();
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