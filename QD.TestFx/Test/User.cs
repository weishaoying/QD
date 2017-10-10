using System;
using System.Collections.Generic;
using System.Text;
using QD.ORM.MappingAttribute.Table;
using QD.ORM.MappingAttribute.Col;
using QD.ORM.MappingAttribute.Relation;
using QD.Found.Core;
//---------------------------------------------------------------- 
// Copyright (C)魏韶颖 2014-2015, All rights reserved. 
// 
// File Name :User 
// Function Description: 
// Version: 1.0.1 
// 
// Author：WeiShaoying 
// Mail:   15910708769@163.com
// Create Time：2015/2/23 23:28:05 
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
        /// 家庭成员数量
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
        /// 姓名首字母
        /// </summary>
        public string FirstName
        //public char FirstName//暂时不支持char
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
        /// 身高
        /// </summary>
        public float Height
        {
            set;
            get;
        }

        [Column]
        /// <summary>
        /// 体重
        /// </summary>
        public float? Weight
        {
            set;
            get;
        }

        [Column("SALARY")]
        /// <summary>
        /// 工资
        /// </summary>
        public double Money
        {
            set;
            get;
        }

        [Column("SCORE")]
        /// <summary>
        /// 成绩
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
        /// 是否是少数民族
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
        /// 男
        /// </summary>
        Male = 1,

        /// <summary>
        /// 女
        /// </summary>
        Female = 2
    }

    public enum EducationBackground
    {
        UnKnown = 0,

        /// <summary>
        /// 小学及以下
        /// </summary>
        XX = 1,

        /// <summary>
        /// 初中
        /// </summary>
        CZ = 2,

        /// <summary>
        /// 高中
        /// </summary>
        GZ = 3,

        /// <summary>
        /// 大学
        /// </summary>
        DX = 4,
    }

}