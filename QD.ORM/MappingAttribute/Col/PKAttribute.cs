using System;
using System.Collections.Generic;
using System.Text;
using QD.ORM.Common;
//---------------------------------------------------------------- 
// Copyright (C)Œ∫…ÿ”± 2014-2015, All rights reserved. 
// 
// File Name :IDAttribute 
// Function Description: ÷˜º¸ Ù–‘
// Version: 1.0.1 
// 
// Author£∫WeiShaoying 
// Mail:   15910708769@163.com
// Create Time£∫2015/2/24 9:21:39 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.MappingAttribute.Col
{
    [AttributeUsage(AttributeTargets.Field | AttributeTargets.Property, AllowMultiple = false)]
    public class PKAttribute : ColumnAttribute
    {
        public PKAttribute()
        {
            this.ColumnName = Constant.ID;
        }

        public PKAttribute(string columnName)
            : base(columnName)
        {
        }
    }
}