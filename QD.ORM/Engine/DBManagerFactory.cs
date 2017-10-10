using System;
using System.Collections.Generic;
using System.Text;
using QD.ORM.Model;
using QD.ORM.Common;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :DBFactory 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/23 10:56:52 
// 
// Modify Time: 2015-2-28
// Modify Description: 将它变成单例，数据库类型和字符串应该是它的成员变量，方法不再是static
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine
{

    /// <summary>
    /// DataBaseManager工厂
    /// Hibernate中SessionFactory只在Web服务启动时创建一次，是重量级的，也是线程安全的。
    /// 它和SessionFactory类似，也应该只初始化一次，并且唯一即可。
    /// </summary>
    public class DBManagerFactory
    {
        private static DBManagerFactory _Instance = null;

        private static object lockObj = new object();

        #region 应该声明为成员变量

        private static DBType dbType = DBType.Oracle;

        //连接字符串
        private static string connStr = DBConfig.DefaultDBConnString;

        #endregion

        private DBManagerFactory()
        {
        }

        //我想实现的目标是：在服务启动时只初始化一次，而客户端调用时不需要再传递参数，单例要求必须有下面的方法，
        public static DBManagerFactory GetInstance(DBType _dbType, string _connStr)
        {
            if (null == _Instance)
            {
                lock (lockObj)
                {
                    if (null == _Instance)
                    {
                        dbType = _dbType;
                        connStr = _connStr;
                        _Instance = new DBManagerFactory();
                    }
                }
            }
            return _Instance;
        }

        //但是调用时，每次都要传递两个参数太麻烦，所以我在这增加了一个重构方法，平时可以都调用它
        public static DBManagerFactory GetInstance()
        {
            if (null == _Instance)
            {
                if (dbType == DBType.UnKnown || string.IsNullOrWhiteSpace(connStr))
                {
                    throw new ORMException(TipConst.DBTypeAndConnStrNotInit);
                }
                return GetInstance(dbType, connStr);
            }
            return _Instance;
        }


        public DataBaseManager CreateDBManager()
        {
            DataBaseManager dbManager = null;
            switch (dbType)
            {
                case DBType.UnKnown:
                    break;
                case DBType.Oracle:
                    dbManager = OracleDBManager.GetInstance(connStr);
                    break;
                case DBType.MySQL:
                    dbManager = MySQLDBManager.GetInstance();
                    break;
                case DBType.SQLServer:
                    dbManager = SQLServerDBManager.GetInstance(connStr);
                    break;
                case DBType.DB2:
                    dbManager = DB2DBManager.GetInstance();
                    break;
            }
            return dbManager;
        }

        ///// <summary>
        ///// 获得Oracle的DBManager
        ///// </summary>
        ///// <returns></returns>
        //public static DataBaseManager GetDBManager()
        //{
        //    return CreateDBManager(DBType.Oracle, null);
        //}

    }
}