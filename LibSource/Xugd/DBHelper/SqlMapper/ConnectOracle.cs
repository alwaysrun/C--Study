using System;
using System.Data;
using System.Collections.Generic;
using System.Linq;

using System.Data.OracleClient;
//using Oracle.DataAccess.Client;
using System.Reflection;
using SHCre.Xugd.CFile;
using System.IO;

namespace SHCre.Xugd.DbHelper
{
    partial class XDapperORM
    {
        /// <summary>
        /// Oracle数据库连接类：使用Oracle时，文件所在的路径中不能包含括号"()"，且需要库‘Oracle.ManagedDataAccess.dll’；
        /// 在Win7-64位下，以管理员方式运行才可以正常连接；
        /// 整个应用程序中DbCreateFrom要统一（同一程序中，不要使用多种类型数据库连接方式）
        /// </summary>
        public class ConnectOracle : IConnectBase
        {
            /// <summary>
            /// 获取当前数据库名称"Oracle"
            /// </summary>
            public string Name { get { return "Oracle"; } }
            /// <summary>
            /// Sql语句中参数的前缀":"
            /// </summary>
            public string ParamPrefix { get { return ":"; } }
            /// <summary>
            /// 为完成操作时所需的虚表" From Dual"
            /// </summary>
            public string FromDual { get { return " From Dual"; } }
            /// <summary>
            /// 数据库连接创建方式（默认使用Oracle.DataAccess.Client，
            /// 如果设为Oracle_Net则使用System.Data.OracleClient）
            /// </summary>
            public XDatabaseType DbCreateFrom {get;set;}
            /// <summary>
            /// 连接字符串
            /// </summary>
            public string ConnectString { get; set; }

            /// <summary>
            /// 获取可用于条件子句的时间值："to_date(Format Value, 'Format')"
            /// </summary>
            /// <param name="dtDate_"></param>
            /// <param name="strDateFormat_">日期格式化字符串</param>
            /// <returns></returns>
            public string GetDateTimeValue(DateTime dtDate_, string strDateFormat_)
            {
                string strOracle = strDateFormat_.Replace("HH","hh24");
                strOracle = strOracle.Replace("mm", "mi");
                return string.Format("to_date('{0}', '{1}')", dtDate_.ToString(strDateFormat_), strOracle);
            }

            /// <summary>
            /// 获取插入或更新的sql语句
            /// </summary>
            /// <param name="strTableName_"></param>
            /// <param name="strKeyColName_"></param>
            /// <param name="lstColNames_"></param>
            /// <returns></returns>
            public string InsertOrUpdateSql(string strTableName_, string strKeyColName_, List<string> lstColNames_)
            {
                string strCount = string.Format("(Select Count(*) As CO From {0} Where {1})", strTableName_, BuildColParam(strKeyColName_, ParamPrefix));
                string strValues = string.Join(",", lstColNames_.Select(z => ParamPrefix + z));
                string strSet = string.Join(",", lstColNames_.Where(zk => zk != strKeyColName_).Select(zp => BuildColParam(zp, ParamPrefix)));

                return string.Format("Merge into {0} using {1} t On(t.CO<>0) " +
                    "When matched then update set {2} Where {3} " +
                    "When not matched then insert ({4}) values({5})", 
                    strTableName_,
                    strCount,
                    strSet,
                    BuildColParam(strKeyColName_, ParamPrefix),
                    string.Join(",", lstColNames_),
                    strValues);
            }

            class CSeqValue
            {
                public int XSeqNext { get; set; }
            }
            /// <summary>
            /// 获取序列的下一个值，需要先建立一个序列：
            /// Create Sequence XMySeq
            ///     Minvalue 1
            ///     Maxvalue 999
            ///     Cycle
            /// </summary>
            /// <param name="strName_">序列名称</param>
            /// <returns></returns>
            public int GetSeqNextVal(string strName_)
            {
                string strSql = string.Format("Select {0}.NextVal As XSeqNext {1}", strName_, FromDual);
                using(var con = GetConnect())
                {
                    var seqFirst = Dapper.SqlMapper.Query<CSeqValue>(con, strSql).First();
                    return seqFirst.XSeqNext;
                }
            }

            /// <summary>
            /// 分页时的子集
            /// </summary>
            /// <param name="strTableName_"></param>
            /// <param name="strOrderClause_"></param>
            /// <param name="strWhereClauses_"></param>
            /// <returns></returns>
            public string GetPagedSubset(string strTableName_, string strOrderClause_, string strWhereClauses_)
            {
                return string.Format("Select t.*, rownum as x_rn from {0} t {1} {2}",
                    strTableName_, strWhereClauses_, strOrderClause_);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="strDBAddr_">数据库地址</param>
            /// <param name="nPort_">端口号</param>
            /// <param name="strUserName_">用户名</param>
            /// <param name="strUserPwd_">密码</param>
            /// <param name="strServiceName_">oracle服务名</param>
            /// <param name="euType_">数据库创建方式</param>
            public ConnectOracle(string strDBAddr_, int nPort_, string strUserName_, string strUserPwd_, string strServiceName_ = "ORCL", XDatabaseType euType_ = XDatabaseType.Oracle_Net)
            {
                DbCreateFrom = euType_;
                ConnectString = BuildConnectString(strDBAddr_, nPort_, strServiceName_, strUserName_, strUserPwd_);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="strDBAddr_">数据库地址</param>
            /// <param name="strUserName_">用户名</param>
            /// <param name="strUserPwd_">密码</param>
            /// <param name="strServiceName_">oracle数据库实例名称（非本地oraClient中的‘服务命名’，而是其对应‘服务标识’中的‘服务名’）</param>
            /// <param name="euType_">数据库类型</param>
            public ConnectOracle(string strDBAddr_, string strUserName_, string strUserPwd_, string strServiceName_ = "ORCL", XDatabaseType euType_=XDatabaseType.Oracle_Net)
                : this(strDBAddr_, 1521, strUserName_, strUserPwd_, strServiceName_, euType_)
            { }

            /// <summary>
            /// 通过配置文件构造
            /// </summary>
            /// <param name="dbConfig_"></param>
            public ConnectOracle(XDatabaseConfig dbConfig_)
            {
                DbCreateFrom = dbConfig_.Type;
                ConnectString = BuildConnectString(dbConfig_.Address, dbConfig_.Port, dbConfig_.ServiceOrDbName, dbConfig_.UserName, dbConfig_.Password);
            }

            Assembly _assOracleDll = null;
            /// <summary>
            /// 获取连接
            /// </summary>
            /// <returns></returns>
            public IDbConnection GetConnect()
            {
                if (XDatabaseType.Oracle_Net == DbCreateFrom)
                {
                    return GetConnectOfNet();
                }
                else
                {
                    // 使用Oracle11中提取的库（分X86与X64）进行数据库连接
                    //string strFile = @"OracleLib\Oracle.DataAccess.dll";
                    //var assDll = Assembly.LoadFrom(strFile);
                    //var con = assDll.CreateInstance("Oracle.DataAccess.Client.OracleConnection") as IDbConnection;
                    //con.ConnectionString = ConnectString;
                    //var con = new Oracle.DataAccess.Client.OracleConnection(ConnectString);

                    // 使用Oracle最新提供的托管库进行数据库连接
                    if (_assOracleDll == null)
                    {
                        string strFile = XPath.GetFullPath("Oracle.ManagedDataAccess.dll");
                        //File.AppendAllText(@"d:\dbtest.txt", strFile + "\n");
                        _assOracleDll = Assembly.LoadFrom(strFile);
                    }
                    var con = _assOracleDll.CreateInstance("Oracle.ManagedDataAccess.Client.OracleConnection") as IDbConnection;
                    con.ConnectionString = ConnectString;
                    //var con = new Oracle.ManagedDataAccess.Client.OracleConnection(ConnectString);

                    con.Open();
                    return con;
                }
            }

            private IDbConnection GetConnectOfNet()
            {
#pragma warning disable 618
                var con = new System.Data.OracleClient.OracleConnection(ConnectString);
                con.Open();
#pragma warning restore 618
                return con;
            }

            internal static string BuildConnectString(string strDBAddr_, int nPort_, string strServiceName_, string strUserName_, string strUserPwd_)
            {
                string strDescript;
                if (string.IsNullOrEmpty(strDBAddr_))
                { // 使用本地Oracle客户端的配置信息进行连接
                    strDescript = strServiceName_;
                }
                else
                { // 直接连接数据库
                    string strAddress = string.Format("(ADDRESS=(PROTOCOL=TCP)(HOST={0})(PORT={1}))", strDBAddr_, nPort_);
                    string strConData;
                    strServiceName_ = strServiceName_.Trim();
                    if (string.IsNullOrEmpty(strServiceName_))
                    {
                        strConData = "(SERVICE_NAME=ORCL)";
                    }
                    else
                    {
                        string[] argNames = strServiceName_.Split('=');
                        if (argNames.Length == 1)
                        {
                            strConData = string.Format("(SERVICE_NAME={0})", strServiceName_);
                        }
                        else
                        {
                            if (strServiceName_[0] == '(')
                                strConData = strServiceName_;
                            else
                                strConData = string.Format("({0})", strServiceName_);
                        }
                    }
                    //string strConData = string.Format("(CONNECT_DATA =({0}))",string.Format("SERVICE_NAME={0}", strServiceName_));
                    strDescript = string.Format("(DESCRIPTION=(ADDRESS_LIST={0})(CONNECT_DATA={1}))", strAddress, strConData);
                }

                return string.Format("Data Source={0};user id={1};password={2}",
                        strDescript,
                        strUserName_,
                        strUserPwd_);
            }
        }
    }
}
