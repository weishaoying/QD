using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :HashCache 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/22 21:05:10 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.Cache.Model
{
    /// <summary>
    /// 缓存应该分为一级缓存(用户Session级)和二级缓存(应用程序共享)，
    /// 所以这里不能用单例模式
    /// </summary>
    public class HashCache : ICache
    {
        //内部通过HashTable来存储数据
        private IDictionary dataDict = new Hashtable();


        public void PutData(object key, object data)
        {
            dataDict[key] = data;
        }

        public object GetData(object key)
        {
            return dataDict[key];
        }

        public void RemoveData(object key)
        {
            dataDict.Remove(key);
        }

        public void ClearAll()
        {
            dataDict.Clear();
        }

        public void Destroy()
        {

        }

        //缓存策略：
        //1.达到一定的数量
    }
}