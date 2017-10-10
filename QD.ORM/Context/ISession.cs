using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)Œ∫…ÿ”± 2014-2015, All rights reserved. 
// 
// File Name :IDBSession 
// Function Description: 
// Version: 1.0.1 
// 
// Author£∫WeiShaoying 
// Mail:   15910708769@163.com
// Create Time£∫2015/2/23 21:39:05 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Context
{
    public abstract class ISession<T>
    {
        public abstract void Save();

        public abstract void Delete();

        public abstract bool ExecuteUpdate();

        public abstract IList<T> DoQuery();

        public abstract IList<T> DoPage(int pageSize, int pageNo, out int totalCnt);
    }
}