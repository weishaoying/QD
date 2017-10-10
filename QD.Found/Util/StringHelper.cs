using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :StringHelper 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/26 10:46:00 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.Found.Util
{
    public class StringHelper
    {
        /// <summary>
        /// ���Դ�Сд�ķָ��ַ������ҿ���ȥ�����ÿ�����Trim
        /// </summary>
        /// <param name="str"></param>
        /// <param name="spliter"></param>
        /// <returns></returns>
        public static string[] SplitIgnoreCase(string str, string spliter)
        {
            if (String.IsNullOrEmpty(str) || String.IsNullOrEmpty(spliter))
            {
                return null;
            }

            List<int> posList = new List<int>();
            List<string> retList = new List<string>();

            //UserName = :U And Password = :P AND X=1 anD Y=2
            string str2 = str.ToUpper();//��д
            string spliter2 = spliter.ToUpper();

            int p1 = 0;//��ʼ����λ��
            int p2 = -1;
            while ((p2 = str2.IndexOf(spliter2, p1)) >= 0)
            {
                //retList.Add(str.Substring(p1, p2 - p1));//�����תΪ��д����ַ���������ֱ����ӵ�����������
                //Ӧ�ü�סλ��
                posList.Add(p2);
                p1 = p2 + 1;
            }

            //�ٴ�ԭʼ�ַ����н�ȡ 
            string val = null;
            p1 = 0;//��ʼ��ȡ��λ��
            foreach (int pos in posList)
            {
                val = str.Substring(p1, pos - p1);
                //����ǿ�������
                if (!String.IsNullOrEmpty(val.Trim()))
                {
                    retList.Add(val.Trim());
                }
                p1 = pos + spliter.Length;//�����ָ�������Ҫ��
            }

            if (posList.Count > 0)
            {
                //���һ��and�����
                val = str.Substring(posList[posList.Count - 1] + spliter.Length);
                if (!string.IsNullOrEmpty(val.Trim()))
                {
                    retList.Add(val.Trim());
                }
            }
            else
            {
                retList.Add(str.Trim());
            }
            return retList.ToArray();
        }






    }
}