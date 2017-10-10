using System;
using System.Collections.Generic;
using System.Text;
using System.Data;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :MappingEngine 
// Function Description: ����ORMӳ��
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/23
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine.Core
{
    /// <summary>
    /// ORMӳ������
    /// </summary>
    /// <typeparam name="T">����������User</typeparam>
    public class MappingEngine<T> : BaseORMEngine
    {

        //private static MappingEngine<T> _Instance = new MappingEngine<T>();

        private MappingEngine()
        {
        }

        public static List<T> Mapping(DataSet dataSet)
        {
            if (null == dataSet || null == dataSet.Tables)
            {
                return null;
            }
            return Mapping(dataSet.Tables[0]);
        }

        public static List<T> Mapping(DataTable dataTable)
        {
            return (List<T>)Mapping(dataTable, typeof(T));
        }

    }
}