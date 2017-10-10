using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :IIdentify 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/25 0:28:52 
// 
// Modify Time: 
// Modify Description: 
// 
//---------------------------------------------------------------

namespace QD.Found.Core
{

    public interface IIdentify
    {
        //这里试了很多情况都不行, 最终的解决方案是：定义为String，如果某个表的主键是int，在属性中声明为string
        //public abstract string Id
        //{
        //    set;
        //    get;
        //}
        //public object Id;
        //public virtual string Id;


        /// <summary>
        /// 唯一标示，所有的表都必须有一个ID字段,可能是String也可能是int
        /// </summary>
        string Id
        {
            set;
            get;
        }
    }
}
