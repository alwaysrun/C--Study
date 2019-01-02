using System;

using System.Data.SqlClient;

namespace SHCre.Xugd.DbHelper
{
    /// <summary>
    /// ���ݿ��������ʱ���׳����쳣���̳���DbException����
    /// �����.NET�Լ�����Sql���ݿ��׳����쳣һ��ΪSqlException���̳���DbException����
    /// ����һ������£����ݿ�����쳣��׽System.Data.Common.DbException���ɡ�
    /// </summary>
    public class XDbException : System.Data.Common.DbException
    {
        /// <summary>
        /// ʹ��ָ���Ĵ�����Ϣ�ʹ�������ʼ�� DbException �����ʵ��
        /// </summary>
        /// <param name="strMsg_">������Ϣ����ͨ������Message��ȡ</param>
        /// <param name="nError_">������룬��ͨ������ErrorCode��ȡ</param>
        public XDbException(string strMsg_, int nError_)
            : base(strMsg_, nError_)
        {
        }

        /// <summary>
        /// ʹ��ָ���Ĵ�����Ϣ�ͶԵ��´��쳣���ڲ��쳣�����ó�ʼ�� DbException �����ʵ��
        /// </summary>
        /// <param name="strMsg_">������Ϣ����ͨ������Message��ȡ</param>
        /// <param name="exInner_">�ڲ��쳣����ͨ������InnerException��ȡ</param>
        public XDbException(string strMsg_, Exception exInner_)
            : base(strMsg_, exInner_)
        {
        }
    } // XDbException

    /// <summary>
    /// ���ݿ���ض�����Ϣ
    /// </summary>
    public static class XDbInfo
    {
        /// <summary>
        /// �ַ�������ʱ��ƥ�������ַ�����ͨ���
        /// </summary>
        public const char MatchAllCharWildCard = '%';
        /// <summary>
        /// �ַ�������ʱ��ƥ�����ⵥ���ַ���ͨ���
        /// </summary>
        public const char MatchOneCharWildCard = '_';

        /// <summary>
        /// ������е�ͨ�����Ϊ��ͨ�ַ�����
        /// </summary>
        /// <param name="chClause_"></param>
        /// <returns></returns>
        public static string EscapeWildCard(char chClause_)
        {
            return "[" + chClause_ + "]";
        }
    }

} // Namespace