using System;
using System.Collections.Generic;
using System.Text;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :TipConst 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/24 10:42:02 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Common
{
    public class TipConst
    {
        public const string ClassMustInheritID = "�����̳���IIdentify";
        public const string TableNameIsEmpty = "����Ϊ��";

        public const string TableDontHaveColumn = "���в������ֶΣ�";

        public const string ChangeTypeError = "����ת���쳣��";

        public const string CloseConnError = "�ر�����ʱ�����쳣";

        public const string MoreThanMaxConnectionCnt = "���������������";


        public const string ConnStrEmpty = "���ݿ������ַ���Ϊ�գ��޷���������";

        public const string DBConnectionIsNull = "���ݿ�����ConnectionΪ��";

        public const string DBCommandIsNull = "���ݿ�ִ������DBCommandΪ��";

        public const string NotOracleConnection = "��ǰ����Oracle����";

        public const string NotOracleCommand = "��ǰ����OracleCommand";

        public const string NotSqlCommand = "��ǰ����SqlCommand";

        public const string PoolInfo = "����������{0}, ������������{1}";

        public const string ReuseOnePoolConn = "�ͷ�һ�����ӣ����=";

        public const string PoolInitFinished = "-------���ӳس�ʼ����ɣ�";

        public const string GetConnFromPool = "�����ӳػ�ȡ���ӣ�";

        public const string PoolConnIsNull = "����Ϊ�գ�����ر�";

        public const string ReusePoolConnFailed = "�����Ѿ��رգ������ٴιر�";

        public const string FreeConnCntIsNotEnough = "���������������㣬��������������=";

        public const string AfterCreateNewConn = "���������Ӻ�";




        public const string DBTypeAndConnStrNotInit = "DBType��ConnStrû�г�ʼ��";

        public const string NOT_SET_SEQNAME = "û��ָ��Sequence����";

        public const string NOT_SET_ID_VALUE = "û����������ID��ֵ";





    }
}