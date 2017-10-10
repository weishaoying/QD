using System;
using System.Data.Common;
using System.Data;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :PoolConnection 
// Function Description: 数据库池连接对象
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/25 10:18:04 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Pool
{
    public sealed class PoolConnection// : ICloneable
    {

        /// <summary>
        /// 数据库连接
        /// </summary>
        public IDbConnection Connection
        {
            set;
            get;
        }

        /// <summary>
        /// 连接编号，整数
        /// </summary>
        public int SN
        {
            set;
            get;
        }

        /// <summary>
        /// 最后活动时间
        /// </summary>
        public DateTime ActiveTime
        {
            set;
            get;
        }

        /// <summary>
        /// 是否正在使用
        /// </summary>
        public bool IsUsed
        {
            set;
            get;
        }

        //public object Clone()
        //{
        //    return this.MemberwiseClone();
        //    //return new PoolConnection()
        //    //{
        //    //    Connection = this.Connection,
        //    //    SN = this.SN,
        //    //    ActiveTime = DateTime.Now,
        //    //    IsUsed = false,
        //    //};
        //}
    }
}