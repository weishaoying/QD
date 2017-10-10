using System;
using System.Collections.Generic;
using System.Text;
using System.Collections;
//---------------------------------------------------------------- 
// Copyright (C)Œ∫…ÿ”± 2014-2015, All rights reserved. 
// 
// File Name :CommonHelper 
// Function Description: 
// Version: 1.0.1 
// 
// Author£∫WeiShaoying 
// Mail:   15910708769@163.com
// Create Time£∫2015/2/23 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.Found.Util
{
    public class CommonHelper
    {

        public static bool IsCollectionEmpty(ICollection conn)
        {
            return null == conn || conn.Count < 1;
        }

        public static bool NotEmpty(ICollection conn)
        {
            return !IsCollectionEmpty(conn);
        }


    }
}