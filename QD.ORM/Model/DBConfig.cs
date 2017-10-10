using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :DBConfig 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/23 11:42:08 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Model
{
    public class DBConfig
    {
        internal const string DefaultDBConnString = "Data Source=RRMESCS; User Id=TEST_DEV; Password=abc123;";

        internal const DBType DefaultDBType = DBType.Oracle;


        public string DataSource
        {
            set;
            get;
        }

        public string Username
        {
            set;
            get;
        }

        public string Password
        {
            set;
            get;
        }

        /// <summary>
        /// 数据库连接的完整字符串
        /// </summary>
        public string DBConnStr
        {
            set;
            get;
        }
    }
}