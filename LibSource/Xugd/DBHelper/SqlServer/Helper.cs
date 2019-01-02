using System;

using System.Data.SqlClient;

namespace SHCre.Xugd.DbHelper
{
    /// <summary>
    /// ���ݲ�����صĸ����ӿ�
    /// </summary>
    public static partial class XSqlServerHelper
    {
        /// <summary>
        /// ��ȡ��ǰ������ʱ��
        /// </summary>
        /// <param name="conDatabase_">Ŀ�����ݿ������</param>
        /// <returns>������ʱ��</returns>
        public static DateTime GetSrvTime(SqlConnection conDatabase_)
        {
            string strQuery = "Select Getdate() As CurTime";

            return (DateTime)XExcute.Scalar(conDatabase_, strQuery);
        }

        /// <summary>
        /// ���������ȡ��ҳ��
        /// </summary>
        /// <param name="nRowCount_">�ܵļ�¼����</param>
        /// <param name="nPageSize_">ÿҳ�ļ�¼����</param>
        /// <returns>��ҳ��</returns>
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
        /// ��ȡ��¼����
        /// </summary>
        /// <param name="conDatabase_">Ŀ�����ݿ������</param>
        /// <param name="strTableName_">����</param>
        /// <param name="strFilter_">����������null��ȡ���м�¼������</param>
        /// <returns>��¼����</returns>
        public static int GetRowCount(SqlConnection conDatabase_, string strTableName_, string strFilter_ = null)
        {
            if (string.IsNullOrEmpty(strFilter_))
                strFilter_ = "1=1";

            string strQuery = string.Format("Select Count(*) From {0} Where {1}",
                        strTableName_, strFilter_);

            return (int)XExcute.Scalar(conDatabase_, strQuery);
        }

        /// <summary>
        /// ���SqlCommand�е�Excute�ķ�װ
        /// </summary>
        public static class XExcute
        {
            /// <summary>
            /// ִ��û�з���ֵ��SQL���
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strSQL_">Ҫִ�е�SQL���</param>
            /// <returns>��Ӱ�������</returns>
            public static int NonQuery(SqlConnection conDatabase_, string strSQL_)
            {
                using (SqlCommand cmdSQL = new SqlCommand(strSQL_, conDatabase_))
                {
                    return cmdSQL.ExecuteNonQuery();
                }
            }

            /// <summary>
            /// ִ�з���һ�������SQL���
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strSQL_">Ҫִ�е�SQL���</param>
            /// <returns>���ؽ�����ĵ�һ�еĵ�һ��</returns>
            public static object Scalar(SqlConnection conDatabase_, string strSQL_)
            {
                using (SqlCommand cmdSQL = new SqlCommand(strSQL_, conDatabase_))
                {
                    return cmdSQL.ExecuteScalar();
                }
            }

            /// <summary>
            /// ִ�з��ؽ������SQL���
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strSQL_">Ҫִ�е�SQL���</param>
            /// <returns>���������</returns>
            public static SqlDataReader Reader(SqlConnection conDatabase_, string strSQL_)
            {
                using (SqlCommand cmdSQL = new SqlCommand(strSQL_, conDatabase_))
                {
                    return cmdSQL.ExecuteReader();
                }
            }
        };

        /// <summary>
        /// ���ݿ����
        /// </summary>
        public static class XDatabase
        {
            #region Const value define
            /// <summary>
            /// �����ݿ����������������ݿ����Ϣ
            /// </summary>
            public const string gc_strMaster = "Master";

            /// <summary>
            /// ���ݿ�Ĭ�����Ӷ˿�
            /// </summary>
            public const string gc_strDefPort = "1433";

            /// <summary>
            /// ���ݿ��ַ��˿ںż�ķָ���
            /// </summary>
            public const string gc_strSeparator = ",";

            /// <summary>
            /// ���ݿ��ַ��˿ںż�ķָ���
            /// </summary>
            public const char gc_chSeparator = ',';
            #endregion

            #region Connect functions
            /// <summary>
            /// ��ȡ���ݿ�Դ�ĵ�ַ��ʹ�ö������� "address,port"
            /// </summary>
            /// <param name="strDBAddr_">���ݿ�Դ</param>
            /// <param name="nPort_">���ݿ�˿ں�</param>
            /// <returns>���ݿ�Դ�ĵ�ַ</returns>
            public static string BuildSource(string strDBAddr_, int nPort_)
            {
                return BuildSource(strDBAddr_, nPort_.ToString());
            }

            /// <summary>
            /// ��ȡ���ݿ�Դ�ĵ�ַ��ʹ�ö������� "address,port"
            /// </summary>
            /// <param name="strDBAddr_">���ݿ�Դ</param>
            /// <param name="strPort_">���ݿ�˿ں�</param>
            /// <returns>���ݿ�Դ�ĵ�ַ</returns>
            public static string BuildSource(string strDBAddr_, string strPort_)
            {
                return strDBAddr_ + gc_strSeparator + strPort_;
            }

            /// <summary>
            /// ������������������ݿ⣺�ɹ�����SqlConnection���ӣ������׳�SqlException�쳣
            /// </summary>
            /// <param name="strConnect_">���Ӳ����ַ���</param>
            /// <returns></returns>
            public static SqlConnection Connect(string strConnect_)
            {
                SqlConnection conDatabase = new SqlConnection(strConnect_);
                conDatabase.Open();

                return conDatabase;
            }

            /// <summary>
            /// ������������������ݿ⣺�ɹ�����SqlConnection���ӣ������׳�SqlException�쳣
            /// </summary>
            /// <param name="strDBAddr_">���ݿ�Դ�ĵ�ַ</param>
            /// <param name="nPort_">���ݿ�˿ں�</param>
            /// <param name="strDBName_">Ҫ���ӵ����ݿ���</param>
            /// <param name="strUserName_">��¼���ݿ���û���</param>
            /// <param name="strUserPwd_">��¼���ݿ������</param>
            /// <returns>����SqlConnection����</returns>
            public static SqlConnection Connect(string strDBAddr_, int nPort_, string strDBName_, string strUserName_, string strUserPwd_)
            {
                SqlConnection conDatabase = new SqlConnection(ConnectString(strDBAddr_, nPort_, strDBName_, strUserName_, strUserPwd_));
                conDatabase.Open();

                return conDatabase;
            }

            /// <summary>
            /// ������������������ݿ⣺�ɹ�����SqlConnection���ӣ������׳�SqlException�쳣
            /// </summary>
            /// <param name="strSource">���ݿ�Դ��ʹ�ö�������"address,port"</param>
            /// <param name="strDBName_">Ҫ���ӵ����ݿ���</param>
            /// <param name="strUserName_">��¼���ݿ���û���</param>
            /// <param name="strUserPwd_">��¼���ݿ������</param>
            /// <returns>����SqlConnection����</returns>
            public static SqlConnection Connect(string strSource, string strDBName_, string strUserName_, string strUserPwd_)
            {
                SqlConnection conDatabase = new SqlConnection(ConnectString(strSource, strDBName_, strUserName_, strUserPwd_));
                conDatabase.Open();
                return conDatabase;
            }

            /// <summary>
            /// �������������ȡ�����ַ���
            /// </summary>
            /// <param name="strDBAddr_">���ݿ�Դ</param>
            /// <param name="nPort_">���ݿ�˿ں�</param>
            /// <param name="strDBName_">Ҫ���ӵ����ݿ���</param>
            /// <param name="strUserName_">��¼���ݿ���û���</param>
            /// <param name="strUserPwd_">��¼���ݿ������</param>
            /// <returns>�����ַ���</returns>
            public static string ConnectString(string strDBAddr_, int nPort_, string strDBName_, string strUserName_, string strUserPwd_)
            {
                return ("Data Source=" + strDBAddr_ + gc_strSeparator + nPort_.ToString() +
                        ";Initial Catalog=" + strDBName_ +
                        ";User ID=" + strUserName_ +
                        ";Password=" + strUserPwd_);
            }

            /// <summary>
            /// �������ַ����л�ȡ������Ϣ
            /// </summary>
            /// <param name="strConString_">�����ַ���</param>
            /// <param name="strDBAddr_">���ݿ�Դ</param>
            /// <param name="nPort_">���ݿ�˿ں�</param>
            /// <param name="strDBName_">Ҫ���ӵ����ݿ���</param>
            /// <param name="strUserName_">��¼���ݿ���û���</param>
            /// <param name="strUserPwd_">��¼���ݿ������</param>
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
            /// �������������ȡ�����ַ���
            /// </summary>
            /// <param name="strDBSource_">���ݿ�Դ�ĵ�ַ��ʹ�ö������� "address,port"</param>
            /// <param name="strDBName_">Ҫ���ӵ����ݿ���</param>
            /// <param name="strUserName_">��¼���ݿ���û���</param>
            /// <param name="strUserPwd_">��¼���ݿ������</param>
            /// <returns>�����ַ���</returns>
            public static string ConnectString(string strDBSource_, string strDBName_, string strUserName_, string strUserPwd_)
            {
                return ("Data Source=" + strDBSource_ +
                        ";Initial Catalog=" + strDBName_ +
                        ";User ID=" + strUserName_ +
                        ";Password=" + strUserPwd_);
            }
            #endregion

            /// <summary>
            /// �ж����ݿ��Ƿ���ڣ�ʧ���׳�SqlException�쳣
            /// </summary>
            /// <param name="strDBSource_">���ݿ�Դ�ĵ�ַ��ʹ�ö������� "address,port"</param>
            /// <param name="strDBName_">Ҫ�жϵ����ݿ���</param>
            /// <param name="strUserName_">��¼���ݿ���û���</param>
            /// <param name="strUserPwd_">��¼���ݿ������</param>
            /// <returns>���ڣ�����true�����򣬷���false</returns>
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
            /// ��������Ĳ����������ݿ⣺���ʧ�ܣ����׳�SqlExceptioni�쳣
            /// </summary>
            /// <param name="strDBSource_">���ݿ�Դ�ĵ�ַ��ʹ�ö������� "address,port"</param>
            /// <param name="strDBName_">Ҫ���������ݿ���</param>
            /// <param name="strDBPath_">���������ݿ�Ĵ��·��</param>
            /// <param name="strUserName_">��¼���ݿ���û���</param>
            /// <param name="strUserPwd_">��¼���ݿ������</param>
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
            /// ɾ��ָ�������ݿ⣻ʧ���׳�SqlException�쳣
            /// </summary>
            /// <param name="strDBSource_">���ݿ�Դ�ĵ�ַ��ʹ�ö������� "address,port"</param>
            /// <param name="strDBName_">Ҫɾ�������ݿ���</param>
            /// <param name="strUserName_">��¼���ݿ���û���</param>
            /// <param name="strUserPwd_">��¼���ݿ������</param>
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
        /// ���ݿ�������������ɾ������жϱ��Ƿ����
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
            /// �����ݿ��д�����ʧ���׳�SqlException�쳣
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strTableName_">Ҫ���������ݱ���</param>
            /// <param name="strColumns_">Ҫ�����ĸ��У�ÿ����Ϊһ���ַ������������������ԣ�����"FileID int Not NULL Identity"</param>
            public static void Create(SqlConnection conDatabase_, string strTableName_, string[] strColumns_)
            {
                string strCreate = string.Format("Create Table {0} ({1})", strTableName_, MergeCols(strColumns_));

                XExcute.NonQuery(conDatabase_, strCreate);
            }

            /// <summary>
            /// �����ݿ��д�����ʧ���׳�SqlException�쳣
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strTableName_">Ҫ���������ݱ���</param>
            /// <param name="strPKCol_">������</param>
            /// <param name="strColumns_">Ҫ�����ĸ��У�ÿ����Ϊһ���ַ������������������ԣ�����"FileID int Not NULL Identity"</param>
            public static void Create(SqlConnection conDatabase_, string strTableName_, string strPKCol_, string[] strColumns_)
            {
                string strKeyName = string.Format("PK_{0}_{1}", strTableName_, strPKCol_.Split(' ', ',')[0]);
                string strCreate = string.Format("Create Table {0} ( {1}, Constraint {2} Primary Key Clustered ({3}) )",
                        strTableName_, MergeCols(strColumns_), strKeyName, strPKCol_);

                XExcute.NonQuery(conDatabase_, strCreate);
            }

            /// <summary>
            /// �ж����ݿ��еı��Ƿ��Ѵ��ڣ�ʧ���׳�SqlException�쳣
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strTableName_">Ҫ�жϵı���</param>
            /// <returns>���ڣ�����true�����򣬷���false</returns>
            public static bool IsExisted(SqlConnection conDatabase_, string strTableName_)
            {
                string strExist = @"Select count(*) from dbo.sysobjects where " +
                        @"id=object_id('" + strTableName_ + @"') " +
                        @"and ObjectProperty(id,N'IsUserTable')=1";

                return (0 != (int)XExcute.Scalar(conDatabase_, strExist));
            }

            /// <summary>
            /// ɾ��ָ�������ݿ��ʧ���׳�SqlException�쳣
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strTableName_">Ҫɾ�������ݱ���</param>
            public static void Delete(SqlConnection conDatabase_, string strTableName_)
            {
                string strDel = "Drop Table " + strTableName_;
                XExcute.NonQuery(conDatabase_, strDel);
            }
        };

        /// <summary>
        /// ���ݿ���ͼ������������ͼ��ɾ����ͼ�Լ��ж���ͼ�Ƿ����
        /// </summary>
        public static class XView
        {
            /// <summary>
            /// �����ݿ��д�����ͼ��ʧ���׳�SqlException�쳣
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strViewName_">��ͼ����</param>
            /// <param name="strViewBody_">��ͼ�壬����ͼ��β�����sql���</param>
            public static void Create(SqlConnection conDatabase_, string strViewName_, string strViewBody_)
            {
                string strCreate = "Create View " + strViewName_ +
                        " as " + strViewBody_;

                XExcute.NonQuery(conDatabase_, strCreate);
            }

            /// <summary>
            /// �ж���ͼ�Ƿ���ڣ�ʧ���׳�SqlException�쳣
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strViewName_">��ͼ����</param>
            /// <returns>��ţ�����true�����򣬷���false</returns>
            public static bool IsExist(SqlConnection conDatabase_, string strViewName_)
            {
                string strExist = @"Select count(*) from dbo.sysobjects where " +
                        @"id=object_id('" + strViewName_ + @"') " +
                        @"and ObjectProperty(id,N'IsView')=1";

                return (0 != (int)XExcute.Scalar(conDatabase_, strExist));
            }

            /// <summary>
            /// �����ݿ���ɾ��ָ������ͼ��ʧ���׳�SqlException�쳣
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strViewName_">��ͼ����</param>
            public static void Delete(SqlConnection conDatabase_, string strViewName_)
            {
                string strDel = "Drop View " + strViewName_;
                XExcute.NonQuery(conDatabase_, strDel);
            }

        };

        /// <summary>
        /// ���ݿ��������������������ж���ɾ��
        /// </summary>
        public static class XIndex
        {
            /// <summary>
            /// Ψһ����
            /// </summary>
            public static string gc_strUnique = "Unique";

            /// <summary>
            /// ȱʡ�������������Ǿۼ�����
            /// </summary>
            public static string gc_strDefault = "Nonclustered";

            /// <summary>
            /// �ۼ�����������ÿ����ֻ�ܴ���һ��
            /// </summary>                
            public static string gc_strCluster = "Clustered";

            //
            // Functions
            //

            /// <summary>
            /// ����ָ�����͵�������ʧ���׳�SqlException�쳣
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strTable_">���������ı���</param>
            /// <param name="strIndex_">Ҫ������������</param>
            /// <param name="strType_">��������</param>
            /// <param name="strCols">������Ӧ���У��硰Col1 ASC, ..., Coln DESC��</param>
            public static void Create(SqlConnection conDatabase_, string strTable_, string strIndex_, string strType_, string strCols)
            {
                string strCreate = string.Format("Create {0} Index {1} On {2} ({3})", strType_, strIndex_, strTable_, strCols);
                XExcute.NonQuery(conDatabase_, strCreate);
            }

            /// <summary>
            /// ��ָ�����ϴ���ָ����������ʧ���׳�SqlException�쳣
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strTable_">Ҫ���������ı�</param>
            /// <param name="strIndex_">Ҫ������������</param>
            /// <param name="strType_">��������</param>
            /// <param name="strCols_">Ҫ������������</param>
            public static void Create(SqlConnection conDatabase_, string strTable_, string strIndex_, string strType_, string[] strCols_)
            {
                Create(conDatabase_, strTable_, strIndex_, strType_, XTable.MergeCols(strCols_));
            }

            /// <summary>
            /// �ж������Ƿ���ڣ�ʧ���׳�SqlException�쳣
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strTable_">�������ڵı�</param>
            /// <param name="strIndex_">������</param>
            /// <returns>��ţ�����true�����򣬷���false</returns>
            public static bool IsExisted(SqlConnection conDatabase_, string strTable_, string strIndex_)
            {
                string strExist = @"Select count(*) from dbo.sysindexes where " +
                        @"id=object_id('" + strTable_ + @"') " +
                        @"and name='" + strIndex_ + @"'";

                return (0 != (int)XExcute.Scalar(conDatabase_, strExist));
            }

            /// <summary>
            /// ɾ��������ʧ���׳�SqlException�쳣
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strTable_">�������ڵı�</param>
            /// <param name="strIndex_">������</param>
            public static void Delete(SqlConnection conDatabase_, string strTable_, string strIndex_)
            {
                string strDel = "Drop Index " + strTable_ + "." + strIndex_;
                XExcute.NonQuery(conDatabase_, strDel);
            }
        };

        /// <summary>
        /// CheckԼ��
        /// </summary>
        public static class XCheck
        {
            /// <summary>
            /// ����CheckԼ��
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strTable_">����</param>
            /// <param name="strCheck_">CheckԼ����</param>
            /// <param name="strExpr_">Check���ʽ</param>
            public static void Create(SqlConnection conDatabase_, string strTable_, string strCheck_, string strExpr_)
            {
                string strCreate = string.Format("Alter Table {0}  Add Constraint {1} Check({2})", strTable_, strCheck_, strExpr_);
                XExcute.NonQuery(conDatabase_, strCreate);
            }

            /// <summary>
            /// ɾ��CheckԼ��
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strTable_">����</param>
            /// <param name="strCheck_">CheckԼ����</param>
            public static void Delete(SqlConnection conDatabase_, string strTable_, string strCheck_)
            {
                string strDel = string.Format("Alter Table {0}  Drop Constraint {1}", strTable_, strCheck_);
                XExcute.NonQuery(conDatabase_, strDel);
            }
        };

        /// <summary>
        /// ���ݿ�������Լ���Ĳ������������ж���ɾ��
        /// </summary>
        public static class XForeignKey
        {
            /// <summary>
            /// �޶���
            /// </summary>
            public const string gc_strNoAction = "NO ACTION";
            /// <summary>
            /// �ݹ鴦��
            /// </summary>
            public const string gc_strCascade = "CASCADE";
            /// <summary>
            /// ��ΪNULL
            /// </summary>
            public const string gc_strSetNull = "SET NULL";
            /// <summary>
            /// ��Ϊȱʡֵ
            /// </summary>
            public const string gc_strSetDefault = "SET DEFAULT";

            /// <summary>
            /// ����������Լ��
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strTable_">������ڵı�</param>
            /// <param name="strKeyName_">�����</param>
            /// <param name="strCol_">������ڵ���</param>
            /// <param name="strRefrence_">���Լ����������������ã���Table(Col)��</param>
            public static void Create(SqlConnection conDatabase_, string strTable_, string strKeyName_, string strCol_, string strRefrence_)
            {
                Create(conDatabase_, strTable_, strKeyName_, strCol_, strRefrence_, gc_strNoAction, gc_strNoAction);
            }

            /// <summary>
            /// ����������Լ��
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strTable_">������ڵı�</param>
            /// <param name="strKeyName_">�����</param>
            /// <param name="strCol_">������ڵ���</param>
            /// <param name="strRefrence_">���Լ����������������ã���Table(Col)��</param>
            /// <param name="strOnDel_">���ɾ��ʱ�Ķ���</param>
            /// <param name="strOnUpdate_">�������ʱ�Ķ���</param>
            public static void Create(SqlConnection conDatabase_, string strTable_, string strKeyName_, string strCol_, string strRefrence_, string strOnDel_, string strOnUpdate_)
            {
                string strCreate = string.Format("Alter Table {0} With Nocheck Add Constraint {1} Foreign Key ({2}) References {3}" +
                        " On Delete {4} On Update {5}", strTable_, strKeyName_, strCol_, strRefrence_, strOnDel_, strOnUpdate_);

                XExcute.NonQuery(conDatabase_, strCreate);
            }

            /// <summary>
            /// �ж�����Ƿ����
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strName_">�����</param>
            /// <returns>���ڣ�����true�����򣬷���false</returns>
            public static bool IsExisted(SqlConnection conDatabase_, string strName_)
            {
                string strExist = "Select count(*) from dbo.sysobjects where " +
                                            @"id=object_id('" + strName_ + @"') " +
                                            @"and ObjectProperty(id,N'IsForeignKey')=1";

                return (0 != (int)XExcute.Scalar(conDatabase_, strExist));
            }

            /// <summary>
            /// ɾ�����
            /// </summary>
            /// <param name="conDatabase_">Ŀ�����ݿ������</param>
            /// <param name="strTable_">������ڵı�</param>
            /// <param name="strKeyName_">�����</param>
            public static void Delete(SqlConnection conDatabase_, string strTable_, string strKeyName_)
            {
                string strDel = string.Format("Alter Table {0}  Drop Constraint {1}", strTable_, strKeyName_);
                XExcute.NonQuery(conDatabase_, strDel);
            }
        };

        /// <summary>
        /// ���ݿ�洢���̴������ж���ɾ��
        /// </summary>
        public static class XProcedure
        {
            const string _strNotePrefix = "--";
            /// <summary>
            /// �Ѷ���ע�����ݣ�ת��Ϊsql2005��ʽ��ע���ַ���
            /// </summary>
            /// <param name="strNotes_">ע������</param>
            /// <returns>ע���ַ���</returns>
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
            /// �����в�������ע�͵Ĵ洢����
            /// </summary>
            /// <param name="conDatabase_">���ݿ�����</param>
            /// <param name="strName_">�洢������</param>
            /// <param name="strBody_">�洢������</param>
            /// <param name="strNote_">�洢���̵�ע��</param>
            /// <param name="strParams_">�洢���̲����б�</param>
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
            /// �����в����Ĵ洢����
            /// </summary>
            /// <param name="conDatabase_">���ݿ�����</param>
            /// <param name="strName_">�洢������</param>
            /// <param name="strBody_">�洢������</param>
            /// <param name="strParams_">�洢���̲����б�</param>
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
            /// ����û�в����Ĵ洢����
            /// </summary>
            /// <param name="conDatabase_">���ݿ�����</param>
            /// <param name="strName_">�洢������</param>
            /// <param name="strBody_">�洢������</param>
            public static void Create(SqlConnection conDatabase_, string strName_, string strBody_)
            {
                string strCreate = "Create Procedure dbo." + strName_ + " \nAs \n" + strBody_;

                SqlCommand cmdCreate = new SqlCommand(strCreate, conDatabase_);
                cmdCreate.ExecuteNonQuery();
            }

            /// <summary>
            /// �жϴ洢�����Ƿ����
            /// </summary>
            /// <param name="conDatabase_">���ݿ�����</param>
            /// <param name="strName_">�洢������</param>
            /// <returns>���ڣ�����true�����򣬷���false</returns>
            public static bool IsExisted(SqlConnection conDatabase_, string strName_)
            {
                string strExist = "Select count(*) from dbo.sysobjects where " +
                        @"id=object_id('" + strName_ + @"') " +
                        @"and ObjectProperty(id,N'IsProcedure')=1";

                SqlCommand cmdExist = new SqlCommand(strExist, conDatabase_);
                return (0 != (int)cmdExist.ExecuteScalar());
            }

            /// <summary>
            /// ɾ���洢����
            /// </summary>
            /// <param name="conDatabase_">���ݿ�����</param>
            /// <param name="strName_">�洢������</param>
            public static void Delete(SqlConnection conDatabase_, string strName_)
            {
                string strDel = "Drop Procedure dbo." + strName_;

                SqlCommand cmdDel = new SqlCommand(strDel, conDatabase_);
                cmdDel.ExecuteNonQuery();
            }
        };

        /// <summary>
        /// �����Ĵ������ж���ɾ��
        /// </summary>
        public static class XFunction
        {
            /// <summary>
            /// �����в���������ֵ�ĺ���
            /// </summary>
            /// <param name="conDatabase_">���ݿ�����</param>
            /// <param name="strName_">������</param>
            /// <param name="strReturn_">��������</param>
            /// <param name="strBody_">������</param>
            /// <param name="strParams_">�����б�</param>
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
            /// �����޲������з���ֵ�ĺ���
            /// </summary>
            /// <param name="conDatabase_">���ݿ�����</param>
            /// <param name="strName_">������</param>
            /// <param name="strReturn_">��������</param>
            /// <param name="strBody_">������</param>
            public static void Create(SqlConnection conDatabase_, string strName_, string strBody_, string strReturn_)
            {
                string strCreate = "Create Procedure dbo." + strName_ + "\n    ()" +
                            "\n    Returns " + strReturn_ +
                            "\nAs \nBegin \n" + strBody_ + " \nEnd";

                SqlCommand cmdCreate = new SqlCommand(strCreate, conDatabase_);
                cmdCreate.ExecuteNonQuery();
            }

            /// <summary>
            /// �����޲������޷���ֵ�ĺ���
            /// </summary>
            /// <param name="conDatabase_">���ݿ�����</param>
            /// <param name="strName_">������</param>
            /// <param name="strBody_">������</param>
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