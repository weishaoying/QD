using System;
using System.Collections.Generic;
using System.Text;
using QD.ORM.Model;
using QD.ORM.Common;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :DBFactory 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/23 10:56:52 
// 
// Modify Time: 2015-2-28
// Modify Description: ������ɵ��������ݿ����ͺ��ַ���Ӧ�������ĳ�Ա����������������static
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine
{

    /// <summary>
    /// DataBaseManager����
    /// Hibernate��SessionFactoryֻ��Web��������ʱ����һ�Σ����������ģ�Ҳ���̰߳�ȫ�ġ�
    /// ����SessionFactory���ƣ�ҲӦ��ֻ��ʼ��һ�Σ�����Ψһ���ɡ�
    /// </summary>
    public class DBManagerFactory
    {
        private static DBManagerFactory _Instance = null;

        private static object lockObj = new object();

        #region Ӧ������Ϊ��Ա����

        private static DBType dbType = DBType.Oracle;

        //�����ַ���
        private static string connStr = DBConfig.DefaultDBConnString;

        #endregion

        private DBManagerFactory()
        {
        }

        //����ʵ�ֵ�Ŀ���ǣ��ڷ�������ʱֻ��ʼ��һ�Σ����ͻ��˵���ʱ����Ҫ�ٴ��ݲ���������Ҫ�����������ķ�����
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

        //���ǵ���ʱ��ÿ�ζ�Ҫ������������̫�鷳������������������һ���ع�������ƽʱ���Զ�������
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
        ///// ���Oracle��DBManager
        ///// </summary>
        ///// <returns></returns>
        //public static DataBaseManager GetDBManager()
        //{
        //    return CreateDBManager(DBType.Oracle, null);
        //}

    }
}