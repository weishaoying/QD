using QD.ORM.Common;
using QD.ORM.Model;
using System;
using System.Text;
using System.Text.RegularExpressions;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :SQLServerDBManager 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/23 10:54:26 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine
{
    internal class SQLServerDBManager : DBManagerAdapter
    {
        string STR_ORDER_BY = "ORDER BY ";

        private SQLServerDBManager()
        {
        }

        /// <summary>
        /// 外界调用
        /// </summary>
        /// <param name="connStr"></param>
        /// <returns></returns>
        public static DataBaseManager GetInstance(string connStr)
        {
            if (null == _DBManager)
            {
                lock (lockObj)
                {
                    if (null == _DBManager)
                    {
                        _DBManager = new SQLServerDBManager(connStr);
                    }
                }
            }
            return _DBManager;
        }

        public SQLServerDBManager(string connStr)
            : base()
        {
            this.bSQLServer = true;
            this.Prefix = Constant.SQLServerParamPrefix;

            //首先得获得数据库连接，所以先获得连接池实例
            _Pool = QD.ORM.Pool.ConnectionPool.GetInstance(DBType.SQLServer, connStr);

            //Console.WriteLine(pool.ToString());
        }

        public override string GetPageSQL(string sql, int pageSize, int pageNo)
        {
            if (String.IsNullOrEmpty(sql))
            {
                throw new ArgumentNullException(sql);
            }
            //增加正则表达式 : SELECT * from T [where xx] [order by xx]
            //
            Regex reg = new Regex(@"^select +([\w\.\*, ]+) +from +([\w]+)", RegexOptions.IgnoreCase);
            Match match = reg.Match(sql);
            if (!match.Success)
            {
                throw new ORMException(sql + "：不是正确的SQL语句");
            }

            bool bMultTables = sql.ToUpper().Contains("JOIN");
            string queryFields = null;
            string tableName = null;
            string sqlWithoutOrderby = sql;
            string orderBy = null;

            if (!bMultTables)
            {
                //下面这些都是在单表查询时才用
                //重新拼接查询字段防止原SQL中表别名如：t1.A, t2.B会导致错误
                queryFields = match.Groups[1].ToString();
                tableName = match.Groups[2].ToString();
                string[] arr = queryFields.Split(',');
                if (null != arr && arr.Length > 0)
                {
                    string str = null;
                    foreach (string item in arr)
                    {
                        if (!item.Trim().Contains("."))
                        {
                            str += "__T1." + item.Trim() + ",";
                        }
                        else
                        {
                            str += item.Trim() + ",";
                        }
                    }
                    queryFields = " " + str.Remove(str.Length - 1);
                }
            }
            if (sql.ToUpper().Contains(STR_ORDER_BY))
            {
                int p = sql.ToUpper().IndexOf(STR_ORDER_BY);
                orderBy = sql.Substring(p);
                //orderby也需要去除原来的表名信息 order by t1.A DESC, t2.B ASC, t3.C Desc
                orderBy = Regex.Replace(orderBy, @"\w+\.", "__T0.");
                sqlWithoutOrderby = sql.Substring(0, p);
            }
            #region 示例SQL
            /*
             1.如果只有一张表
                SELECT T2.RN, T1.* FROM PERSONS T1,
                (
	                SELECT TOP 20 ROW_NUMBER() OVER (ORDER BY T.USERNAME DESC, ID DESC) RN, ID FROM PERSONS T
                ) T2 WHERE T1.ID = T2.ID AND T2.RN > 10 ORDER BY T2.RN ASC  
             
             2.如果多表关联
                SELECT T2.RN, T1.* FROM ($PERSONS) T1,
                (
	                SELECT TOP 20 ROW_NUMBER() OVER (ORDER BY T.USERNAME DESC, ID DESC) RN, ID FROM ($PERSONS)
                ) T2 WHERE T1.ID = T2.ID AND T2.RN > 10 ORDER BY T2.RN ASC 
             */
            //SQL2005之后 新增ROW_NUMBER()和Oracle中的ROWNUM类似 
            #endregion

            int startRowNum = (pageNo - 1) * pageSize;
            int endRowNum = pageSize * pageNo;

            StringBuilder sb = new StringBuilder();

            if (bMultTables)
            {
                sb.Append("SELECT __T2.RN, __T1.* FROM ");
                sb.Append("(");
                sb.Append(sqlWithoutOrderby);
                sb.Append(")");
            }
            else
            {
                sb.Append("SELECT __T2.RN, ");
                sb.Append(queryFields);
                sb.Append(" FROM ").Append(tableName);
            }
            sb.Append(" __T1,");
            sb.Append(" ( ");
            sb.AppendFormat("SELECT TOP {0} ROW_NUMBER() OVER ( ", endRowNum);
            if (!string.IsNullOrEmpty(orderBy))
            {
                sb.Append(orderBy).Append(",");
            }
            else
            {
                sb.Append(STR_ORDER_BY);
            }
            sb.Append(" ID DESC) RN, ID FROM ");
            if (bMultTables)
            {
                sb.Append("(");
                sb.Append(sqlWithoutOrderby);
                sb.Append(") ");
            }
            else
            {
                sb.Append(tableName);
            }
            sb.Append(" __T0 ");
            sb.Append(" ) ");
            sb.AppendFormat(" __T2 WHERE __T1.ID = __T2.ID AND __T2.RN > {0} ", startRowNum);
            sb.Append(" ORDER BY __T2.RN ASC ");

            return sb.ToString().ToUpper();
        }
    }
}