using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :IConnectionPool 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/27 23:46:34 
// 
// Modify Time: 
// Modify Description: 
// 
//---------------------------------------------------------------

namespace QD.ORM.Pool
{
    /// <summary>
    /// 连接池接口
    /// </summary>
    interface IConnectionPool
    {
        /// <summary>
        /// 从连接池中获取一个连接
        /// </summary>
        /// <returns></returns>
        PoolConnection GetConnection();

        /// <summary>
        /// 关闭某个连接
        /// </summary>
        /// <param name="poolConn"></param>
        void CloseConnection(PoolConnection poolConn);

        /// <summary>
        /// 关闭某个连接
        /// </summary>
        /// <param name="SN">连接的编号</param>
        void CloseConnection(int SN);

        /// <summary>
        /// 销毁连接池
        /// </summary>
        void Destroy();

    }
}
