using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :DB2DBManager 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/23 11:21:27 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine
{
    internal class DB2DBManager : DBManagerAdapter
    {
        #region ����ģʽ
        private static DB2DBManager _DBManager = new DB2DBManager();

        private DB2DBManager()
        {
        }

        public static DB2DBManager GetInstance()
        {
            return _DBManager;
        } 
        #endregion

        #region MyRegion
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
        #endregion
    }
}