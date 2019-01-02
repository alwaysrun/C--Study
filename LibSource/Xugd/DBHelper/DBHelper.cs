using System;

using System.Data.SqlClient;

namespace SHCre.Xugd.DbHelper
{
    /// <summary>
    /// 数据库操作出错时，抛出的异常（继承自DbException）；
    /// 如果是.NET自己操作Sql数据库抛出的异常一般为SqlException（继承自DbException）；
    /// 所以一般情况下，数据库操作异常捕捉System.Data.Common.DbException即可。
    /// </summary>
    public class XDbException : System.Data.Common.DbException
    {
        /// <summary>
        /// 使用指定的错误信息和错误代码初始化 DbException 类的新实例
        /// </summary>
        /// <param name="strMsg_">错误信息，可通过属性Message获取</param>
        /// <param name="nError_">错误代码，可通过属性ErrorCode获取</param>
        public XDbException(string strMsg_, int nError_)
            : base(strMsg_, nError_)
        {
        }

        /// <summary>
        /// 使用指定的错误信息和对导致此异常的内部异常的引用初始化 DbException 类的新实例
        /// </summary>
        /// <param name="strMsg_">错误信息，可通过属性Message获取</param>
        /// <param name="exInner_">内部异常，可通过属性InnerException获取</param>
        public XDbException(string strMsg_, Exception exInner_)
            : base(strMsg_, exInner_)
        {
        }
    } // XDbException

    /// <summary>
    /// 数据库相关定义信息
    /// </summary>
    public static class XDbInfo
    {
        /// <summary>
        /// 字符串查找时，匹配任意字符串的通配符
        /// </summary>
        public const char MatchAllCharWildCard = '%';
        /// <summary>
        /// 字符串查找时，匹配任意单个字符的通配符
        /// </summary>
        public const char MatchOneCharWildCard = '_';

        /// <summary>
        /// 把语句中的通配符作为普通字符看待
        /// </summary>
        /// <param name="chClause_"></param>
        /// <returns></returns>
        public static string EscapeWildCard(char chClause_)
        {
            return "[" + chClause_ + "]";
        }
    }

} // Namespace