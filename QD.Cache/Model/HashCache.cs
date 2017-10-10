using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :HashCache 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/22 21:05:10 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.Cache.Model
{
    /// <summary>
    /// ����Ӧ�÷�Ϊһ������(�û�Session��)�Ͷ�������(Ӧ�ó�����)��
    /// �������ﲻ���õ���ģʽ
    /// </summary>
    public class HashCache : ICache
    {
        //�ڲ�ͨ��HashTable���洢����
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

        //������ԣ�
        //1.�ﵽһ��������
    }
}