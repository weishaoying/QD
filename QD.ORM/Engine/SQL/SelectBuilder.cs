using System;
using System.Collections.Generic;
using System.Text;
using QD.ORM.Engine.Core;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :SelectBuilder 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/26 8:37:35 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine.SQL
{
    class SelectBuilder : SQLBuilder
    {
        private static SelectBuilder _Instance = new SelectBuilder();

        private SelectBuilder()
        { }

        public static SelectBuilder GetInstance()
        {
            return _Instance;
        }

        //QueryByOQL();
        //Save(object obj);
        //Update("update user set Name = :N and age = :Age where id = 1")
        //Delete(object obj);

        //���ڲ�ѯ��˵�ص������
        public override string BuildSQL<T>(string oql)
        {
            if (String.IsNullOrEmpty(oql))
            {
                throw new ArgumentNullException();
            }
            Type type = typeof(T);

            //from User where UserName = :U and Password = :P
            string tableName = ORMUtil.GetTableName(type);
            if (string.IsNullOrEmpty(tableName))
            {
                throw new ORMException("");
            }
            //���û��where���ǲ�ѯȫ��
            if (!oql.ToLower().Contains(" where "))
            {
                return ORMUtil.GenerateSelectAllSQL(type);
            }
            //TODO:��ȡwhere����Ĳ��֣�����and�ָ�õ�Property=Value
            //�ٲ�ѯ���Զ�Ӧ���ֶ�


            return null;
        }

        public override string BuildSQL(object obj)
        {
            return null;
        }
    }
}