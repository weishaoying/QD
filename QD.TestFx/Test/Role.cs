using System;
using System.Collections.Generic;
using System.Text;
using QD.ORM.MappingAttribute.Table;
using QD.ORM.MappingAttribute.Col;
using QD.Found.Core;
//---------------------------------------------------------------- 
// Copyright (C)Œ∫…ÿ”± 2014-2015, All rights reserved. 
// 
// File Name :Role 
// Function Description: 
// Version: 1.0.1 
// 
// Author£∫WeiShaoying 
// Mail:   15910708769@163.com
// Create Time£∫2015/2/24 8:18:57 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.TestFx.Test
{
    [Table("T_ROLE")]
    public class Role : IIdentify
    {
        //[PK("ID")]
        [PK]
        public string Id
        {
            set;
            get;
        }

        [Column("ROLE_NAME")]
        public string Name
        {
            set;
            get;
        }

        [Column]
        public string Remark
        {
            set;
            get;
        }
    }
}