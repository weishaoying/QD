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
using System.Data.SqlClient;
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
    public class ConnectionPool : IDisposable, IConnectionPool
    {
        private static ConnectionPool _Instance = null;

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
        /// 当前已经打开的数量，连接的最大编号
        /// </summary>
        private int _CurtMaxSN = 0;

        /// <summary>
        /// 空闲、可用连接数量
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
        public static ConnectionPool GetInstance(DBType dbType, string connStr)
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
                        _Instance = new ConnectionPool();
                    }
                }
            }
            return _Instance;
        }

        //构造函数中直接创建5个默认的数据库连接
        private ConnectionPool()
        {
            this._ConnList = new List<PoolConnection>();

            OpenNewConnection(DeftConnCount);
            this._FreeCount = DeftConnCount;
            this._CurtMaxSN = DeftConnCount;
            Console.WriteLine(TipConst.PoolInitFinished + ToString());
            Console.WriteLine();
            //TODO:开启定时器 
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
                    if (this._CurtMaxSN >= MaxConnCount)
                    {
                        throw new QDException(TipConst.MoreThanMaxConnectionCnt);
                    }
                    //如果剩下的数量不够stepCnt，开启到最大数量为止，不要抛异常
                    int newCnt = StepCount;
                    if (this._CurtMaxSN + StepCount > MaxConnCount)
                    {
                        newCnt = MaxConnCount - _CurtMaxSN;
                    }
                    Console.WriteLine(TipConst.FreeConnCntIsNotEnough + StepCount);
                    OpenNewConnection(StepCount);
                    Console.WriteLine(TipConst.AfterCreateNewConn + ToString());
                }
                //直接循环即可，每个线程使用数据库的时间不定
                //也可以用另外一个List管理已经使用或未使用的连接
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
        /// 真正打开若干个新的数据库连接
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

        /// <summary>
        /// 真正的创建新的连接并返回,这是真正的创建Connection
        /// 根据数据库的类型和连接字获得相应的数据库连接，已经打开的
        /// </summary>
        /// <returns></returns>
        private IDbConnection CreateNewConnection()
        {
            if (String.IsNullOrEmpty(_DBConnString))
            {
                throw new QDException(TipConst.ConnStrEmpty);
            }
            /*
             * ADO.net 中数据库连接方式(微软提供)
                微软提供了以下四种数据库连接方式：
                System.Data.OleDb.OleDbConnection
                System.Data.SqlClient.SqlConnection
                System.Data.Odbc.OdbcConnection
                System.Data.OracleClient.OracleConnection
             * 
             * 
             */
            //OleDbConnection conn = new OleDbConnection(@"Provider=Microsoft.Jet.OLEDB.4.0;Data Source=D:\MyWeb\81\05\GrocerToGo.mdb");
            //OleDbConnection conn = new OleDbConnection(
            //@"Provider=Microsoft.Jet.OLEDB.4.0;Password=;
            //User ID=Admin;Data Source=grocertogo.mdb;");
            //OleDbConnection conn = new OleDbConnection(
            //"Provider=MSDAORA; Data Source=ORACLE8i7;Persist Security Info=False;Integrated Security=yes");
            //OleDbConnection conn = new OleDbConnection(
            //"Provider=Microsoft.Jet.OLEDB.4.0; Data Source=c:\bin\LocalAccess40.mdb");
            //OleDbConnection conn = new OleDbConnection(
            //"Provider=SQLOLEDB;Data Source=MySQLServer;Integrated Security=SSPI");

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

                        //当设置Integrated Security为True时采用windows身份验证模式，连接语句前面的UserID, PW是不起作用的。
                        //只有设置为False或省略该项的时候，才按照 UserID, PW 来连接。 
                        //Integrated Security 还可以设置为：sspi ，相当于 True，建议用这个代替 True。 
                        //Data Source=myServerAddress;Initial Catalog=myDataBase;Integrated Security=SSPI; 
                        //Data Source=myServerAddress;Initial Catalog=myDataBase;Integrated Security=true; 
                        //Data Source=myServerAddress;Initial Catalog=myDataBase;;User ID=myUsername;Password=myPassword Integrated Security=false; 

                        //Initial Catalog是你要连接的数据库的名字

                        //SqlConnection conn1 = new SqlConnection("Server=(local);Integrated Security=SSPI;database=Pubs");
                        //SqlConnection conn2 = new SqlConnection("server=(local)\\NetSDK;database=pubs;Integrated Security=SSPI");
                        //SqlConnection conn3 = new SqlConnection("Data Source=localhost;Integrated Security=SSPI;Initial Catalog=Northwind;");
                        //SqlConnection conn4 = new SqlConnection("data source=(local);initial catalog=xr;integrated security=SSPI;persist security info=False;workstation id=XURUI;packet size=4096; ");
                        //SqlConnection conn5 = new SqlConnection("Persist Security Info=False;Integrated Security=SSPI;database=northwind;server=mySQLServer");
                        //SqlConnection conn6 = new SqlConnection("uid=sa;pwd=passwords;initial catalog=pubs;data source=127.0.0.1;Connect Timeout=900");
                    }
                    break;
                case DBType.DB2:
                    { }
                    break;
                default:
                    break;
            }
            conn.Open();

            #region 如果打开两次会报错：连接未关闭,连接的当前状态为打开
            //conn.Open(); 
            #endregion
            return conn;
        }

        /// <summary>
        /// 释放连接到连接池
        /// </summary>
        /// <param name="poolConn"></param>
        public void CloseConnection(PoolConnection poolConn)
        {
            //也要加锁
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

        /// <summary>
        /// 关闭某个数据库连接，供客户端调用
        /// </summary>
        /// <param name="SN"></param>
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
                    //关闭连接释放资源
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