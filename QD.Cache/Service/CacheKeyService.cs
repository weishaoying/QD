using System;
using System.Collections.Generic;
using System.Text;
using QD.Found.Core;
//---------------------------------------------------------------- 
// Copyright (C)Œ∫…ÿ”± 2014-2015, All rights reserved. 
// 
// File Name :CacheKeyService 
// Function Description: 
// Version: 1.0.1 
// 
// Author£∫WeiShaoying 
// Mail:   15910708769@163.com
// Create Time£∫2015/2/25 0:24:56 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.Cache.Service
{
    public class CacheKeyService : ICacheKeyService
    {
        public const char Symbol = '#';

        public string GenerateKey(IIdentify obj)
        {
            return GenerateKey(obj.GetType(), obj.Id.ToString());
        }

        public string GenerateKey(Type type, string Id)
        {
            return type.AssemblyQualifiedName + Symbol + Id;
        }




    }
}