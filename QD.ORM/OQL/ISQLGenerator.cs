using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :ISQLGenerator 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/25 0:50:45 
// 
// Modify Time: 
// Modify Description: 
// 
//---------------------------------------------------------------

namespace QD.ORM.OQL
{
    public interface ISQLGenerator
    {
        //建造者模式还是策略模式

        //protected string QuerySQL = null;

        //从HQL->SQL,或者是从查询对象->SQL

        string GetQueryListSQL();

        string GetQueryPageSQL();

        string GetSaveSQL();

        string GetUpdateSQL();

        string GetDeleteSQL();

    }
}
