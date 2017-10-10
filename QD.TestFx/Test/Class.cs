using System;
using System.Collections.Generic;
using System.Text;
using QD.ORM.MappingAttribute.Table;
using QD.ORM.MappingAttribute.Col;
using QD.Found.Core;
//---------------------------------------------------------------- 
// Copyright (C)Œ∫…ÿ”± 2014-2015, All rights reserved. 
// 
// File Name :Class 
// Function Description: 
// Version: 1.0.1 
// 
// Author£∫WeiShaoying 
// Mail:   15910708769@163.com
// Create Time£∫2015/2/24 8:15:26 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.TestFx.Test
{
    [Table("T_CLass", QD.ORM.Model.PKGenerateStrategy.GUID, "ID")]
    public class Class : IIdentify
    {
        [PK]
        public string Id
        {
            set;
            get;
        }

        [Column("CLASS_NAME")]
        public string Name
        {
            set;
            get;
        }

        [Column]
        public string SN
        {
            set;
            get;
        }

    }
}