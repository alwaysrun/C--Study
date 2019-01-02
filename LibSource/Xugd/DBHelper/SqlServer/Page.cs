using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SHCre.Xugd.DbHelper
{
    partial class XSqlServerHelper
    {
        /// <summary>
        /// 对表进行分页操作: 获取页信息时，如果没有设定Fields，则返回所有列； 获取页信息时，如果失败（抛出异常），不会改变当前页。
        /// 必须保证数据库中有以下存储过程（用于分页的XspPageGet和获取分页数的XspPageCount）：
        /// XspPageGet 
        /// (
        /// @SrcTable As nvarchar(256),
        /// @OrderBy As nvarchar(64),
        /// @FieldList As nvarchar(512)='*',
        /// @Filter As nvarchar(1024)='',
        /// @PageNum As int=1,
        /// @PageSize As int=10
        /// )
        /// 
        /// XspPageCount
        /// (
        /// @SrcTable As nvarchar(256),
        /// @Filter As nvarchar(1024)=''
        /// )
        /// </summary>
        public class XDbPage
        {
            private int _nCurPage = 1;
            private int _nPageCount;
            private int _nPageSize;
            private string _strFields = string.Empty;
            private string _strFilter = string.Empty;
            private string _strOrder = string.Empty;
            private string _strTable = string.Empty;

            /// <summary>
            /// 缺省构造 
            /// </summary>
            public XDbPage()
            {
                this._strTable = string.Empty;
                this._strOrder = string.Empty;
                this._strFields = string.Empty;
                this._strFilter = string.Empty;
                this._nCurPage = 1;
            }


            /// <summary>
            /// 返回首页
            /// </summary>
            /// <param name="connection">数据库连接</param>
            /// <returns>记录信息列表</returns>
            public List<object[]> First(SqlConnection connection)
            {
                return this.GetRecord(connection, 1);
            }
            /// <summary>
            /// 返回指定的页
            /// </summary>
            /// <param name="connection">数据库连接</param>
            /// <param name="nPage_">要返回的页（必须在1到PageCount间），否则抛出ArgumentOutOfRangeException异常</param>
            /// <returns>记录信息列表</returns>
            public List<object[]> Get(SqlConnection connection, int nPage_)
            {
                if ((nPage_ < 1) || (nPage_ > this._nPageCount))
                {
                    throw new ArgumentOutOfRangeException("Page can not less than 1 or greater than PageCount");
                }
                return this.GetRecord(connection, nPage_);
            }

            private List<object[]> GetRecord(SqlConnection connection, int nPage_)
            {

                List<object[]> list = new List<object[]>(this._nPageSize);
                using (SqlCommand command = new SqlCommand("XspPageGet", connection))
                {
                    command.Parameters.Add(new SqlParameter("@SrcTable", SqlDbType.NVarChar, 0x100)).Value = this._strTable;
                    command.Parameters.Add(new SqlParameter("@OrderBy", SqlDbType.NVarChar, 0x40)).Value = this._strOrder;
                    command.Parameters.Add(new SqlParameter("@FieldList", SqlDbType.NVarChar, 0x200)).Value = this._strFields;
                    command.Parameters.Add(new SqlParameter("@Filter", SqlDbType.NVarChar, 0x100)).Value = this._strFilter;
                    command.Parameters.Add(new SqlParameter("@PageNum", SqlDbType.Int)).Value = nPage_;
                    command.Parameters.Add(new SqlParameter("@PageSize", SqlDbType.Int)).Value = this._nPageSize;
                    command.CommandType = CommandType.StoredProcedure;
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            object[] values = new object[reader.FieldCount];
                            reader.GetValues(values);
                            list.Add(values);
                        }
                        reader.Close();
                    }
                }
                this._nCurPage = nPage_;
                return list;

            }
            /// <summary>
            /// 初始化分页信息（对表中所有内容）
            /// </summary>
            /// <param name="connection">数据库连接</param>
            /// <param name="strTable_">表名称</param>
            /// <param name="strOrder_">排序的列</param>
            /// <param name="nPageSize_">每页大小</param>
            /// <returns>总分页数</returns>
            public int Init(SqlConnection connection, string strTable_, string strOrder_, int nPageSize_)
            {
                return this.Init(connection, strTable_, strOrder_, string.Empty, nPageSize_);
            }
            /// <summary>
            /// 根据指定的过滤条件，初始化分页信息
            /// </summary>
            /// <param name="connection">数据库连接</param>
            /// <param name="strTable_">表名称</param>
            /// <param name="strOrder_">排序的列</param>
            /// <param name="strFilter_">过滤条件（Where后面的信息）</param>
            /// <param name="nPageSize_">每页大小</param>
            /// <returns>总分页数</returns>
            public int Init(SqlConnection connection, string strTable_, string strOrder_, string strFilter_, int nPageSize_)
            {
                if (nPageSize_ < 1)
                {
                    throw new ArgumentException("Page size must be large than 0");
                }
                int num = Total(connection, strTable_, strFilter_);
                this._nPageCount = num / nPageSize_;
                if ((num % nPageSize_) != 0)
                {
                    this._nPageCount++;
                }
                this._strTable = strTable_;
                this._strOrder = strOrder_;
                this._strFields = string.Empty;
                this._strFilter = strFilter_;
                this._nCurPage = 1;
                this._nPageSize = nPageSize_;
                return this._nPageCount;
            }
            /// <summary>
            /// 返回尾页
            /// </summary>
            /// <param name="connection">数据库连接</param>
            /// <returns>记录信息列表</returns>
            public List<object[]> Last(SqlConnection connection)
            {
                return this.GetRecord(connection, this._nPageCount);
            }
            /// <summary>
            /// 返回下一页
            /// </summary>
            /// <param name="connection">数据库连接</param>
            /// <returns>记录信息列表</returns>
            public List<object[]> Next(SqlConnection connection)
            {
                int num = this._nCurPage;
                if (num < this._nPageCount)
                {
                    num++;
                }
                return this.GetRecord(connection, num);
            }
            /// <summary>
            /// 返回上一页
            /// </summary>
            /// <param name="connection">数据库连接</param>
            /// <returns>记录信息列表</returns>
            public List<object[]> Prev(SqlConnection connection)
            {
                int num = this._nCurPage;
                if (num > 1)
                {
                    num--;
                }
                return this.GetRecord(connection, num);
            }
            /// <summary>
            /// 重新设定每页大小（显示条数），必须在init之后调用; 当前页会对应调整为：保证当前显示的第一条数据还在当前页中
            /// </summary>
            /// <param name="connection">数据库连接</param>
            /// <param name="nPageSize_">每页大小（显示条数）</param>
            /// <returns>总分页数</returns>
            public int SetPageSize(SqlConnection connection, int nPageSize_)
            {
                if (nPageSize_ < 1)
                {
                    throw new ArgumentException("Page size must be large than 0");
                }
                int num = Total(connection, this._strTable, this._strFilter);
                int num2 = (this._nCurPage - 1) * this._nPageSize;
                this._nPageCount = num / nPageSize_;
                if ((num % nPageSize_) != 0)
                {
                    this._nPageCount++;
                }
                this._nPageSize = nPageSize_;
                this._nCurPage = (num2 / nPageSize_) + 1;
                return this._nPageCount;
            }
            /// <summary>
            /// 获取总记录数
            /// </summary>
            /// <param name="connection"></param>
            /// <param name="strTable_"></param>
            /// <param name="strFilter_"></param>
            /// <returns></returns>
            public static int Total(SqlConnection connection, string strTable_, string strFilter_)
            {
                int num;

                using (SqlCommand command = new SqlCommand("XspPageCount", connection))
                {
                    command.Parameters.Add(new SqlParameter("@SrcTable", SqlDbType.NVarChar, 0x100)).Value = strTable_;
                    command.Parameters.Add(new SqlParameter("@Filter", SqlDbType.NVarChar, 0x100)).Value = strFilter_;
                    command.CommandType = CommandType.StoredProcedure;
                    num = (int)command.ExecuteScalar();
                }

                return num;
            }

            /// <summary>
            /// 当前显示的页
            /// </summary>
            public int CurPage
            {
                get
                {
                    return this._nCurPage;
                }
            }
            /// <summary>
            /// 要获取的列信息(Select后面的信息，如"Col1, Col2...") 
            /// </summary>
            public string Fields
            {
                get
                {
                    return this._strFields;
                }
                set
                {
                    this._strFields = value;
                }
            }
            /// <summary>
            /// 分页时的过滤条件（sql中的where后内容）
            /// </summary>
            public string Filter
            {
                get
                {
                    return this._strFilter;
                }
            }
            /// <summary>
            /// 排序的列（Order by后面的信息）， 此列上一定要有索引; 如果更改了，则当前页信息将无效
            /// </summary>
            public string Order
            {
                get
                {
                    return this._strOrder;
                }
                set
                {
                    this._strOrder = value;
                }
            }
            /// <summary>
            /// 总页数
            /// </summary>
            public int PageCount
            {
                get
                {
                    return this._nPageCount;
                }
            }
            /// <summary>
            /// 每页的大小
            /// </summary>
            public int PageSize
            {
                get
                {
                    return this._nPageSize;
                }
            }
            /// <summary>
            /// 表名称（From后面的信息）
            /// </summary>
            public string Table
            {
                get
                {
                    return this._strTable;
                }
            }

        }
    }// XSqlServerHelper
}
