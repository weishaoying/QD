using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :CacheStrategy 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/22 21:19:46 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.Cache.Model
{
    /// <summary>
    /// 在某个项目中用到的缓存策略应该是固定的，某一个
    /// </summary>
    public enum CacheStrategy
    {
        /// <summary>
        /// 缓存中的数据没有限制，可以无限存放
        /// </summary>
        NoLimit = 1,

        /// <summary>
        /// 达到指定的数量后，前面的缓存数据会被后面放入的替换
        /// </summary>
        Quantity = 2,

        /// <summary>
        /// 
        /// </summary>
        Time = 3,
    }
}