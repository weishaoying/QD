using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :PKStrategy 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/23 14:27:52 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Model
{
    /// <summary>
    /// 主键的生成策略
    /// </summary>
    public enum PKGenerateStrategy
    {
        UnKnown = 0,

        GUID = 1,

        SEQ = 2,

        /// <summary>
        /// 自定义主键，通过程序维护
        /// </summary>
        Assign = 3,

        /// <summary>
        /// 自动增长
        /// </summary>
        AutoIncrease = 4
    }
}