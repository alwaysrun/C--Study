
namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 接口中需要的一些常量的定义：
    /// 以Len为后缀的为字符串的长度(如果作为PInvoke使用时，需要+1来存储结尾'\0')；
    /// 以Size为后缀的为Byte数组的长度
    /// </summary>
    public static class CLen
    {
        /// <summary>
        /// 钥匙保护密码允许重试最小次数（3）
        /// </summary>
        public const short PinErrMinCount = 3;

        /// <summary>
        /// 钥匙保护密码允许重试最大次数（10）
        /// </summary>
        public const short PinErrMaxCount = 10;

        /// <summary>
        /// 钥匙保护密码允许重试默认次数（5）
        /// </summary>
        public const short PinErrDefCount = 5;

        /// <summary>
        /// 错误口令次数清除周期，小时数（24）
        /// </summary>
        public const int PinErrResetDefHours = 24;

        /// <summary>
        /// 钥匙序列号的长度（15）
        /// </summary>
        public const int CKeySNLen = 16 - 1;

        /// <summary>
        /// 网卡Mac地址长度（17）
        /// </summary>
        public const int MacAddrLen = 18 - 1;

        /// <summary>
        /// 按字节存放时网卡Mac地址长度（6）
        /// </summary>
        public const int MacAddrSize = 6;

        /// <summary>
        /// 钥匙中工号的长度（15）
        /// </summary>
        public const int NumberLen = 16 - 1;

        /// <summary>
        /// 数据库中编号的最大长度
        /// </summary>
        public const int NumberMaxLen = 64 - 1;

        /// <summary>
        /// IP地址按字节存放时最大长度（16）
        /// </summary>
        public const int IPAddrMaxSize = 16;

        /// <summary>
        /// IPv4地址长度（15）
        /// </summary>
        public const int IPv4AddrMaxLen = 16 - 1;

        /// <summary>
        /// 硬件序列号（如硬盘、CPU等）最大长度（63）
        /// </summary>
        public const int HardSNMaxLen = 64 - 1;

        /// <summary>
        /// 存放在钥匙系统区（与密钥一起）的用户名最大长度
        /// </summary>
        public const int NameInkeyMaxLen = 16 - 1;

        /// <summary>
        /// 用户名的最大长度（63）
        /// </summary>
        public const int NameMaxLen = 64 - 1;

        /// <summary>
        /// 电子邮件（地址）的最大长度（63）
        /// </summary>
        public const int EmailMaxLen = 64 - 1;

        /// <summary>
        /// 口令的最大长度（31）
        /// </summary>
        public const int PswMaxLen = 32 - 1;

        /// <summary>
        /// 口令的最小长度（6）
        /// </summary>
        public const int PswMinLen = 6;

        /// <summary>
        /// 职务的最大长度（15）
        /// </summary>
        public const int DutyMaxLen = 16 - 1;

        /// <summary>
        /// 部门名称的最大长度（31）
        /// </summary>
        public const int DeptNameMaxLen = 32 - 1;

        /// <summary>
        /// 服务器地址（IP或域名）的最大长度（63）
        /// </summary>
        public const int DomainMaxLen = 64 - 1;

        /// <summary>
        /// Dos文件名最大长度（15）
        /// </summary>
        public const int DosNameMaxLen = 16 - 1;

        /// <summary>
        /// 信息的最大长度（255）
        /// </summary>
        public const int InfoMaxLen = 256 - 1;

        /// <summary>
        /// 路径的最大长度（259）
        /// </summary>
        public const int PathMaxLen = 260 - 1;

        /// <summary>
        /// 描述信息的最大长度（1023）
        /// </summary>
        public const int DescripMaxLen = 1024 - 1;

        /// <summary>
        /// Crc校验值的大小（4）
        /// </summary>
        public const int CrcSize = 4;

        /// <summary>
        /// MD5值的大小（16）
        /// </summary>
        public const int MD5Size = 16;

        /// <summary>
        /// 散列值（SHA1）的长度（20）
        /// </summary>
        public const int HashSize = 20;

        /// <summary>
        /// 对称算法密钥最小长度（16）
        /// </summary>
        public const int SymmKeyMinSize = 16;

        /// <summary>
        /// 对称算法密钥最大长度（32）
        /// </summary>
        public const int SymmKeyMaxSize = 32;

        /// <summary>
        /// 对称算法中加密块的最大长度（16）
        /// </summary>
        public const int SymmBlockMaxSize = 16;

        /// <summary>
        /// RSA算法的密钥最大长度(512/2*8=2048B)（512）
        /// </summary>
        public const int RSAKeyMaxSize = 512;

        /// <summary>
        /// RSA算法的密钥最小长度(256/2*8=1024B)（256）
        /// </summary>
        public const int RSAKeyMinSize = 256;

        /// <summary>
        /// RSA签名结果的最大长度（256）
        /// </summary>
        public const int RSASignMaxSize = 256;

        /// <summary>
        /// RSA加密（一个数据块）的最大长度（256）
        /// </summary>
        public const int RSAEncryptMaxSize = 256;
    }
}
