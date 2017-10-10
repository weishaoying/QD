using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :CacheStrategy 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/22 21:19:46 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.Cache.Model
{
    /// <summary>
    /// ��ĳ����Ŀ���õ��Ļ������Ӧ���ǹ̶��ģ�ĳһ��
    /// </summary>
    public enum CacheStrategy
    {
        /// <summary>
        /// �����е�����û�����ƣ��������޴��
        /// </summary>
        NoLimit = 1,

        /// <summary>
        /// �ﵽָ����������ǰ��Ļ������ݻᱻ���������滻
        /// </summary>
        Quantity = 2,

        /// <summary>
        /// 
        /// </summary>
        Time = 3,
    }
}