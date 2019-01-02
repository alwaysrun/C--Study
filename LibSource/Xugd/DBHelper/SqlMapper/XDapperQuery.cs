using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;

namespace SHCre.Xugd.DbHelper
{
    partial class XDapperORM
    {
        #region "IsExists"
        class CExistsValue
        {
            public bool IsExists { get; set; }
        }

        /// <summary>
        /// 判断记录是否存在：
        /// Exists(Select * From TableName Where KeyCol=KeyValue)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName_"></param>
        /// <param name="strKeyColName_"></param>
        /// <param name="tKeyValue_"></param>
        /// <returns></returns>
        public bool IsExists<T>(string strTableName_, string strKeyColName_, T tKeyValue_)
        {
            string strCmd;
            return IsExists(strTableName_, strKeyColName_, tKeyValue_, out strCmd);
        }

        /// <summary>
        /// 判断记录是否存在：
        /// Exists(Select * From TableName Where KeyCol=KeyValue)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName_"></param>
        /// <param name="strKeyColName_"></param>
        /// <param name="tKeyValue_"></param>
        /// <param name="strCmdText_"></param>
        /// <returns></returns>
        public bool IsExists<T>(string strTableName_, string strKeyColName_, T tKeyValue_, out string strCmdText_)
        {
            strCmdText_ = string.Format("Select Count(*) As IsExists {0} Where Exists(Select * From {1} Where {2}={3}ColKey)",
                _dbConnect.FromDual, strTableName_, strKeyColName_, _dbConnect.ParamPrefix);

            var ColKey = tKeyValue_;
            var existKey = new { ColKey };
            var objExists = Query<CExistsValue>(strCmdText_, existKey).First();
            return objExists.IsExists;
        }

        /// <summary>
        /// 判断记录是否存在：
        /// Exists(Select * From TableName Where KeyData_Attributes)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName_"></param>
        /// <param name="tKeyData_"></param>
        /// <returns></returns>
        public bool IsExists<T>(string strTableName_, T tKeyData_) where T : IXDapperData
        {
            string strCmdText_;
            return IsExists(strTableName_, tKeyData_, out strCmdText_);
        }

        /// <summary>
        /// 判断记录是否存在：
        /// Exists(Select * From TableName Where KeyData_Attributes)
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="strTableName_"></param>
        /// <param name="tKeyData_"></param>
        /// <param name="strCmdText_"></param>
        /// <returns></returns>
        public bool IsExists<T>(string strTableName_, T tKeyData_, out string strCmdText_) where T : IXDapperData
        {
            List<string> lstParams = GetParamNames(tKeyData_.GetType());

            string strWhere = string.Join(" And ", lstParams.Select(zp => BuildColParam(zp)));
            strCmdText_ = string.Format("Select Count(*) As IsExists {0} Where Exists(Select * From {1} Where {2})",
                _dbConnect.FromDual, strTableName_, strWhere);

            var objExists = Query<CExistsValue>(strCmdText_, tKeyData_).First();
            return objExists.IsExists;
        }

        /// <summary>
        /// 判断记录是否存在；如果Clause为空，则为判断表中是否存在记录：
        /// Exists(Select * From TableName Where Clause)
        /// </summary>
        /// <param name="strTableName_"></param>
        /// <param name="existsClause_"></param>
        /// <returns></returns>
        public bool IsExists(string strTableName_, params WhereClause[] existsClause_)
        {
            string strCmd;
            return IsExists(strTableName_, out strCmd, existsClause_);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strTableName_"></param>
        /// <param name="strCmdText_"></param>
        /// <param name="existsClause_"></param>
        /// <returns></returns>
        public bool IsExists(string strTableName_, out string strCmdText_, params WhereClause[] existsClause_)
        {
            strCmdText_ = string.Format("Select Count(*) As IsExists {0} Where Exists(Select * From {1} Where {2})",
                _dbConnect.FromDual, strTableName_, GetAndClause(existsClause_));

            var objExists = Query<CExistsValue>(strCmdText_).First();
            return objExists.IsExists;
        }
        #endregion

        #region "Aggregate"
        class CCountValue
        {
            public int NValue { get; set; }
        }

        /// <summary>
        /// 获取记录的数量
        /// </summary>
        /// <param name="strTable_">表名</param>
        /// <param name="whClause_"></param>
        /// <returns></returns>
        public int GetCount(string strTable_, params WhereClause[] whClause_)
        {
            string strCmdText = string.Format("Select Count(*) As NValue From {0} Where {1}", strTable_, GetAndClause(whClause_));

            var objCount = Query<CCountValue>(strCmdText).First();
            return objCount.NValue;
        }


        class CAggregateValue
        {
            public double DValue { get; set; }
        }
        int _cnDefPresion = 6;

        /// <summary>
        /// 获取指定列的最大值
        /// </summary>
        /// <param name="strTable_">表名</param>
        /// <param name="strCol_">列名（可用表达式，如Col*100)</param>
        /// <param name="whClause_">条件语句</param>
        /// <returns></returns>
        public double GetMax(string strTable_, string strCol_, params WhereClause[] whClause_)
        {
            return GetMax(strTable_, strCol_, _cnDefPresion, whClause_);
        }

        /// <summary>
        /// 获取指定列的最大值
        /// </summary>
        /// <param name="strTable_">表名</param>
        /// <param name="strCol_">列名（可用表达式，如Col*100)</param>
        /// <param name="whClause_">条件语句</param>
        /// <param name="nPrecision_">精度，小数位数</param>
        /// <returns></returns>
        public double GetMax(string strTable_, string strCol_, int nPrecision_, params WhereClause[] whClause_)
        {
            string strCmdText = string.Format("Select Round(Max({0}), {1}) As DValue From {2} Where {3}", strCol_, nPrecision_, strTable_, GetAndClause(whClause_));

            var objMax = Query<CAggregateValue>(strCmdText).First();
            return objMax.DValue;
        }

        /// <summary>
        /// 获取指定列的最小值
        /// </summary>
        /// <param name="strTable_">表名</param>
        /// <param name="strCol_">列名（可用表达式，如Col*100)</param>
        /// <param name="whClause_">条件语句</param>
        /// <returns></returns>
        public double GetMin(string strTable_, string strCol_, params WhereClause[] whClause_)
        {
            return GetMin(strTable_, strCol_, _cnDefPresion, whClause_);
        }

        /// <summary>
        /// 获取指定列的最小值
        /// </summary>
        /// <param name="strTable_">表名</param>
        /// <param name="strCol_">列名（可用表达式，如Col*100)</param>
        /// <param name="whClause_">条件语句</param>
        /// <param name="nPrecision_">精度，小数位数</param>
        /// <returns></returns>
        public double GetMin(string strTable_, string strCol_, int nPrecision_, params WhereClause[] whClause_)
        {
            string strCmdText = string.Format("Select Round(Min({0}), {1}) As DValue From {2} Where {3}", strCol_, nPrecision_, strTable_, GetAndClause(whClause_));

            var objMin = Query<CAggregateValue>(strCmdText).First();
            return objMin.DValue;
        }

        /// <summary>
        /// 获取指定列的和
        /// </summary>
        /// <param name="strTable_">表名</param>
        /// <param name="strCol_">列名（可用表达式，如Col*100)</param>
        /// <param name="whClause_">条件语句</param>
        /// <returns></returns>
        public double GetSum(string strTable_, string strCol_, params WhereClause[] whClause_)
        {
            return GetSum(strTable_, strCol_, _cnDefPresion, whClause_);
        }

        /// <summary>
        /// 获取指定列的和
        /// </summary>
        /// <param name="strTable_">表名</param>
        /// <param name="strCol_">列名（可用表达式，如Col*100)</param>
        /// <param name="whClause_">条件语句</param>
        /// <param name="nPrecision_">精度，小数位数</param>
        /// <returns></returns>
        public double GetSum(string strTable_, string strCol_, int nPrecision_, params WhereClause[] whClause_)
        {
            string strCmdText = string.Format("Select Round(Sum({0}), {1}) As DValue From {2} Where {3}", strCol_, nPrecision_, strTable_, GetAndClause(whClause_));

            var objSum = Query<CAggregateValue>(strCmdText).First();
            return objSum.DValue;
        }

        /// <summary>
        /// 获取指定列的平均值
        /// </summary>
        /// <param name="strTable_">表名</param>
        /// <param name="strCol_">列名（可用表达式，如Col*100)</param>
        /// <param name="whClause_">条件语句</param>
        /// <returns></returns>
        public double GetAvg(string strTable_, string strCol_, params WhereClause[] whClause_)
        {
            return GetAvg(strTable_, strCol_, _cnDefPresion, whClause_);
        }

        /// <summary>
        /// 获取指定列的平均值
        /// </summary>
        /// <param name="strTable_">表名</param>
        /// <param name="strCol_">列名（可用表达式，如Col*100)</param>
        /// <param name="whClause_">条件语句</param>
        /// <param name="nPrecision_">精度，小数位数</param>
        /// <returns></returns>
        public double GetAvg(string strTable_, string strCol_, int nPrecision_, params WhereClause[] whClause_)
        {
            string strCmdText = string.Format("Select Round(Avg({0}), {1}) As DValue From {2} Where {3}", strCol_, nPrecision_, strTable_, GetAndClause(whClause_));

            var objAvg = Query<CAggregateValue>(strCmdText).First();
            return objAvg.DValue;
        }
        #endregion

        #region "Get"
        /// <summary>
        /// 获取当前数据库时间，如果获取失败，则返回系统的当前时间
        /// </summary>
        /// <returns></returns>
        public DateTime GetDateTime()
        {
            try
            {
                string strSql = "Select Current_Timestamp As ColDate" + _dbConnect.FromDual;
                return Query<DateTime>(strSql).First();
            }
            catch (Exception)
            {
            }

            return DateTime.Now;
        }

        /// <summary>
        /// 获取序列的下一个值；
        /// 序列对数据库所需依赖设置，请看对应的IConnectBase.GetSeqNextVal中的注释说明
        /// </summary>
        /// <param name="strName_">序列名称</param>
        /// <returns></returns>
        public int GetSeqNextVal(string strName_)
        {
            return _dbConnect.GetSeqNextVal(strName_);
        }

        /// <summary>
        /// 获取单条记录的查询，查询不到数据返回null：
        /// Select TReturn_Attributes From TableName Where KeyData_Attributes
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="tKeyData_">生成where子句的数据类</param>
        /// <param name="strClauseAfterWhere_">可放在Where之后的子句</param>
        /// <returns></returns>
        public TReturn GetSingle<TReturn, TValue>(string strTableName_, TValue tKeyData_, string strClauseAfterWhere_)
            where TReturn : class
            where TValue : IXDapperData
        {
            string strCmdText = null;
            return GetSingle<TReturn, TValue>(strTableName_, tKeyData_, out strCmdText, strClauseAfterWhere_) as TReturn;
        }

        /// <summary>
        /// 获取单条记录的查询，查询不到数据返回null：
        /// Select TReturn_Attributes From TableName Where KeyData_Attributes
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="tKeyData_">生成where子句的数据类</param>
        /// <param name="strCmdText_">用于查询的sql语句</param>
        /// <param name="strClauseAfterWhere_">可放在Where之后的子句</param>
        /// <returns></returns>
        public TReturn GetSingle<TReturn, TValue>(string strTableName_, TValue tKeyData_, out string strCmdText_, string strClauseAfterWhere_ = null)
            where TReturn : class
            where TValue : IXDapperData
        {
            List<string> lstCols = GetParamNames(typeof(TReturn));
            List<string> lstWhere = GetParamNames(tKeyData_.GetType());

            string strCols = string.Join(",", lstCols);
            string strWhere = string.Join(" And ", lstWhere.Select(zp => BuildColParam(zp)));
            strCmdText_ = string.Format("Select {0} From {1} Where {2} ", strCols, strTableName_, strWhere);
            if (!string.IsNullOrEmpty(strClauseAfterWhere_))
                strCmdText_ += strClauseAfterWhere_;

            return (Query<TReturn>(strCmdText_, tKeyData_) as List<TReturn>).FirstOrDefault();
        }

        /// <summary>
        /// 获取单条记录的查询，查询不到数据返回null：
        /// Select TReturn_Attributes From TableName Where KeyColName=KeyValue
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="strKeyColName_">条件子句的列名</param>
        /// <param name="tKeyValue_">条件子句的列值</param>
        /// <param name="strClauseAfterWhere_">可放在Where之后的子句</param>
        /// <returns></returns>
        public TReturn GetSingle<TReturn, TValue>(string strTableName_, string strKeyColName_, TValue tKeyValue_, string strClauseAfterWhere_ = null) where TReturn : class
        {
            string strCmdText = null;
            return GetSingle<TReturn, TValue>(strTableName_, strKeyColName_, tKeyValue_, out strCmdText, strClauseAfterWhere_) as TReturn;
        }

        /// <summary>
        /// 获取单条记录的查询，查询不到数据返回null：
        /// Select TReturn_Attributes From TableName Where KeyColName=KeyValue
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="strKeyColName_">条件子句的列名</param>
        /// <param name="tKeyValue_">条件子句的列值</param>
        /// <param name="strCmdText_">用于查询的sql子句</param>
        /// <param name="strClauseAfterWhere_">可放在Where之后的子句</param>
        /// <returns></returns>
        public TReturn GetSingle<TReturn, TValue>(string strTableName_, string strKeyColName_, TValue tKeyValue_, out string strCmdText_, string strClauseAfterWhere_ = null) where TReturn : class
        {
            List<string> lstCols = GetParamNames(typeof(TReturn));

            string strCols = string.Join(",", lstCols);
            strCmdText_ = string.Format("Select {0} From {1} Where {2} ", strCols, strTableName_,
                string.Format("{0}={1}ColKey", strKeyColName_, _dbConnect.ParamPrefix));
            if (!string.IsNullOrEmpty(strClauseAfterWhere_))
                strCmdText_ += strClauseAfterWhere_;

            var ColKey = tKeyValue_;
            var queryInfo = new { ColKey };
            return (Query<TReturn>(strCmdText_, queryInfo) as List<TReturn>).FirstOrDefault();
        }

        /// <summary>
        /// 获取单条记录的查询，查询不到数据返回null：
        /// Select TReturn_Attributes From TableName Where Clause
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="aryClauses_">条件子句</param>
        /// <returns></returns>
        public TReturn GetSingle<TReturn>(string strTableName_, params WhereClause[] aryClauses_) where TReturn : class
        {
            return GetSingle<TReturn>(strTableName_, null, aryClauses_);
        }

        /// <summary>
        /// 获取单条记录的查询，查询不到数据返回null：
        /// Select TReturn_Attributes From TableName Where Clause
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="aryClauses_">条件子句</param>
        /// <param name="strClauseAfterWhere_">可放在Where之后的子句</param>
        /// <returns></returns>
        public TReturn GetSingle<TReturn>(string strTableName_, string strClauseAfterWhere_, params WhereClause[] aryClauses_) where TReturn : class
        {
            string strCmdText = null;
            return GetSingle<TReturn>(strTableName_, out strCmdText, strClauseAfterWhere_, aryClauses_);
        }

        /// <summary>
        /// 获取单条记录的查询，查询不到数据返回null：
        /// Select TReturn_Attributes From TableName Where Clause
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="strCmdText_">用于查询的sql子句</param>
        /// <param name="aryClauses_">条件子句：如果是多条子句，则通过And连接</param>
        /// <param name="strClauseAfterWhere_">可放在Where之后的子句</param>
        /// <returns></returns>
        public TReturn GetSingle<TReturn>(string strTableName_, out string strCmdText_, string strClauseAfterWhere_, params WhereClause[] aryClauses_) where TReturn : class
        {
            List<string> lstCols = GetParamNames(typeof(TReturn));

            string strCols = string.Join(",", lstCols);
            strCmdText_ = string.Format("Select {0} From {1} Where {2} ", strCols, strTableName_, GetAndClause(aryClauses_));
            if (!string.IsNullOrEmpty(strClauseAfterWhere_))
                strCmdText_ += strClauseAfterWhere_;

            return (Query<TReturn>(strCmdText_, null) as List<TReturn>).FirstOrDefault();
        }

        /// <summary>
        /// 获取多条记录的查询：
        /// Select TReturn_Attributes From TableName Where KeyData_Attributes
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="tKeyData_">生成where子句的数据类</param>
        /// <param name="strClauseAfterWhere_">可放在Where之后的子句</param>
        /// <returns></returns>
        public List<TReturn> GetMulti<TReturn, TValue>(string strTableName_, TValue tKeyData_, string strClauseAfterWhere_) 
            where TReturn : class
            where TValue:IXDapperData
        {
            string strCmdText = null;
            return GetMulti<TReturn, TValue>(strTableName_, tKeyData_, out strCmdText, strClauseAfterWhere_) as List<TReturn>;
        }

        /// <summary>
        /// 获取多条记录的查询：
        /// Select TReturn_Attributes From TableName Where KeyData_Attributes
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="tKeyData_">生成where子句的数据类</param>
        /// <param name="strCmdText_">用于查询的sql语句</param>
        /// <param name="strClauseAfterWhere_">可放在Where之后的子句</param>
        /// <returns></returns>
        public List<TReturn> GetMulti<TReturn, TValue>(string strTableName_, TValue tKeyData_, out string strCmdText_, string strClauseAfterWhere_ = null) 
            where TReturn : class
            where TValue:IXDapperData
        {
            List<string> lstCols = GetParamNames(typeof(TReturn));
            List<string> lstWhere = GetParamNames(tKeyData_.GetType());

            string strCols = string.Join(",", lstCols);
            string strWhere = string.Join(" And ", lstWhere.Select(zp => BuildColParam(zp)));
            strCmdText_ = string.Format("Select {0} From {1} Where {2} ", strCols, strTableName_, strWhere);
            if (!string.IsNullOrEmpty(strClauseAfterWhere_))
                strCmdText_ += strClauseAfterWhere_;

            return Query<TReturn>(strCmdText_, tKeyData_) as List<TReturn>;
        }

        /// <summary>
        /// 获取多条记录的查询：
        /// Select TReturn_Attributes From TableName Where KeyColName=KeyValue
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="strKeyColName_">条件子句的列名</param>
        /// <param name="tKeyValue_">条件子句的列值</param>
        /// <param name="strClauseAfterWhere_">可放在Where之后的子句</param>
        /// <returns></returns>
        public List<TReturn> GetMulti<TReturn, TValue>(string strTableName_, string strKeyColName_, TValue tKeyValue_, string strClauseAfterWhere_ = null) where TReturn : class
        {
            string strCmdText = null;
            return GetMulti<TReturn, TValue>(strTableName_, strKeyColName_, tKeyValue_, out strCmdText, strClauseAfterWhere_) as List<TReturn>;
        }

        /// <summary>
        /// 获取多条记录的查询：
        /// Select TReturn_Attributes From TableName Where KeyColName=KeyValue
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <typeparam name="TValue"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="strKeyColName_">条件子句的列名</param>
        /// <param name="tKeyValue_">条件子句的列值</param>
        /// <param name="strCmdText_">用于查询的sql子句</param>
        /// <param name="strClauseAfterWhere_">可放在Where之后的子句</param>
        /// <returns></returns>
        public List<TReturn> GetMulti<TReturn, TValue>(string strTableName_, string strKeyColName_, TValue tKeyValue_, out string strCmdText_, string strClauseAfterWhere_ = null) where TReturn : class
        {
            List<string> lstCols = GetParamNames(typeof(TReturn));

            string strCols = string.Join(",", lstCols);
            strCmdText_ = string.Format("Select {0} From {1} Where {2} ", strCols, strTableName_,
                string.Format("{0}={1}ColKey", strKeyColName_, _dbConnect.ParamPrefix));
            if (!string.IsNullOrEmpty(strClauseAfterWhere_))
                strCmdText_ += strClauseAfterWhere_;

            var ColKey = tKeyValue_;
            var queryInfo = new { ColKey };
            return Query<TReturn>(strCmdText_, queryInfo) as List<TReturn>;
        }

        /// <summary>
        /// 获取多条记录的查询：
        /// Select TReturn_Attributes From TableName Where Clause
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="aryClauses_">条件子句</param>
        /// <returns></returns>
        public List<TReturn> GetMulti<TReturn>(string strTableName_, params WhereClause[] aryClauses_) where TReturn : class
        {
            return GetMulti<TReturn>(strTableName_, null, aryClauses_);
        }

        /// <summary>
        /// 获取多条记录的查询：
        /// Select TReturn_Attributes From TableName Where Clause
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="aryClauses_">条件子句</param>
        /// <param name="strClauseAfterWhere_">可放在Where之后的子句</param>
        /// <returns></returns>
        public List<TReturn> GetMulti<TReturn>(string strTableName_, string strClauseAfterWhere_, params WhereClause[] aryClauses_) where TReturn : class
        {
            string strCmdText = null;
            return GetMulti<TReturn>(strTableName_, out strCmdText, strClauseAfterWhere_, aryClauses_) as List<TReturn>;

        }

        /// <summary>
        /// 获取多条记录的查询：
        /// Select TReturn_Attributes From TableName Where Clause
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="strCmdText_">用于查询的sql子句</param>
        /// <param name="aryClauses_">条件子句：如果是多条子句，则通过And连接</param>
        /// <param name="strClauseAfterWhere_">可放在Where之后的子句</param>
        /// <returns></returns>
        public List<TReturn> GetMulti<TReturn>(string strTableName_, out string strCmdText_, string strClauseAfterWhere_, params WhereClause[] aryClauses_) where TReturn : class
        {
            List<string> lstCols = GetParamNames(typeof(TReturn));

            string strCols = string.Join(",", lstCols);
            strCmdText_ = string.Format("Select {0} From {1} Where {2} ", strCols, strTableName_, GetAndClause(aryClauses_));
            if (!string.IsNullOrEmpty(strClauseAfterWhere_))
                strCmdText_ += strClauseAfterWhere_;

            return Query<TReturn>(strCmdText_, null) as List<TReturn>;
        }
        #endregion

        /// <summary>
        /// 查询数据
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="strSql_">Sql语句</param>
        /// <param name="tData_">用于匹配Sql参数的数据</param>
        /// <param name="bBuffered_">是否缓存</param>
        /// <param name="cmdType_">Sql语句类型</param>
        /// <returns></returns>
        public List<TReturn> Query<TReturn>(string strSql_, dynamic tData_ = null, bool bBuffered_ = true, CommandType? cmdType_ = null)
        {
            using (var con = _dbConnect.GetConnect())
            {
                return Dapper.SqlMapper.Query<TReturn>(con, strSql_, tData_, null, bBuffered_, CmdTimeoutSeconds, cmdType_) as List<TReturn>;
            }
        }

    } // class
}
