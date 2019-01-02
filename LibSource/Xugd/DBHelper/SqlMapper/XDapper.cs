using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace SHCre.Xugd.DbHelper
{
    /// <summary>
    /// 通过Dapper操作数据库的类：
    /// 操作的数据类的格式
    /// className
    /// {
    ///   public Type ColName {get;set;}
    /// }
    /// ColName必须是数据库的列名，且必须为属性；
    /// Type支持基本类型（不包括Enum），如果是ulong，只支持（0~long.MaxValue）间的值。
    /// 作为条件Where的子句，如果包含多个，则通过And连接
    /// </summary>
    public partial class XDapperORM
    {
        IConnectBase _dbConnect = null;
        /// <summary>
        /// 执行命令的超时时间，不设定在使用默认时间
        /// </summary>
        public int? CmdTimeoutSeconds { get; set; }
        /// <summary>
        /// 获取数据库连接接口
        /// </summary>
        public IConnectBase BaseConnect
        {
            get { return _dbConnect; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="dbConnect_">数据库连接信息</param>
        /// <param name="nTimeoutSeconds_">执行命令的超时时间，不设定在使用默认时间</param>
        public XDapperORM(IConnectBase dbConnect_, int? nTimeoutSeconds_ = null)
        {
            _dbConnect = dbConnect_;
            CmdTimeoutSeconds = nTimeoutSeconds_;
        }

        /// <summary>
        /// 创建匹配日期时间的条件[Stat，End]或[Start,End)（IncludeEnd=false时）：
        /// dtStart为空则&lt;=dtEnd；
        /// dtEnd为空则&gt;=dtStart;
        /// </summary>
        /// <param name="strFieldName_">字段名</param>
        /// <param name="dtStart_">起始日期</param>
        /// <param name="dtEnd_">结束日期</param>
        /// <param name="strDateFormat_">日期格式化字符串(支持yy,yyyy,MM,dd,hh,HH,mm,ss)</param>
        /// <param name="bIncludeEnd_">true时包含结束时间，false时不包含结束时间</param>
        /// <returns>匹配条件</returns>
        public WhereClause BuildDateTimeClause(string strFieldName_, DateTime? dtStart_, DateTime? dtEnd_, string strDateFormat_ = "yyyyMMdd", bool bIncludeEnd_ = false)
        {
            return WhereClause.BuildDateTimeClause(_dbConnect, strFieldName_, dtStart_, dtEnd_, strDateFormat_, bIncludeEnd_);
        }

        /// <summary>
        /// 测试数据库连接是否正常
        /// </summary>
        /// <returns></returns>
        public bool CanConnect()
        {
            bool bConnect = false;
            try
            {
                ConnectTest();

                bConnect = true;
            }
            catch { }

            return bConnect;
        }

        /// <summary>
        /// 测试能否连接到数据库，如果失败则抛出异常
        /// </summary>
        public void ConnectTest()
        {
            using (var con = _dbConnect.GetConnect())
            {
                con.Close();
            }
        }

        #region "Util"
        string GetAndClause(params WhereClause[] aryClauses_)
        {
            return aryClauses_.Length == 0 ? "1=1" : string.Join(" And ", aryClauses_.Select(zp => zp.GetClause()));
        }

        private string BuildColParam(string strColName_)
        {
            return BuildColParam(strColName_, _dbConnect.ParamPrefix);
        }

        internal static string BuildColParam(string strColName_, string strPrefix_)
        {
            return string.Format("{0}={1}{0}", strColName_, strPrefix_);
        }

        static ConcurrentDictionary<Type, List<string>> _dicParamNameCache = new ConcurrentDictionary<Type, List<string>>();

        static List<string> GetParamNames(Type objType_)
        {
            List<string> lstParamNames;
            if (!_dicParamNameCache.TryGetValue(objType_, out lstParamNames))
            {
                lstParamNames = new List<string>();
                foreach (var prop in objType_.GetProperties(BindingFlags.GetProperty | BindingFlags.Instance | BindingFlags.Public))
                {
                    lstParamNames.Add(prop.Name);
                }

                if (lstParamNames.Count == 0)
                    throw new ArgumentException(string.Format("{0} no property", objType_.FullName));

                _dicParamNameCache[objType_] = lstParamNames;
            }

            return lstParamNames;
        }
        #endregion
    }
}
