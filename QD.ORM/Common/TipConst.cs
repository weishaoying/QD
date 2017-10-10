using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :TipConst 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/24 10:42:02 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Common
{
    public class TipConst
    {
        public const string ClassMustInheritID = "类必须继承自IIdentify";
        public const string TableNameIsEmpty = "表名为空";

        public const string TableDontHaveColumn = "表中不包含字段：";

        public const string ChangeTypeError = "类型转换异常：";

        public const string CloseConnError = "关闭连接时发生异常";

        public const string MoreThanMaxConnectionCnt = "超过了最大连接数";


        public const string ConnStrEmpty = "数据库连接字符串为空，无法创建连接";

        public const string DBConnectionIsNull = "数据库连接Connection为空";

        public const string DBCommandIsNull = "数据库执行命令DBCommand为空";

        public const string NotOracleConnection = "当前不是Oracle连接";

        public const string NotOracleCommand = "当前不是OracleCommand";

        public const string NotSqlCommand = "当前不是SqlCommand";

        public const string PoolInfo = "总连接数：{0}, 可用连接数：{1}";

        public const string ReuseOnePoolConn = "释放一个连接，编号=";

        public const string PoolInitFinished = "-------连接池初始化完成：";

        public const string GetConnFromPool = "从连接池获取连接：";

        public const string PoolConnIsNull = "连接为空，无需关闭";

        public const string ReusePoolConnFailed = "连接已经关闭，无需再次关闭";

        public const string FreeConnCntIsNotEnough = "可用连接数量不足，创建新连接数量=";

        public const string AfterCreateNewConn = "创建新连接后：";




        public const string DBTypeAndConnStrNotInit = "DBType和ConnStr没有初始化";

        public const string NOT_SET_SEQNAME = "没有指定Sequence名称";

        public const string NOT_SET_ID_VALUE = "没有设置主键ID的值";





    }
}