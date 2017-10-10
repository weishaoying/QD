using System;
using System.Collections.Generic;
using System.Text;
using QD.ORM.Engine.Core;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :SelectBuilder 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/26 8:37:35 
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

        //对于查询来说重点是这个
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
            //如果没有where就是查询全部
            if (!oql.ToLower().Contains(" where "))
            {
                return ORMUtil.GenerateSelectAllSQL(type);
            }
            //TODO:截取where后面的部分，按照and分割，得到Property=Value
            //再查询属性对应的字段


            return null;
        }

        public override string BuildSQL(object obj)
        {
            return null;
        }
    }
}