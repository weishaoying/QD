#pragma warning disable 0618
using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
using QD.ORM.Engine;
using QD.Found;
using QD.ORM.Common;
using QD.ORM.Model;
using System.Data.OracleClient;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :ConnectionPool 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:  15910708769@163.com
// Create Time：2015/2/25 10:01:28 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Pool
{
    /// <summary>
    /// 数据库连接池，可以支持多种数据库，就是一个连接工厂
    /// </summary>
    public class ConnectionFactory : IDisposable, IConnectionPool
    {
        private static ConnectionFactory _Instance = null;

        private IList<PoolConnection> _ConnList = null;

        private static object lockObj = new Object();

        private static DBType _DBType = DBConfig.DefaultDBType;

        private static string _DBConnString = DBConfig.DefaultDBConnString;

        private const int DeftConnCount = 5;

        private const int MinFreeConnCount = 2;

        private const int StepCount = 2;

        private const int MaxConnCount = 10000;

        //最大非活动时间
        private TimeSpan ts = new TimeSpan(0, 15, 0);

        /// <summary>
        /// 当前连接的最大编号
        /// </summary>
        private int _CurtMaxSN = 0;

        /// <summary>
        /// 可用连接数量
        /// </summary>
        private int _FreeCount = 0;


        //定时器定时扫面长时间不活动的连接
        //private System.Threading.Timer t;

        /// <summary>
        /// 获得连接池的方法，需要指定数据库类型，和连接字符串
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static ConnectionFactory GetInstance(DBType dbType, string connStr)
        {
            if (null == _Instance)
            {
                //为空时才需要加锁，不为空的话直接就返回了，不会进来
                lock (lockObj)
                {
                    if (null == _Instance)
                    {
                        //只初始化一次
                        if (!string.IsNullOrEmpty(connStr))
                        {
                            _DBConnString = connStr;
                        }
                        if (dbType != DBType.UnKnown)
                        {
                            _DBType = dbType;
                        }
                        _Instance = new ConnectionFactory();
                    }
                }
            }
            return _Instance;
        }

        //构造函数中直接创建5个默认的数据库连接
        private ConnectionFactory()
        {
            this._ConnList = new List<PoolConnection>();
            #region MyRegion
            //for (int i = 1; i <= DeftConnCount; i++)
            //{
            //    PoolConnection pc = new PoolConnection();
            //    pc.Connection = GetDBConnection();
            //    pc.SN = i;
            //    pc.ActiveTime = DateTime.Now;
            //    pc.IsUsed = false;
            //    _ConnList.Add(pc);
            //} 
            #endregion
            this.OpenNewConnection(DeftConnCount);
            this._FreeCount = DeftConnCount;
            this._CurtMaxSN = DeftConnCount;
            Console.WriteLine(TipConst.PoolInitFinished + ToString());
            Console.WriteLine();
            //TODO:开启定时器
        }

        /// <summary>
        /// 创建新的连接并返回,这是真正的创建Connection
        /// 根据数据库的类型和连接字获得相应的数据库连接，已经打开的
        /// </summary>
        /// <returns></returns>
        private IDbConnection CreateNewConnection()
        {
            if (String.IsNullOrEmpty(_DBConnString))
            {
                throw new QDException(TipConst.ConnStrEmpty);
            }
            IDbConnection conn = null;
            switch (_DBType)
            {
                case DBType.UnKnown:
                    break;
                case DBType.Oracle:
                    {
                        conn = new OracleConnection(_DBConnString);
                    }
                    break;
                case DBType.MySQL:
                    { }
                    break;
                case DBType.SQLServer:
                    {
                        conn = new System.Data.SqlClient.SqlConnection(_DBConnString);
                    }
                    break;
                case DBType.DB2:
                    { }
                    break;
                default:
                    break;
            }
            conn.Open();
            #region MyRegion
            //如果打开两次会报错：连接未关闭,连接的当前状态为打开。
            //conn.Open(); 
            #endregion
            return conn;
        }

        /// <summary>
        /// 获得一个可用的连接
        /// </summary>
        /// <returns></returns>
        public PoolConnection GetConnection()
        {
            lock (lockObj)
            {
                if (this._FreeCount <= MinFreeConnCount)
                {
                    if (this._CurtMaxSN >= MaxConnCount || (this._CurtMaxSN + StepCount >= MaxConnCount))
                    {
                        throw new QDException(TipConst.MoreThanMaxConnectionCnt);
                    }
                    Console.WriteLine(TipConst.FreeConnCntIsNotEnough + StepCount);
                    OpenNewConnection(StepCount);
                    Console.WriteLine(TipConst.AfterCreateNewConn + ToString());
                }
                //可以用另外一个List管理已经使用或未使用的连接
                foreach (PoolConnection item in _ConnList)
                {
                    if (item.IsUsed)
                    {
                        continue;
                    }
                    //更新活动时间
                    item.ActiveTime = DateTime.Now;
                    item.IsUsed = true;
                    this._FreeCount -= 1;
                    Console.WriteLine(TipConst.GetConnFromPool + ToString());
                    return item;
                }
            }
            return null;
        }

        /// <summary>
        /// 真正打开count个新的数据库连接
        /// </summary>
        /// <param name="count"></param>
        private void OpenNewConnection(int count)
        {
            for (int i = 1; i <= count; i++)
            {
                PoolConnection pc = new PoolConnection();
                pc.Connection = CreateNewConnection();
                pc.SN = ++_CurtMaxSN;
                pc.ActiveTime = DateTime.Now;
                pc.IsUsed = false;
                this._ConnList.Add(pc);
                this._FreeCount += 1;
            }
            //return true;
        }


        public void CloseConnection(PoolConnection poolConn)
        {
            lock (lockObj)
            {
                if (null == poolConn)
                {
                    Console.WriteLine(TipConst.PoolConnIsNull);
                }
                else if (poolConn.IsUsed)
                {
                    poolConn.IsUsed = false;
                    this._FreeCount += 1;
                    Console.WriteLine(TipConst.ReuseOnePoolConn + poolConn.SN + "\t" + ToString());
                    Console.WriteLine();
                }
                else
                {
                    Console.WriteLine(TipConst.ReusePoolConnFailed);
                }
            }
        }

        public void CloseConnection(int SN)
        {
            foreach (PoolConnection item in this._ConnList)
            {
                if (item.SN == SN)
                {
                    CloseConnection(item);
                }
            }
        }

        public override string ToString()
        {
            return string.Format(TipConst.PoolInfo, this._CurtMaxSN, this._FreeCount);
        }

        public void Destroy()
        {
            foreach (PoolConnection item in this._ConnList)
            {
                try
                {
                    item.Connection.Close();
                    item.Connection.Dispose();
                    item.Connection = null;
                }
                catch (Exception ex)
                {
                    throw new QDException(TipConst.CloseConnError, ex);
                }
            }
            this._ConnList.Clear();
            this._ConnList = null;
            this._CurtMaxSN = 0;
            this._FreeCount = 0;
            //TODO:关闭定时器
        }

        public void Dispose()
        {
            Destroy();
            //GC.SuppressFinalize(this);
            //GC.Collect();
        }



        /**
         * 定时器主程序
         * foreach (PoolConnection item in _ConnList)
         * {
         *      if(item.ActiveTime+ timespan <= DateTime.Now)
         *      {
         *          CloseConnection(item.SN);
         *      }
         * }
         */
    }
}