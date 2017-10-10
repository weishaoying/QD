using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Reflection;
using QD.Found.Util;
using QD.ORM.Engine;
using QD.ORM.Engine.Core;
using QD.TestFx.Test;
using QD.ORM.Common;
using QD.ORM.Model;
using System.Threading;
using System.Data.Common;
using System.Text.RegularExpressions;
using System.Text;
using QD.ORM;
//using QD.ORM.Pool;

namespace QD.TestFx
{
    class Program
    {
        static void Main(string[] args)
        {
            //增加正则表达式 : SELECT * from T [where xx] [order by xx]
            //string s1 = "select id, username, lastname from t_person";



            //Regex reg = new Regex(@"^select +([\w\*, ]+) +from [\w]+", RegexOptions.IgnoreCase);
            //Match match = reg.Match(s1);
            //if (!match.Success)
            //{
            //    throw new ORMException(s1 + "：不是正确的SQL语句");
            //}
            //else
            //{
            //    Console.WriteLine("S");
            //}
            //return;

            //orderby也需要去除原来的表名信息 
            //Regex reg = new Regex(@"\w+\.");
            ////找到的所有匹配项
            //MatchCollection mc = reg.Matches("order by t1.A DESC, t2.B ASC, t3.C Desc");
            //for (int i = 0; i < mc.Count; i++)
            //{
            //    Console.WriteLine("---->{0} index = {1}", mc[i].Value, mc[i].Index);
            //}
            //string src = "order by t1.A DESC,BBB , EEE, t3.C Desc";
            ////string dest = Regex.Replace(src, @"\w+(?=\.)", "__T0");
            //string dest = Regex.Replace(src, @"\w+\.", "__T0.");
            //Console.WriteLine(dest);
            //return;

            //Regex reg = new Regex("abc");
            ////找到的所有匹配项
            //MatchCollection mc = reg.Matches("0abc123xxabGJAabc.$");
            //for (int i = 0; i < mc.Count; i++)
            //{
            //    Console.WriteLine("---->{0} index = {1}", mc[i].Value, mc[i].Index);
            //}

            ////要匹配的字符串
            //string text = "1A 2B 3C 4D 5E 6F 7G 8H 9I 10J 11Q 12J 13K 14L 15M 16N ffee80 #800080";
            ////正则表达式
            //string pattern = @"((\d+)([a-z]))\s+";

            ////使用RegexOptions.IgnoreCase枚举值表示不区分大小写
            //Regex r = new Regex(pattern, RegexOptions.IgnoreCase);

            ////使用正则表达式匹配字符串，仅返回第一个匹配项
            //Match match = r.Match(text);
            //while (match.Success)
            //{
            //    //显示匹配开始处的索引值和匹配到的值
            //    System.Console.WriteLine("----Match=[" + match + "]");
            //    CaptureCollection cc = match.Captures;
            //    foreach (Capture c in cc)
            //    {
            //        Console.WriteLine("\tCapture=[" + c + "]");
            //    }

            //    //对其中的每个匹配分组，总原则就是先整体后部分，由外到里，从左至右
            //    for (int i = 0; i < match.Groups.Count; i++)
            //    {
            //        //以上面为例，总共有4个Group
            //        //Groups[0]就是完整的匹配[1A ]
            //        //Groups[1]就是外面()括起来的内容[1A]
            //        //Groups[2]就是里面左侧的()括起来的内容[1]
            //        //Groups[3]就是外面右侧的()括起来的内容[A]

            //        //获得所有的分组信息，也就是()括起来的部分
            //        Group group = match.Groups[i];
            //        System.Console.WriteLine("\t\tGroups[{0}]=[{1}]", i, group);


            //        for (int j = 0; j < group.Captures.Count; j++)
            //        {
            //            Capture capture = group.Captures[j];
            //            Console.WriteLine("\t\t\tCaptures[{0}]=[{1}]", j, capture);
            //        }
            //    }
            //    //进行下一次匹配.
            //    match = match.NextMatch();
            //}

            //return;


            //StringBuilder sb = new StringBuilder();
            //sb.AppendFormat("abc{0}", 123);
            //sb.AppendFormat("def{0}", 456);
            //Console.WriteLine(sb.ToString());
            //return;


            //string sql = "select id, username, lastname from person";
            //string sql = "select id, username, lastname from persons t order by t.username desc, t.lastname asc";
            //String ret = GetPageSQL(sql, 10, 14);
            //Console.WriteLine("---------\n" + ret);

            //sql = "select t1.*, t2.CLASS_NAME, t2.CLASS_SN from T_STUDENT t1 inner join T_CLASS t2 on t1.CLASS_ID = t2.ID order by t1.STU_NAME desc, t2.CLASS_NAME";
            //ret = GetPageSQL(sql, 10, 3);
            //Console.WriteLine("---------\n" + ret);

            //return;

            //int a9 = 1;
            //string fmt = @"^SELECT +COUNT\([^\\]+\)( +AS \w+)? +FROM +\w+";
            //Regex reg = new Regex(fmt, RegexOptions.IgnoreCase);//忽略模式中的大小写，即文本大小写都可以
            //string[] testStrs = {
            ////正确的
            //"seleCT CoUNt( Id ) FROM t_user Where NAME LIKE :N AND ID != :ID",
            //"SELECT COUNT(*) FROM T_USER1  Where Name = 'zs' ",
            //"select count(*)  FROM    T_1USER  WHERE NAME = :N ",
            //"SELECT count(T1.AGE)   as CNT FROM    T_1USER  WHERE NAME = :N AND xx=1 and yy>2 and zz !=3",

            ////错误的
            //"SELECT count(T1.AGE) AS CNT AS XXX FROM    T_1USER  WHERE NAME = :N ",//只能有一个as
            //"SELECT count[T1.AGE] as CNT FROM  T_1USER  WHERE NAME = :N ",//[]
            //"SELECT count(T1.AGE\\) as CNT FROM  T_1USER  WHERE NAME = :N ",//
            //"SELECT count T1.AGE AS CNT FROM  T_1USER  WHERE NAME = :N ",
            //};
            //foreach (string item in testStrs)
            //{
            //    Console.WriteLine("---------" + reg.IsMatch(item));
            //}

            //if (a9 > 0)
            //{
            //    return;
            //}

            //Dictionary<int, string> dict = new Dictionary<int, string>();
            //dict[0] = "a";
            //dict.Add(1, "b");
            ////dict.Add(0, "c");
            //dict[1] = "d";
            //dict[1] = "e";
            //dict.Remove(2);
            //return;
            //int a = Thread.CurrentThread.ManagedThreadId;
            //Console.WriteLine(a);
            //return;

            //DBManager dbManager = DBManagerFactory.GetDBManager(ORM.Model.DBType.Oracle);

            //系统启动时调用一次
            //DBManagerFactory.GetInstance(ORM.Model.DBType.Oracle, DBConfig.DefaultDBConnString);

            //DBManagerFactory factory = DBManagerFactory.GetInstance();
            string connStr = "Data Source=.;Initial Catalog=Test;uid=sa;pwd=abc123;Connect Timeout=900";
            DBManagerFactory factory = DBManagerFactory.GetInstance(DBType.SQLServer, connStr);
            DataBaseManager dbManager = factory.CreateDBManager();

            Role r0 = dbManager.LoadById<Role>("754fffc9-2fa1-4cbd-ad94-974eb1d737af");
            Console.WriteLine(ObjectHelper.ToString(r0));

            Class c0 = dbManager.LoadById<Class>("2");
            Console.WriteLine(ObjectHelper.ToString(c0));

            //string sqlu = @"SELECT T.USERNAME, T.REAL_NAME, T.AGE, T.SCORE, T.EDU FROM T_USER T WHERE T.USERNAME NOT LIKE :UN ORDER BY 1 ASC";
            //string sqlu = @"SELECT T.* FROM T_USER T WHERE T.USERNAME NOT LIKE :UN ORDER BY 1 ASC";
            //IList<User> userList2 = dbManager.PageQueryWithParam<User>(sqlu, "UN", "%A%", 5, 2);

            String sql = "SELECT T2.CLASS_NAME, T2.SN AS CLASS_SN, T1.* FROM T_USER T1 INNER JOIN T_CLASS T2 ON T1.CLASS_ID = T2.ID ORDER BY T1.REAL_NAME ASC, T2.CLASS_NAME";
            IList<User> userList2 = dbManager.PageQuery<User>(sql, 2, 4);

            foreach (User item in userList2)
            {
                Console.WriteLine(ObjectHelper.ToString(item));
                Console.WriteLine();
            }
            return;

            //QD.ORM.Pool.PoolConnection pc = null;
            IList<User> userList1 = dbManager.QueryAllByType<User>();
            foreach (User item in userList1)
            {
                Console.WriteLine(ObjectHelper.ToString(item));
            }
            return;


            //List<object[]> objList = dbManager.QueryObjectArrayListBySQL("SELECT T.ID, T.USERNAME, T.REAL_NAME, T.BIRTHDATE, T.HEIGHT, T.IS_MINORITY FROM T_USER T");

            //if (CommonHelper.NotEmpty(objList))
            //{
            //    Console.WriteLine(objList.Count);
            //    return;
            //}



            //dbManager.DeleteEntityById<User>("fdcf17a0-75ed-4f43-9223-b2230fb383e0");

            using (DbTransaction ts = dbManager.BeginTransaction())
            {
                bool result = true;
                try
                {
                    string updateSQl = @"UPDATE T_USER SET REAL_NAME = :R, 
                AGE = :A, BIRTHDATE = :BD ,SCORE=   :S
                WHERE ID = :ID";
                    int rowCnt = dbManager.ExecuteUpdateBySQLWithParams<User>(updateSQl,
                        new string[] { "R", "A", "BD", "S", "ID" },
                        new object[] { "刘媛媛", 24, 
                            DateTime.ParseExact("1986-03-04 11:12:14","yyyy-MM-dd HH:mm:ss", null),
                            1024.4819,
                            "2e4ec958-87e1-4ddf-8646-94bea956a6f9" });



                    //两种方法：1.把它放在上面事务外面
                    //2.在查询结果集中也加入事务
                    //User userDB = dbManager.LoadById<User>("6922a73c-34c8-44f6-bca4-37bd33e7bc2b");
                    //Console.WriteLine(ObjectHelper.ToString(userDB));
                    //return;

                    //User user1 = new User();
                    //user1.Username = "Test4";
                    //user1.RealName = "巫启贤";
                    ////user1.Age = 44;
                    ////user1.Score = 88.7007;
                    ////user1.Sex = Gender.Male;
                    ////user1.Weight = 118.01F;
                    ////user1.BeginWorkDate = null;

                    //dbManager.SaveEntity(user1);

                    ////result = DeleteEntireProcess(targetProcessId, out errorMsg);

                    ////dbManager.DeleteEntityById<Role>("a066e2fe-8008-4761-a8ea-dfce42146f7c");

                    //userDB.Username = "TDW";
                    //userDB.RealName = "佟大为";
                    //userDB.BeginWorkDate = null;// DateTime.ParseExact("2008-11-20 09:15:17", "yyyy-MM-dd HH:mm:ss", null);
                    //userDB.Sex = Gender.Male;
                    //userDB.IsMinority = true;
                    //userDB.IsDY = false;
                    //userDB.BirthDate = DateTime.ParseExact("1948-10-12 19:35:57", "yyyy-MM-dd HH:mm:ss", null);
                    //userDB.Money = 25002.18;
                    //userDB.Score = 1999.8008;
                    //userDB.Edu = EducationBackground.GZ;
                    //userDB.FamilyCnt = 10;
                    //userDB.FirstName = "T";
                    //userDB.Age = 67;
                    //userDB.Height = 184F;
                    //userDB.Weight = 99.1121F;


                    //int rowCnt = dbManager.UpdateEntity(userDB);
                    result = rowCnt > 0;


                    Console.WriteLine("------更新成功");
                    //Role r = new Role() { Name = "产品经理", Remark = "产品研发" };
                    //dbManager.SaveEntity(r);

                    //r = new Role() { Name = "CTO2", Remark = "首席执行官" };
                    //dbManager.SaveEntity(r);

                    int b = 0;
                    //int a = 33 / b;

                    if (result)
                    {
                        ts.Commit();
                    }
                    else
                    {
                        ts.Rollback();
                    }
                }
                catch (Exception ex)
                {
                    ts.Rollback();
                    //throw ex;
                    Console.WriteLine(ex.StackTrace);
                }
                finally
                {
                    dbManager.AfterTransaction();
                }
            }//using 结束时Transaction自动销毁

            return;






            #region 测试count

            //string cntSql = "SELECT count(*) FROM T_ROLE T WHERE ROLE_NAME != :A and remark > :B";
            //int t = dbManager.CountBySQLWithParams(cntSql, new string[] { "A", "B" }, new object[] { "架构师", "1" });

            ////cntSql = "SELECT count(*) FROM T_ROLE T";
            ////int t = dbManager.CountBySQL(cntSql);
            //Console.WriteLine(t); 
            //return;
            #endregion

            int pageSize = 2;
            int pageNo = 3;

            #region 分页查询
            //没有查询ID
            //IList<Role> roleList = dbManager.PageQueryWithParam<Role>("SELECT T.ROLE_NAME, T.REMARK FROM T_ROLE T WHERE ROLE_NAME != :A ORDER BY ROLE_NAME", "A", "哈哈", pageSize, pageNo);
            //foreach (Role item in roleList)
            //{
            //    Console.WriteLine(ObjectHelper.ToString(item));
            //}

            //DataBaseManager dbManager12 = factory.CreateDBManager();
            ////查询ID
            //roleList = dbManager12.PageQueryWithParams<Role>("SELECT ROLE_NAME, REMARK FROM T_ROLE T_1 WHERE ROLE_NAME != :A and remark > :B ORDER BY ROLE_NAME",
            //    new string[] { "A", "B" },
            //    new object[] { "哈哈", "1" }, pageSize, pageNo);
            //foreach (Role item in roleList)
            //{
            //    Console.WriteLine(ObjectHelper.ToString(item));
            //}
            ////不再需要控制连接了
            ////dbManager.CloseConnection();
            ////dbManager12.CloseConnection();
            //return;
            #endregion


            #region 测试保存功能

            Role r2 = new Role() { Name = "魏韶颖", Remark = "管理公司财务" };
            dbManager.SaveEntity(r2);

            //IList<Role> roleList = dbManager.QueryAllByType<Role>();
            //foreach (Role item in roleList)
            //{
            //    Console.WriteLine(ObjectHelper.ToString(item));
            //}


            //User u = new User()
            //{
            //    Username = "WXB",
            //    RealName = "魏韶颖",
            //    Age = 29,
            //    BirthDate = new DateTime(1987, 2, 11, 18, 12, 25),
            //    BeginWorkDate = DateTime.ParseExact("2008-02-28 20:05:09", "yyyy-MM-dd HH:mm:ss", null),
            //    FirstName = "W",
            //    IsDY = false,
            //    IsMinority = false,
            //    Score = 697.88,
            //    Edu = EducationBackground.GZ,
            //    FamilyCnt = 5,
            //    Money = 20991.77,
            //    Sex = Gender.Male,
            //    Height = 178.9F,
            //    Weight = 132.86F,
            //};

            //dbManager.Save(u);


            //IList<User> userList = dbManager.QueryAllByType<User>();
            //Console.WriteLine("\n");
            //foreach (User user in userList)
            //{
            //    Console.WriteLine(QD.Found.Util.ObjectHelper.ToString(user));
            //}
            //return; 
            #endregion


            #region 删除功能
            //dbManager.Delete(new Class() { Id = "1" });
            //Console.WriteLine("000000");
            //dbManager.DeleteById<User>("587eb2a8-febb-428f-a272-01fd754ed23a");
            //IList<User> userList = dbManager.QueryAllByType<User>();
            IList<User> userList = dbManager.QueryListByOQLWithParam<User>("from User where Username like :UN", "UN", "%Xiao%");
            Console.WriteLine("数量=" + userList.Count + "\n");
            foreach (User user in userList)
            {
                Console.WriteLine(QD.Found.Util.ObjectHelper.ToString(user));
            }

            return;
            #endregion

            //string str = null;
            //str = " ";// string.Empty;
            //Console.WriteLine(string.IsNullOrWhiteSpace(str));
            //return;
            //string strAfterWhere = "Id > :A";
            //string strAfterWhere = " SN LIKE :B";
            //string strAfterWhere = "UNAME= :B";
            //string strAfterWhere = "WDATE Between :B";

            //string[] arr0 = strAfterWhere.Split(new string[] { "=", "!=", ">", "<", "like", "Like", "LIKE", "between", "Between", "BETWEEN" }, StringSplitOptions.RemoveEmptyEntries);
            //foreach (string item in arr0)
            //{
            //    Console.WriteLine(item);
            //}
            //return;


            //DateTime dt1 = DateTime.ParseExact("2012-10-02 08:10:15", "yyyy-MM-dd HH:mm:ss", null);
            //Console.WriteLine(dt1.ToString());
            //DateTime dt2 = new DateTime(2013, 10, 2, 8, 10, 15);
            //Console.WriteLine(dt2.ToString());

            //string str = "abc1234xxEND";
            //Console.WriteLine(str.Substring(5));

            //string str = "嘉文UserName = :U订单 And Password = :P AND X=1 anD Y=2 and Haha=END AND.张三丰";
            ////string str = "UserName = :U  Password = :P aNd X=1 Y=2 ";
            //string[] arr = StringHelper.SplitIgnoreCase(str, "and");
            //foreach (string item in arr)
            //{
            //    Console.WriteLine(item);
            //}
            //return;


            //ConnectionPool pool = ConnectionPool.GetInstance("");
            //PoolConnection conn1 = pool.GetConnection();
            //conn1.

            //Parent p1 = new class1();
            //Parent p2 = new class2();
            //p1.prinf();
            //p2.prinf();
            //class1 c1 = new class1();
            //class2 c2 = new class2();
            //c1.prinf();
            //c2.prinf();
            //return;

            //string guid = Guid.NewGuid().ToString();
            //Console.WriteLine(guid + ", length = " + guid.Length);

            //double obj = (double)Activator.CreateInstance(typeof(double));
            //Console.WriteLine(obj);
            //User user1 = Activator.CreateInstance(typeof(User)) as User;
            //Console.WriteLine(ObjectHelper.ToString(user1));
            //return;

            #region MyRegion
            //PropertyInfo[] props = typeof(User).GetProperties(BindingFlags.Public | BindingFlags.Instance);
            //foreach (PropertyInfo item in props)
            //{
            //    Attribute attr = Attribute.GetCustomAttribute(item, typeof(ColumnAttribute));
            //    if (attr is ColumnAttribute)
            //    {
            //        ColumnAttribute ca = attr as ColumnAttribute;
            //        Console.WriteLine(ca.ColumnName);
            //    }
            //    if (null == attr)
            //    {
            //        Console.WriteLine("不能被映射");
            //    }
            //}
            //if (true)
            //{
            //    return;
            //} 
            #endregion


            #region MyRegion
            //OracleConnection conn = new OracleConnection("Data Source=RRMESCS; User Id=RRMESCS_DEV; Password=abc123;");
            //string sql = "SELECT * FROM TEST";
            //OracleCommand cmd = new OracleCommand(sql, conn);
            //cmd.CommandType = System.Data.CommandType.Text;
            //try
            //{
            //    conn.Open();
            //    OracleDataReader reader = cmd.ExecuteReader();
            //    this.lblMsg.Text = "ID, 姓名<br/>";
            //    while (reader.Read())
            //    {
            //        this.lblMsg.Text += reader["ID"] + "&nbsp;&nbsp;" + reader["NAME"] + "<br/>";
            //    }
            //    reader.Close();
            //}
            //catch (Exception ex)
            //{
            //    Response.Write(ex.ToString());
            //}
            //finally
            //{
            //    conn.Close();
            //} 
            #endregion

            //Console.WriteLine("---Before GetDataSetFromSQL");
            //DataSet dataSet = dbManager.GetDataSetFromSQL("Select * from T_USER");
            //Console.WriteLine("---After GetDataSetFromSQL");
            //DataSet dataSet2 = dbManager.GetDataSetFromSQL("Select * from T_USER");
            //dataSet2 = dbManager.GetDataSetFromSQL("Select * from T_USER");
            //dataSet2 = dbManager.GetDataSetFromSQL("Select * from T_USER");

            //DBManager dbManager2 = DBManagerFactory.GetDBManager();
            //dataSet2 = dbManager2.GetDataSetFromSQL("Select * from T_USER");



            //DataTable table = dataSet.Tables[0];
            //Console.WriteLine("记录条数：" + table.Rows.Count);
            //#region MyRegion
            ////for (int i = 0; i < table.Rows.Count; i++)
            ////{
            ////    for (int j = 0; j < table.Columns.Count; j++)
            ////    {
            ////        Console.Write(table.Rows[i].ItemArray[j] + "\t");
            ////    }
            ////    Console.WriteLine();
            ////} 
            //#endregion

            //IList<User> userList = MappingEngine<User>.Mapping(dataSet);


            //IList<User> userList = dbManager.QueryListBySQLWithParam<User>("Select * from T_USER Where HEIGHT > :A", "A", 170.5);
            //IList<User> userList = dbManager.QueryListBySQLWithParams<User>("Select * from T_USER Where HEIGHT > :A and WORK_DATE > :B",
            //    new string[] { "A", "B" },
            //    new object[] { 170.5, 
            //        //new DateTime(2012, 10, 2, 8, 10, 15) });
            //DateTime.ParseExact("2012-10-02 08:10:15", "yyyy-MM-dd HH:mm:ss", null)});

            //foreach (User user in userList)
            //{
            //    Console.WriteLine(QD.Found.Util.ObjectHelper.ToString(user));
            //}

            //IList<Class> classList = dbManager.QueryListBySQLWithParams<Class>("select * from T_CLASS where Id> :A and CLASS_NAME LIKE :B AND Sn like :C",
            //    new string[] { "A", "B", "C" },
            //    new object[] { 0, "%2%", "S%2%" });
            //IList<Class> classList = dbManager.QueryListBySQLWithParams<Class>("select * from T_CLASS where SN= :A",//Id = :A",// AND SN = :C",
            //    new string[] { "A" },//, "C" },
            //    new object[] { "S624 AND 2>1" });//3 or 2>1" });//, "S624 OR 1=1" });


            //IList<Class> classList = dbManager.QueryAllByType<Class>();
            ////一个参数
            //IList<Class> classList = dbManager.QueryListBySQLWithParam<Class>("select * from T_CLASS where Id> :A",// and CLASS_NAME LIKE :B AND Sn like :C",
            //    "A", 1);

            //dbManager.Delete(new Class() { Id = "1" });
            //Console.WriteLine("000000");
            //dbManager.DeleteById<Class>("1");
            //Console.WriteLine("111");
            IList<Class> classList = dbManager.QueryListByOQLWithParams<Class>("from Class WhEre SN !=1 and Id != :A AND SN Like :B AND Name like :C",
                new string[] { "A", "B", "C" },
                new object[] { -2, "S%2%", "%2%" });//
            foreach (Class item in classList)
            {
                Console.WriteLine(QD.Found.Util.ObjectHelper.ToString(item));
            }

            //dbManager.CloseConnection();
            //dbManager.CloseConnection();
            //DataBaseManager dbManager2 = DBManagerFactory.GetDBManager();
            //dbManager2.CloseConnection();
            //dbManager2.CloseConnection();
        }

    }

    public abstract class Parent
    {
        public virtual void prinf()
        {
            Console.WriteLine("--父类");
        }
    }
    public class class1 : Parent
    {
        public override void prinf()
        {
            Console.WriteLine("--111");
        }
    }
    public class class2 : Parent
    {
        public new void prinf()
        {
            Console.WriteLine("--222");
        }
    }
}