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
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :ConnectionPool 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:  15910708769@163.com
// Create Time��2015/2/25 10:01:28 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Pool
{
    /// <summary>
    /// ���ݿ����ӳأ�����֧�ֶ������ݿ⣬����һ�����ӹ���
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

        //���ǻʱ��
        private TimeSpan ts = new TimeSpan(0, 15, 0);

        /// <summary>
        /// ��ǰ�Ѿ��򿪵����������ӵ������
        /// </summary>
        private int _CurtMaxSN = 0;

        /// <summary>
        /// ���С�������������
        /// </summary>
        private int _FreeCount = 0;


        //��ʱ����ʱɨ�泤ʱ�䲻�������
        //private System.Threading.Timer t;

        /// <summary>
        /// ������ӳصķ�������Ҫָ�����ݿ����ͣ��������ַ���
        /// </summary>
        /// <param name="dbType"></param>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static ConnectionPool GetInstance(DBType dbType, string connStr)
        {
            if (null == _Instance)
            {
                //Ϊ��ʱ����Ҫ��������Ϊ�յĻ�ֱ�Ӿͷ����ˣ��������
                lock (lockObj)
                {
                    if (null == _Instance)
                    {
                        //ֻ��ʼ��һ��
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

        //���캯����ֱ�Ӵ���5��Ĭ�ϵ����ݿ�����
        private ConnectionPool()
        {
            this._ConnList = new List<PoolConnection>();

            OpenNewConnection(DeftConnCount);
            this._FreeCount = DeftConnCount;
            this._CurtMaxSN = DeftConnCount;
            Console.WriteLine(TipConst.PoolInitFinished + ToString());
            Console.WriteLine();
            //TODO:������ʱ�� 
        }

        /// <summary>
        /// ���һ�����õ�����
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
                    //���ʣ�µ���������stepCnt���������������Ϊֹ����Ҫ���쳣
                    int newCnt = StepCount;
                    if (this._CurtMaxSN + StepCount > MaxConnCount)
                    {
                        newCnt = MaxConnCount - _CurtMaxSN;
                    }
                    Console.WriteLine(TipConst.FreeConnCntIsNotEnough + StepCount);
                    OpenNewConnection(StepCount);
                    Console.WriteLine(TipConst.AfterCreateNewConn + ToString());
                }
                //ֱ��ѭ�����ɣ�ÿ���߳�ʹ�����ݿ��ʱ�䲻��
                //Ҳ����������һ��List�����Ѿ�ʹ�û�δʹ�õ�����
                foreach (PoolConnection item in _ConnList)
                {
                    if (item.IsUsed)
                    {
                        continue;
                    }
                    //���»ʱ��
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
        /// ���������ɸ��µ����ݿ�����
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
        /// �����Ĵ����µ����Ӳ�����,���������Ĵ���Connection
        /// �������ݿ�����ͺ������ֻ����Ӧ�����ݿ����ӣ��Ѿ��򿪵�
        /// </summary>
        /// <returns></returns>
        private IDbConnection CreateNewConnection()
        {
            if (String.IsNullOrEmpty(_DBConnString))
            {
                throw new QDException(TipConst.ConnStrEmpty);
            }
            /*
             * ADO.net �����ݿ����ӷ�ʽ(΢���ṩ)
                ΢���ṩ�������������ݿ����ӷ�ʽ��
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

                        //������Integrated SecurityΪTrueʱ����windows�����֤ģʽ���������ǰ���UserID, PW�ǲ������õġ�
                        //ֻ������ΪFalse��ʡ�Ը����ʱ�򣬲Ű��� UserID, PW �����ӡ� 
                        //Integrated Security ����������Ϊ��sspi ���൱�� True��������������� True�� 
                        //Data Source=myServerAddress;Initial Catalog=myDataBase;Integrated Security=SSPI; 
                        //Data Source=myServerAddress;Initial Catalog=myDataBase;Integrated Security=true; 
                        //Data Source=myServerAddress;Initial Catalog=myDataBase;;User ID=myUsername;Password=myPassword Integrated Security=false; 

                        //Initial Catalog����Ҫ���ӵ����ݿ������

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

            #region ��������λᱨ������δ�ر�,���ӵĵ�ǰ״̬Ϊ��
            //conn.Open(); 
            #endregion
            return conn;
        }

        /// <summary>
        /// �ͷ����ӵ����ӳ�
        /// </summary>
        /// <param name="poolConn"></param>
        public void CloseConnection(PoolConnection poolConn)
        {
            //ҲҪ����
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
        /// �ر�ĳ�����ݿ����ӣ����ͻ��˵���
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
                    //�ر������ͷ���Դ
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
            //TODO:�رն�ʱ��
        }

        public void Dispose()
        {
            Destroy();
            //GC.SuppressFinalize(this);
            //GC.Collect();
        }



        /**
         * ��ʱ��������
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