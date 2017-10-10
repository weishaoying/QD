using System;
using System.Collections.Generic;
using System.Data;
using QD.Cache.Model;
using QD.ORM.Engine.Core;
using QD.ORM.Engine.SQL;
using QD.Found.Core;
using QD.ORM.Common;
using QD.ORM.Pool;
using QD.ORM.Model;
using System.Data.Common;
using System.Collections;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :DBFactory 
// Function Description: 数据库操作管理器
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/23 10:30:46 
// 
// Modify Time: 
// Modify Description: Hibernate中大量使用ManyToOne等，但是我建议直接使用string PkId指向外键对象
// 对于多表复杂查询，有2种方式
// 1.可以增加一个查询结果类如 XXXVO,然后把复杂的查询写成视图的方式，类中的属性和视图对应, 这种方式直接做好了映射，但有的项目组不推荐写视图，存储过程等
// 2.QueryObjectArrayListBySQLWithParams()
//    返回List<object[]>，这样就避免了视图，但是还得自己在循环中给VO各个属性赋值，较繁琐
// 这两种方式都可以，推荐使用第一种
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine
{
    public abstract class DataBaseManager : IDisposable
    {

        /// <summary>
        /// SQL语句中变量名前缀
        /// </summary>
        protected char Prefix = Constant.SQLServerParamPrefix;

        //protected ConnectionFactory _Pool;

        #region DataBaseManager只有一个对象，多个线程同时访问，DbConnection、DbCommand、DataSet等不能是成员变量，否则线程不安全
        //protected IDbConnection _DBConn = null;

        //protected IDbCommand _DBCommand = null;

        //protected IDataReader _DataReader = null;

        //protected DataSet _DataSet = null; 
        #endregion

        //缓存对象
        protected ICache _Cache = null;

        public DataBaseManager()
        {
            //TODO: 查询对象时，是否级联查询外键对象，不过暂时按照外键直接保存为string方式
            //如果
            //_Cache = new HashCache();
        }

        /// <summary>
        /// 从数据库连接池中获得一个连接
        /// </summary>
        /// <returns></returns>
        protected abstract PoolConnection GetConnection();

        /// <summary>
        /// 关闭某个连接
        /// </summary>
        /// <param name="conn"></param>
        protected abstract void CloseConnection(PoolConnection conn);

        /// <summary>
        /// 关闭当前线程使用的连接
        /// </summary>
        protected abstract void CloseConnection();

        /// <summary>
        /// 推荐使用这个，开启并返回事务
        /// </summary>
        /// <returns></returns>
        public abstract DbTransaction BeginTransaction();

        /// <summary>
        /// 开启并返回事务和数据库连接
        /// </summary>
        /// <param name="conn">返回当前的连接</param>
        /// <returns></returns>
        protected abstract DbTransaction BeginTransaction(out PoolConnection conn);

        /// <summary>
        /// 移除字典中的事务和数据库连接
        /// </summary>
        protected abstract void RemoveTxAndConn();

        /// <summary>
        /// 事务的后续处理
        /// </summary>
        public void AfterTransaction()
        {
            RemoveTxAndConn();
            CloseConnection();
        }

        protected void AfterTransaction(PoolConnection conn)
        {
            RemoveTxAndConn();
            CloseConnection(conn);
        }

        #region 基于线程安全的原因，不再提供创建DbCommand等方法

        //public abstract IDbCommand CreateCommand(string sqlText, IDbConnection conn);

        //public abstract IDataReader GetReader(IDbCommand dbCmd); 

        //public abstract void InitReader();

        //public abstract void InitDataSet();

        #region 将非SQL（OQL或对象）构建成SQL命令

        //protected abstract IDbCommand CreateSelectCommand<T>(string oql, string[] paramNames, object[] paramValues);

        //protected abstract IDbCommand CreateInsertCommand(IIdentify obj);

        //protected abstract IDbCommand CreateUpdateCommand<T>(string oql, string[] paramNames, object[] paramValues);

        //protected abstract IDbCommand CreateDeleteCommand(IIdentify obj);

        //protected abstract IDbCommand CreateDeleteCommand<T>(object id);

        #endregion
        #endregion


        /// <summary>
        /// 通过SQL和参数数组初始化命令，支持各种查询，插入，删除，修改等命令，内部使用
        /// 很重要的基础方法
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        protected abstract IDbCommand CreateCommandByParams(IDbConnection dbConn, string sqlText, string[] paramNames, object[] paramValues);

        /// <summary>
        /// 通过查询命令获得结果集
        /// </summary>
        /// <param name="dbCmd"></param>
        /// <returns></returns>
        public abstract DataSet QueryDataSetByCommand(IDbCommand dbCmd);

        /// <summary>
        /// 通过SQL和查询参数获得数据集，可以提供给外界
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public abstract DataSet QueryDataSetBySQLWithParams(string sql, string[] paramNames, object[] paramValues);
        #region MyRegion
        //{
        //执行时调用的是子类的方法
        //PoolConnection conn = GetConnection();
        //IDbCommand cmd = CreateCommandWithParams(sql, paramNames, paramValues);
        //DataSet dataSet = QueryDataSetByCommand(cmd);
        ////用完之后马上释放
        ////CloseConnection(conn);
        //return dataSet;
        //} 
        #endregion

        /// <summary>
        /// 通过不带参数的SQL获得数据集
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual DataSet QueryDataSetBySQL(string sql)
        {
            return QueryDataSetBySQLWithParams(sql, null, null);
        }


        public abstract string GetPageSQL(string sql, int pageSize, int pageNo);

        
        //扩展方法

        #region 列表查询BySQL

        /// <summary>
        /// SQL查询,无参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual List<T> QueryListBySQL<T>(string sql)
        {
            return QueryListBySQLWithParams<T>(sql, null, null);
        }

        /// <summary>
        /// SQL查询带一个参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public virtual List<T> QueryListBySQLWithParam<T>(string sql, string paramName, object paramValue)
        {
            return this.QueryListBySQLWithParams<T>(sql,
                new string[] { paramName },
                new object[] { paramValue });
        }

        /// <summary>
        /// 复杂SQL查询，带多个参数，如果SQL语句中带Like模糊查询,则不能加'',加了反而查询不到结果,
        /// 经过多次测试发现：占位符的方式可以防止SQL注入，如值中输入OR 1=1，OR 2>1等都无效，它是把整个输入作为一个字符串，不会拼到SQL语句中
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public virtual List<T> QueryListBySQLWithParams<T>(string sql, string[] paramNames, object[] paramValues)
        {
            //这个方法是虚函数，运行时绑定
            DataSet dataSet = QueryDataSetBySQLWithParams(sql, paramNames, paramValues);
            return MappingEngine<T>.Mapping(dataSet);
        }

        #endregion

        #region 返回List<object[]>,不做映射

        public virtual List<object[]> QueryObjectArrayListBySQL(string sql)
        {
            return QueryObjectArrayListBySQLWithParams(sql, null, null);
        }

        public virtual List<object[]> QueryObjectArrayListBySQLWithParam(string sql, string paramName, object paramValue)
        {
            return QueryObjectArrayListBySQLWithParams(sql,
                new string[] { paramName },
                new object[] { paramValue });
        }


        public virtual List<object[]> QueryObjectArrayListBySQLWithParams(string sql, string[] paramNames, object[] paramValues)
        {
            DataSet dataSet = QueryDataSetBySQLWithParams(sql, paramNames, paramValues);
            //不做映射，直接将其以对象数组的方式返回
            if (null == dataSet || null == dataSet.Tables[0])
            {
                return null;
            }
            List<object[]> dataList = new List<object[]>();
            DataTable dt = dataSet.Tables[0];
            int cnt = dt.Columns.Count;//列的个数

            for (int i = 0; i < dt.Rows.Count; i++)
            {
                object[] objs = new object[cnt];
                for (int j = 0; j < cnt; j++)
                {
                    objs[j] = dt.Rows[i].ItemArray[j];
                }
                dataList.Add(objs);
            }
            return dataList;
        }

        #endregion


        #region 对象列表查询，仅对外提供查询所有的方法

        /// <summary>
        /// 查询某个类的所有对象列表，适合数据量较小且没有分页时
        /// 对象列表查询，不带参数
        /// 本来想传递一个模板对象,按照对象的属性和值来查询,但是这种方式很难表达模糊查询,不等于，所以放弃这种方式，只保留着一个方法，且它不需要参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual List<T> QueryAllByType<T>()
        {
            string sql = ORMUtil.GenerateSelectAllSQL(typeof(T));
            return QueryListBySQLWithParams<T>(sql, null, null);
        }

        #region 这种方式不好用，不对外提供了

        /// <summary>
        /// 对象列表查询，带一个参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        private List<T> QueryListByObjectWithParam<T>(T obj, string paramName, object paramValue)
        {
            return QueryListByObjectWithParams(obj, new string[] { paramName }, new object[] { paramValue });
        }

        /// <summary>
        /// 这种方式不好用，不对外提供，对象查询，带多个参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="paramNames">哪些参数作为条件</param>
        /// <param name="paramValues">参数值</param>
        /// <returns></returns>
        private List<T> QueryListByObjectWithParams<T>(T obj, string[] paramNames, object[] paramValues)
        {
            //将Obj->SQL
            string sql = ORMUtil.GenerateSelectAllSQL(typeof(T));
            return QueryListBySQLWithParams<T>(sql, paramNames, paramValues);
        }
        #endregion

        #endregion


        #region OQL列表查询

        /// <summary>
        /// 暂时未实现，OQL多参数查询，类似HQL语句，如from User where UserName = :U and Password = :P
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oql"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public abstract List<T> QueryListByOQLWithParams<T>(string oql, string[] paramNames, object[] paramValues);

        /// <summary>
        /// 暂时未实现，OQL查询，带一个参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oql"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <returns></returns>
        public virtual List<T> QueryListByOQLWithParam<T>(string oql, string paramName, object paramValue)
        {
            return QueryListByOQLWithParams<T>(oql, new string[] { paramName }, new object[] { paramValue });
        }

        /// <summary>
        /// 暂时未实现，OQL查询，无参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oql"></param>
        /// <returns></returns>
        public virtual List<T> QueryListByOQL<T>(string oql)
        {
            return QueryListByOQLWithParams<T>(oql, null, null);
        }

        #endregion

        #region 分页查询

        /// <summary>
        /// SQL分页查询，无参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pageSize">每页条数</param>
        /// <param name="pageNo">查询第几页</param>
        /// <returns></returns>
        public virtual List<T> PageQuery<T>(string sql, int pageSize, int pageNo)
        {
            return PageQueryWithParams<T>(sql, null, null, pageSize, pageNo);
        }

        /// <summary>
        /// SQL分页查询，带1个参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramName"></param>
        /// <param name="paramValue"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        public virtual List<T> PageQueryWithParam<T>(string sql, string paramName, object paramValue, int pageSize, int pageNo)
        {
            return PageQueryWithParams<T>(sql, new string[] { paramName }, new object[] { paramValue }, pageSize, pageNo);
        }

        /// <summary>
        /// SQL分页查询，带多个参数
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        public virtual List<T> PageQueryWithParams<T>(string sql, string[] paramNames, object[] paramValues, int pageSize, int pageNo)
        {
            string pageSQL = GetPageSQL(sql, pageSize, pageNo);
            return QueryListBySQLWithParams<T>(pageSQL, paramNames, paramValues);
        }

        #endregion

        #region 通过SQL查询条数，分页相关

        public abstract int CountBySQLWithParams(string sql, string[] paramNames, object[] paramValues);
        #region MyRegion
        //{
        //    if (string.IsNullOrWhiteSpace(sql))
        //    {
        //        throw new ArgumentNullException("sql");
        //    }
        //    if (!sql.ToUpper().TrimStart().StartsWith("SELECT COUNT"))
        //    {
        //        throw new ORMException("查询数量应该以SELECT COUNT开头");
        //    }
        //    GetConnection();
        //    CreateCommandWithParams(sql, paramNames, paramValues);
        //    object obj = this._DBCommand.ExecuteScalar();
        //    if (obj == DBNull.Value)
        //    {
        //        return 0;
        //    }
        //    int count;
        //    if (int.TryParse(obj.ToString(), out count))
        //    {
        //        return count;
        //    }
        //    return 0;
        //} 
        #endregion

        public virtual int CountBySQLWithParam(string sql, string paramName, object paramValue)
        {
            return CountBySQLWithParams(sql, new string[] { paramName }, new object[] { paramValue });
        }

        public virtual int CountBySQL(string sql)
        {
            return CountBySQLWithParams(sql, null, null);
        }

        #endregion


        #region 通过ID查询单个对象
        public T LoadById<T>(string Id)
        {
            string sql = ORMUtil.GenerateLoadUniqueSQL<T>(Prefix);
            List<T> list = QueryListBySQLWithParam<T>(sql, Constant.ID, Id);
            if (null == list)
            {
                return default(T);
            }
            return list[0];
        }
        #endregion


        #region 事务型方法

        #region 保存只有一个

        public abstract void SaveEntity(IIdentify obj);

        #endregion

        #region 更新对象，通过SQL语句或对象

        /// <summary>
        /// 推荐使用
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>更新的行数</returns>
        public abstract int UpdateEntity(IIdentify obj);


        /// <summary>
        /// 基本可用，但未完整测试
        /// 通过SQL语句执行Update, Delete操作
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        public abstract int ExecuteUpdateBySQLWithParams<T>(string sql, string[] paramNames, object[] paramValues);


        #region UpdateByOQL 这个方法没有实现 Update User Set RealName = :RN Where Id = :ID
        //TODO:
        //private void UpdateByOQLWithParams<T>(string oql, string[] paramNames, object[] paramValues)
        //{
        //    //GetConnection();
        //    //CreateUpdateCommand<T>(oql, paramNames, paramValues);
        //    //this._DBCommand.ExecuteNonQuery();
        //} 
        #endregion

        #endregion

        #region 删除可以通过对象、ID，和ExecuteUpdate共3种方式，但常用的就是下面两种

        //定义抽象方法，放在子类中具体实现
        public abstract void DeleteEntityById<T>(string Id);


        public abstract void DeleteEntity(IIdentify obj);

        #endregion

        #endregion


        #region 关闭，释放，这些方法都不需要提供给外界，但是子类可以重写所以声明为保护类型

        //
        //protected virtual void CloseConnection(PoolConnection conn)
        //{
        //    //if (null != conn && ConnectionState.Closed != conn.Connection.State)
        //    //{
        //    //    try
        //    //    {
        //    //    }
        //    //    catch (Exception ex)
        //    //    {
        //    //        throw;
        //    //    }
        //    //    conn.Close();
        //    //    conn.Dispose();
        //    //    conn = null;
        //    //}
        //}

        protected virtual void CloseCommand(IDbCommand cmd)
        {
            if (null != cmd)
            {
                cmd.Dispose();
                cmd = null;
            }
        }

        //没有成员变量所以不用这种方式
        //public virtual void CloseReader()
        //{
        //    CloseReader(this._DataReader);
        //}

        protected virtual void CloseReader(IDataReader reader)
        {
            if (null != reader)
            {
                reader.Close();
                reader.Dispose();
                reader = null;
            }
        }

        //public virtual void CloseDataSet()
        //{
        //    this.CloseDataSet(this._DataSet);
        //}

        protected virtual void CloseDataSet(DataSet dataSet)
        {
            if (null != dataSet)
            {
                dataSet.Clear();
                dataSet.Dispose();
                dataSet = null;
            }
        }

        public void Dispose()
        {
            //CloseDataSet(this._DataSet);
            //CloseReader(this._DataReader);
            //CloseCommand(this._DBCommand);
            //CloseConnection();
        }

        public void Destroy()
        {
            this.Dispose();
        }

        #endregion

    }
}