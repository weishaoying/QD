using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :IDriver 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/17 
// 
// Modify Time: 
// Modify Description: 
// 
//---------------------------------------------------------------

namespace QD.ORM.Driver
{
    interface IDriver
    {
        IDbConnection CreateConnection();

        //暂时未使用

    }
}
