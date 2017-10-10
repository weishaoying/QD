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
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :DBFactory 
// Function Description: ���ݿ����������
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/23 10:30:46 
// 
// Modify Time: 
// Modify Description: Hibernate�д���ʹ��ManyToOne�ȣ������ҽ���ֱ��ʹ��string PkIdָ���������
// ���ڶ���Ӳ�ѯ����2�ַ�ʽ
// 1.��������һ����ѯ������� XXXVO,Ȼ��Ѹ��ӵĲ�ѯд����ͼ�ķ�ʽ�����е����Ժ���ͼ��Ӧ, ���ַ�ʽֱ��������ӳ�䣬���е���Ŀ�鲻�Ƽ�д��ͼ���洢���̵�
// 2.QueryObjectArrayListBySQLWithParams()
//    ����List<object[]>�������ͱ�������ͼ�����ǻ����Լ���ѭ���и�VO�������Ը�ֵ���Ϸ���
// �����ַ�ʽ�����ԣ��Ƽ�ʹ�õ�һ��
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine
{
    public abstract class DataBaseManager : IDisposable
    {

        /// <summary>
        /// SQL����б�����ǰ׺
        /// </summary>
        protected char Prefix = Constant.SQLServerParamPrefix;

        //protected ConnectionFactory _Pool;

        #region DataBaseManagerֻ��һ�����󣬶���߳�ͬʱ���ʣ�DbConnection��DbCommand��DataSet�Ȳ����ǳ�Ա�����������̲߳���ȫ
        //protected IDbConnection _DBConn = null;

        //protected IDbCommand _DBCommand = null;

        //protected IDataReader _DataReader = null;

        //protected DataSet _DataSet = null; 
        #endregion

        //�������
        protected ICache _Cache = null;

        public DataBaseManager()
        {
            //TODO: ��ѯ����ʱ���Ƿ�����ѯ������󣬲�����ʱ�������ֱ�ӱ���Ϊstring��ʽ
            //���
            //_Cache = new HashCache();
        }

        /// <summary>
        /// �����ݿ����ӳ��л��һ������
        /// </summary>
        /// <returns></returns>
        protected abstract PoolConnection GetConnection();

        /// <summary>
        /// �ر�ĳ������
        /// </summary>
        /// <param name="conn"></param>
        protected abstract void CloseConnection(PoolConnection conn);

        /// <summary>
        /// �رյ�ǰ�߳�ʹ�õ�����
        /// </summary>
        protected abstract void CloseConnection();

        /// <summary>
        /// �Ƽ�ʹ���������������������
        /// </summary>
        /// <returns></returns>
        public abstract DbTransaction BeginTransaction();

        /// <summary>
        /// ������������������ݿ�����
        /// </summary>
        /// <param name="conn">���ص�ǰ������</param>
        /// <returns></returns>
        protected abstract DbTransaction BeginTransaction(out PoolConnection conn);

        /// <summary>
        /// �Ƴ��ֵ��е���������ݿ�����
        /// </summary>
        protected abstract void RemoveTxAndConn();

        /// <summary>
        /// ����ĺ�������
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

        #region �����̰߳�ȫ��ԭ�򣬲����ṩ����DbCommand�ȷ���

        //public abstract IDbCommand CreateCommand(string sqlText, IDbConnection conn);

        //public abstract IDataReader GetReader(IDbCommand dbCmd); 

        //public abstract void InitReader();

        //public abstract void InitDataSet();

        #region ����SQL��OQL����󣩹�����SQL����

        //protected abstract IDbCommand CreateSelectCommand<T>(string oql, string[] paramNames, object[] paramValues);

        //protected abstract IDbCommand CreateInsertCommand(IIdentify obj);

        //protected abstract IDbCommand CreateUpdateCommand<T>(string oql, string[] paramNames, object[] paramValues);

        //protected abstract IDbCommand CreateDeleteCommand(IIdentify obj);

        //protected abstract IDbCommand CreateDeleteCommand<T>(object id);

        #endregion
        #endregion


        /// <summary>
        /// ͨ��SQL�Ͳ��������ʼ�����֧�ָ��ֲ�ѯ�����룬ɾ�����޸ĵ�����ڲ�ʹ��
        /// ����Ҫ�Ļ�������
        /// </summary>
        /// <param name="sqlText"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        protected abstract IDbCommand CreateCommandByParams(IDbConnection dbConn, string sqlText, string[] paramNames, object[] paramValues);

        /// <summary>
        /// ͨ����ѯ�����ý����
        /// </summary>
        /// <param name="dbCmd"></param>
        /// <returns></returns>
        public abstract DataSet QueryDataSetByCommand(IDbCommand dbCmd);

        /// <summary>
        /// ͨ��SQL�Ͳ�ѯ����������ݼ��������ṩ�����
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public abstract DataSet QueryDataSetBySQLWithParams(string sql, string[] paramNames, object[] paramValues);
        #region MyRegion
        //{
        //ִ��ʱ���õ�������ķ���
        //PoolConnection conn = GetConnection();
        //IDbCommand cmd = CreateCommandWithParams(sql, paramNames, paramValues);
        //DataSet dataSet = QueryDataSetByCommand(cmd);
        ////����֮�������ͷ�
        ////CloseConnection(conn);
        //return dataSet;
        //} 
        #endregion

        /// <summary>
        /// ͨ������������SQL������ݼ�
        /// </summary>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual DataSet QueryDataSetBySQL(string sql)
        {
            return QueryDataSetBySQLWithParams(sql, null, null);
        }


        public abstract string GetPageSQL(string sql, int pageSize, int pageNo);

        
        //��չ����

        #region �б��ѯBySQL

        /// <summary>
        /// SQL��ѯ,�޲���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <returns></returns>
        public virtual List<T> QueryListBySQL<T>(string sql)
        {
            return QueryListBySQLWithParams<T>(sql, null, null);
        }

        /// <summary>
        /// SQL��ѯ��һ������
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
        /// ����SQL��ѯ����������������SQL����д�Likeģ����ѯ,���ܼ�'',���˷�����ѯ�������,
        /// ������β��Է��֣�ռλ���ķ�ʽ���Է�ֹSQLע�룬��ֵ������OR 1=1��OR 2>1�ȶ���Ч�����ǰ�����������Ϊһ���ַ���������ƴ��SQL�����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public virtual List<T> QueryListBySQLWithParams<T>(string sql, string[] paramNames, object[] paramValues)
        {
            //����������麯��������ʱ��
            DataSet dataSet = QueryDataSetBySQLWithParams(sql, paramNames, paramValues);
            return MappingEngine<T>.Mapping(dataSet);
        }

        #endregion

        #region ����List<object[]>,����ӳ��

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
            //����ӳ�䣬ֱ�ӽ����Զ�������ķ�ʽ����
            if (null == dataSet || null == dataSet.Tables[0])
            {
                return null;
            }
            List<object[]> dataList = new List<object[]>();
            DataTable dt = dataSet.Tables[0];
            int cnt = dt.Columns.Count;//�еĸ���

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


        #region �����б��ѯ���������ṩ��ѯ���еķ���

        /// <summary>
        /// ��ѯĳ��������ж����б��ʺ���������С��û�з�ҳʱ
        /// �����б��ѯ����������
        /// �����봫��һ��ģ�����,���ն�������Ժ�ֵ����ѯ,�������ַ�ʽ���ѱ��ģ����ѯ,�����ڣ����Է������ַ�ʽ��ֻ������һ����������������Ҫ����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <returns></returns>
        public virtual List<T> QueryAllByType<T>()
        {
            string sql = ORMUtil.GenerateSelectAllSQL(typeof(T));
            return QueryListBySQLWithParams<T>(sql, null, null);
        }

        #region ���ַ�ʽ�����ã��������ṩ��

        /// <summary>
        /// �����б��ѯ����һ������
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
        /// ���ַ�ʽ�����ã��������ṩ�������ѯ�����������
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj"></param>
        /// <param name="paramNames">��Щ������Ϊ����</param>
        /// <param name="paramValues">����ֵ</param>
        /// <returns></returns>
        private List<T> QueryListByObjectWithParams<T>(T obj, string[] paramNames, object[] paramValues)
        {
            //��Obj->SQL
            string sql = ORMUtil.GenerateSelectAllSQL(typeof(T));
            return QueryListBySQLWithParams<T>(sql, paramNames, paramValues);
        }
        #endregion

        #endregion


        #region OQL�б��ѯ

        /// <summary>
        /// ��ʱδʵ�֣�OQL�������ѯ������HQL��䣬��from User where UserName = :U and Password = :P
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oql"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        /// <returns></returns>
        public abstract List<T> QueryListByOQLWithParams<T>(string oql, string[] paramNames, object[] paramValues);

        /// <summary>
        /// ��ʱδʵ�֣�OQL��ѯ����һ������
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
        /// ��ʱδʵ�֣�OQL��ѯ���޲���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="oql"></param>
        /// <returns></returns>
        public virtual List<T> QueryListByOQL<T>(string oql)
        {
            return QueryListByOQLWithParams<T>(oql, null, null);
        }

        #endregion

        #region ��ҳ��ѯ

        /// <summary>
        /// SQL��ҳ��ѯ���޲���
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="pageSize">ÿҳ����</param>
        /// <param name="pageNo">��ѯ�ڼ�ҳ</param>
        /// <returns></returns>
        public virtual List<T> PageQuery<T>(string sql, int pageSize, int pageNo)
        {
            return PageQueryWithParams<T>(sql, null, null, pageSize, pageNo);
        }

        /// <summary>
        /// SQL��ҳ��ѯ����1������
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
        /// SQL��ҳ��ѯ�����������
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

        #region ͨ��SQL��ѯ��������ҳ���

        public abstract int CountBySQLWithParams(string sql, string[] paramNames, object[] paramValues);
        #region MyRegion
        //{
        //    if (string.IsNullOrWhiteSpace(sql))
        //    {
        //        throw new ArgumentNullException("sql");
        //    }
        //    if (!sql.ToUpper().TrimStart().StartsWith("SELECT COUNT"))
        //    {
        //        throw new ORMException("��ѯ����Ӧ����SELECT COUNT��ͷ");
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


        #region ͨ��ID��ѯ��������
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


        #region �����ͷ���

        #region ����ֻ��һ��

        public abstract void SaveEntity(IIdentify obj);

        #endregion

        #region ���¶���ͨ��SQL�������

        /// <summary>
        /// �Ƽ�ʹ��
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>���µ�����</returns>
        public abstract int UpdateEntity(IIdentify obj);


        /// <summary>
        /// �������ã���δ��������
        /// ͨ��SQL���ִ��Update, Delete����
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="sql"></param>
        /// <param name="paramNames"></param>
        /// <param name="paramValues"></param>
        public abstract int ExecuteUpdateBySQLWithParams<T>(string sql, string[] paramNames, object[] paramValues);


        #region UpdateByOQL �������û��ʵ�� Update User Set RealName = :RN Where Id = :ID
        //TODO:
        //private void UpdateByOQLWithParams<T>(string oql, string[] paramNames, object[] paramValues)
        //{
        //    //GetConnection();
        //    //CreateUpdateCommand<T>(oql, paramNames, paramValues);
        //    //this._DBCommand.ExecuteNonQuery();
        //} 
        #endregion

        #endregion

        #region ɾ������ͨ������ID����ExecuteUpdate��3�ַ�ʽ�������õľ�����������

        //������󷽷������������о���ʵ��
        public abstract void DeleteEntityById<T>(string Id);


        public abstract void DeleteEntity(IIdentify obj);

        #endregion

        #endregion


        #region �رգ��ͷţ���Щ����������Ҫ�ṩ����磬�������������д��������Ϊ��������

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

        //û�г�Ա�������Բ������ַ�ʽ
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