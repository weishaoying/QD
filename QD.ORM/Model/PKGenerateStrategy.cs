using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :PKStrategy 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/23 14:27:52 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Model
{
    /// <summary>
    /// ���������ɲ���
    /// </summary>
    public enum PKGenerateStrategy
    {
        UnKnown = 0,

        GUID = 1,

        SEQ = 2,

        /// <summary>
        /// �Զ���������ͨ������ά��
        /// </summary>
        Assign = 3,

        /// <summary>
        /// �Զ�����
        /// </summary>
        AutoIncrease = 4
    }
}