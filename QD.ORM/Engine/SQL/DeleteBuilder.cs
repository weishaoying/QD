using System;
using System.Collections.Generic;
using System.Text;
using QD.ORM.Common;
using QD.Found.Core;
using QD.ORM.Engine.Core;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :DeleteBuilder 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/26 8:39:33 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.ORM.Engine.SQL
{
    class DeleteBuilder : SQLBuilder
    {
        private static DeleteBuilder _Instance = new DeleteBuilder();

        private DeleteBuilder()
        { }

        public static DeleteBuilder GetInstance()
        {
            return _Instance;
        }

        public override string BuildSQL<T>(string oql)
        {
            throw new NotImplementedException();
        }

        //����ɾ���ܼ�
        public override string BuildSQL(object obj)
        {
            if (!(obj is QD.Found.Core.IIdentify))
            {
                throw new ORMException(TipConst.ClassMustInheritID);
            }
            IIdentify idObj = obj as IIdentify;
            String tableName = ORMUtil.GetTableName(obj.GetType());
            if (String.IsNullOrEmpty(tableName))
            {
                throw new ORMException(TipConst.TableNameIsEmpty);
            }
            //TODO:Ĭ����������ID, ���п�������������ID�����ﲻ����׳���Ժ���ϲ�ѯ����������

            //����ֱ�ӷ�������SQL�����ǵ���SQLע�룬������ȫ
            //string str = String.Format("DELETE FROM {0} WHERE ID = {1}", tableName, idObj.Id);

            return String.Format("DELETE FROM {0} WHERE ID = :ID");
        }
    }
}