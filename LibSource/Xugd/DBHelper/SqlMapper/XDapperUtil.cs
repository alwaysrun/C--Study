using System;
using System.Data;
using SHCre.Xugd.Common;
using System.Collections.Generic;

namespace SHCre.Xugd.DbHelper
{
    partial class XDapperORM
    {
        /// <summary>
        /// 数据库连接基类
        /// </summary>
        public interface IConnectBase
        {
            /// <summary>
            /// 获取当前数据库名称
            /// </summary>
            string Name {get;}
            /// <summary>
            /// Sql语句中参数的前缀
            /// </summary>
            string ParamPrefix { get; }
            /// <summary>
            /// 为完成操作时所需的虚表
            /// </summary>
            string FromDual {get;}
            /// <summary>
            /// 连接字符串
            /// </summary>
            string ConnectString { get; set; }

            /// <summary>
            /// 获取可用于条件子句的时间值
            /// </summary>
            /// <param name="dtDate_"></param>
            /// <param name="strDateFormat_">日期格式化字符串</param>
            /// <returns></returns>
            string GetDateTimeValue(DateTime dtDate_, string strDateFormat_);

            /// <summary>
            /// 获取插入或更新的sql语句
            /// </summary>
            /// <param name="strTableName_"></param>
            /// <param name="strKeyColName_">作为判断条件的列名</param>
            /// <param name="lstColNames_">所有列名</param>
            /// <returns></returns>
            string InsertOrUpdateSql(string strTableName_, string strKeyColName_, List<string> lstColNames_);

            /// <summary>
            /// 获取序列的下一个值
            /// </summary>
            /// <param name="strName_">序列名称</param>
            /// <returns></returns>
            int GetSeqNextVal(string strName_);

            /// <summary>
            /// 分页时的子集
            /// </summary>
            /// <param name="strTableName_"></param>
            /// <param name="strOrderClause_"></param>
            /// <param name="strWhereClauses_"></param>
            /// <returns></returns>
            string GetPagedSubset(string strTableName_, string strOrderClause_, string strWhereClauses_);

            /// <summary>
            /// 获取连接
            /// </summary>
            /// <returns></returns>
            IDbConnection GetConnect();
        }

        /// <summary>
        /// 操作数据库语句信息
        /// </summary>
        public class SqlDataInfo
        {
            /// <summary>
            /// Sql语句
            /// </summary>
            public string Sql { get; set; }
            /// <summary>
            /// Sql语句中参数对应的数据
            /// </summary>
            public object Data { get; set; }
            /// <summary>
            /// Sql语句类型
            /// </summary>
            public CommandType? CmdType { get; set; }
        }

        /// <summary>
        /// Where条件子句，时间相关构造语句通过XDapper.BuildDateTimeClause()
        /// </summary>
        public class WhereClause
        {
            /// <summary>
            /// 子句(可以是任意可放在where中的表达式，如 a and b or c)
            /// </summary>
            public string Clause { get; set; }

            /// <summary>
            /// 
            /// </summary>
            public WhereClause() { }

            /// <summary>
            /// 
            /// </summary>
            /// <param name="strClause_"></param>
            public WhereClause(string strClause_)
            {
                Clause = strClause_;
            }

            /// <summary>
            /// 获取子句
            /// </summary>
            /// <returns></returns>
            public string GetClause()
            {
                return string.IsNullOrEmpty(Clause) ? "1=1" : "(" + Clause + ")";
            }

            /// <summary>
            /// And组合条件，修改自身并返回：
            /// (selfClause) and (AddClause)
            /// </summary>
            /// <param name="addClause_">要Add的条件</param>
            /// <returns>返回自身</returns>
            public WhereClause And(WhereClause addClause_)
            {
                if (!string.IsNullOrEmpty(addClause_.Clause))
                {
                    string strAdd = addClause_.Clause.Trim();
                    if (string.IsNullOrEmpty(Clause))
                    {
                        Clause = strAdd;
                    }
                    else
                    {
                        if(Clause.StartsWith("(") && Clause.EndsWith(")"))
                            Clause = string.Format("{0} and ({1})", Clause, strAdd);
                        else
                            Clause = string.Format("({0}) and ({1})", Clause, strAdd);
                    }
                }

                return this;
            }

            /// <summary>
            /// or组合条件，修改自身并返回：
            /// (selfClause) Or (AddClause)
            /// </summary>
            /// <param name="orClause_">要or的条件</param>
            /// <returns>返回自身</returns>
            public WhereClause Or(WhereClause orClause_)
            {
                if (!string.IsNullOrEmpty(orClause_.Clause))
                {
                    string strOr = orClause_.Clause.Trim();
                    if (string.IsNullOrEmpty(Clause))
                    {
                        Clause = strOr;
                    }
                    else
                    {
                        if (Clause.StartsWith("(") && Clause.EndsWith(")"))
                            Clause = string.Format("{0} or ({1})", Clause, strOr);
                        else
                            Clause = string.Format("({0}) or ({1})", Clause, strOr);
                    }
                }

                return this;
            }

            /// <summary>
            /// Not条件，修改自身并返回：
            /// not(selfClause)
            /// </summary>
            /// <returns>返回自身</returns>
            public WhereClause Not()
            {
                if(!string.IsNullOrEmpty(Clause))
                {
                    Clause = string.Format("not({0})", Clause);
                }

                return this;
            }

            /// <summary>
            /// 
            /// </summary>
            /// <returns></returns>
            public override string ToString()
            {
                return GetClause();
            }

            /// <summary>
            /// Like时通配符位置
            /// </summary>
            [Flags]
            public enum WildCharPosition
            {
                /// <summary>
                /// 没有：使用=代替Like
                /// </summary>
                None = 0,
                /// <summary>
                /// 在头：即%value
                /// </summary>
                Start = 1,
                /// <summary>
                /// 在尾：即value%
                /// </summary>
                End = 2,
                /// <summary>
                /// 尾首全部都有:%value%
                /// </summary>
                Both = Start | End
            }

            /// <summary>
            /// 因Oracle与SqlServer对时间的处理不同，所以需要传递Connect来处理时间。
            /// 创建匹配日期时间的条件[Stat，End]或[Start,End)（IncludeEnd=false时）：
            /// dtStart为空则&lt;=dtEnd；
            /// dtEnd为空则&gt;=dtStart;
            /// </summary>
            /// <param name="dbConn_">数据库连接(XDapperORM.BaseConnect)</param>
            /// <param name="strFieldName_">字段名</param>
            /// <param name="dtStart_">起始日期</param>
            /// <param name="dtEnd_">结束日期</param>
            /// <param name="strDateFormat_">日期格式化字符串(支持yy,yyyy,MM,dd,hh,HH,mm,ss)</param>
            /// <param name="bIncludeEnd_">true时包含结束时间，false时不包含结束时间</param>
            /// <returns>匹配条件</returns>
            public static WhereClause BuildDateTimeClause(IConnectBase dbConn_, string strFieldName_, DateTime? dtStart_, DateTime? dtEnd_, string strDateFormat_ = "yyyyMMdd", bool bIncludeEnd_=false)
            {
                if (string.IsNullOrEmpty(strFieldName_))
                    throw new ArgumentException("Invalid search field name");
                if ((dtStart_ == null) && (dtEnd_ == null))
                    throw new ArgumentException("Invalid search date interval");

                string strClause;
                string strEndCmp = "<";
                if (bIncludeEnd_)
                    strEndCmp = "<=";
                if (dtStart_ == null)
                {
                    strClause = string.Format("{0}{1}{2}", strFieldName_, strEndCmp, dbConn_.GetDateTimeValue(dtEnd_.Value, strDateFormat_));
                }
                else if (dtEnd_ == null)
                {
                    strClause = string.Format("{0}>={1}", strFieldName_, dbConn_.GetDateTimeValue(dtStart_.Value, strDateFormat_));
                }
                else
                {
                    if (dtStart_ > dtEnd_)
                        throw new ArgumentException("Start-date must not larger than end-date");
                    strClause = string.Format("{0}>={1} And {0}{2}{3}", strFieldName_,
                        dbConn_.GetDateTimeValue(dtStart_.Value, strDateFormat_), 
                        strEndCmp, dbConn_.GetDateTimeValue(dtEnd_.Value, strDateFormat_));
                }

                return new WhereClause(strClause);
            }

            /// <summary>
            /// 查询所有的子句
            /// </summary>
            /// <returns></returns>
            public static WhereClause BuildAllClause()
            {
                return new WhereClause("1=1");
            }

            /// <summary>
            /// 创建字符串的查找子句：
            /// 如果完全匹配则传入WildCharPosition.None；
            /// 如果是国际字符strValuePre传入"N"，否则string.empty
            /// </summary>
            /// <param name="strFieldName_">字段名</param>
            /// <param name="strToSearch_">要查找的内容</param>
            /// <param name="euPosition_">通配符添加的位置（如果为None，则使用'='；否则使用'Like'）</param>
            /// <param name="strValuePre_">值的前缀，如果是Unicode则使用"N"，否则为空即可</param>
            /// <returns>匹配条件(Field Like '%Value%'; Field='Value')</returns>
            public static WhereClause BuildLikeClause(string strFieldName_, string strToSearch_, WildCharPosition euPosition_ = WildCharPosition.None, string strValuePre_ = "N")
            {
                if (string.IsNullOrEmpty(strFieldName_))
                    throw new ArgumentException("Invalid search field name");
                if (string.IsNullOrEmpty(strToSearch_))
                    throw new ArgumentException("Invalid search value");

                string strJoiner;
                string strValue = strToSearch_.Trim();
                if (euPosition_ == WildCharPosition.None)
                {
                    strJoiner = "=";
                }
                else
                {
                    strJoiner = "Like";

                    if (XFlag.Check(euPosition_, WildCharPosition.Start))
                        strValue = XDbInfo.MatchAllCharWildCard + strValue;
                    if (XFlag.Check(euPosition_, WildCharPosition.End))
                        strValue += XDbInfo.MatchAllCharWildCard;
                }

                if (strValuePre_ == null)
                    strValuePre_ = string.Empty;
                return new WhereClause(string.Format("{0} {1} {2}'{3}'", strFieldName_, strJoiner, strValuePre_, strValue));
            }

            /// <summary>
            /// 创建值类型的相等匹配的条件，字符串使用BuildLikeClause
            /// </summary>
            /// <typeparam name="T">要查找字段的类型</typeparam>
            /// <param name="strFieldName_">字段名</param>
            /// <param name="tValue_">要查找的值</param>
            /// <returns>匹配条件(Field=Value)</returns>
            public static WhereClause BuildEqualClause<T>(string strFieldName_, T tValue_) where T : struct
            {
                if (string.IsNullOrEmpty(strFieldName_))
                    throw new ArgumentException("Invalid search field name");

                string strClause = null;
                if (tValue_ is bool)
                {
                    bool bValue = bool.Parse(tValue_.ToString());
                    strClause = string.Format("{0}={1}", strFieldName_, bValue ? 1 : 0);
                }
                else
                {
                    strClause = string.Format("{0}={1}", strFieldName_, tValue_);
                }

                return new WhereClause(strClause);
            }

            /// <summary>
            /// 创建值范围匹配的条件[tLower_, tUpper_]
            /// </summary>
            /// <typeparam name="T">要查找字段的类型</typeparam>
            /// <param name="strFieldName_">字段名</param>
            /// <param name="tLower_">下限值</param>
            /// <param name="tUpper_">上限值</param>
            /// <returns>匹配条件(Field between Lower And Upper;)</returns>
            public static WhereClause BuildRangeSearch<T>(string strFieldName_, T? tLower_, T? tUpper_) where T : struct, IComparable
            {
                if (string.IsNullOrEmpty(strFieldName_))
                    throw new ArgumentException("Invalid search field name");
                if ((tLower_ == null) && (tUpper_ == null))
                    throw new ArgumentException("Invalid search range");

                string strClause = null;
                if (tLower_ == null)
                {
                    strClause = string.Format("{0}<={1}", strFieldName_, tUpper_.Value);
                }
                else if (tUpper_ == null)
                {
                    strClause = string.Format("{0}>={1}", strFieldName_, tLower_.Value);
                }
                else
                {
                    if (tLower_.Value.CompareTo(tUpper_.Value) > 0)
                        throw new ArgumentException("Upper must not larger then Lower");

                    strClause = string.Format("{0} between {1} and {2}", strFieldName_, tLower_.Value, tUpper_.Value);
                }

                return new WhereClause(strClause);
            }

            /// <summary>
            /// 创建匹配空字段的条件
            /// </summary>
            /// <param name="strFieldName_">字段名</param>
            /// <returns>匹配条件</returns>
            public static WhereClause BuildNullClause(string strFieldName_)
            {
                return new WhereClause(string.Format("{0} is NULL", strFieldName_));
            }

            /// <summary>
            /// 创建匹配非空字段的条件
            /// </summary>
            /// <param name="strFieldName_">字段名</param>
            /// <returns>匹配条件</returns>
            public static WhereClause BuildNotNullClause(string strFieldName_)
            {
                return new WhereClause(string.Format("{0} is Not NULL", strFieldName_));
            }

            #region "Not test"
            ///// <summary>
            ///// 创建包含指定标识的条件
            ///// </summary>
            ///// <param name="strFieldName_">字段名</param>
            ///// <param name="nFlag_">包含的标识</param>
            ///// <returns>匹配条件</returns>
            //public static WhereClause BuildIncludeFlagClause(string strFieldName_, uint nFlag_)
            //{
            //    return new WhereClause(string.Format("({0} & {1})={1}", strFieldName_, nFlag_));
            //}

            ///// <summary>
            ///// 创建不包含指定标识的条件
            ///// </summary>
            ///// <param name="strFieldName_">字段名</param>
            ///// <param name="nFlag_">不包含的标识</param>
            ///// <returns>匹配条件</returns>
            //public static WhereClause BuildExcludeFlagClause(string strFieldName_, uint nFlag_)
            //{
            //    return new WhereClause(string.Format("({0} & {1})=0", strFieldName_, nFlag_));
            //}

            //#region "IP Search"
            //private static string Byte2Varbinary(byte[] bySrc_)
            //{
            //    StringBuilder sbDest = new StringBuilder(22 * bySrc_.Length + 1);
            //    foreach (byte bElem in bySrc_)
            //    {
            //        if (sbDest.Length > 0)
            //            sbDest.Append("+");
            //        sbDest.Append(string.Format("Cast({0} As binary(1))", bElem));
            //    }

            //    return "(" + sbDest.ToString() + ")";
            //}

            ///// <summary>
            ///// 构造IP地址匹配条件：IP地址一个Binary方式存放时
            ///// </summary>
            ///// <param name="strFieldName_">字段名</param>
            ///// <param name="strIpStart_">起始IP地址</param>
            ///// <param name="strIpEnd_">结束IP地址</param>
            ///// <returns>匹配条件</returns>
            //public static WhereClause BuildIpAddrClause(string strFieldName_, string strIpStart_, string strIpEnd_)
            //{
            //    if (string.IsNullOrEmpty(strFieldName_))
            //        throw new ArgumentException("Invalid search field name");
            //    if (string.IsNullOrEmpty(strIpStart_) || string.IsNullOrEmpty(strIpEnd_))
            //        throw new ArgumentException("Invalid search ip addr range");

            //    byte[] byStart = (XConvert.IPAddrString2Bytes(strIpStart_));
            //    byte[] byEnd = (XConvert.IPAddrString2Bytes(strIpEnd_));
            //    if (XCompare.Compare(byStart, byEnd) > 0)
            //        throw new ArgumentException("IP start must not larger than ip end");

            //    return new WhereClause(string.Format("{0} between {1} And {2}",
            //        strFieldName_, Byte2Varbinary(byStart), Byte2Varbinary(byEnd)));
            //}
            //#endregion
            #endregion
        }
    }
}
