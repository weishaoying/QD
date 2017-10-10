using QD.ORM.Common;
using QD.ORM.Model;
using QD.ORM.Pool;
using System;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :OracleDBManager 
// Function Description: �涨��ö�����Ͷ���1��ʼ���������Ͷ���0,1��ʾ
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/23 10:53:12 
// 
// Modify Time: 
// Modify Description: Լ����ÿ��ʵ����Ͷ�Ӧ��������һ������ID,
//  ������������ַ�ʽ��
//      1.����A������B�������磺 class People {public Country MyCount{set;get;} } ����ǵ��͵�ManyToOne
//      2.ֱ����A����string������磺 class People{public string CountryId{set;get;} }
//      ���ַ�ʽ�����ԣ������Ƽ�ʹ�õ�2�֣���ֱ��
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine
{
    internal class OracleDBManager : DBManagerAdapter, IDisposable
    {
        private OracleDBManager()
        {
        }

        //���������
        public OracleDBManager(string connStr)
            : base()
        {
            this.bOracle = true;
            this.Prefix = Constant.OracleParamPrefix;

            //���ȵû�����ݿ����ӣ������Ȼ�����ӳ�ʵ��
            _Pool = ConnectionPool.GetInstance(DBType.Oracle, connStr);
            //Console.WriteLine(pool.ToString());
        }

        /// <summary>
        /// �����ã�ͨ�����ݿ������ַ��������������ʵ��
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static DBManagerAdapter GetInstance(string connStr)
        {
            if (null == _DBManager)
            {
                lock (lockObj)
                {
                    if (null == _DBManager)
                    {
                        _DBManager = new OracleDBManager(connStr);
                    }
                }
            }
            return _DBManager;
        }

        /// <summary>
        /// ���oracle��ҳSQL����д
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        public override string GetPageSQL(string sql, int pageSize, int pageNo)
        {
            //RowId��ÿ�����ݶ���һ��ROWID, Rownum���к�
            //����Oracle�е�ROWNUM�ؼ��ַ�ҳ�����Ǵ�1��ʼ����Ȼ��

            int startRowNum = (pageNo - 1) * pageSize;
            int endRowNum = pageSize * pageNo;

            StringBuilder sb = new StringBuilder();
            sb.Append("SELECT * FROM (");
            sb.Append("     SELECT T_QDFX_1.*, ROWNUM RN FROM (");
            sb.Append(sql);
            sb.AppendFormat(" ) T_QDFX_1 WHERE ROWNUM <= {0} ) ", endRowNum);
            sb.AppendFormat("WHERE RN > {0}", startRowNum);
            return sb.ToString().ToUpper();
        }

    }
}