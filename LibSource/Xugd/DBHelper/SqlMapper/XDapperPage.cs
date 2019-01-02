using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SHCre.Xugd.DbHelper
{
    partial class XDapperORM
    {
        /// <summary>
        /// 分页查询（必须提供排序列），返回Start开始的PageSize条记录
        /// </summary>
        /// <typeparam name="TReturn"></typeparam>
        /// <param name="strTableName_">表名</param>
        /// <param name="strOrderClause_">分页的排序依据</param>
        /// <param name="nStartIndex_">要查询数据的起始位置(从1开始)</param>
        /// <param name="nPageSize_">每页的条数</param>
        /// <param name="strCmdText_">用于查询的sql子句</param>
        /// <param name="aryClauses_">条件子句：如果是多条子句，则通过And连接</param>
        /// <returns></returns>
        public List<TReturn> GetInPage<TReturn>(string strTableName_, string strOrderClause_, int nStartIndex_, int nPageSize_, out string strCmdText_, params WhereClause[] aryClauses_) where TReturn : class
        {
            if (nStartIndex_ < 1 || nPageSize_<1)
                throw new ArgumentException("GetInPage: Start-Index and Page-Size must a positive integer");
            if (string.IsNullOrEmpty(strOrderClause_))
                throw new ArgumentException("GetInPage: order clause for sql must supply");

            List<string> lstCols = GetParamNames(typeof(TReturn));
            string strCols = string.Join(",", lstCols);
            string strOrder = string.Format(" Order By {0} ", strOrderClause_);
            string strWhere = string.Empty;
            if (aryClauses_.Length != 0)
                strWhere = "Where " + GetAndClause(aryClauses_);

            strCmdText_ = string.Format("Select {0} From ({1}) Sub Where x_rn>={2} and x_rn<{3}",
                strCols, 
                _dbConnect.GetPagedSubset(strTableName_, strOrder, strWhere), 
                nStartIndex_, nStartIndex_+nPageSize_);

            return Query<TReturn>(strCmdText_, null) as List<TReturn>;
        }
        
    }
}
