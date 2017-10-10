using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)Œ∫…ÿ”± 2014-2015, All rights reserved. 
// 
// File Name :SQLFactory 
// Function Description: 
// Version: 1.0.1 
// 
// Author£∫WeiShaoying 
// Mail:   15910708769@163.com
// Create Time£∫2015/2/26 8:48:47 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine.SQL
{
    public class SQLFactory
    {
        public static SQLBuilder CreateSQLBuilder(SQLBuilderType type)
        {
            SQLBuilder builder = null;
            switch (type)
            {
                case SQLBuilderType.Insert:
                    builder = InsertBuilder.GetInstance();
                    break;
                case SQLBuilderType.Update:
                    builder = UpdateBuilder.GetInstance();
                    break;
                case SQLBuilderType.Delete:
                    builder = DeleteBuilder.GetInstance();
                    break;
                case SQLBuilderType.Select:
                    builder = SelectBuilder.GetInstance();
                    break;
                default:
                    break;
            }
            return builder;
        }
    }

    public enum SQLBuilderType
    {
        Insert,
        Update,
        Delete,
        Select,
    }

}