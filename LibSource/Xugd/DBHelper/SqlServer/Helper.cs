using System;

using System.Data.SqlClient;

namespace SHCre.Xugd.DbHelper
{
    /// <summary>
    /// 数据操作相关的辅助接口
    /// </summary>
    public static partial class XSqlServerHelper
    {
        /// <summary>
        /// 获取当前服务器时间
        /// </summary>
        /// <param name="conDatabase_">目标数据库的连接</param>
        /// <returns>服务器时间</returns>
        public static DateTime GetSrvTime(SqlConnection conDatabase_)
        {
            string strQuery = "Select Getdate() As CurTime";

            return (DateTime)XExcute.Scalar(conDatabase_, strQuery);
        }

        /// <summary>
        /// 根据输入获取分页数
        /// </summary>
        /// <param name="nRowCount_">总的记录条数</param>
        /// <param name="nPageSize_">每页的记录条数</param>
        /// <returns>分页数</returns>
        public static int CalcPageCount(int nRowCount_, int nPageSize_)
        {
            if (nRowCount_ < 0 || nPageSize_ <= 0)
                throw new ArgumentException("Count and size must large than zero");

            if (nRowCount_ == 0)
                return 0;

            int nPageCount = nRowCount_ / nPageSize_;
            if ((nRowCount_ % nPageSize_) != 0)
                ++nPageCount;

            return nPageCount;
        }

        /// <summary>
        /// 获取记录条数
        /// </summary>
        /// <param name="conDatabase_">目标数据库的连接</param>
        /// <param name="strTableName_">表名</param>
        /// <param name="strFilter_">过滤条件（null获取所有记录条数）</param>
        /// <returns>记录条数</returns>
        public static int GetRowCount(SqlConnection conDatabase_, string strTableName_, string strFilter_ = null)
        {
            if (string.IsNullOrEmpty(strFilter_))
                strFilter_ = "1=1";

            string strQuery = string.Format("Select Count(*) From {0} Where {1}",
                        strTableName_, strFilter_);

            return (int)XExcute.Scalar(conDatabase_, strQuery);
        }

        /// <summary>
        /// 针对SqlCommand中的Excute的封装
        /// </summary>
        public static class XExcute
        {
            /// <summary>
            /// 执行没有返回值的SQL语句
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strSQL_">要执行的SQL语句</param>
            /// <returns>受影响的行数</returns>
            public static int NonQuery(SqlConnection conDatabase_, string strSQL_)
            {
                using (SqlCommand cmdSQL = new SqlCommand(strSQL_, conDatabase_))
                {
                    return cmdSQL.ExecuteNonQuery();
                }
            }

            /// <summary>
            /// 执行返回一个对象的SQL语句
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strSQL_">要执行的SQL语句</param>
            /// <returns>返回结果集的第一行的第一列</returns>
            public static object Scalar(SqlConnection conDatabase_, string strSQL_)
            {
                using (SqlCommand cmdSQL = new SqlCommand(strSQL_, conDatabase_))
                {
                    return cmdSQL.ExecuteScalar();
                }
            }

            /// <summary>
            /// 执行返回结果集的SQL语句
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strSQL_">要执行的SQL语句</param>
            /// <returns>整个结果集</returns>
            public static SqlDataReader Reader(SqlConnection conDatabase_, string strSQL_)
            {
                using (SqlCommand cmdSQL = new SqlCommand(strSQL_, conDatabase_))
                {
                    return cmdSQL.ExecuteReader();
                }
            }
        };

        /// <summary>
        /// 数据库操作
        /// </summary>
        public static class XDatabase
        {
            #region Const value define
            /// <summary>
            /// 主数据库名，包含其他数据库的信息
            /// </summary>
            public const string gc_strMaster = "Master";

            /// <summary>
            /// 数据库默认连接端口
            /// </summary>
            public const string gc_strDefPort = "1433";

            /// <summary>
            /// 数据库地址与端口号间的分隔符
            /// </summary>
            public const string gc_strSeparator = ",";

            /// <summary>
            /// 数据库地址与端口号间的分隔符
            /// </summary>
            public const char gc_chSeparator = ',';
            #endregion

            #region Connect functions
            /// <summary>
            /// 获取数据库源的地址，使用逗号连接 "address,port"
            /// </summary>
            /// <param name="strDBAddr_">数据库源</param>
            /// <param name="nPort_">数据库端口号</param>
            /// <returns>数据库源的地址</returns>
            public static string BuildSource(string strDBAddr_, int nPort_)
            {
                return BuildSource(strDBAddr_, nPort_.ToString());
            }

            /// <summary>
            /// 获取数据库源的地址，使用逗号连接 "address,port"
            /// </summary>
            /// <param name="strDBAddr_">数据库源</param>
            /// <param name="strPort_">数据库端口号</param>
            /// <returns>数据库源的地址</returns>
            public static string BuildSource(string strDBAddr_, string strPort_)
            {
                return strDBAddr_ + gc_strSeparator + strPort_;
            }

            /// <summary>
            /// 根据输入参数连接数据库：成功返回SqlConnection连接；否则抛出SqlException异常
            /// </summary>
            /// <param name="strConnect_">连接参数字符串</param>
            /// <returns></returns>
            public static SqlConnection Connect(string strConnect_)
            {
                SqlConnection conDatabase = new SqlConnection(strConnect_);
                conDatabase.Open();

                return conDatabase;
            }

            /// <summary>
            /// 根据输入参数连接数据库：成功返回SqlConnection连接；否则抛出SqlException异常
            /// </summary>
            /// <param name="strDBAddr_">数据库源的地址</param>
            /// <param name="nPort_">数据库端口号</param>
            /// <param name="strDBName_">要连接的数据库名</param>
            /// <param name="strUserName_">登录数据库的用户名</param>
            /// <param name="strUserPwd_">登录数据库的密码</param>
            /// <returns>返回SqlConnection连接</returns>
            public static SqlConnection Connect(string strDBAddr_, int nPort_, string strDBName_, string strUserName_, string strUserPwd_)
            {
                SqlConnection conDatabase = new SqlConnection(ConnectString(strDBAddr_, nPort_, strDBName_, strUserName_, strUserPwd_));
                conDatabase.Open();

                return conDatabase;
            }

            /// <summary>
            /// 根据输入参数连接数据库：成功返回SqlConnection连接；否则抛出SqlException异常
            /// </summary>
            /// <param name="strSource">数据库源，使用逗号连接"address,port"</param>
            /// <param name="strDBName_">要连接的数据库名</param>
            /// <param name="strUserName_">登录数据库的用户名</param>
            /// <param name="strUserPwd_">登录数据库的密码</param>
            /// <returns>返回SqlConnection连接</returns>
            public static SqlConnection Connect(string strSource, string strDBName_, string strUserName_, string strUserPwd_)
            {
                SqlConnection conDatabase = new SqlConnection(ConnectString(strSource, strDBName_, strUserName_, strUserPwd_));
                conDatabase.Open();
                return conDatabase;
            }

            /// <summary>
            /// 根据输入参数获取连接字符串
            /// </summary>
            /// <param name="strDBAddr_">数据库源</param>
            /// <param name="nPort_">数据库端口号</param>
            /// <param name="strDBName_">要连接的数据库名</param>
            /// <param name="strUserName_">登录数据库的用户名</param>
            /// <param name="strUserPwd_">登录数据库的密码</param>
            /// <returns>连接字符串</returns>
            public static string ConnectString(string strDBAddr_, int nPort_, string strDBName_, string strUserName_, string strUserPwd_)
            {
                return ("Data Source=" + strDBAddr_ + gc_strSeparator + nPort_.ToString() +
                        ";Initial Catalog=" + strDBName_ +
                        ";User ID=" + strUserName_ +
                        ";Password=" + strUserPwd_);
            }

            /// <summary>
            /// 从连接字符串中获取各项信息
            /// </summary>
            /// <param name="strConString_">连接字符串</param>
            /// <param name="strDBAddr_">数据库源</param>
            /// <param name="nPort_">数据库端口号</param>
            /// <param name="strDBName_">要连接的数据库名</param>
            /// <param name="strUserName_">登录数据库的用户名</param>
            /// <param name="strUserPwd_">登录数据库的密码</param>
            public static void SplitConnectString(string strConString_, out string strDBAddr_, out int nPort_, out string strDBName_, out string strUserName_, out string strUserPwd_)
            {
                SqlConnectionStringBuilder sbConString = new SqlConnectionStringBuilder(strConString_);

                string[] strDataSrc = sbConString.DataSource.Split(',');
                if (strDataSrc.Length == 0)
                    throw new ArgumentException("Invalid connect string(not include data source)");
                strDBAddr_ = strDataSrc[0];
                nPort_ = 0;
                if (strDataSrc.Length > 1)
                    int.TryParse(strDataSrc[1], out nPort_);
                if (nPort_ == 0)
                    nPort_ = 1433;

                strDBName_ = sbConString.InitialCatalog;
                strUserName_ = sbConString.UserID;
                strUserPwd_ = sbConString.Password;
            }

            /// <summary>
            /// 根据输入参数获取连接字符串
            /// </summary>
            /// <param name="strDBSource_">数据库源的地址，使用逗号连接 "address,port"</param>
            /// <param name="strDBName_">要连接的数据库名</param>
            /// <param name="strUserName_">登录数据库的用户名</param>
            /// <param name="strUserPwd_">登录数据库的密码</param>
            /// <returns>连接字符串</returns>
            public static string ConnectString(string strDBSource_, string strDBName_, string strUserName_, string strUserPwd_)
            {
                return ("Data Source=" + strDBSource_ +
                        ";Initial Catalog=" + strDBName_ +
                        ";User ID=" + strUserName_ +
                        ";Password=" + strUserPwd_);
            }
            #endregion

            /// <summary>
            /// 判断数据库是否存在；失败抛出SqlException异常
            /// </summary>
            /// <param name="strDBSource_">数据库源的地址，使用逗号连接 "address,port"</param>
            /// <param name="strDBName_">要判断的数据库名</param>
            /// <param name="strUserName_">登录数据库的用户名</param>
            /// <param name="strUserPwd_">登录数据库的密码</param>
            /// <returns>存在，返回true；否则，返回false</returns>
            public static bool IsExisted(string strDBSource_, string strDBName_, string strUserName_, string strUserPwd_)
            {
                bool bExisted = false;

                using (SqlConnection conMaster = Connect(strDBSource_, gc_strMaster, strUserName_, strUserPwd_))
                {
                    string strExist = @"Select count(*) from dbo.sysdatabases where name='" + strDBName_ + @"'";
                    bExisted = (0 != (int)XExcute.Scalar(conMaster, strExist));

                    conMaster.Close();
                }

                return bExisted;
            }

            /// <summary>
            /// 根据输入的参数创建数据库：如果失败，则抛出SqlExceptioni异常
            /// </summary>
            /// <param name="strDBSource_">数据库源的地址，使用逗号连接 "address,port"</param>
            /// <param name="strDBName_">要创建的数据库名</param>
            /// <param name="strDBPath_">创建的数据库的存放路径</param>
            /// <param name="strUserName_">登录数据库的用户名</param>
            /// <param name="strUserPwd_">登录数据库的密码</param>
            public static void Create(string strDBSource_, string strDBName_, string strDBPath_, string strUserName_, string strUserPwd_)
            {
                string strFile = System.IO.Path.Combine(strDBPath_.TrimEnd(), strDBName_);
                using (SqlConnection conMaster = Connect(strDBSource_, gc_strMaster, strUserName_, strUserPwd_))
                {
                    string strCreate = "Create Database " + strDBName_ +
                            " on Primary" +
                            "(" +
                            @"Name=N'" + strDBName_ + @"_D', " +
                            @"Filename=N'" + strFile + @".mdf', " +
                            "Size=4MB, Maxsize=UNLIMITED, Filegrowth=10%)" +
                            " Log on" +
                            "(" +
                            @"Name=N'" + strDBName_ + @"_L', " +
                            @"Filename=N'" + strFile + @".ldf', " +
                            "Size=1MB, Maxsize=UNLIMITED, Filegrowth=10%)" +
                            " Collate chinese_prc_Ci_as"; // sort in chinese

                    XExcute.NonQuery(conMaster, strCreate);

                    conMaster.Close();
                }
            }

            /// <summary>
            /// 删除指定的数据库；失败抛出SqlException异常
            /// </summary>
            /// <param name="strDBSource_">数据库源的地址，使用逗号连接 "address,port"</param>
            /// <param name="strDBName_">要删除的数据库名</param>
            /// <param name="strUserName_">登录数据库的用户名</param>
            /// <param name="strUserPwd_">登录数据库的密码</param>
            public static void Delete(string strDBSource_, string strDBName_, string strUserName_, string strUserPwd_)
            {
                using (SqlConnection conMaster = Connect(strDBSource_, XDatabase.gc_strMaster, strUserName_, strUserPwd_))
                {
                    string strDel = @"Drop database " + strDBName_;
                    XExcute.NonQuery(conMaster, strDel);

                    conMaster.Close();
                }
            }
        };

        /// <summary>
        /// 数据库表操作：创建表、删除表和判断表是否存在
        /// </summary>
        public static class XTable
        {
            internal static string MergeCols(string[] strCols)
            {
                string strRet = string.Empty;
                foreach (string str in strCols)
                {
                    strRet += (str + ",");
                }

                return strRet.TrimEnd(',');
            }
            /// <summary>
            /// 在数据库中创建表；失败抛出SqlException异常
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strTableName_">要创建的数据表名</param>
            /// <param name="strColumns_">要创建的各列，每个列为一个字符串（包括列名和属性）。如"FileID int Not NULL Identity"</param>
            public static void Create(SqlConnection conDatabase_, string strTableName_, string[] strColumns_)
            {
                string strCreate = string.Format("Create Table {0} ({1})", strTableName_, MergeCols(strColumns_));

                XExcute.NonQuery(conDatabase_, strCreate);
            }

            /// <summary>
            /// 在数据库中创建表；失败抛出SqlException异常
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strTableName_">要创建的数据表名</param>
            /// <param name="strPKCol_">主键列</param>
            /// <param name="strColumns_">要创建的各列，每个列为一个字符串（包括列名和属性）。如"FileID int Not NULL Identity"</param>
            public static void Create(SqlConnection conDatabase_, string strTableName_, string strPKCol_, string[] strColumns_)
            {
                string strKeyName = string.Format("PK_{0}_{1}", strTableName_, strPKCol_.Split(' ', ',')[0]);
                string strCreate = string.Format("Create Table {0} ( {1}, Constraint {2} Primary Key Clustered ({3}) )",
                        strTableName_, MergeCols(strColumns_), strKeyName, strPKCol_);

                XExcute.NonQuery(conDatabase_, strCreate);
            }

            /// <summary>
            /// 判断数据库中的表是否已存在；失败抛出SqlException异常
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strTableName_">要判断的表名</param>
            /// <returns>存在，返回true；否则，返回false</returns>
            public static bool IsExisted(SqlConnection conDatabase_, string strTableName_)
            {
                string strExist = @"Select count(*) from dbo.sysobjects where " +
                        @"id=object_id('" + strTableName_ + @"') " +
                        @"and ObjectProperty(id,N'IsUserTable')=1";

                return (0 != (int)XExcute.Scalar(conDatabase_, strExist));
            }

            /// <summary>
            /// 删除指定的数据库表；失败抛出SqlException异常
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strTableName_">要删除的数据表名</param>
            public static void Delete(SqlConnection conDatabase_, string strTableName_)
            {
                string strDel = "Drop Table " + strTableName_;
                XExcute.NonQuery(conDatabase_, strDel);
            }
        };

        /// <summary>
        /// 数据库视图操作：创建视图、删除视图以及判断视图是否存在
        /// </summary>
        public static class XView
        {
            /// <summary>
            /// 在数据库中创建视图；失败抛出SqlException异常
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strViewName_">视图名称</param>
            /// <param name="strViewBody_">视图体，即视图如何产生的sql语句</param>
            public static void Create(SqlConnection conDatabase_, string strViewName_, string strViewBody_)
            {
                string strCreate = "Create View " + strViewName_ +
                        " as " + strViewBody_;

                XExcute.NonQuery(conDatabase_, strCreate);
            }

            /// <summary>
            /// 判断视图是否存在；失败抛出SqlException异常
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strViewName_">视图名称</param>
            /// <returns>存放，返回true；否则，返回false</returns>
            public static bool IsExist(SqlConnection conDatabase_, string strViewName_)
            {
                string strExist = @"Select count(*) from dbo.sysobjects where " +
                        @"id=object_id('" + strViewName_ + @"') " +
                        @"and ObjectProperty(id,N'IsView')=1";

                return (0 != (int)XExcute.Scalar(conDatabase_, strExist));
            }

            /// <summary>
            /// 在数据库中删除指定的视图；失败抛出SqlException异常
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strViewName_">视图名称</param>
            public static void Delete(SqlConnection conDatabase_, string strViewName_)
            {
                string strDel = "Drop View " + strViewName_;
                XExcute.NonQuery(conDatabase_, strDel);
            }

        };

        /// <summary>
        /// 数据库表的索引操作：创建、判断与删除
        /// </summary>
        public static class XIndex
        {
            /// <summary>
            /// 唯一索引
            /// </summary>
            public static string gc_strUnique = "Unique";

            /// <summary>
            /// 缺省创建的索引，非聚集索引
            /// </summary>
            public static string gc_strDefault = "Nonclustered";

            /// <summary>
            /// 聚集索引索引，每个表只能创建一个
            /// </summary>                
            public static string gc_strCluster = "Clustered";

            //
            // Functions
            //

            /// <summary>
            /// 创建指定类型的索引；失败抛出SqlException异常
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strTable_">创建索引的表名</param>
            /// <param name="strIndex_">要创建的索引名</param>
            /// <param name="strType_">索引类型</param>
            /// <param name="strCols">索引对应的列，如“Col1 ASC, ..., Coln DESC”</param>
            public static void Create(SqlConnection conDatabase_, string strTable_, string strIndex_, string strType_, string strCols)
            {
                string strCreate = string.Format("Create {0} Index {1} On {2} ({3})", strType_, strIndex_, strTable_, strCols);
                XExcute.NonQuery(conDatabase_, strCreate);
            }

            /// <summary>
            /// 在指定列上创建指定的索引；失败抛出SqlException异常
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strTable_">要创建索引的表</param>
            /// <param name="strIndex_">要创建的索引名</param>
            /// <param name="strType_">索引类型</param>
            /// <param name="strCols_">要创建索引的列</param>
            public static void Create(SqlConnection conDatabase_, string strTable_, string strIndex_, string strType_, string[] strCols_)
            {
                Create(conDatabase_, strTable_, strIndex_, strType_, XTable.MergeCols(strCols_));
            }

            /// <summary>
            /// 判断索引是否存在；失败抛出SqlException异常
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strTable_">索引所在的表</param>
            /// <param name="strIndex_">索引名</param>
            /// <returns>存放，返回true；否则，返回false</returns>
            public static bool IsExisted(SqlConnection conDatabase_, string strTable_, string strIndex_)
            {
                string strExist = @"Select count(*) from dbo.sysindexes where " +
                        @"id=object_id('" + strTable_ + @"') " +
                        @"and name='" + strIndex_ + @"'";

                return (0 != (int)XExcute.Scalar(conDatabase_, strExist));
            }

            /// <summary>
            /// 删除索引；失败抛出SqlException异常
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strTable_">索引所在的表</param>
            /// <param name="strIndex_">索引名</param>
            public static void Delete(SqlConnection conDatabase_, string strTable_, string strIndex_)
            {
                string strDel = "Drop Index " + strTable_ + "." + strIndex_;
                XExcute.NonQuery(conDatabase_, strDel);
            }
        };

        /// <summary>
        /// Check约束
        /// </summary>
        public static class XCheck
        {
            /// <summary>
            /// 创建Check约束
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strTable_">表名</param>
            /// <param name="strCheck_">Check约束名</param>
            /// <param name="strExpr_">Check表达式</param>
            public static void Create(SqlConnection conDatabase_, string strTable_, string strCheck_, string strExpr_)
            {
                string strCreate = string.Format("Alter Table {0}  Add Constraint {1} Check({2})", strTable_, strCheck_, strExpr_);
                XExcute.NonQuery(conDatabase_, strCreate);
            }

            /// <summary>
            /// 删除Check约束
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strTable_">表名</param>
            /// <param name="strCheck_">Check约束名</param>
            public static void Delete(SqlConnection conDatabase_, string strTable_, string strCheck_)
            {
                string strDel = string.Format("Alter Table {0}  Drop Constraint {1}", strTable_, strCheck_);
                XExcute.NonQuery(conDatabase_, strDel);
            }
        };

        /// <summary>
        /// 数据库表中外键约束的操作：创建、判断与删除
        /// </summary>
        public static class XForeignKey
        {
            /// <summary>
            /// 无动作
            /// </summary>
            public const string gc_strNoAction = "NO ACTION";
            /// <summary>
            /// 递归处理
            /// </summary>
            public const string gc_strCascade = "CASCADE";
            /// <summary>
            /// 设为NULL
            /// </summary>
            public const string gc_strSetNull = "SET NULL";
            /// <summary>
            /// 设为缺省值
            /// </summary>
            public const string gc_strSetDefault = "SET DEFAULT";

            /// <summary>
            /// 创建表的外键约束
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strTable_">外键所在的表</param>
            /// <param name="strKeyName_">外键名</param>
            /// <param name="strCol_">外键所在的列</param>
            /// <param name="strRefrence_">外键约束（对其他表的引用，如Table(Col)）</param>
            public static void Create(SqlConnection conDatabase_, string strTable_, string strKeyName_, string strCol_, string strRefrence_)
            {
                Create(conDatabase_, strTable_, strKeyName_, strCol_, strRefrence_, gc_strNoAction, gc_strNoAction);
            }

            /// <summary>
            /// 创建表的外键约束
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strTable_">外键所在的表</param>
            /// <param name="strKeyName_">外键名</param>
            /// <param name="strCol_">外键所在的列</param>
            /// <param name="strRefrence_">外键约束（对其他表的引用，如Table(Col)）</param>
            /// <param name="strOnDel_">外键删除时的动作</param>
            /// <param name="strOnUpdate_">外键更新时的动作</param>
            public static void Create(SqlConnection conDatabase_, string strTable_, string strKeyName_, string strCol_, string strRefrence_, string strOnDel_, string strOnUpdate_)
            {
                string strCreate = string.Format("Alter Table {0} With Nocheck Add Constraint {1} Foreign Key ({2}) References {3}" +
                        " On Delete {4} On Update {5}", strTable_, strKeyName_, strCol_, strRefrence_, strOnDel_, strOnUpdate_);

                XExcute.NonQuery(conDatabase_, strCreate);
            }

            /// <summary>
            /// 判断外键是否存在
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strName_">外键名</param>
            /// <returns>存在，返回true；否则，返回false</returns>
            public static bool IsExisted(SqlConnection conDatabase_, string strName_)
            {
                string strExist = "Select count(*) from dbo.sysobjects where " +
                                            @"id=object_id('" + strName_ + @"') " +
                                            @"and ObjectProperty(id,N'IsForeignKey')=1";

                return (0 != (int)XExcute.Scalar(conDatabase_, strExist));
            }

            /// <summary>
            /// 删除外键
            /// </summary>
            /// <param name="conDatabase_">目标数据库的连接</param>
            /// <param name="strTable_">外键所在的表</param>
            /// <param name="strKeyName_">外键名</param>
            public static void Delete(SqlConnection conDatabase_, string strTable_, string strKeyName_)
            {
                string strDel = string.Format("Alter Table {0}  Drop Constraint {1}", strTable_, strKeyName_);
                XExcute.NonQuery(conDatabase_, strDel);
            }
        };

        /// <summary>
        /// 数据库存储过程创建、判断与删除
        /// </summary>
        public static class XProcedure
        {
            const string _strNotePrefix = "--";
            /// <summary>
            /// 把多条注释内容，转换为sql2005格式的注释字符串
            /// </summary>
            /// <param name="strNotes_">注释内容</param>
            /// <returns>注释字符串</returns>
            public static string GetNote(string[] strNotes_)
            {
                string strNote = "";
                foreach (string str in strNotes_)
                {
                    strNote += _strNotePrefix + str + "\n";
                }

                return strNote;
            }

            /// <summary>
            /// 创建有参数并带注释的存储过程
            /// </summary>
            /// <param name="conDatabase_">数据库连接</param>
            /// <param name="strName_">存储过程名</param>
            /// <param name="strBody_">存储过程体</param>
            /// <param name="strNote_">存储过程的注释</param>
            /// <param name="strParams_">存储过程参数列表</param>
            public static void Create(SqlConnection conDatabase_, string strName_, string strBody_, string strNote_, string[] strParams_)
            {
                string strCreate = strNote_ + "\nCreate Procedure dbo." + strName_ + " ";
                foreach (string strParam in strParams_)
                {
                    strCreate += "\n    " + strParam;
                    strCreate += ",";
                }
                strCreate = strCreate.TrimEnd(',');
                strCreate += "\nAs \n" + strBody_;

                SqlCommand cmdCreate = new SqlCommand(strCreate, conDatabase_);
                cmdCreate.ExecuteNonQuery();
            }

            /// <summary>
            /// 创建有参数的存储过程
            /// </summary>
            /// <param name="conDatabase_">数据库连接</param>
            /// <param name="strName_">存储过程名</param>
            /// <param name="strBody_">存储过程体</param>
            /// <param name="strParams_">存储过程参数列表</param>
            public static void Create(SqlConnection conDatabase_, string strName_, string strBody_, string[] strParams_)
            {
                string strCreate = "Create Procedure dbo." + strName_ + " ";
                foreach (string strParam in strParams_)
                {
                    strCreate += "\n    " + strParam;
                    strCreate += ",";
                }
                strCreate = strCreate.TrimEnd(',');
                strCreate += " \nAs \n" + strBody_;

                SqlCommand cmdCreate = new SqlCommand(strCreate, conDatabase_);
                cmdCreate.ExecuteNonQuery();
            }

            /// <summary>
            /// 创建没有参数的存储过程
            /// </summary>
            /// <param name="conDatabase_">数据库连接</param>
            /// <param name="strName_">存储过程名</param>
            /// <param name="strBody_">存储过程体</param>
            public static void Create(SqlConnection conDatabase_, string strName_, string strBody_)
            {
                string strCreate = "Create Procedure dbo." + strName_ + " \nAs \n" + strBody_;

                SqlCommand cmdCreate = new SqlCommand(strCreate, conDatabase_);
                cmdCreate.ExecuteNonQuery();
            }

            /// <summary>
            /// 判断存储过程是否存在
            /// </summary>
            /// <param name="conDatabase_">数据库连接</param>
            /// <param name="strName_">存储过程名</param>
            /// <returns>存在，返回true；否则，返回false</returns>
            public static bool IsExisted(SqlConnection conDatabase_, string strName_)
            {
                string strExist = "Select count(*) from dbo.sysobjects where " +
                        @"id=object_id('" + strName_ + @"') " +
                        @"and ObjectProperty(id,N'IsProcedure')=1";

                SqlCommand cmdExist = new SqlCommand(strExist, conDatabase_);
                return (0 != (int)cmdExist.ExecuteScalar());
            }

            /// <summary>
            /// 删除存储过程
            /// </summary>
            /// <param name="conDatabase_">数据库连接</param>
            /// <param name="strName_">存储过程名</param>
            public static void Delete(SqlConnection conDatabase_, string strName_)
            {
                string strDel = "Drop Procedure dbo." + strName_;

                SqlCommand cmdDel = new SqlCommand(strDel, conDatabase_);
                cmdDel.ExecuteNonQuery();
            }
        };

        /// <summary>
        /// 函数的创建、判断与删除
        /// </summary>
        public static class XFunction
        {
            /// <summary>
            /// 创建有参数、返回值的函数
            /// </summary>
            /// <param name="conDatabase_">数据库连接</param>
            /// <param name="strName_">函数名</param>
            /// <param name="strReturn_">返回类型</param>
            /// <param name="strBody_">函数体</param>
            /// <param name="strParams_">参数列表</param>
            public static void Create(SqlConnection conDatabase_, string strName_, string strBody_, string strReturn_, string[] strParams_)
            {
                string strCreate = "Create Function dbo." + strName_ + "\n    ( ";
                foreach (string strParam in strParams_)
                {
                    strCreate += "\n    " + strParam;
                    strCreate += ",";
                }
                strCreate = strCreate.TrimEnd(',');
                strCreate += "\n    ) " +
                        "\n    Returns " + strReturn_ +
                        "\nAs \nBegin \n" + strBody_ + " \nEnd";

                SqlCommand cmdCreate = new SqlCommand(strCreate, conDatabase_);
                cmdCreate.ExecuteNonQuery();
            }

            /// <summary>
            /// 创建无参数、有返回值的函数
            /// </summary>
            /// <param name="conDatabase_">数据库连接</param>
            /// <param name="strName_">函数名</param>
            /// <param name="strReturn_">返回类型</param>
            /// <param name="strBody_">函数体</param>
            public static void Create(SqlConnection conDatabase_, string strName_, string strBody_, string strReturn_)
            {
                string strCreate = "Create Procedure dbo." + strName_ + "\n    ()" +
                            "\n    Returns " + strReturn_ +
                            "\nAs \nBegin \n" + strBody_ + " \nEnd";

                SqlCommand cmdCreate = new SqlCommand(strCreate, conDatabase_);
                cmdCreate.ExecuteNonQuery();
            }

            /// <summary>
            /// 创建无参数、无返回值的函数
            /// </summary>
            /// <param name="conDatabase_">数据库连接</param>
            /// <param name="strName_">函数名</param>
            /// <param name="strBody_">函数体</param>
            public static void Create(SqlConnection conDatabase_, string strName_, string strBody_)
            {
                string strCreate = "Create Procedure dbo." + strName_ + "\n    ()" +
                            "\nAs \nBegin \n" + strBody_ + " \nEnd";

                SqlCommand cmdCreate = new SqlCommand(strCreate, conDatabase_);
                cmdCreate.ExecuteNonQuery();
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="conDatabase_"></param>
            /// <param name="strName_"></param>
            /// <returns></returns>
            public static bool IsExisted(SqlConnection conDatabase_, string strName_)
            {
                string strExist = "Select count(*) from dbo.sysobjects where " +
                        @"id=object_id('" + strName_ + @"') " +
                        @"and (ObjectProperty(id,N'IsScalarFunction')=1 Or ObjectProperty(id,N'IsTableFunction')=1)";

                SqlCommand cmdExist = new SqlCommand(strExist, conDatabase_);
                return (0 != (int)cmdExist.ExecuteScalar());
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="conDatabase_"></param>
            /// <param name="strName_"></param>
            public static void Delete(SqlConnection conDatabase_, string strName_)
            {
                string strDel = "Drop Function dbo." + strName_;

                SqlCommand cmdDel = new SqlCommand(strDel, conDatabase_);
                cmdDel.ExecuteNonQuery();
            }
        };
    }// XSqlServerHelper
} // Namespace