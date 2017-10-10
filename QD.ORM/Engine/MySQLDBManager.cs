using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)Œ∫…ÿ”± 2014-2015, All rights reserved. 
// 
// File Name :MySQLDBManager 
// Function Description: 
// Version: 1.0.1 
// 
// Author£∫WeiShaoying 
// Mail:   15910708769@163.com
// Create Time£∫2015/2/23 10:53:52 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine
{
    internal class MySQLDBManager : DBManagerAdapter
    {
        private static MySQLDBManager _DBManager = new MySQLDBManager();

        private MySQLDBManager()
        { }

        public static MySQLDBManager GetInstance()
        {
            return _DBManager;
        }

        //public override System.Data.IDbConnection GetConnection()
        //{
        //    throw new NotImplementedException();
        //}

        //public override System.Data.IDbCommand CreateCommand()
        //{
        //    throw new NotImplementedException();
        //}

        //public override System.Data.IDataReader GetReader()
        //{
        //    throw new NotImplementedException();
        //}
    }
}