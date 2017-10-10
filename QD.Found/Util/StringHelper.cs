using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :StringHelper 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/26 10:46:00 
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
        /// 忽略大小写的分割字符串，且可以去除空项，每项都经过Trim
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
            string str2 = str.ToUpper();//大写
            string spliter2 = spliter.ToUpper();

            int p1 = 0;//开始检索位置
            int p2 = -1;
            while ((p2 = str2.IndexOf(spliter2, p1)) >= 0)
            {
                //retList.Add(str.Substring(p1, p2 - p1));//这个是转为大写后的字符串，不能直接添加到返回数组中
                //应该记住位置
                posList.Add(p2);
                p1 = p2 + 1;
            }

            //再从原始字符串中截取 
            string val = null;
            p1 = 0;//开始截取的位置
            foreach (int pos in posList)
            {
                val = str.Substring(p1, pos - p1);
                //如果是空项，则不添加
                if (!String.IsNullOrEmpty(val.Trim()))
                {
                    retList.Add(val.Trim());
                }
                p1 = pos + spliter.Length;//跳过分隔符，不要它
            }

            if (posList.Count > 0)
            {
                //最后一个and到最后
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