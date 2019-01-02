using System;
using System.Data;
using System.Data.SqlClient;
using System.Collections.Generic;
using System.Linq;

namespace SHCre.Xugd.DbHelper
{
    partial class XDapperORM
    {
        /// <summary>
        /// SqlServer数据库连接类
        /// </summary>
        public class ConnectSqlServer : IConnectBase
        {
            /// <summary>
            /// 获取当前数据库名称"SqlServer"
            /// </summary>
            public string Name { get { return "SqlServer"; } }
            /// <summary>
            /// Sql语句中参数的前缀"@"
            /// </summary>
            public string ParamPrefix { get { return "@"; } }
            /// <summary>
            /// 为完成操作时所需的虚表""
            /// </summary>
            public string FromDual { get { return string.Empty; } }
            /// <summary>
            /// 连接字符串
            /// </summary>
            public string ConnectString { get; set; }

            /// <summary>
            /// 获取可用于条件子句的时间值：'Format Value'
            /// </summary>
            /// <param name="dtDate_"></param>
            /// <param name="strDateFormat_">日期格式化字符串</param>
            /// <returns></returns>
            public string GetDateTimeValue(DateTime dtDate_, string strDateFormat_)
            {
                return string.Format("'{0}'", dtDate_.ToString(strDateFormat_));
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
                string strCount = string.Format("Exists(Select * From {0} Where {1})", strTableName_, BuildColParam(strKeyColName_, ParamPrefix));

                string strCols = string.Join(",", lstColNames_);
                string strValues = string.Join(",", lstColNames_.Select(z => ParamPrefix + z));
                string strInsert = string.Format("Insert Into {0} ({1}) Values ({2})", strTableName_, strCols, strValues);

                string strSet = string.Join(",", lstColNames_.Where(zk => zk != strKeyColName_).Select(zp => BuildColParam(zp, ParamPrefix)));
                string strUpdate = string.Format("Update {0} Set {1} Where {2}", strTableName_, strSet,
                    BuildColParam(strKeyColName_, ParamPrefix));

                return string.Format("If {0} {1} Else {2}", strCount, strUpdate, strInsert);
            }

            /// <summary>
            /// 获取序列的下一个值，通过建立一个包含自增长列的表来模拟；
            /// 每次插入数据并获取Indentity（Scope_identity()），然后删除新插入的数据；
            /// 如果Indentity达到最大值，则重设（DBCC CheckIdent）:
            /// 
            /// Table:
            /// CREATE TABLE [dbo].[XMySeq](
            ///     [SeqId] [int] IDENTITY(1,1) NOT NULL,
            ///     [SeqValue] [char](1) NOT NULL)
            ///     
            /// Produre:
            /// ALTER PROCEDURE dbo.XSeqGetValue
            /// (
            ///     @NextVal int OUTPUT
            /// )
            /// AS
            ///     SET NOCOUNT ON;
            /// 
            ///     Begin Try
            ///         Begin Tran
            ///         Insert Into [dbo].[XMySeq](SeqValue) Values('0');
            ///         Set  @NextVal = Scope_identity(); 
            ///         Delete From [dbo].[XMySeq] With(Readpast);
            /// 
            ///         -- 达到最大值后，重设
            ///         if(@NextVal>=254)
            ///         Begin
            ///             DBCC CheckIdent('[dbo].[XMySeq]', Reseed, 0);
            ///         End;
            ///         Commit Tran
            ///     End Try
            ///         Begin Catch
            ///         Rollback Tran;
            /// 
            ///         DECLARE @ErrorMessage NVARCHAR(4000);
            ///         DECLARE @ErrorSeverity INT;
            ///         DECLARE @ErrorState INT;	
            ///         SELECT
            ///         @ErrorMessage = ERROR_MESSAGE(),
            ///         @ErrorSeverity = ERROR_SEVERITY(),
            ///         @ErrorState = ERROR_STATE();
            /// 
            ///         -- 抛出异常
            ///         RAISERROR(@ErrorMessage, @ErrorSeverity, @ErrorState);
            ///     End Catch
            /// </summary>
            /// <param name="strName_">模拟序列的存储过程名称</param>
            /// <returns></returns>
            public int GetSeqNextVal(string strName_)
            {
                using (var con = GetConnect())
                {
                    var sqlParam = new Dapper.DynamicParameters();
                    sqlParam.Add("@NextVal", 0, DbType.Int32, ParameterDirection.Output);
                    Dapper.SqlMapper.Execute(con, strName_, sqlParam, null, null, CommandType.StoredProcedure);
                    return sqlParam.Get<int>("@NextVal");
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
                return string.Format("Select *, Row_Number() Over({0}) As x_rn From {1} {2}",
                    strOrderClause_, strTableName_, strWhereClauses_);
            }

            /// <summary>
            /// Port为0，则说明连接时不使用端口
            /// </summary>
            /// <param name="strDBAddr_">数据库地址</param>
            /// <param name="strDBName_">要连接的数据库名</param>
            /// <param name="nPort_">端口号</param>
            /// <param name="strUserName_">用户名</param>
            /// <param name="strUserPwd_">密码</param>
            public ConnectSqlServer(string strDBAddr_, int nPort_, string strDBName_, string strUserName_, string strUserPwd_)
            {
                ConnectString = BuildConnectString(strDBAddr_, nPort_, strDBName_, strUserName_, strUserPwd_);
            }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="strDBAddr_">数据库地址</param>
            /// <param name="strDBName_">要连接的数据库名</param>
            /// <param name="strUserName_">用户名</param>
            /// <param name="strUserPwd_">密码</param>
            public ConnectSqlServer(string strDBAddr_, string strDBName_, string strUserName_, string strUserPwd_)
                : this(strDBAddr_, 1433, strDBName_, strUserName_, strUserPwd_)
            { }

            /// <summary>
            /// 通过配置文件构造：Port为0，则说明连接时不使用端口
            /// </summary>
            /// <param name="dbConfig_"></param>
            public ConnectSqlServer(XDatabaseConfig dbConfig_)
            {
                ConnectString = BuildConnectString(dbConfig_.Address, dbConfig_.Port, dbConfig_.ServiceOrDbName, dbConfig_.UserName, dbConfig_.Password);
            }

            internal static string BuildConnectString(string strDBAddr_, int nPort_, string strDBName_, string strUserName_, string strUserPwd_)
            {
                string strPort = string.Empty;
                if (nPort_ != 0)
                    strPort = string.Format(",{0}", nPort_);
                return string.Format("Data Source={0}{1};Initial Catalog={2};User ID={3};Password={4}",
                    strDBAddr_, strPort, strDBName_, strUserName_, strUserPwd_);
            }

            /// <summary>
            /// 获取连接
            /// </summary>
            /// <returns></returns>
            public IDbConnection GetConnect()
            {
                var con = new SqlConnection(ConnectString);
                con.Open();
                return con;
            }
        }
    }
}
