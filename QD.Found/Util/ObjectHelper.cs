using System;
using System.Collections.Generic;
using System.Text;
using System.Reflection;
//---------------------------------------------------------------- 
// Copyright (C)Œ∫…ÿ”± 2014-2015, All rights reserved. 
// 
// File Name :ObjectHelper 
// Function Description: 
// Version: 1.0.1 
// 
// Author£∫WeiShaoying 
// Mail:   15910708769@163.com
// Create Time£∫2015/2/24 9:58:32 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.Found.Util
{
    public class ObjectHelper
    {

        public static string ToString(object obj)
        {
            Type t1 = obj.GetType();
            PropertyInfo[] props = t1.GetProperties(BindingFlags.Public | BindingFlags.Instance);
            if (CommonHelper.IsCollectionEmpty(props))
            {
                return null;
            }
            StringBuilder sb = new StringBuilder();
            foreach (PropertyInfo item in props)
            {
                //∂‘”⁄public List<Role> RoleList£¨IsClass=true, IsArray=false
                if ((item.PropertyType.IsClass && item.PropertyType != typeof(string))
                    || item.PropertyType.IsArray)
                {
                    continue;
                }
                sb.Append(item.Name);
                sb.Append("=");
                object value = item.GetValue(obj, null);
                if (null != value)
                {
                    sb.Append(value.ToString());
                }
                sb.Append("\t,");
            }
            if (sb.Length > 1)
            {
                return sb.Remove(sb.Length - 1, 1).ToString();
            }
            return null;
        }


    }
}