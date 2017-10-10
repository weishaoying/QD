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
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :DeftDBManager 
// Function Description: 数据库管理适配器类
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/23 12:19:31 
// 
// Modify Time: 2016年3月9日
// Modify Description: 将之前OracleDBManager中的逻辑都移到该类中，增加SQLServerDBManager，这样几乎所有逻辑都能复用
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine
{
    internal class DBManagerAdapter : DataBaseManager
    {
        //当前数据库类型，在获得具体的Manager时给其赋值
        protected bool bOracle = false;
        protected bool bSQLServer = false;
        protected bool bMySQL = false;
        //不同数据库前缀也不同

        //子类中单例模式
        protected static DBManagerAdapter _DBManager = null;

        protected static object lockObj = new object();

        //数据库连接不能作为公共成员
        //private PoolConnection poolConnection;

        protected ConnectionPool _Pool;


        #region 连接字典，事务字典
        /// <summary>
        /// 事务字典，放进去是为了重复利用
        /// </summary>
        private IDictionary<int, DbTransaction> _TxDictionary = new Dictionary<int, DbTransaction>();

        /// <summary>
        /// 开启了事务的数据库连接字典
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
        /// 获得一个数据库连接,并打开
        /// </summary>
        protected override PoolConnection GetConnection()
        {
            //这3句是有必要的，注意: 一旦开启事务之后，事务是属于某个连接的，必须将该连接也放到字典中，
            //然后客户端调用Save，Update等方法时，需要获取连接，就返回这个有事务的连接，及Command
            //否则报错: Transaction 对象未与 Connection 对象关联
            if (_ConnDictionary.ContainsKey(CurtThreadId))
            {
                //如果当前线程已经开始连接
                //Console.WriteLine("-------GetConnection()从线程字典获得");
                return _ConnDictionary[CurtThreadId];//如果已开启事务，必须拿到开启事务的那个，不能返回一个新的连接
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
        /// 当前线程是否已开启了事务
        /// </summary>
        /// <returns></returns>
        private bool IsCurtThreadBeginTranscation()
        {
            //每个线程的ID是不同的，
            return _TxDictionary.ContainsKey(CurtThreadId);
        }

        //开启事务
        public override DbTransaction BeginTransaction()
        {
            PoolConnection conn = GetConnection();
            DbTransaction tx = ((DbConnection)conn.Connection).BeginTransaction();
            AddTxAndConnToDict(tx, conn);
            return tx;
        }

        /// <summary>
        /// 开启事务并返回连接
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

        //当前是单例的，所以多线程环境下，
        //事务和Connection要对应起来，否则事务提交或回滚都是错误的

        /// <summary>
        /// 记录当前的Transaction和Connection
        /// </summary>
        /// <param name="tx"></param>
        /// <param name="pc"></param>
        public void AddTxAndConnToDict(DbTransaction tx, PoolConnection pc)
        {
            //不需要lock, 即使多线程，每个线程都有自己的ID

            int threadId = Thread.CurrentThread.ManagedThreadId;
            //这种方式是安全的，txDict.Add()重复添加会报错
            _TxDictionary[threadId] = tx;
            _ConnDictionary[threadId] = pc;
        }

        /// <summary>
        /// 这里只是移除字典中的事务和数据库连接，并不是移除连接中的事务,
        /// 客户端使用时应该用try,catch提交或回滚事务
        /// </summary>
        protected override void RemoveTxAndConn()
        {
            int threadId = Thread.CurrentThread.ManagedThreadId;

            //即使不存在也不会报错
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


        #region 不再使用

        //public  IDbCommand CreateCommand(string sqlText, IDbConnection conn)
        //{
        //    if (!(conn is OracleConnection))
        //    {
        //        throw new ORMException(TipConst.NotOracleConnection);
        //    }
        //    OracleConnection oracleConn = conn as OracleConnection;
        //    //判断连接是否打开
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

            //加上下面这几句，查询就也可以放在开启事务之后了，但是是否有必要呢？
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


        //带参数的复杂命令
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
                    //2016年3月19日 兼容SQLServer的int主键
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
            //用完之后马上释放
            CloseConnection(conn);
            return dataSet;
        }

        //这种方式还不健壮
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
            //param.Direction = ParameterDirection.Input;//默认就是
            param.DbType = QDConvert.ConvertToDBType(paramValue);
            //2016年3月19日 兼容SQLServer的int主键
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
            //设置参数
            for (int i = 0; i < paramNames.Length; i++)
            {
                AddParameterToCmd(cmd, paramNames[i], paramValues[i]);
            }
            return cmd;
        }

        #region 将非SQL参数（OQL或对象）构建成SQL命令

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
            //例子：
            //from User where UserName = :U and Password = :P

            string sqlText = null;
            bool bNoParam = false;
            //where后面的部分
            string strAfterWhere = null;

            //如果没有where就是查询全部
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
                //不需要设置参数
                return GetDbCommand(conn, sqlText);
            }

            //构建Where
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
            //    throw new ORMException("查询数量应该以SELECT COUNT开头");
            //}
            string fmt = @"^SELECT +COUNT\([^\\]+\)( +AS \w+)? +FROM +\w+";
            Regex reg = new Regex(fmt, RegexOptions.IgnoreCase);//忽略模式中的大小写，即文本大小写都可以
            if (reg.IsMatch(sql.Trim()))
            {
                throw new ORMException("查询数量SQL不正确，正确格式为：SELECT COUNT(T.ID) FROM TABLE ");
            }
            PoolConnection conn = GetConnection();
            IDbCommand cmd = CreateCommandByParams(conn.Connection, sql, paramNames, paramValues);
            //返回第一行第一列
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
                //如果开启了线程就使用
                _DBCommand.Transaction = GetCurrentTx();
            }
            _DBCommand.ExecuteNonQuery();
            //CloseConnection(conn);
        }

        //尚未测试
        public override int ExecuteUpdateBySQLWithParams<T>(string sql, string[] paramNames, object[] paramValues)
        {
            if (string.IsNullOrWhiteSpace(sql) || (!sql.ToUpper().StartsWith("UPDATE") && !sql.ToUpper().StartsWith("DELETE")))
            {
                throw new ORMException("不是有效的更新SQL语句");
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
            //CloseConnection(conn);//在AfterTransaction中会关闭，这里关不关都行
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

        //TODO:这个方法还不健壮，如where flag = 1 and Id>:id，（where条件中有常亮）只有一个变量，但分割后数组有两项，结果会变成flag =:id 且后面数组越界
        private static string GenerateSQLWherePart(string[] paramNames, Type type, string oqlAfterWhere, CmdType cmdType)
        {
            //hql可能写的不规范，如and,AND或And等，这样导致无法分割

            StringBuilder sbWhere = new StringBuilder();
            //列名+符号，如：ID>
            List<string> colInfoList = new List<string>();

            //TODO:分隔符还有OR，如：where Id>:ID and (age > :AGE or height > :H)，这样太复杂了，先不支持
            string[] arr = StringHelper.SplitIgnoreCase(oqlAfterWhere, "and");
            foreach (string item in arr)
            {
                //注意不一定是=，如：Id > :A
                //常用的过滤条件包括下面这些
                string[] temp = item.Split(new string[] { "=", "!=", ">", "<", "like", "Like", "LIKE", "between", "Between", "BETWEEN" }, StringSplitOptions.RemoveEmptyEntries);
                string prop = temp[0].Trim();
                //string val = temp[1].Trim();

                //查询属性对应的字段
                string colName = ORMUtil.GetColumnName(type, prop);
                if (null == colName)
                {
                    throw new ORMException("属性：" + prop + "没有映射字段");
                }
                //默认
                string symb = "=";

                //好麻烦，可能是>,<,!=,Like,= 且其中空格数任意多个,还要考虑大小写，很难准确知道后面的符号
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

        //被UpdateEntity() 调用, 将所有的字段都更新
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
                #region 可以通过while获得，省略
                //Attribute[] attr = Attribute.GetCustomAttributes(pi);

                //if (attr[0] is ColumnAttribute)
                //{
                //    colName = ((ColumnAttribute)attr[0]).ColumnName;
                //    if (string.IsNullOrEmpty(colName))
                //    {
                //        colName = pi.Name.ToUpper();
                //    }
                //}
                //#region 外键字段
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
                //    //OneToManyAttribute, ManyToManyAttribute 外键不在当前表中
                //    continue;
                //} 
                #endregion

                //暂时不管ManyToOne，ManyToMany等
                ColumnAttribute attr = Attribute.GetCustomAttribute(prop, typeof(ColumnAttribute)) as ColumnAttribute;
                if (null == attr || attr is PKAttribute)//主键不需要更新
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
                    //如果属性值为空也有可能，比如修改时将备注，某个时间置空                    
                }

                //如果是值类型没问题
                if (prop.PropertyType.IsValueType)
                {
                    //枚举对应int,不用特殊处理

                }
                else if (typeof(string) != prop.PropertyType)//pi.PropertyType.IsClass)
                {
                    //不是值类型，又不是string
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
            //设置参数
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
                //2016年3月19日 兼容SQLServer的int主键
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
        /// 更新实体对象
        /// </summary>
        /// <param name="obj">应该是包含ID的完整的对象</param>
        /// <returns>更新的行数</returns>
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


        //关于外键有两种方式：
        //1.在类A中有类B的属性如： class People {public Country MyCount{set;get;} } 这就是典型的ManyToOne
        //2.直接在A中有string的外键如： class People{public string CountryId{set;get;} }
        //两种方式都可以，但是推荐使用第2种，简单直接
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
            //是否是自增长主键
            bool bAutoID = false;
            if (bSQLServer && PKGenerateStrategy.UnKnown == tableAttr.PKStaregy)
            {
                //如果是SQLServer则默认ID自增长，可以只写表名 省略后面2个参数 [Table("T_STUDENT", PKGenerateStrategy.AutoIncrease, "")]
                bAutoID = true;
            }
            else
            {
                bAutoID = PKGenerateStrategy.AutoIncrease.Equals(tableAttr.PKStaregy);
            }
            //插入数据时框架要维护ID, 根据主键生成方式
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
                        //TODO:SEQ方式，查询下一个序列
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
                        //判断ID是否有值，如果没有排除异常
                        if (String.IsNullOrEmpty(obj.Id))
                        {
                            throw new QDException(TipConst.NOT_SET_ID_VALUE);
                        }
                    }
                    break;
                case PKGenerateStrategy.AutoIncrease:
                    //SQLServer 自增长主键
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
                #region 方式1,效率慢
                //Attribute[] attrs = Attribute.GetCustomAttributes(pi);

                //#region 属性上可能有多个注解，这种方式可以得到需要的注解
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
                //    //一般直接写成普通的string FKId，而不用这种方式
                //    colName = ((ManyToOneAttribute)attrs[0]).FKColumn;
                //}
                //else if (attrs[0] is OneToOneAttribute)
                //{
                //    colName = ((OneToOneAttribute)attrs[0]).FKColumn;
                //}
                //else
                //{
                //    //OneToManyAttribute, ManyToManyAttribute 外键不在当前表中
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
                //自增长时不设置ID的值
                if (bAutoID && "ID".Equals(colName.ToUpper()))
                {
                    continue;
                }
                object val = pi.GetValue(obj, null);
                if (null == val)
                {
                    //如果属性没有赋值，DBNull
                    //continue;
                }

                //如果是值类型没问题
                if (pi.PropertyType.IsValueType)
                {
                    //枚举不用特殊处理
                }
                //如果将图片等存到数据库则使用byte[]
                else if (typeof(String) != pi.PropertyType && !pi.PropertyType.IsArray)//pi.PropertyType.IsClass)
                {
                    //TODO:如果是ManyToOne，应该得到该属性关联的对象的ID
                    //User{ public TClass MyClass{}}

                    //所以一般不会执行这里
                    continue;
                }

                ++i;
                sql1 += (colName + ",");
                sql2 += (Prefix + "P" + i + ",");

                //同时设置
                pnList.Add("P" + i);
                pvList.Add(QDConvert.ToDBValue(val, pi.PropertyType));
                ptList.Add(pi.PropertyType);
                //Console.WriteLine(pi.Name + "=" + val + ", DBValue=[" + QDConvert.ToDBValue(val, pi.PropertyType) + "], type=" + pi.PropertyType.Name);
            }
            sql1 = sql1.Remove(sql1.Length - 1) + ")";
            sql2 = sql2.Remove(sql2.Length - 1) + ")";

            string sqlText = sql1 + sql2;
            IDbCommand _DBCommand = GetDbCommand(conn, sqlText);
            //设置参数
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


        //不推荐，没实现
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
            //例子：
            //update User set UserName = :U, Password = :P where Id = :ID and xxx=:yy
            //TODO:这里需要专门写一个语法解析器，有难度，先不做这个


            //string sqlText = null;
            //bool bNoParam = false;
            ////where后面的部分
            //string strAfterWhere = null;

            ////如果没有where就是查询全部
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
            //    //不需要设置参数
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
            //TODO:默认主键是ID, 但有可能主键列名称不是ID，这里不够健壮，以后加上查询主键的列名。
            //约定每个类中必须有一个没有业务含义的Id主键

            //如果有参数使用占位符，防止SQL注入
            string sqlText = String.Format("DELETE FROM {0} WHERE ID = " + Prefix + "Id", tableName);
            IDbCommand _DBCommand = GetDbCommand(conn, sqlText);
            //设置参数
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
        /// 关闭当前线程使用的连接
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





        //抽象方法是必须要重写的，如果父类中是虚函数，则可重写也可不重写


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