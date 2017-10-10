using System;
using System.Data.Common;
using System.Data;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :PoolConnection 
// Function Description: ���ݿ�����Ӷ���
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/25 10:18:04 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Pool
{
    public sealed class PoolConnection// : ICloneable
    {

        /// <summary>
        /// ���ݿ�����
        /// </summary>
        public IDbConnection Connection
        {
            set;
            get;
        }

        /// <summary>
        /// ���ӱ�ţ�����
        /// </summary>
        public int SN
        {
            set;
            get;
        }

        /// <summary>
        /// ���ʱ��
        /// </summary>
        public DateTime ActiveTime
        {
            set;
            get;
        }

        /// <summary>
        /// �Ƿ�����ʹ��
        /// </summary>
        public bool IsUsed
        {
            set;
            get;
        }

        //public object Clone()
        //{
        //    return this.MemberwiseClone();
        //    //return new PoolConnection()
        //    //{
        //    //    Connection = this.Connection,
        //    //    SN = this.SN,
        //    //    ActiveTime = DateTime.Now,
        //    //    IsUsed = false,
        //    //};
        //}
    }
}