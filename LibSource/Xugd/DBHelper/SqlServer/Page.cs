using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;

namespace SHCre.Xugd.DbHelper
{
    partial class XSqlServerHelper
    {
        /// <summary>
        /// �Ա���з�ҳ����: ��ȡҳ��Ϣʱ�����û���趨Fields���򷵻������У� ��ȡҳ��Ϣʱ�����ʧ�ܣ��׳��쳣��������ı䵱ǰҳ��
        /// ���뱣֤���ݿ��������´洢���̣����ڷ�ҳ��XspPageGet�ͻ�ȡ��ҳ����XspPageCount����
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
            /// ȱʡ���� 
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
            /// ������ҳ
            /// </summary>
            /// <param name="connection">���ݿ�����</param>
            /// <returns>��¼��Ϣ�б�</returns>
            public List<object[]> First(SqlConnection connection)
            {
                return this.GetRecord(connection, 1);
            }
            /// <summary>
            /// ����ָ����ҳ
            /// </summary>
            /// <param name="connection">���ݿ�����</param>
            /// <param name="nPage_">Ҫ���ص�ҳ��������1��PageCount�䣩�������׳�ArgumentOutOfRangeException�쳣</param>
            /// <returns>��¼��Ϣ�б�</returns>
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
            /// ��ʼ����ҳ��Ϣ���Ա����������ݣ�
            /// </summary>
            /// <param name="connection">���ݿ�����</param>
            /// <param name="strTable_">������</param>
            /// <param name="strOrder_">�������</param>
            /// <param name="nPageSize_">ÿҳ��С</param>
            /// <returns>�ܷ�ҳ��</returns>
            public int Init(SqlConnection connection, string strTable_, string strOrder_, int nPageSize_)
            {
                return this.Init(connection, strTable_, strOrder_, string.Empty, nPageSize_);
            }
            /// <summary>
            /// ����ָ���Ĺ�����������ʼ����ҳ��Ϣ
            /// </summary>
            /// <param name="connection">���ݿ�����</param>
            /// <param name="strTable_">������</param>
            /// <param name="strOrder_">�������</param>
            /// <param name="strFilter_">����������Where�������Ϣ��</param>
            /// <param name="nPageSize_">ÿҳ��С</param>
            /// <returns>�ܷ�ҳ��</returns>
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
            /// ����βҳ
            /// </summary>
            /// <param name="connection">���ݿ�����</param>
            /// <returns>��¼��Ϣ�б�</returns>
            public List<object[]> Last(SqlConnection connection)
            {
                return this.GetRecord(connection, this._nPageCount);
            }
            /// <summary>
            /// ������һҳ
            /// </summary>
            /// <param name="connection">���ݿ�����</param>
            /// <returns>��¼��Ϣ�б�</returns>
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
            /// ������һҳ
            /// </summary>
            /// <param name="connection">���ݿ�����</param>
            /// <returns>��¼��Ϣ�б�</returns>
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
            /// �����趨ÿҳ��С����ʾ��������������init֮�����; ��ǰҳ���Ӧ����Ϊ����֤��ǰ��ʾ�ĵ�һ�����ݻ��ڵ�ǰҳ��
            /// </summary>
            /// <param name="connection">���ݿ�����</param>
            /// <param name="nPageSize_">ÿҳ��С����ʾ������</param>
            /// <returns>�ܷ�ҳ��</returns>
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
            /// ��ȡ�ܼ�¼��
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
            /// ��ǰ��ʾ��ҳ
            /// </summary>
            public int CurPage
            {
                get
                {
                    return this._nCurPage;
                }
            }
            /// <summary>
            /// Ҫ��ȡ������Ϣ(Select�������Ϣ����"Col1, Col2...") 
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
            /// ��ҳʱ�Ĺ���������sql�е�where�����ݣ�
            /// </summary>
            public string Filter
            {
                get
                {
                    return this._strFilter;
                }
            }
            /// <summary>
            /// ������У�Order by�������Ϣ���� ������һ��Ҫ������; ��������ˣ���ǰҳ��Ϣ����Ч
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
            /// ��ҳ��
            /// </summary>
            public int PageCount
            {
                get
                {
                    return this._nPageCount;
                }
            }
            /// <summary>
            /// ÿҳ�Ĵ�С
            /// </summary>
            public int PageSize
            {
                get
                {
                    return this._nPageSize;
                }
            }
            /// <summary>
            /// �����ƣ�From�������Ϣ��
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
