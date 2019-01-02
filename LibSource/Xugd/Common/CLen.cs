
namespace SHCre.Xugd.Common
{
    /// <summary>
    /// �ӿ�����Ҫ��һЩ�����Ķ��壺
    /// ��LenΪ��׺��Ϊ�ַ����ĳ���(�����ΪPInvokeʹ��ʱ����Ҫ+1���洢��β'\0')��
    /// ��SizeΪ��׺��ΪByte����ĳ���
    /// </summary>
    public static class CLen
    {
        /// <summary>
        /// Կ�ױ�����������������С������3��
        /// </summary>
        public const short PinErrMinCount = 3;

        /// <summary>
        /// Կ�ױ�����������������������10��
        /// </summary>
        public const short PinErrMaxCount = 10;

        /// <summary>
        /// Կ�ױ���������������Ĭ�ϴ�����5��
        /// </summary>
        public const short PinErrDefCount = 5;

        /// <summary>
        /// ����������������ڣ�Сʱ����24��
        /// </summary>
        public const int PinErrResetDefHours = 24;

        /// <summary>
        /// Կ�����кŵĳ��ȣ�15��
        /// </summary>
        public const int CKeySNLen = 16 - 1;

        /// <summary>
        /// ����Mac��ַ���ȣ�17��
        /// </summary>
        public const int MacAddrLen = 18 - 1;

        /// <summary>
        /// ���ֽڴ��ʱ����Mac��ַ���ȣ�6��
        /// </summary>
        public const int MacAddrSize = 6;

        /// <summary>
        /// Կ���й��ŵĳ��ȣ�15��
        /// </summary>
        public const int NumberLen = 16 - 1;

        /// <summary>
        /// ���ݿ��б�ŵ���󳤶�
        /// </summary>
        public const int NumberMaxLen = 64 - 1;

        /// <summary>
        /// IP��ַ���ֽڴ��ʱ��󳤶ȣ�16��
        /// </summary>
        public const int IPAddrMaxSize = 16;

        /// <summary>
        /// IPv4��ַ���ȣ�15��
        /// </summary>
        public const int IPv4AddrMaxLen = 16 - 1;

        /// <summary>
        /// Ӳ�����кţ���Ӳ�̡�CPU�ȣ���󳤶ȣ�63��
        /// </summary>
        public const int HardSNMaxLen = 64 - 1;

        /// <summary>
        /// �����Կ��ϵͳ��������Կһ�𣩵��û�����󳤶�
        /// </summary>
        public const int NameInkeyMaxLen = 16 - 1;

        /// <summary>
        /// �û�������󳤶ȣ�63��
        /// </summary>
        public const int NameMaxLen = 64 - 1;

        /// <summary>
        /// �����ʼ�����ַ������󳤶ȣ�63��
        /// </summary>
        public const int EmailMaxLen = 64 - 1;

        /// <summary>
        /// �������󳤶ȣ�31��
        /// </summary>
        public const int PswMaxLen = 32 - 1;

        /// <summary>
        /// �������С���ȣ�6��
        /// </summary>
        public const int PswMinLen = 6;

        /// <summary>
        /// ְ�����󳤶ȣ�15��
        /// </summary>
        public const int DutyMaxLen = 16 - 1;

        /// <summary>
        /// �������Ƶ���󳤶ȣ�31��
        /// </summary>
        public const int DeptNameMaxLen = 32 - 1;

        /// <summary>
        /// ��������ַ��IP������������󳤶ȣ�63��
        /// </summary>
        public const int DomainMaxLen = 64 - 1;

        /// <summary>
        /// Dos�ļ�����󳤶ȣ�15��
        /// </summary>
        public const int DosNameMaxLen = 16 - 1;

        /// <summary>
        /// ��Ϣ����󳤶ȣ�255��
        /// </summary>
        public const int InfoMaxLen = 256 - 1;

        /// <summary>
        /// ·������󳤶ȣ�259��
        /// </summary>
        public const int PathMaxLen = 260 - 1;

        /// <summary>
        /// ������Ϣ����󳤶ȣ�1023��
        /// </summary>
        public const int DescripMaxLen = 1024 - 1;

        /// <summary>
        /// CrcУ��ֵ�Ĵ�С��4��
        /// </summary>
        public const int CrcSize = 4;

        /// <summary>
        /// MD5ֵ�Ĵ�С��16��
        /// </summary>
        public const int MD5Size = 16;

        /// <summary>
        /// ɢ��ֵ��SHA1���ĳ��ȣ�20��
        /// </summary>
        public const int HashSize = 20;

        /// <summary>
        /// �Գ��㷨��Կ��С���ȣ�16��
        /// </summary>
        public const int SymmKeyMinSize = 16;

        /// <summary>
        /// �Գ��㷨��Կ��󳤶ȣ�32��
        /// </summary>
        public const int SymmKeyMaxSize = 32;

        /// <summary>
        /// �Գ��㷨�м��ܿ����󳤶ȣ�16��
        /// </summary>
        public const int SymmBlockMaxSize = 16;

        /// <summary>
        /// RSA�㷨����Կ��󳤶�(512/2*8=2048B)��512��
        /// </summary>
        public const int RSAKeyMaxSize = 512;

        /// <summary>
        /// RSA�㷨����Կ��С����(256/2*8=1024B)��256��
        /// </summary>
        public const int RSAKeyMinSize = 256;

        /// <summary>
        /// RSAǩ���������󳤶ȣ�256��
        /// </summary>
        public const int RSASignMaxSize = 256;

        /// <summary>
        /// RSA���ܣ�һ�����ݿ飩����󳤶ȣ�256��
        /// </summary>
        public const int RSAEncryptMaxSize = 256;
    }
}
