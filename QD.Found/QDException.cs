using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)Œ∫…ÿ”± 2014-2015, All rights reserved. 
// 
// File Name :QDException 
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

namespace QD.Found
{
    public class QDException : Exception
    {
        public QDException()
            : base("An exception occurred in the QD Framework Layer")
        {
        }

        public QDException(string message)
            : base(message)
        {
        }

        public QDException(Exception ex)
            : base(ex.Message, ex)
        {
        }

        public QDException(string message, Exception ex)
            : base(message, ex)
        {
        }
    }
}