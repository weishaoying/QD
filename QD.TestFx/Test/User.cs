using System;
using System.Collections.Generic;
using System.Text;
using QD.ORM.MappingAttribute.Table;
using QD.ORM.MappingAttribute.Col;
using QD.ORM.MappingAttribute.Relation;
using QD.Found.Core;
//---------------------------------------------------------------- 
// Copyright (C)κ��ӱ 2014-2015, All rights reserved. 
// 
// File Name :User 
// Function Description: 
// Version: 1.0.1 
// 
// Author��WeiShaoying 
// Mail:   15910708769@163.com
// Create Time��2015/2/23 23:28:05 
// 
// Modify Time: 
// Modify Description: 
// 
//----------------------------------------------------------------

namespace QD.TestFx.Test
{
    [Table("T_USER")]
    public class User : IIdentify
    {
        [Column("FAMILY_CNT")]
        /// <summary>
        /// ��ͥ��Ա����
        /// </summary>
        public int? FamilyCnt
        {
            set;
            get;
        }

        [PK]
        public string Id
        {
            set;
            get;
        }

        [Column]
        public string Username
        {
            set;
            get;
        }

        [Column("REal_namE")]
        public string RealName
        {
            set;
            get;
        }

        [Column("FIRST_NAME")]
        /// <summary>
        /// ��������ĸ
        /// </summary>
        public string FirstName
        //public char FirstName//��ʱ��֧��char
        {
            set;
            get;
        }

        [Column]
        public int Age
        {
            set;
            get;
        }


        [Column]
        /// <summary>
        /// ���
        /// </summary>
        public float Height
        {
            set;
            get;
        }

        [Column]
        /// <summary>
        /// ����
        /// </summary>
        public float? Weight
        {
            set;
            get;
        }

        [Column("SALARY")]
        /// <summary>
        /// ����
        /// </summary>
        public double Money
        {
            set;
            get;
        }

        [Column("SCORE")]
        /// <summary>
        /// �ɼ�
        /// </summary>
        public double? Score
        {
            set;
            get;
        }

        [Column("BIRTHDATE")]
        public DateTime BirthDate
        {
            set;
            get;
        }

        [Column("WORK_DATE")]
        public DateTime? BeginWorkDate
        {
            set;
            get;
        }

        [Column("IS_MINORITY")]
        /// <summary>
        /// �Ƿ�����������
        /// </summary>
        public bool IsMinority
        {
            set;
            get;
        }

        [Column("IS_DY")]
        public bool? IsDY
        {
            set;
            get;
        }

        [Column]
        public Gender Sex
        {
            set;
            get;
        }

        [Column]
        public EducationBackground? Edu
        {
            set;
            get;
        }

        [ManyToOne("CLASS_ID", "T_CLASS")]
        //[ManyToOne("CLASS_ID")]
        public Class Class
        {
            set;
            get;
        }

        [ManyToMany("T_REF_USER_ROLE", "USER_ID", "ROLE_ID")]
        //[ManyToMany("ID", "T_REF_USER_ROLE", "USER_ID", "ROLE_ID", "ID")]
        public List<Role> RoleList
        {
            set;
            get;
        }
    }



    public enum Gender
    {
        UnKnown = 5,

        /// <summary>
        /// ��
        /// </summary>
        Male = 1,

        /// <summary>
        /// Ů
        /// </summary>
        Female = 2
    }

    public enum EducationBackground
    {
        UnKnown = 0,

        /// <summary>
        /// Сѧ������
        /// </summary>
        XX = 1,

        /// <summary>
        /// ����
        /// </summary>
        CZ = 2,

        /// <summary>
        /// ����
        /// </summary>
        GZ = 3,

        /// <summary>
        /// ��ѧ
        /// </summary>
        DX = 4,
    }

}