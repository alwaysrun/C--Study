using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SHCre.Xugd.DbHelper
{
    partial class XDapperORM
    {

        #region "Insert"
        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="tData_">要插入的数据</param>
        /// <returns>影响的条数</returns>
        public int Insert<T>(string strTableName_, T tData_) where T : class
        {
            string strCmdText = null;
            return Insert(strTableName_, tData_, out strCmdText);
        }

        /// <summary>
        /// 插入数据
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="tData_">要插入的数据</param>
        /// <param name="strCmdText_">用于插入的sql子句</param>
        /// <returns>影响的条数</returns>
        public int Insert<T>(string strTableName_, T tData_, out string strCmdText_) where T : class
        {
            List<string> lstParams = GetParamNames(tData_.GetType());

            string strCols = string.Join(",", lstParams);
            string strValues = string.Join(",", lstParams.Select(z => _dbConnect.ParamPrefix + z));
            strCmdText_ = string.Format("Insert Into {0} ({1}) Values ({2})", strTableName_, strCols, strValues);

            return Execute(strCmdText_, tData_);
        }
        #endregion

        /// <summary>
        /// 插入或更新数据：值不存在则插入，否则更新
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName_"></param>
        /// <param name="strKeyColName_"></param>
        /// <param name="tData_"></param>
        /// <param name="strCmdText_"></param>
        /// <returns></returns>
        public int InsertOrUpdate<T>(string strTableName_, string strKeyColName_, T tData_, out string strCmdText_) where T : IXDapperData
        {
            List<string> lstParams = GetParamNames(tData_.GetType());

            strCmdText_ = _dbConnect.InsertOrUpdateSql(strTableName_, strKeyColName_, lstParams);

            return Execute(strCmdText_, tData_);
        }

        #region "Update"
        /// <summary>
        /// 更新数据：KeyColName对应的字段（属性名与此相同）必须包含在tData中，并更新tData中除KeyColName对应项外的所有字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName_"></param>
        /// <param name="strKeyColName_"></param>
        /// <param name="tData_"></param>
        /// <returns></returns>
        public int Update<T>(string strTableName_, string strKeyColName_, T tData_)
        {
            string strCmdText = null;
            return Update(strTableName_, strKeyColName_, tData_, out strCmdText);
        }

        /// <summary>
        /// 更新数据：KeyColName对应的字段（属性名与此相同）必须包含在tData中，并更新tData中出KeyColName对应项外的所有字段
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName_"></param>
        /// <param name="strKeyColName_"></param>
        /// <param name="tData_"></param>
        /// <param name="strCmdText_"></param>
        /// <returns></returns>
        public int Update<T>(string strTableName_, string strKeyColName_, T tData_, out string strCmdText_)
        {
            List<string> lstParams = GetParamNames(tData_.GetType());

            string strUpdates = string.Join(",", lstParams.Where(zk => zk != strKeyColName_).Select(zp => BuildColParam(zp)));
            strCmdText_ = string.Format("Update {0} Set {1} Where {2}", strTableName_, strUpdates,
                BuildColParam(strKeyColName_));

            return Execute(strCmdText_, tData_);
        }

        /// <summary>
        /// 更新数据：
        /// Update TableName Set KeyCol=KeyValue Where UpdateCol=UpdateValue;
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TUpdate"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="strKeyColName_">条件子句的列名</param>
        /// <param name="tKeyValue_">条件子句的列值</param>
        /// <param name="strUpdateColName_">设定子句的列名</param>
        /// <param name="tUpdateValue_">设定子句的列值</param>
        /// <returns>更新的条数</returns>
        public int Update<TKey, TUpdate>(string strTableName_, string strKeyColName_, TKey tKeyValue_, string strUpdateColName_, TUpdate tUpdateValue_)
        {
            string strCmdText = null;
            return Update(strTableName_, strKeyColName_, tKeyValue_, strUpdateColName_, tUpdateValue_, out strCmdText);
        }

        /// <summary>
        /// 更新数据：
        /// Update TableName Set KeyCol=KeyValue Where UpdateCol=UpdateValue;
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TUpdate"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="strKeyColName_">条件子句的列名</param>
        /// <param name="tKeyValue_">条件子句的列值</param>
        /// <param name="strUpdateColName_">设定子句的列名</param>
        /// <param name="tUpdateValue_">设定子句的列值</param>
        /// <param name="strCmdText_">用于更新的sql子句</param>
        /// <returns>更新的条数</returns>
        public int Update<TKey, TUpdate>(string strTableName_, string strUpdateColName_, TUpdate tUpdateValue_, string strKeyColName_, TKey tKeyValue_, out string strCmdText_)
        {
            strCmdText_ = string.Format("Update {0} Set {1}={2}ColSet Where {3}={2}ColKey",
                strTableName_,
                strUpdateColName_, _dbConnect.ParamPrefix,
                strKeyColName_);

            var ColSet = tUpdateValue_;
            var ColKey = tKeyValue_;
            return Execute(strCmdText_, new { ColSet, ColKey });
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TUpdate"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="tUpdate_">要更新的数据</param>
        /// <param name="tKey_">用于条件的数据</param>
        /// <returns>更新的条数</returns>
        public int Update<TKey, TUpdate>(string strTableName_, TUpdate tUpdate_, TKey tKey_)
            where TKey : IXDapperData
            where TUpdate : IXDapperData
        {
            string strCmdText = null;
            return Update(strTableName_, tUpdate_, tKey_, out strCmdText);
        }

        /// <summary>
        /// 更新数据
        /// </summary>
        /// <typeparam name="TKey"></typeparam>
        /// <typeparam name="TUpdate"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="tUpdate_">要更新的数据</param>
        /// <param name="tKey_">用于条件子句的数据</param>
        /// <param name="strCmdText_">用于更新的sql子句</param>
        /// <returns>更新的条数</returns>
        public int Update<TKey, TUpdate>(string strTableName_, TUpdate tUpdate_, TKey tKey_, out string strCmdText_)
            where TKey : IXDapperData
            where TUpdate : IXDapperData
        {
            var lstUpdate = GetParamNames(tUpdate_.GetType());
            var lstKeys = GetParamNames(tKey_.GetType());

            string strUpdate = string.Join(",", lstUpdate.Select(zp => BuildColParam(zp)));
            string strKeys = string.Join(" And ", lstKeys.Select(zp => BuildColParam(zp)));
            strCmdText_ = string.Format("Update {0} Set {1} Where {2}", strTableName_, strUpdate, strKeys);

            Dapper.DynamicParameters dyParams = new Dapper.DynamicParameters();
            dyParams.AddDynamicParams(tUpdate_);
            dyParams.AddDynamicParams(tKey_);
            return Execute(strCmdText_, dyParams);
        }

        /// <summary>
        /// 更新数据(使用时需要加上泛型约束，否则与Update&lt;TKey, TUpdate&gt;冲突)：如果条件子句为空，则为更新所有:
        /// Update TableName Set Update_Attribute Where Caluse
        /// </summary>
        /// <typeparam name="TUpdate"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="tUpdate_">要更新的数据</param>
        /// <param name="aryClauses_">条件子句</param>
        /// <returns>更新的条数</returns>
        public int Update<TUpdate>(string strTableName_, TUpdate tUpdate_, params WhereClause[] aryClauses_)
        {
            string strCmdText = null;
            return Update(strTableName_, tUpdate_, out strCmdText, aryClauses_);
        }

        /// <summary>
        /// 更新数据：如果条件子句为空，则为更新所有:
        /// Update TableName Set Update_Attribute Where Caluse
        /// </summary>
        /// <typeparam name="TUpdate"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="tUpdate_">要更新的数据</param>
        /// <param name="strCmdText_">用于更新的sql子句</param>
        /// <param name="aryClauses_">条件子句：如果是多条子句，则通过And连接</param>
        /// <returns>更新的条数</returns>
        public int Update<TUpdate>(string strTableName_, TUpdate tUpdate_, out string strCmdText_, params WhereClause[] aryClauses_)
        {
            var lstUpdate = GetParamNames(tUpdate_.GetType());

            string strUpdate = string.Join(",", lstUpdate.Select(zp => BuildColParam(zp)));
            strCmdText_ = string.Format("Update {0} Set {1} Where {2}", strTableName_, strUpdate, GetAndClause(aryClauses_));

            return Execute(strCmdText_, tUpdate_);
        }
        #endregion

        #region "Delete"
        /// <summary>
        /// 删除数据:
        /// Delete From TableName Where KeyData_Attributes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="tKeyData_">用于条件子句的数据</param>
        /// <returns>删除的条数</returns>
        public int Delete<T>(string strTableName_, T tKeyData_) where T : IXDapperData
        {
            string strCmdText = null;
            return Delete(strTableName_, tKeyData_, out strCmdText);
        }

        /// <summary>
        /// 删除数据:
        /// Delete From TableName Where KeyData_Attributes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="tKeyData_">用于条件子句的数据</param>
        /// <param name="strCmdText_">用于删除的sql子句</param>
        /// <returns>删除的条数</returns>
        public int Delete<T>(string strTableName_, T tKeyData_, out string strCmdText_) where T : IXDapperData
        {
            List<string> lstParams = GetParamNames(tKeyData_.GetType());

            string strWhere = string.Join(" And ", lstParams.Select(zp => BuildColParam(zp)));
            strCmdText_ = string.Format("Delete From {0} Where {1}", strTableName_, strWhere);

            return Execute(strCmdText_, tKeyData_);
        }

        /// <summary>
        /// 删除数据:
        /// Delete From TableName Where KeyCol=KeyValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="strKeyColName_">条件子句的列名</param>
        /// <param name="tKeyValue_">条件子句的列值</param>
        /// <returns>删除的条数</returns>
        public int Delete<T>(string strTableName_, string strKeyColName_, T tKeyValue_)
        {
            string strCmdText = null;
            return Delete(strTableName_, strKeyColName_, tKeyValue_, out strCmdText);
        }

        /// <summary>
        /// 删除数据:
        /// Delete From TableName Where KeyCol=KeyValue
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="strKeyColName_">条件子句的列名</param>
        /// <param name="tKeyValue_">条件子句的列值</param>
        /// <param name="strCmdText_">用于删除的sql子句</param>
        /// <returns>删除的条数</returns>
        public int Delete<T>(string strTableName_, string strKeyColName_, T tKeyValue_, out string strCmdText_)
        {
            strCmdText_ = string.Format("Delete From {0} Where {1}={2}ColKey", strTableName_, strKeyColName_, _dbConnect.ParamPrefix);

            var ColKey = tKeyValue_;
            var delKey = new { ColKey };
            return Execute(strCmdText_, delKey);
        }

        /// <summary>
        /// 删除满足条件的子句，如果子句为空则为删除所有:
        /// Delete From TableName Where Clause
        /// </summary>
        /// <param name="strTableName_">表名</param>
        /// <param name="aryClauses_">条件子句</param>
        /// <returns>删除的条数</returns>
        public int Delete(string strTableName_, params WhereClause[] aryClauses_)
        {
            string strCmdText_;
            return Delete(strTableName_, out strCmdText_, aryClauses_);
        }

        /// <summary>
        /// 删除满足条件的子句，如果子句为空则为删除所有:
        /// Delete From TableName Where Clause
        /// </summary>
        /// <param name="strTableName_">表名</param>
        /// <param name="strCmdText_">用于删除的sql子句</param>
        /// <param name="aryClauses_">条件子句</param>
        /// <returns>删除的条数</returns>
        public int Delete(string strTableName_, out string strCmdText_, params WhereClause[] aryClauses_)
        {
            strCmdText_ = string.Format("Delete From  {0} Where {1}", strTableName_, GetAndClause(aryClauses_));

            return Execute(strCmdText_, null);
        }
        #endregion

        /// <summary>
        /// 调用存储过程
        /// </summary>
        /// <param name="strSPName_">存储过程名</param>
        /// <param name="dyParams_">存储过程所需参数：
        /// 输入参数：dyParams_.Add("@InVal", "JustForTest", DbType.AnsiString, ParameterDirection.Input, 32)；
        /// 添加输出参数：dyParams_.Add("@OuttVal", 0, DbType.Int32, ParameterDirection.Output)
        /// </param>
        /// <returns>受影响行数</returns>
        public int ExecuteSP(string strSPName_, Dapper.DynamicParameters dyParams_)
        {
            using (var con = _dbConnect.GetConnect())
            {
                return Dapper.SqlMapper.Execute(con, strSPName_, dyParams_, null, CmdTimeoutSeconds, CommandType.StoredProcedure);
            }
        }

        /// <summary>
        /// 执行Sql语句
        /// </summary>
        /// <param name="strSql_">Sql语句或存储过程名</param>
        /// <param name="tParams_">用于匹配Sql参数的数据</param>
        /// <param name="cmdType_">Sql语句类型</param>
        /// <param name="bInTransaction_">是否作为事务处理</param>
        /// <returns>受影响行数</returns>
        public int Execute(string strSql_, dynamic tParams_ = null, CommandType? cmdType_ = null, bool bInTransaction_ = false)
        {
            using (var con = _dbConnect.GetConnect())
            {
                if (bInTransaction_)
                {
                    int nCmdResult = 0;
                    var tranact = con.BeginTransaction();
                    try
                    {
                        nCmdResult = Dapper.SqlMapper.Execute(con, strSql_, tParams_, tranact, CmdTimeoutSeconds, cmdType_);
                        tranact.Commit();
                    }
                    catch (Exception)
                    {
                        try
                        {
                            tranact.Rollback();
                        }
                        catch { }

                        throw;
                    }

                    return nCmdResult;
                }
                else
                {
                    return Dapper.SqlMapper.Execute(con, strSql_, tParams_, null, CmdTimeoutSeconds, cmdType_);
                }
            }
        }

        /// <summary>
        /// 通过事务一次执行多条sql命令：
        /// 要么全部成功，要么全部失败
        /// </summary>
        /// <param name="sqlData_"></param>
        /// <returns>所有命令执行完成后受影响行数之和</returns>
        public int Execute(params SqlDataInfo[] sqlData_)
        {
            using (var con = _dbConnect.GetConnect())
            {
                int nCmdResult = 0;
                var tranact = con.BeginTransaction();
                try
                {
                    foreach (var data in sqlData_)
                        nCmdResult += Dapper.SqlMapper.Execute(con, data.Sql, data.Data, tranact, CmdTimeoutSeconds, data.CmdType);
                    tranact.Commit();
                }
                catch (Exception)
                {
                    try
                    {
                        tranact.Rollback();
                    }
                    catch { }

                    throw;
                }

                return nCmdResult;
            }
        }

    } // class
}
