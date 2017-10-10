#pragma warning disable 0618
using QD.Found;
using QD.Found.Core;
using QD.Found.Util;
using QD.ORM.Common;
using QD.ORM.Engine.Core;
using QD.ORM.MappingAttribute.Col;
using QD.ORM.MappingAttribute.Table;
using QD.ORM.Model;
using QD.ORM.Pool;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Data.OracleClient;
using System.Data.SqlClient;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :DeftDBManager 
// Function Description: ���ݿ������������
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/23 12:19:31 
// 
// Modify Time: 2016��3��9��
// Modify Description: ��֮ǰOracleDBManager�е��߼����Ƶ������У�����SQLServerDBManager���������������߼����ܸ���
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine
{
    internal class DBManagerAdapter : DataBaseManager
    {
        //��ǰ���ݿ����ͣ��ڻ�þ����Managerʱ���丳ֵ
        protected bool bOracle = false;
        protected bool bSQLServer = false;
        protected bool bMySQL = false;
        //��ͬ���ݿ�ǰ׺Ҳ��ͬ

        //�����е���ģʽ
        protected static DBManagerAdapter _DBManager = null;

        protected static object lockObj = new object();

        //���ݿ����Ӳ�����Ϊ������Ա
        //private PoolConnection poolConnection;

        protected ConnectionPool _Pool;


        #region �����ֵ䣬�����ֵ�
        /// <summary>
        /// �����ֵ䣬�Ž�ȥ��Ϊ���ظ�����
        /// </summary>
        private IDictionary<int, DbTransaction> _TxDictionary = new Dictionary<int, DbTransaction>();

        /// <summary>
        /// ��������������ݿ������ֵ�
        /// </summary>
        private IDictionary<int, PoolConnection> _ConnDictionary = new Dictionary<int, PoolConnection>();

        #endregion

        private int CurtThreadId
        {
            get
            {
                return Thread.CurrentThread.ManagedThreadId;
            }
        }

        /// <summary>
        /// ���һ�����ݿ�����,����
        /// </summary>
        protected override PoolConnection GetConnection()
        {
            //��3�����б�Ҫ�ģ�ע��: һ����������֮������������ĳ�����ӵģ����뽫������Ҳ�ŵ��ֵ��У�
            //Ȼ��ͻ��˵���Save��Update�ȷ���ʱ����Ҫ��ȡ���ӣ��ͷ����������������ӣ���Command
            //���򱨴�: Transaction ����δ�� Connection �������
            if (_ConnDictionary.ContainsKey(CurtThreadId))
            {
                //�����ǰ�߳��Ѿ���ʼ����
                //Console.WriteLine("-------GetConnection()���߳��ֵ���");
                return _ConnDictionary[CurtThreadId];//����ѿ������񣬱����õ�����������Ǹ������ܷ���һ���µ�����
            }
            PoolConnection conn = _Pool.GetConnection();
            if (null == conn || null == conn.Connection)
            {
                throw new ORMException(TipConst.DBConnectionIsNull);
            }
            if (conn.Connection.State != ConnectionState.Open)
            {
                conn.Connection.Open();
            }
            return conn;
        }

        /// <summary>
        /// ��ǰ�߳��Ƿ��ѿ���������
        /// </summary>
        /// <returns></returns>
        private bool IsCurtThreadBeginTranscation()
        {
            //ÿ���̵߳�ID�ǲ�ͬ�ģ�
            return _TxDictionary.ContainsKey(CurtThreadId);
        }

        //��������
        public override DbTransaction BeginTransaction()
        {
            PoolConnection conn = GetConnection();
            DbTransaction tx = ((DbConnection)conn.Connection).BeginTransaction();
            AddTxAndConnToDict(tx, conn);
            return tx;
        }

        /// <summary>
        /// �������񲢷�������
        /// </summary>
        /// <param name="conn"></param>
        /// <returns></returns>
        protected override DbTransaction BeginTransaction(out PoolConnection conn)
        {
            conn = GetConnection();
            DbTransaction tx = ((DbConnection)conn.Connection).BeginTransaction();
            AddTxAndConnToDict(tx, conn);
            return tx;
        }

        //��ǰ�ǵ����ģ����Զ��̻߳����£�
        //�����ConnectionҪ��Ӧ���������������ύ��ع����Ǵ����

        /// <summary>
        /// ��¼��ǰ��Transaction��Connection
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="pc"></param>
        public void AddTxAndConnToDict(DbTransaction tx, PoolConnection pc)
        {
            //����Ҫlock, ��ʹ���̣߳�ÿ���̶߳����Լ���ID

            int threadId = Thread.CurrentThread.ManagedThreadId;
            //���ַ�ʽ�ǰ�ȫ�ģ�txDict.Add()�ظ���ӻᱨ��
            _TxDictionary[threadId] = tx;
            _ConnDictionary[threadId] = pc;
        }

        /// <summary>
        /// ����ֻ���Ƴ��ֵ��е���������ݿ����ӣ��������Ƴ������е�����,
        /// �ͻ���ʹ��ʱӦ����try,catch�ύ��ع�����
        /// </summary>
        protected override void RemoveTxAndConn()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            //��ʹ������Ҳ���ᱨ��
            _TxDictionary.Remove(threadId);
            _ConnDictionary.Remove(threadId);
        }

        public DbTransaction GetCurrentTx()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;
            if (_TxDictionary.ContainsKey(threadId))
            {
                return _TxDictionary[threadId];
            }
            return null;
        }


        #region ����ʹ��

        //public  IDbCommand CreateCommand(string sqlText, IDbConnection conn)
        //{
        //    if (!(conn is OracleConnection))
        //    {
        //        throw new ORMException(TipConst.NotOracleConnection);
        //    }
        //    OracleConnection oracleConn = conn as OracleConnection;
        //    //�ж������Ƿ��
        //    if (conn.State != ConnectionState.Open)
        //    {
        //        oracleConn.Open();
        //    }
        //    IDbCommand cmd = new OracleCommand(sqlText, oracleConn);
        //    //this._DBCommand = cmd;
        //    return cmd;
        //}

        //public  IDataReader GetReader(System.Data.IDbCommand dbCmd)
        //{
        //    if (!(dbCmd is OracleCommand))
        //    {
        //        throw new ORMException(TipConst.NotOracleCommand);
        //    }
        //    OracleCommand oracleCmd = dbCmd as OracleCommand;
        //    OracleDataReader reader = oracleCmd.ExecuteReader();
        //    //this._DataReader = reader;
        //    return reader;
        //}

        //public override void InitCommand(string sqlText)
        //{
        //    CreateCommandWithParams(sqlText, null, null);
        //}

        #endregion

        public override DataSet QueryDataSetByCommand(IDbCommand dbCmd)
        {
            if (bSQLServer && !(dbCmd is SqlCommand))
            {
                throw new ORMException(TipConst.NotSqlCommand);
            }
            else if (bOracle && !(dbCmd is OracleCommand))
            {
                throw new ORMException(TipConst.NotOracleCommand);
            }
            //OracleDataAdapter adapter = new OracleDataAdapter(dbCmd as OracleCommand);
            DataSet dataSet = new DataSet();

            //���������⼸�䣬��ѯ��Ҳ���Է��ڿ�������֮���ˣ������Ƿ��б�Ҫ�أ�
            if (IsCurtThreadBeginTranscation())
            {
                dbCmd.Transaction = GetCurrentTx();
            }
            if (bSQLServer)
            {
                new SqlDataAdapter(dbCmd as SqlCommand).Fill(dataSet);
            }
            else
            {
                new OracleDataAdapter(dbCmd as OracleCommand).Fill(dataSet);
            }
            return dataSet;
        }


        //�������ĸ�������
        protected override IDbCommand CreateCommandByParams(IDbConnection dbConn, string sqlText, string[] paramNames, object[] paramValues)
        {
            IDbCommand dbCommand = GetDbCommand(dbConn, sqlText);
            if (CommonHelper.NotEmpty(paramNames) && CommonHelper.NotEmpty(paramValues)
                && (paramNames.Length == paramValues.Length))
            {
                for (int i = 0; i < paramNames.Length; i++)
                {
                    string pn = paramNames[i];
                    DbParameter param = dbCommand.CreateParameter() as DbParameter;
                    param.ParameterName = pn;
                    param.Value = paramValues[i];
                    param.Direction = ParameterDirection.Input;
                    param.DbType = QDConvert.ConvertToDBType(paramValues[i]);
                    //2016��3��19�� ����SQLServer��int����
                    if ("ID".Equals(pn.ToUpper()))
                    {
                        int v;
                        if (null != paramValues[i] && int.TryParse(paramValues[i].ToString(), out v))
                        {
                            param.DbType = DbType.Int32;
                        }
                    }
                    dbCommand.Parameters.Add(param);
                }
            }
            return dbCommand;
        }

        private IDbCommand GetDbCommand(IDbConnection dbConn, string sqlText)
        {
            IDbCommand dbCommand = null;
            if (bSQLServer)
            {
                dbCommand = new SqlCommand(sqlText, dbConn as SqlConnection);
            }
            else
            {
                dbCommand = new OracleCommand(sqlText, dbConn as OracleConnection);
            }
            return dbCommand;
        }

        public override DataSet QueryDataSetBySQLWithParams(string sql, string[] paramNames, object[] paramValues)
        {
            PoolConnection conn = GetConnection();
            IDbCommand cmd = CreateCommandByParams(conn.Connection, sql, paramNames, paramValues);
            DataSet dataSet = QueryDataSetByCommand(cmd);
            //����֮�������ͷ�
            CloseConnection(conn);
            return dataSet;
        }

        //���ַ�ʽ������׳
        public override List<T> QueryListByOQLWithParams<T>(string oql, string[] paramNames, object[] paramValues)
        {
            PoolConnection conn = GetConnection();
            IDbCommand cmd = CreateSelectCommand<T>(conn.Connection, oql, paramNames, paramValues);
            DataSet dataSet = QueryDataSetByCommand(cmd);
            CloseConnection(conn);
            return MappingEngine<T>.Mapping(dataSet);
        }

        private void AddParameterToCmd(IDbCommand dbCommand, string paramName, object paramValue)
        {
            DbParameter param = dbCommand.CreateParameter() as DbParameter;
            param.ParameterName = paramName;
            param.Value = paramValue;
            //param.Direction = ParameterDirection.Input;//Ĭ�Ͼ���
            param.DbType = QDConvert.ConvertToDBType(paramValue);
            //2016��3��19�� ����SQLServer��int����
            if ("ID".Equals(paramName.ToUpper()))
            {
                int v;
                if (null != paramValue && int.TryParse(paramValue.ToString(), out v))
                {
                    param.DbType = DbType.Int32;
                }
            }
            dbCommand.Parameters.Add(param);
        }

        private IDbCommand GetCommand(IDbConnection conn, string sqlText, string[] paramNames, object[] paramValues)
        {
            IDbCommand cmd = GetDbCommand(conn, sqlText);
            //���ò���
            for (int i = 0; i < paramNames.Length; i++)
            {
                AddParameterToCmd(cmd, paramNames[i], paramValues[i]);
            }
            return cmd;
        }

        #region ����SQL������OQL����󣩹�����SQL����

        protected IDbCommand CreateSelectCommand<T>(IDbConnection conn, string oql, string[] paramNames, object[] paramValues)
        {
            if (String.IsNullOrEmpty(oql))
            {
                throw new ArgumentNullException("oql");
            }
            //PoolConnection poolConn = GetConnFromPool();
            //IDbConnection conn = poolConn.Connection;

            Type type = typeof(T);
            string tableName = ORMUtil.GetTableName(type);
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ORMException(TipConst.TableNameIsEmpty);
            }
            //���ӣ�
            //from User where UserName = :U and Password = :P

            string sqlText = null;
            bool bNoParam = false;
            //where����Ĳ���
            string strAfterWhere = null;

            //���û��where���ǲ�ѯȫ��
            if (!oql.ToUpper().Contains(Constant.Str_WHERE))
            {
                bNoParam = true;
            }
            else
            {
                int begin = oql.ToUpper().IndexOf(Constant.Str_WHERE) + Constant.Str_WHERE.Length;
                strAfterWhere = oql.Substring(begin);
                if (string.IsNullOrEmpty(strAfterWhere) || string.IsNullOrEmpty(strAfterWhere.Trim()))
                {
                    bNoParam = true;
                }
            }
            if (bNoParam)
            {
                sqlText = ORMUtil.GenerateSelectAllSQL(type);
                //����Ҫ���ò���
                return GetDbCommand(conn, sqlText);
            }

            //����Where
            sqlText = GenerateSQLWherePart(paramNames, type, strAfterWhere, CmdType.Select);
            return GetCommand(conn, sqlText, paramNames, paramValues);
        }


        public override int CountBySQLWithParams(string sql, string[] paramNames, object[] paramValues)
        {
            if (string.IsNullOrWhiteSpace(sql))
            {
                throw new ArgumentNullException("sql");
            }

            //if (!sql.ToUpper().TrimStart().StartsWith("SELECT COUNT"))
            //{
            //    throw new ORMException("��ѯ����Ӧ����SELECT COUNT��ͷ");
            //}
            string fmt = @"^SELECT +COUNT\([^\\]+\)( +AS \w+)? +FROM +\w+";
            Regex reg = new Regex(fmt, RegexOptions.IgnoreCase);//����ģʽ�еĴ�Сд�����ı���Сд������
            if (reg.IsMatch(sql.Trim()))
            {
                throw new ORMException("��ѯ����SQL����ȷ����ȷ��ʽΪ��SELECT COUNT(T.ID) FROM TABLE ");
            }
            PoolConnection conn = GetConnection();
            IDbCommand cmd = CreateCommandByParams(conn.Connection, sql, paramNames, paramValues);
            //���ص�һ�е�һ��
            object obj = cmd.ExecuteScalar();
            CloseConnection(conn);
            if (obj == DBNull.Value)
            {
                return 0;
            }
            int count;
            if (int.TryParse(obj.ToString(), out count))
            {
                return count;
            }
            return 0;
        }

        public override void SaveEntity(IIdentify obj)
        {
            PoolConnection conn = GetConnection();
            IDbCommand _DBCommand = CreateInsertCommand(conn.Connection, obj);
            if (IsCurtThreadBeginTranscation())
            {
                //����������߳̾�ʹ��
                _DBCommand.Transaction = GetCurrentTx();
            }
            _DBCommand.ExecuteNonQuery();
            //CloseConnection(conn);
        }

        //��δ����
        public override int ExecuteUpdateBySQLWithParams<T>(string sql, string[] paramNames, object[] paramValues)
        {
            if (string.IsNullOrWhiteSpace(sql) || (!sql.ToUpper().StartsWith("UPDATE") && !sql.ToUpper().StartsWith("DELETE")))
            {
                throw new ORMException("������Ч�ĸ���SQL���");
            }
            PoolConnection conn = GetConnection();
            IDbCommand _DBCommand = CreateCommandByParams(conn.Connection, sql, paramNames, paramValues);
            if (IsCurtThreadBeginTranscation())
            {
                _DBCommand.Transaction = GetCurrentTx();
            }
            return _DBCommand.ExecuteNonQuery();
        }

        public override void DeleteEntityById<T>(string Id)
        {
            PoolConnection conn = GetConnection();
            IDbCommand _DBCommand = CreateDeleteCommand<T>(conn.Connection, Id);
            if (IsCurtThreadBeginTranscation())
            {
                _DBCommand.Transaction = GetCurrentTx();
            }
            _DBCommand.ExecuteNonQuery();
            //CloseConnection(conn);//��AfterTransaction�л�رգ�����ز��ض���
        }

        public override void DeleteEntity(IIdentify obj)
        {
            PoolConnection conn = GetConnection();
            IDbCommand _DBCommand = CreateDeleteCommand(conn.Connection, obj);
            if (IsCurtThreadBeginTranscation())
            {
                _DBCommand.Transaction = GetCurrentTx();
            }
            _DBCommand.ExecuteNonQuery();
            //CloseConnection(conn);
        }

        //TODO:�������������׳����where flag = 1 and Id>:id����where�������г�����ֻ��һ�����������ָ������������������flag =:id �Һ�������Խ��
        private static string GenerateSQLWherePart(string[] paramNames, Type type, string oqlAfterWhere, CmdType cmdType)
        {
            //hql����д�Ĳ��淶����and,AND��And�ȣ����������޷��ָ�

            StringBuilder sbWhere = new StringBuilder();
            //����+���ţ��磺ID>
            List<string> colInfoList = new List<string>();

            //TODO:�ָ�������OR���磺where Id>:ID and (age > :AGE or height > :H)������̫�����ˣ��Ȳ�֧��
            string[] arr = StringHelper.SplitIgnoreCase(oqlAfterWhere, "and");
            foreach (string item in arr)
            {
                //ע�ⲻһ����=���磺Id > :A
                //���õĹ�����������������Щ
                string[] temp = item.Split(new string[] { "=", "!=", ">", "<", "like", "Like", "LIKE", "between", "Between", "BETWEEN" }, StringSplitOptions.RemoveEmptyEntries);
                string prop = temp[0].Trim();
                //string val = temp[1].Trim();

                //��ѯ���Զ�Ӧ���ֶ�
                string colName = ORMUtil.GetColumnName(type, prop);
                if (null == colName)
                {
                    throw new ORMException("���ԣ�" + prop + "û��ӳ���ֶ�");
                }
                //Ĭ��
                string symb = "=";

                //���鷳��������>,<,!=,Like,= �����пո���������,��Ҫ���Ǵ�Сд������׼ȷ֪������ķ���
                if (item.Contains("!="))
                {
                    symb = "!=";
                }
                else if (item.Contains(">"))
                {
                    symb = ">";
                }
                else if (item.Contains("<"))
                {
                    symb = "<";
                }
                else if (item.Contains("like") || item.Contains("Like") || item.Contains("LIKE"))
                {
                    symb = "LIKE";
                }
                else if (item.Contains("between") || item.Contains("Between") || item.Contains("BETWEEN"))
                {
                    symb = "BETWEEN";
                }

                colInfoList.Add(colName + " " + symb + " ");
            }

            string sqlText = null;
            switch (cmdType)
            {
                case CmdType.Select:
                    sqlText = ORMUtil.GenerateSelectAllSQL(type);
                    break;
                case CmdType.Update:
                    sqlText = ORMUtil.GenerateUpdateSQL(type);
                    break;
            }

            sqlText += (" " + Constant.Str_WHERE);
            for (int i = 0; i < colInfoList.Count; i++)
            {
                string colName_Symbol = colInfoList[i];
                if (i != 0)
                {
                    sbWhere.Append(" AND");
                }
                sbWhere.Append(" " + colName_Symbol);
                sbWhere.Append(":" + paramNames[i]);
            }
            sqlText += sbWhere.ToString();
            return sqlText;
        }

        //��UpdateEntity() ����, �����е��ֶζ�����
        private IDbCommand CreateUpdateCommand(IDbConnection conn, IIdentify obj)
        {
            if (null == obj)
            {
                throw new ArgumentNullException("obj");
            }
            if (string.IsNullOrEmpty(obj.Id))
            {
                throw new ArgumentNullException("obj.Id");
            }
            Type type = obj.GetType();
            string tableName = ORMUtil.GetTableName(type);
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ORMException(TipConst.TableNameIsEmpty);
            }

            //UPDATE BD_SPD SET SPD_SN = :P1, LP_SN= :P2, ORDER_ID = :P3 WHERE ID = :ID
            string sql = "UPDATE " + tableName + " SET ";

            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            string colName = null;
            int i = 0;
            List<string> pnList = new List<string>();
            List<object> pvList = new List<object>();
            List<Type> ptList = new List<Type>();

            foreach (PropertyInfo prop in props)
            {
                #region ����ͨ��while��ã�ʡ��
                //Attribute[] attr = Attribute.GetCustomAttributes(pi);

                //if (attr[0] is ColumnAttribute)
                //{
                //    colName = ((ColumnAttribute)attr[0]).ColumnName;
                //    if (string.IsNullOrEmpty(colName))
                //    {
                //        colName = pi.Name.ToUpper();
                //    }
                //}
                //#region ����ֶ�
                ////else if (attr[0] is ManyToOneAttribute)
                ////{
                ////    colName = ((ManyToOneAttribute)attr[0]).FKColumn;
                ////}
                ////else if (attr[0] is OneToOneAttribute)
                ////{
                ////    colName = ((OneToOneAttribute)attr[0]).FKColumn;
                ////} 
                //#endregion
                //else
                //{
                //    //OneToManyAttribute, ManyToManyAttribute ������ڵ�ǰ����
                //    continue;
                //} 
                #endregion

                //��ʱ����ManyToOne��ManyToMany��
                ColumnAttribute attr = Attribute.GetCustomAttribute(prop, typeof(ColumnAttribute)) as ColumnAttribute;
                if (null == attr || attr is PKAttribute)//��������Ҫ����
                {
                    continue;
                }
                colName = ((ColumnAttribute)attr).ColumnName;
                if (string.IsNullOrEmpty(colName))
                {
                    colName = prop.Name.ToUpper();
                }

                object val = prop.GetValue(obj, null);
                if (null == val)
                {
                    //�������ֵΪ��Ҳ�п��ܣ������޸�ʱ����ע��ĳ��ʱ���ÿ�                    
                }

                //�����ֵ����û����
                if (prop.PropertyType.IsValueType)
                {
                    //ö�ٶ�Ӧint,�������⴦��

                }
                else if (typeof(string) != prop.PropertyType)//pi.PropertyType.IsClass)
                {
                    //����ֵ���ͣ��ֲ���string
                    continue;
                }

                ++i;
                sql += (colName + " = " + Prefix + "P" + i + ",");

                pnList.Add("P" + i);
                pvList.Add(QDConvert.ToDBValue(val, prop.PropertyType));
                ptList.Add(prop.PropertyType);
            }

            string sqlText = sql.Remove(sql.Length - 1) + " WHERE ID = " + Prefix + "ID ";
            pnList.Add("ID");
            pvList.Add(obj.Id);
            ptList.Add(obj.Id.GetType());

            IDbCommand dbCommand = GetDbCommand(conn, sqlText);
            //���ò���
            for (int j = 0; j < pnList.Count; j++)
            {
                DbParameter param = dbCommand.CreateParameter() as DbParameter;
                param.ParameterName = pnList[j];
                if (null != pvList[j])
                {
                    param.Value = pvList[j];
                }
                else
                {
                    param.Value = DBNull.Value;
                }
                param.DbType = QDConvert.ConvertToDBType(ptList[j]);
                //2016��3��19�� ����SQLServer��int����
                if ("ID".Equals(pnList[j].ToUpper()))
                {
                    int v;
                    if (null != pvList[j] && int.TryParse(pvList[j].ToString(), out v))
                    {
                        param.DbType = DbType.Int32;
                    }
                }
                dbCommand.Parameters.Add(param);
            }
            return dbCommand;
        }



        ///**
        // * SysRole _SysRoleDB = ISysRoleDao.GetEntityById(_SysRole.Id);
        //    _SysRoleDB.Name = _SysRole.Name;
        //    _SysRoleDB.OrderNum = _SysRole.OrderNum;
        //    _SysRoleDB.Description = _SysRole.Description;
        //    ISysRoleDao.UpdateEntity(_SysRoleDB);
        // */
        /// <summary>
        /// ����ʵ�����
        /// </summary>
        /// <param name="obj">Ӧ���ǰ���ID�������Ķ���</param>
        /// <returns>���µ�����</returns>
        public override int UpdateEntity(IIdentify obj)
        {
            PoolConnection conn = GetConnection();
            IDbCommand _DBCommand = CreateUpdateCommand(conn.Connection, obj);
            if (IsCurtThreadBeginTranscation())
            {
                _DBCommand.Transaction = GetCurrentTx();
            }
            return _DBCommand.ExecuteNonQuery();
        }


        //������������ַ�ʽ��
        //1.����A������B�������磺 class People {public Country MyCount{set;get;} } ����ǵ��͵�ManyToOne
        //2.ֱ����A����string������磺 class People{public string CountryId{set;get;} }
        //���ַ�ʽ�����ԣ������Ƽ�ʹ�õ�2�֣���ֱ��
        protected IDbCommand CreateInsertCommand(IDbConnection conn, IIdentify obj)
        {
            Type type = obj.GetType();
            string tableName = null;

            TableAttribute tableAttr = Attribute.GetCustomAttribute(type, typeof(TableAttribute)) as TableAttribute;
            if (null != tableAttr)
            {
                if (string.IsNullOrEmpty(tableAttr.TableName))
                {
                    tableName = type.Name.ToUpper();
                }
                tableName = tableAttr.TableName;
            }

            if (string.IsNullOrEmpty(tableName))
            {
                throw new ORMException(TipConst.TableNameIsEmpty);
            }
            //�Ƿ�������������
            bool bAutoID = false;
            if (bSQLServer && PKGenerateStrategy.UnKnown == tableAttr.PKStaregy)
            {
                //�����SQLServer��Ĭ��ID������������ֻд���� ʡ�Ժ���2������ [Table("T_STUDENT", PKGenerateStrategy.AutoIncrease, "")]
                bAutoID = true;
            }
            else
            {
                bAutoID = PKGenerateStrategy.AutoIncrease.Equals(tableAttr.PKStaregy);
            }
            //��������ʱ���Ҫά��ID, �����������ɷ�ʽ
            switch (tableAttr.PKStaregy)
            {
                case PKGenerateStrategy.UnKnown:
                case PKGenerateStrategy.GUID:
                    {
                        obj.Id = Guid.NewGuid().ToString();
                    }
                    break;
                case PKGenerateStrategy.SEQ:
                    {
                        //TODO:SEQ��ʽ����ѯ��һ������
                        if (string.IsNullOrEmpty(tableAttr.SeqName))
                        {
                            throw new QDException(TipConst.NOT_SET_SEQNAME);
                        }

                        string seqSQL = "SELECT " + tableAttr.SeqName + ".NEXTVAL FROM DUAL";
                        IDbCommand cmd = GetDbCommand(conn, seqSQL);
                        object seq = cmd.ExecuteScalar();
                        obj.Id = seq.ToString();
                    }
                    break;
                case PKGenerateStrategy.Assign:
                    {
                        //�ж�ID�Ƿ���ֵ�����û���ų��쳣
                        if (String.IsNullOrEmpty(obj.Id))
                        {
                            throw new QDException(TipConst.NOT_SET_ID_VALUE);
                        }
                    }
                    break;
                case PKGenerateStrategy.AutoIncrease:
                    //SQLServer ����������
                    break;
                default:
                    break;
            }
            //INSERT INTO T_USER (ID, USERNAME,PASSWORD, REALNAME) VALUES (:P1, :P2, :P3, P4);
            string sql1 = "INSERT INTO " + tableName + " ( ";
            string sql2 = " VALUES ( ";
            PropertyInfo[] props = type.GetProperties(BindingFlags.Public | BindingFlags.Instance);

            string colName = null;
            int i = 0;
            List<string> pnList = new List<string>();
            List<object> pvList = new List<object>();
            List<Type> ptList = new List<Type>();

            foreach (PropertyInfo pi in props)
            {
                #region ��ʽ1,Ч����
                //Attribute[] attrs = Attribute.GetCustomAttributes(pi);

                //#region �����Ͽ����ж��ע�⣬���ַ�ʽ���Եõ���Ҫ��ע��
                //Attribute attr = attrs[0];
                //int z = 0;
                //while (z < attrs.Length && (!(attr is ColumnAttribute) && !(attr is ManyToOneAttribute) && !(attr is OneToOneAttribute)))
                //{
                //    z++;
                //    attr = attrs[z];
                //}                
                //if (null == attr)
                //{
                //    continue;
                //}
                //#endregion

                //if (attrs[0] is ColumnAttribute)
                //{
                //    colName = ((ColumnAttribute)attrs[0]).ColumnName;
                //    if (string.IsNullOrEmpty(colName))
                //    {
                //        colName = pi.Name.ToUpper();
                //    }
                //}
                //else if (attrs[0] is ManyToOneAttribute)
                //{
                //    //һ��ֱ��д����ͨ��string FKId�����������ַ�ʽ
                //    colName = ((ManyToOneAttribute)attrs[0]).FKColumn;
                //}
                //else if (attrs[0] is OneToOneAttribute)
                //{
                //    colName = ((OneToOneAttribute)attrs[0]).FKColumn;
                //}
                //else
                //{
                //    //OneToManyAttribute, ManyToManyAttribute ������ڵ�ǰ����
                //    continue;
                //} 
                #endregion

                ColumnAttribute attr = Attribute.GetCustomAttribute(pi, typeof(ColumnAttribute)) as ColumnAttribute;
                if (null == attr)
                {
                    continue;
                }
                colName = ((ColumnAttribute)attr).ColumnName;
                if (string.IsNullOrEmpty(colName))
                {
                    colName = pi.Name.ToUpper();
                }
                //������ʱ������ID��ֵ
                if (bAutoID && "ID".Equals(colName.ToUpper()))
                {
                    continue;
                }
                object val = pi.GetValue(obj, null);
                if (null == val)
                {
                    //�������û�и�ֵ��DBNull
                    //continue;
                }

                //�����ֵ����û����
                if (pi.PropertyType.IsValueType)
                {
                    //ö�ٲ������⴦��
                }
                //�����ͼƬ�ȴ浽���ݿ���ʹ��byte[]
                else if (typeof(String) != pi.PropertyType && !pi.PropertyType.IsArray)//pi.PropertyType.IsClass)
                {
                    //TODO:�����ManyToOne��Ӧ�õõ������Թ����Ķ����ID
                    //User{ public TClass MyClass{}}

                    //����һ�㲻��ִ������
                    continue;
                }

                ++i;
                sql1 += (colName + ",");
                sql2 += (Prefix + "P" + i + ",");

                //ͬʱ����
                pnList.Add("P" + i);
                pvList.Add(QDConvert.ToDBValue(val, pi.PropertyType));
                ptList.Add(pi.PropertyType);
                //Console.WriteLine(pi.Name + "=" + val + ", DBValue=[" + QDConvert.ToDBValue(val, pi.PropertyType) + "], type=" + pi.PropertyType.Name);
            }
            sql1 = sql1.Remove(sql1.Length - 1) + ")";
            sql2 = sql2.Remove(sql2.Length - 1) + ")";

            string sqlText = sql1 + sql2;
            IDbCommand _DBCommand = GetDbCommand(conn, sqlText);
            //���ò���
            for (int j = 0; j < pnList.Count; j++)
            {
                //AddParameterToCmd(_DBCommand, pnList[j], pvList[j]);

                DbParameter param = _DBCommand.CreateParameter() as DbParameter;
                param.ParameterName = pnList[j];
                if (null != pvList[j])
                {
                    param.Value = pvList[j];
                }
                else
                {
                    param.Value = DBNull.Value;
                }
                //param.Direction = ParameterDirection.Input;
                param.DbType = QDConvert.ConvertToDBType(ptList[j]);
                _DBCommand.Parameters.Add(param);
            }
            return _DBCommand;
        }


        //���Ƽ���ûʵ��
        protected IDbCommand CreateUpdateCommand<T>(IDbConnection conn, string oql, string[] paramNames, object[] paramValues)
        {
            if (String.IsNullOrEmpty(oql))
            {
                throw new ArgumentNullException("oql");
            }

            Type type = typeof(T);
            string tableName = ORMUtil.GetTableName(type);
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ORMException(TipConst.TableNameIsEmpty);
            }
            //���ӣ�
            //update User set UserName = :U, Password = :P where Id = :ID and xxx=:yy
            //TODO:������Ҫר��дһ���﷨�����������Ѷȣ��Ȳ������


            //string sqlText = null;
            //bool bNoParam = false;
            ////where����Ĳ���
            //string strAfterWhere = null;

            ////���û��where���ǲ�ѯȫ��
            //if (!oql.ToUpper().Contains(Str_WHERE))
            //{
            //    bNoParam = true;
            //}
            //else
            //{
            //    int begin = oql.ToUpper().IndexOf(Str_WHERE) + Str_WHERE.Length;
            //    strAfterWhere = oql.Substring(begin);
            //    if (string.IsNullOrEmpty(strAfterWhere) || string.IsNullOrEmpty(strAfterWhere.Trim()))
            //    {
            //        bNoParam = true;
            //    }
            //}
            //if (bNoParam)
            //{
            //    sqlText = Util.GenerateSelectAllSQL(type);
            //    this._DBCommand = GetDbCommand(this._DBConn, sqlText);
            //    //����Ҫ���ò���
            //    return;
            //}

            //Update tabe xxx set p1 = :A , p2 = :B, p3=:C where Id = :ID

            return null;
        }

        protected IDbCommand CreateDeleteCommand<T>(IDbConnection conn, object id)
        {
            return GetDeleteCmd(conn, typeof(T), id);
        }

        private IDbCommand GetDeleteCmd(IDbConnection conn, Type type, object id)
        {
            //GetConnFromPool();
            String tableName = ORMUtil.GetTableName(type);
            if (String.IsNullOrEmpty(tableName))
            {
                throw new ORMException(TipConst.TableNameIsEmpty);
            }
            //TODO:Ĭ��������ID, ���п������������Ʋ���ID�����ﲻ����׳���Ժ���ϲ�ѯ������������
            //Լ��ÿ�����б�����һ��û��ҵ�����Id����

            //����в���ʹ��ռλ������ֹSQLע��
            string sqlText = String.Format("DELETE FROM {0} WHERE ID = " + Prefix + "Id", tableName);
            IDbCommand _DBCommand = GetDbCommand(conn, sqlText);
            //���ò���
            AddParameterToCmd(_DBCommand, Constant.ID, id);
            return _DBCommand;
        }

        protected IDbCommand CreateDeleteCommand(IDbConnection conn, IIdentify obj)
        {
            return GetDeleteCmd(conn, obj.GetType(), obj.Id);
        }



        #endregion


        #region MyRegion

        //public override void InitReader()
        //{
        //    if (null == this._DBCommand)
        //    {
        //        throw new ORMException(TipConst.DBCommandIsNull);
        //    }
        //    this._DataReader = (this._DBCommand as OracleCommand).ExecuteReader();
        //}

        //public override void InitDataSet()
        //{
        //    if (null == this._DBCommand)
        //    {
        //        throw new ORMException(TipConst.DBCommandIsNull);
        //    }
        //    OracleDataAdapter adapter = new OracleDataAdapter(this._DBCommand as OracleCommand);
        //    this._DataSet = new DataSet();
        //    adapter.Fill(_DataSet);
        //    //this._DataSet = dataSet;
        //}

        //public override DataSet GetDataSet(string sql)
        //{
        //    IDbConnection conn = this.GetConnection();
        //    IDbCommand cmd = this.CreateCommand(sql, conn);
        //    return this.GetDataSet(cmd);
        //}

        //public void Dispose()
        //{
        //    CloseDataSet();
        //    CloseReader();
        //    CloseCommand();
        //    CloseConnection();
        //} 
        #endregion

        protected override void CloseConnection(PoolConnection conn)
        {
            _Pool.CloseConnection(conn);
        }

        /// <summary>
        /// �رյ�ǰ�߳�ʹ�õ�����
        /// </summary>
        protected override void CloseConnection()
        {
            if (_ConnDictionary.ContainsKey(CurtThreadId))
            {
                PoolConnection pc = _ConnDictionary[CurtThreadId];
                if (null != pc)
                {
                    CloseConnection(pc);
                }
            }
        }





        //���󷽷��Ǳ���Ҫ��д�ģ�������������麯���������дҲ�ɲ���д


        //public override System.Data.DataSet QueryDataSetByCommand(System.Data.IDbCommand dbCmd)
        //{
        //    throw new NotImplementedException();
        //}

        //protected override System.Data.IDbCommand CreateCommandByParams(IDbConnection dbConn, string sqlText, string[] paramNames, object[] paramValues)
        //{
        //    throw new NotImplementedException();
        //}

        public override string GetPageSQL(string sql, int pageSize, int pageNo)
        {
            throw new NotImplementedException();
        }

        //public override List<T> QueryListByOQLWithParams<T>(string oql, string[] paramNames, object[] paramValues)
        //{
        //    throw new NotImplementedException();
        //}

        //public override int CountBySQLWithParams(string sql, string[] paramNames, object[] paramValues)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void SaveEntity(Found.Core.IIdentify obj)
        //{
        //    throw new NotImplementedException();
        //}

        //public override int ExecuteUpdateBySQLWithParams<T>(string sql, string[] paramNames, object[] paramValues)
        //{
        //    throw new NotImplementedException();
        //}

        //public override System.Data.DataSet QueryDataSetBySQLWithParams(string sql, string[] paramNames, object[] paramValues)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void DeleteEntityById<T>(string Id)
        //{
        //    throw new NotImplementedException();
        //}

        //public override void DeleteEntity(Found.Core.IIdentify obj)
        //{
        //    throw new NotImplementedException();
        //}

        //protected override void RemoveTxAndConn()
        //{
        //    throw new NotImplementedException();
        //}

        ////public override T LoadById<T>(string Id)
        ////{
        ////    throw new NotImplementedException();
        ////}

        //public override int UpdateEntity(Found.Core.IIdentify obj)
        //{
        //    throw new NotImplementedException();
        //}

        //protected override Pool.PoolConnection GetConnection()
        //{
        //    throw new NotImplementedException();
        //}

        //protected override void CloseConnection(Pool.PoolConnection conn)
        //{
        //    throw new NotImplementedException();
        //}

        //protected override void CloseConnection()
        //{
        //    throw new NotImplementedException();
        //}

        //public override System.Data.Common.DbTransaction BeginTransaction()
        //{
        //    throw new NotImplementedException();
        //}

        //protected override System.Data.Common.DbTransaction BeginTransaction(out QD.ORM.Pool.PoolConnection conn)
        //{
        //    throw new NotImplementedException();
        //}

    }

    public enum CmdType
    {
        Select,
        Update,
    }
}