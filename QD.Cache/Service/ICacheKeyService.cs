using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :ICacheKeyService 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/25 0:22:36 
// 
// Modify Time: 
// Modify Description: 
// 
//---------------------------------------------------------------

namespace QD.Cache.Service
{
    public interface ICacheKeyService
    {
        /// <summary>
        /// 生成缓存Key
        /// </summary>
        /// <param name="obj">必须要实现Id接口</param>
        /// <returns></returns>
        string GenerateKey(QD.Found.Core.IIdentify obj);

        string GenerateKey(Type type, string Id);


    }
}
