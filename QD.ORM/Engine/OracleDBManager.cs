using QD.ORM.Common;
using QD.ORM.Model;
using QD.ORM.Pool;
using System;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :OracleDBManager 
// Function Description: 规定：枚举类型都从1开始，布尔类型都用0,1表示
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/23 10:53:12 
// 
// Modify Time: 
// Modify Description: 约定：每个实体类和对应表都必须有一个主键ID,
//  关于外键有两种方式：
//      1.在类A中有类B的属性如： class People {public Country MyCount{set;get;} } 这就是典型的ManyToOne
//      2.直接在A中有string的外键如： class People{public string CountryId{set;get;} }
//      两种方式都可以，但是推荐使用第2种，简单直接
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine
{
    internal class OracleDBManager : DBManagerAdapter, IDisposable
    {
        private OracleDBManager()
        {
        }

        //被下面调用
        public OracleDBManager(string connStr)
            : base()
        {
            this.bOracle = true;
            this.Prefix = Constant.OracleParamPrefix;

            //首先得获得数据库连接，所以先获得连接池实例
            _Pool = ConnectionPool.GetInstance(DBType.Oracle, connStr);
            //Console.WriteLine(pool.ToString());
        }

        /// <summary>
        /// 外界调用：通过数据库连接字符串来创建并获得实例
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
        /// 获得oracle分页SQL，大写
        /// </summary>
        /// <param name="sql"></param>
        /// <param name="pageSize"></param>
        /// <param name="pageNo"></param>
        /// <returns></returns>
        public override string GetPageSQL(string sql, int pageSize, int pageNo)
        {
            //RowId：每行数据都有一个ROWID, Rownum：行号
            //利用Oracle中的ROWNUM关键字分页，它是从1开始的自然数

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