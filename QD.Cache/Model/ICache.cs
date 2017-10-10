using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :ICache 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/22 20:47:03 
// 
// Modify Time: 
// Modify Description: 
// 
//---------------------------------------------------------------

namespace QD.Cache.Model
{
    public interface ICache
    {
        /// <summary>
        ///  下面的key都没有使用string，而是使用object类型，既可以放入单个对象，也可以放入集合对象
        /// </summary>
        /// <param name="key"></param>
        /// <param name="data"></param>
        void PutData(object key, object data);

        object GetData(object key);

        void RemoveData(object key);

        void ClearAll();

        void Destroy();
    }
}
