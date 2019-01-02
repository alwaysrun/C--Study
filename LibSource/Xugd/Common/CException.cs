using System;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// 异常基类，可通过GetMessage获取具体异常信息
    /// </summary>
    public class XBaseException : SystemException
    {
        /// <summary>
        /// 
        /// </summary>
        public XBaseException()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        public XBaseException(string strInfo_) : base(strInfo_) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XBaseException(string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        { }

        /// <summary>
        /// 获取消息（包括类名)
        /// </summary>
        /// <returns></returns>
        public virtual string GetMessage()
        {
            return string.Format("{0}({1})", XReflex.GetTypeName(this, false), this.Message);
        }
    }

    /// <summary>
    /// 反射相关错误
    /// </summary>
    public class XReflectException : XBaseException
    {
        /// <summary>
        /// 
        /// </summary>
        public XReflectException()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        public XReflectException(string strInfo_) : base(strInfo_) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XReflectException(string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strType_"></param>
        /// <param name="strFile_"></param>
        /// <param name="tInst_"></param>
        public XReflectException(string strType_, string strFile_, Type tInst_)
            : base(string.Format("Load {0} from {1} failed", strType_, strFile_))
        {
            ReflectType = tInst_;
        }


        /// <summary>
        /// 反射的类型
        /// </summary>
        public Type ReflectType { get; set; }
        
        /// <summary>
        /// 获取消息（包括类名)
        /// </summary>
        /// <returns></returns>
        public override string GetMessage()
        {
            string strType = string.Empty;
            if (ReflectType != null)
                strType = string.Format("<{0}>", ReflectType.Name);
            return string.Format("XReflectException{0}({1})", strType, this.Message);
        }
    }

    /// <summary>
    /// 未找到
    /// </summary>
    public class XNotFoundException:XBaseException
    {
        /// <summary>
        /// 
        /// </summary>
        public XNotFoundException()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        public XNotFoundException(string strInfo_) : base(strInfo_) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XNotFoundException(string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        { }

        /// <summary>
        /// 获取消息（包括类名)
        /// </summary>
        /// <returns></returns>
        public override string GetMessage()
        {
            return string.Format("XNotFoundException({0})", this.Message);
        }
    }

    /// <summary>
    /// 未找到
    /// </summary>
    public class XExistedException : XBaseException
    {
        /// <summary>
        /// 
        /// </summary>
        public XExistedException()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        public XExistedException(string strInfo_) : base(strInfo_) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XExistedException(string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        { }

        /// <summary>
        /// 获取消息（包括类名)
        /// </summary>
        /// <returns></returns>
        public override string GetMessage()
        {
            return string.Format("XExistedException({0})", this.Message);
        }
    }

    /// <summary>
    /// 操作（命令）失败
    /// </summary>
    public class XOperateException : XBaseException
    {
        /// <summary>
        /// 
        /// </summary>
        public XOperateException()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        public XOperateException(string strInfo_) : base(strInfo_) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XOperateException(string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        { }

        /// <summary>
        /// 获取消息（包括类名)
        /// </summary>
        /// <returns></returns>
        public override string GetMessage()
        {
            return string.Format("XOperateException({0})", this.Message);
        }
    }

    /// <summary>
    /// 缺少条件
    /// </summary>
    public class XConditionException : XBaseException
    {
        /// <summary>
        /// 
        /// </summary>
        public XConditionException()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        public XConditionException(string strInfo_) : base(strInfo_) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XConditionException(string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        { }

        /// <summary>
        /// 获取消息（包括类名)
        /// </summary>
        /// <returns></returns>
        public override string GetMessage()
        {
            return string.Format("XConditionException({0})", this.Message);
        }
    }

    /// <summary>
    /// 冲突
    /// </summary>
    public class XConflictException : XBaseException
    {
        /// <summary>
        /// 
        /// </summary>
        public XConflictException()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        public XConflictException(string strInfo_) : base(strInfo_) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XConflictException(string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        { }

        /// <summary>
        /// 获取消息（包括类名)
        /// </summary>
        /// <returns></returns>
        public override string GetMessage()
        {
            return string.Format("XConflictException({0})", this.Message);
        }
    }

    /// <summary>
    /// 数据相关错误
    /// </summary>
    public class XDataException : XBaseException
    {
        /// <summary>
        /// 
        /// </summary>
        public XDataException()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        public XDataException(string strInfo_) : base(strInfo_) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XDataException(string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        { }
    }

    /// <summary>
    /// 连接
    /// </summary>
    public class XConnectException : XBaseException
    {
        /// <summary>
        /// 
        /// </summary>
        public XConnectException()
            : base()
        { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        public XConnectException(string strInfo_) : base(strInfo_) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XConnectException(string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        { }

        ///// <summary>
        ///// 获取消息（包括类名)
        ///// </summary>
        ///// <returns></returns>
        //public override string GetMessage()
        //{
        //    return string.Format("XConnectException({0})", this.Message);
        //}
    }

    /// <summary>
    /// 未登录相关异常
    /// </summary>
    public class XNotLoginException : XConnectException
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        public XNotLoginException(string strInfo_) : base(strInfo_) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strInfo_"></param>
        /// <param name="exInner_"></param>
        public XNotLoginException(string strInfo_, Exception exInner_)
            : base(strInfo_, exInner_)
        { }

        /// <summary>
        /// 谁
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 获取消息（包括类名)
        /// </summary>
        /// <returns></returns>
        public override string GetMessage()
        {
            return string.Format("XNotLoginException({0}: {1})", UserName, Message);
        }
    }

    #region "SHExcepions"
    /// <summary>
    /// 常用错误码（与windows错误码统一）
    /// </summary>
    public enum SHErrorCode
    {
        /// <summary>
        /// 无效的函数(1)
        /// </summary>
        InvalidFunction = 1,
        /// <summary>
        /// 文件未找到(2)
        /// </summary>
        FileNotFound = 2,
        /// <summary>
        /// 路径未找到(3)
        /// </summary>
        PathNotFound = 3,
        /// <summary>
        /// 打开文件过多(4)
        /// </summary>
        TooManyOpenFiles = 4,
        /// <summary>
        /// 拒绝访问（没有权限）(5)
        /// </summary>
        AccessDenied = 5,
        /// <summary>
        /// 无效的句柄(6)
        /// </summary>
        InvalidHandle = 6,
        /// <summary>
        /// 内存不足（申请内存失败）(8)
        /// </summary>
        NotEnoughMemory = 8,
        /// <summary>
        /// 无效的块(9)
        /// </summary>
        InvalidBlock = 9,
        /// <summary>
        /// 格式错误(11)
        /// </summary>
        BadFormat = 11,
        /// <summary>
        /// 数据无效(13)
        /// </summary>
        InvalidData = 13,
        /// <summary>
        /// 超出内存(14)
        /// </summary>
        OutOfMemory = 14,
        /// <summary>
        /// 无效的驱动(15)
        /// </summary>
        InvalidDrive = 15,
        /// <summary>
        /// 没有更多的文件(18)
        /// </summary>
        NoMoreFile = 18,
        /// <summary>
        /// 写保护(19)
        /// </summary>
        WriteProtect = 19,
        /// <summary>
        /// 未准备好(21)
        /// </summary>
        NotReady = 21,
        /// <summary>
        /// 错误的命令(22)
        /// </summary>
        BadCommand = 22,
        /// <summary>
        /// 校验码（CRC）验证失败(23)
        /// </summary>
        ErrorCrc = 23,
        /// <summary>
        /// 错误的长度(24)
        /// </summary>
        BadLength = 24,
        /// <summary>
        /// 写失败(29)
        /// </summary>
        WriteFault = 29,
        /// <summary>
        /// 读失败(30)
        /// </summary>
        ReadFault = 30,
        /// <summary>
        /// 坏的盘(34)
        /// </summary>
        WrongDisk = 34,
        /// <summary>
        /// 句柄尾（达到文件末尾）(38)
        /// </summary>
        HanleEOF = 38,
        /// <summary>
        /// 不支持(50)
        /// </summary>
        NotSupported = 50,
        /// <summary>
        /// 驱动不存在(55)
        /// </summary>
        DevNotExist = 55,
        /// <summary>
        /// 文件已存在(80)
        /// </summary>
        FileExists = 80,
        /// <summary>
        /// 不能创建文件或目录(82)
        /// </summary>
        CannotMake = 82,
        /// <summary>
        /// 无效的口令(86)
        /// </summary>
        InvalidPassword = 86,
        /// <summary>
        /// 无效的参数(87)
        /// </summary>
        InvalidParam = 87,
        /// <summary>
        /// 驱动被锁定(108)
        /// </summary>
        DriveLocked = 108,
        /// <summary>
        /// 打开失败(110)
        /// </summary>
        OpenFailed = 110,
        /// <summary>
        /// 缓冲区溢出(111)
        /// </summary>
        BufferOverFlow = 111,
        /// <summary>
        /// 调用未实现(120)
        /// </summary>
        CallNotImplemented = 120,
        /// <summary>
        /// 缓冲区（内存）不足(122)
        /// </summary>
        InsufficientBuffer = 122,
        /// <summary>
        /// 无效的名称(123)
        /// </summary>
        InvalidName = 123,
        /// <summary>
        /// 进程未找到(127)
        /// </summary>
        ProcNotFound = 127,
        /// <summary>
        /// 不是根目录(144)
        /// </summary>
        DirNotRoot = 144,
        /// <summary>
        /// 目录非空(145)
        /// </summary>
        DirNotEmpty = 145,
        /// <summary>
        /// 未锁定(158)
        /// </summary>
        NotLocked = 158,
        /// <summary>
        /// 错误的路径名(161)
        /// </summary>
        BadPathName = 161,
        /// <summary>
        /// 锁定失败(164)
        /// </summary>
        LockFailed = 164,
        /// <summary>
        /// 忙碌(170)
        /// </summary>
        Busy = 170,
        /// <summary>
        /// 已经存在(183)
        /// </summary>
        AlreadyExists = 183,
        /// <summary>
        /// 锁定(212)
        /// </summary>
        Locked = 212,
        /// <summary>
        /// 错误的文件类型(222)
        /// </summary>
        BadFileType = 222,
        /// <summary>
        /// 文件太大（超过2G）(223)
        /// </summary>
        FileTooLarge = 223,
        /// <summary>
        /// 没有数据(232)
        /// </summary>
        NoData = 232,
        /// <summary>
        /// 有更多的数据(234)
        /// </summary>
        MoreData = 234,
        /// <summary>
        /// 等待（一般为信号相关）超时(258)
        /// </summary>
        WaitTimeOut = 258,
        /// <summary>
        /// 没有更多的项(259)
        /// </summary>
        NoMoreItem = 259,
        /// <summary>
        /// 不能复制(266)
        /// </summary>
        CannotCopy = 266,
        /// <summary>
        /// 是目录(267)
        /// </summary>
        Directory = 267,
        /// <summary>
        /// 不是所有者（Owner）(288)
        /// </summary>
        NotOwner = 288,
        /// <summary>
        /// 无效的地址(487)
        /// </summary>
        InvalidAddress = 487,
        /// <summary>
        /// 无效的消息(1002)
        /// </summary>
        InvalidMessage = 1002,
        /// <summary>
        /// 不能（无法）完成(1003)
        /// </summary>
        CannotComplete = 1003,
        /// <summary>
        /// 无效的标志(1004)
        /// </summary>
        InvalidFlags = 1004,
        /// <summary>
        /// 文件无效(1005)
        /// </summary>
        FileInvalid = 1006,
        /// <summary>
        /// 没有令牌（Token）(1008)
        /// </summary>
        NoToken = 1008,
        /// <summary>
        /// 错误的注册表数据库(1009)
        /// </summary>
        BadDB = 1009,
        /// <summary>
        /// 错误的注册表键(1010)
        /// </summary>
        BadKey = 1010,
        /// <summary>
        /// 服务已在运行(1065)
        /// </summary>
        ServiceAlreadyRunning = 1056,
        /// <summary>
        /// 服务被禁用(1058)
        /// </summary>
        ServiceDisabled = 1058,
        /// <summary>
        /// 服务不存在(1060)
        /// </summary>
        ServiceNotExists = 1060,
        /// <summary>
        /// 服务未激活(1062)
        /// </summary>
        ServiceNotActive = 1062,
        /// <summary>
        /// 数据库不存在(1065)
        /// </summary>
        DatabaseNotExist = 1065,
        /// <summary>
        /// 服务已存在(1073)
        /// </summary>
        ServiceExists = 1073,
        /// <summary>
        /// 服务名已存在（重名）(1078)
        /// </summary>
        ServiceNameSake = 1078,
        /// <summary>
        /// 无效的块长度(1106)
        /// </summary>
        InvalidBlockLen = 1106,
        /// <summary>
        /// 库初始化失败(1114)
        /// </summary>
        DllInitFailed = 1114,
        /// <summary>
        /// Windows版本太低
        /// </summary>
        WinVerLow = 1150,
        /// <summary>
        /// 无效的库(1154)
        /// </summary>
        InvalidDll = 1154,
        /// <summary>
        /// 库未找到(1157)
        /// </summary>
        DllNotFound = 1157,
        /// <summary>
        /// 未找到(1168)
        /// </summary>
        NotFound = 1168,
        /// <summary>
        /// 不匹配(1169)
        /// </summary>
        NoMatch = 1169,
        /// <summary>
        /// 错误的设备(1200)
        /// </summary>
        DadDevice = 1200,
        /// <summary>
        /// 错误的供应者(1204)
        /// </summary>
        BadProvider = 1204,
        /// <summary>
        /// 无效的组名(1209)
        /// </summary>
        InvalidGroupName = 1209,
        /// <summary>
        /// 无效的域名(1212)
        /// </summary>
        InvalidDomainName = 1212,
        /// <summary>
        /// 无效的服务名(1213)
        /// </summary>
        InvalidServiceName = 1213,
        /// <summary>
        /// 无效的网络名(1214)
        /// </summary>
        InvalidNetName = 1214,
        /// <summary>
        /// 没有网络(1222)
        /// </summary>
        NoNetwork = 1222,
        /// <summary>
        /// 已取消(1223)
        /// </summary>
        Cancelled = 1223,
        /// <summary>
        /// 无效的连接(1229)
        /// </summary>
        InvalidConnection = 1229,
        /// <summary>
        /// 连接激活(1230)
        /// </summary>
        ConnectionActive = 1230,
        /// <summary>
        /// 重试(1237)
        /// </summary>
        Retry = 1237,
        /// <summary>
        /// 错误的地址(1241)
        /// </summary>
        IncorrectAddress = 1241,
        /// <summary>
        /// 已注册(1242)
        /// </summary>
        AlreadyRegistered = 1242,
        /// <summary>
        /// 服务未找到(1243)
        /// </summary>
        ServiceNotFound = 1243,
        /// <summary>
        /// 未验证(1244)
        /// </summary>
        NotAuthenticated = 1244,
        /// <summary>
        /// 没有登录(1245)
        /// </summary>
        NotLoggedOn = 1245,
        /// <summary>
        /// 已初始化(1247)
        /// </summary>
        AlreadyInitialized = 1247,
        /// <summary>
        /// 没有更多的设备(1248)
        /// </summary>
        NoMoreDevice = 1248,
        /// <summary>
        /// 恢复失败(1279)
        /// </summary>
        RecoveryFailure = 1279,
        /// <summary>
        /// 无效的所有者（Owner）(1307)
        /// </summary>
        InvalidOwner = 1307,
        /// <summary>
        /// 用户已存在(1316)
        /// </summary>
        UserExists = 1316,
        /// <summary>
        /// 没有指定用户(1317)
        /// </summary>
        NoSuchUser = 1317,
        /// <summary>
        /// 组已存在(1318)
        /// </summary>
        GroupExists = 1218,
        /// <summary>
        /// 没有指定的组(1319)
        /// </summary>
        NoSuchGroup = 1319,
        /// <summary>
        /// 错误的口令(1323)
        /// </summary>
        WrongPassword = 1323,
        /// <summary>
        /// 登录失败(1326)
        /// </summary>
        LogonFailure = 1326,
        /// <summary>
        /// 口令过期(1330)
        /// </summary>
        PasswordExpired = 1330,
        /// <summary>
        /// 账户无效(1331)
        /// </summary>
        AccountDisabled = 1331,
        /// <summary>
        /// 没有指定的域(1355)
        /// </summary>
        NoSuchDomain = 1355,
        /// <summary>
        /// 域已存在(1356)
        /// </summary>
        DomainExists = 1356,
        /// <summary>
        /// 令牌（Token）已在使用(1375)
        /// </summary>
        TokenAlreadyInUse = 1375,
        /// <summary>
        /// 文件已破坏(1392)
        /// </summary>
        FileCorrupt = 1392,
        /// <summary>
        /// 磁盘已破坏(1393)
        /// </summary>
        DiskCorrupt = 1393,
        /// <summary>
        /// 无效的索引(1413)
        /// </summary>
        InvalidIndex = 1413,
        /// <summary>
        /// 粘贴板未打开(1418)
        /// </summary>
        ClipboardNotOpen = 1418,
        /// <summary>
        /// 超时(1460)
        /// </summary>
        TimeOut = 1460,
        /// <summary>
        /// 错误的大小(1462)
        /// </summary>
        InCorrectSize = 1462,
        /// <summary>
        /// 数据格式不匹配(1429)
        /// </summary>
        DataTypeMismatch = 1629,
        /// <summary>
        /// 不支持的类型(1430)
        /// </summary>
        UnsupportType = 1630,
        /// <summary>
        /// 创建失败(1431)
        /// </summary>
        CreateFailed = 1631,
        /// <summary>
        /// 产品版本错误(1438)
        /// </summary>
        ProductVersion = 1638,
        /// <summary>
        /// 账户过期(1793)
        /// </summary>
        AccountExpired = 1793,
        /// <summary>
        /// 未知端口(1796)
        /// </summary>
        UnknownPort = 1796,
        /// <summary>
        /// 无效的数据类型(1804)
        /// </summary>
        InvalidDataType = 1804,
        /// <summary>
        /// 无效的时间(1901)
        /// </summary>
        InvalidTime = 1901,
        /// <summary>
        /// 口令必须更换(1907)
        /// </summary>
        PasswordMustChange = 1907,
        /// <summary>
        /// 账户被锁定(1909)
        /// </summary>
        AccountLockedOut = 1909,
        /// <summary>
        /// 错误的用户名(2202)
        /// </summary>
        BadUserName = 2202,
        /// <summary>
        /// 未连接(2250)
        /// </summary>
        NotConnect = 2250,
        /// <summary>
        /// 激活的连接(2402)
        /// </summary>
        ActiveConnections = 2402,
        /// <summary>
        /// 设备在使用(2404)
        /// </summary>
        DeviceInUse = 2404,
        /// <summary>
        /// 为空(4306)
        /// </summary>
        Empty = 4306,
        /// <summary>
        /// 不为空(4307)
        /// </summary>
        NotEmpty = 4307,
        /// <summary>
        /// 对象未找到(4312)
        /// </summary>
        ObjectNotFound = 4312,
        /// <summary>
        /// 数据库失败(4313)
        /// </summary>
        DatabaseFailure = 4313,
        /// <summary>
        /// 数据库已满(4314)
        /// </summary>
        DatabaseFull = 4314,
        /// <summary>
        /// 无效的操作(4317)
        /// </summary>
        InvalidOperation = 4317,
        /// <summary>
        /// 设备未准备好(4319)
        /// </summary>
        DeviceNotAvailable = 4319,
        /// <summary>
        /// 请求被重用(4320)
        /// </summary>
        RequestRefused = 4320,
        /// <summary>
        /// 依赖未找到(5002)
        /// </summary>
        DependencyNotFound = 5002,
        /// <summary>
        /// 依赖已存在(5003)
        /// </summary>
        DependencyAlreadExists = 5003,
        /// <summary>
        /// 资源未准备好(5006)
        /// </summary>
        ResourceNotAvailable = 5006,
        /// <summary>
        /// 资源未找到(5007)
        /// </summary>
        ResourceNotFound = 5007,
        /// <summary>
        /// 对象已存在(5010)
        /// </summary>
        ObjectAlreadyExists = 5010,
        /// <summary>
        /// 无效的状态(5023)
        /// </summary>
        InvalidState = 5023,
        /// <summary>
        /// 加密失败(6000)
        /// </summary>
        EncryptionFailed = 6000,
        /// <summary>
        /// 解密失败(6001)
        /// </summary>
        DecryptionFailed = 6001,
        /// <summary>
        /// 文件已加密(6002)
        /// </summary>
        FileEncrypted = 6002,
        /// <summary>
        /// 没有加密文件系统(6004)
        /// </summary>
        NoEFS = 6004,
        /// <summary>
        /// 错误的加密文件系统(6005)
        /// </summary>
        WrongEFS = 6005,
        /// <summary>
        /// 没有用户键（UserKeys）(6006)
        /// </summary>
        NoUserKeys = 6006,
        /// <summary>
        /// 文件未加密(6007)
        /// </summary>
        FileNotEncrypt = 6007,
        /// <summary>
        /// 不是导出的格式(6008)
        /// </summary>
        NotExportFormat = 6008,
        /// <summary>
        /// 文件未只读(6009)
        /// </summary>
        FileReadOnly = 6009
    };

	/// <summary>
	/// 我们定义的所有异常基类，
	/// 所有我们自己封装的C++库抛出的异常都为此类或其子类
	/// </summary>
	public class SHException 
		:  ApplicationException
	{
		int m_ErrorCode; 
	
		/// <summary>
		/// 构造函数，构造异常基类
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		/// <param name="nErrCode_">错误码（参考SHErrorCode）</param>
		public SHException(string strMsg_, int nErrCode_) 
			: base(strMsg_)
		{
			m_ErrorCode = nErrCode_;
		}

        /// <summary>
        /// 构造函数，构造异常基类
        /// </summary>
        /// <param name="strMsg_">错误说明</param>
        /// <param name="euErrCode_">错误码</param>
        public SHException(string strMsg_, SHErrorCode euErrCode_)
            : this(strMsg_, (int)euErrCode_)
        {
        }

		/// <summary>
		/// 获取错误码（参考SHErrorCode）
		/// </summary>
		public int ErrorCode
		{
            get
            {
			    return m_ErrorCode; 
            }
		}
	};

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 内存不足（申请失败）异常，继承自SHException
	/// </summary>
	public class SHMemException :  SHException
	{
	
		/// <summary>
		/// 构造函数，构造错误码为NotEnoughMemory的内存异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		public SHMemException(string strMsg_)
			: base(strMsg_, (int)SHErrorCode.NotEnoughMemory)
		{
		}
	};

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 用户操作相关异常，继承自SHException
	/// </summary>
	public class SHUserException :  SHException
	{
	
		/// <summary>
		/// 构造函数，构造用户相关异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		/// <param name="nErrCode_">错误码（参考SHErrorCode）</param>
		public SHUserException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	};

	/// <summary>
	/// 用户取消当前操作时，抛出的异常（一般用于回调），继承自SHUserException
	/// </summary>
	public class SHUserCancelException :  SHUserException
	{
	
		/// <summary>
		/// 构造函数，构造错误码为Cancelled的操作取消异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		public SHUserCancelException(string strMsg_)
			: base(strMsg_, (int)SHErrorCode.Cancelled)
		{
		}
	};

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Windows操作相关错误异常，继承自SHException
	/// </summary>
	public class SHWinException :  SHException
	{
	
		/// <summary>
		/// 构造函数，构造Windows操作相关错误异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		/// <param name="nErrCode_">错误码（参考SHErrorCode）</param>
		public SHWinException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	};

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 文件操作相关错误异常，继承自SHException
	/// </summary>
	public class SHFileException :  SHException
	{
	
		/// <summary>
		/// 构造函数，构造文件操作相关错误异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		/// <param name="nErrCode_">错误码（参考SHErrorCode）</param>
		public SHFileException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	/// <summary>
	/// 注册表操作相关错误异常，继承自SHFileException
	/// </summary>
	public class SHRegistryException :  SHFileException
	{
	
		/// <summary>
		/// 构造函数，构造注册表操作相关错误异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		/// <param name="nErrCode_">错误码（参考SHErrorCode）</param>
		public SHRegistryException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	/// <summary>
	/// 动态库操作（加载等）操作相关错误异常，继承自SHFileException
	/// </summary>
	public class SHDllException :  SHFileException
	{	
		/// <summary>
		/// 构造函数，构造动态库（dll）操作相关错误异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		/// <param name="nErrCode_">错误码（参考SHErrorCode）</param>
		public SHDllException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 信号量（临界区、互斥、事件等）操作相关错误异常，继承自SHException
	/// </summary>
	public class SHSynException :  SHException
	{
	
		/// <summary>
		/// 构造函数，构造信号量（临界区、互斥、事件等）操作相关错误异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		/// <param name="nErrCode_">错误码（参考SHErrorCode）</param>
		public SHSynException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 参数相关错误异常，继承自SHException
	/// </summary>
	public class SHParamException :  SHException
	{
	
		/// <summary>
		/// 构造函数，构造参数相关错误异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		/// <param name="nErrCode_">错误码（参考SHErrorCode）</param>
		public SHParamException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	/// <summary>
	/// 参数为NULL的错误异常，继承自SHParamException
	/// </summary>
	public class SHNullParamException :  SHParamException 
	{
	
		/// <summary>
		/// 构造函数，构造参数为空（NULL）的异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		/// <param name="nErrCode_">错误码（参考SHErrorCode）</param>
		public SHNullParamException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}

		/// <summary>
		/// 构造函数，构造参数为空（NULL），错误码为InvalidParam的异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		public SHNullParamException(string strMsg_)
			: base(strMsg_, (int)SHErrorCode.InvalidParam)
		{
		}
	}; 

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 网络操作相关错误异常，继承自SHException
	/// </summary>
	public class SHNetException :  SHException
	{
	
		/// <summary>
		/// 构造函数，构造网络操作相关错误异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		/// <param name="nErrCode_">错误码（参考SHErrorCode）</param>
		public SHNetException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 钥匙（CKey）操作相关错误异常，继承自SHException
	/// </summary>
	public class SHKeyException :  SHException
	{
	
		/// <summary>
		/// 构造函数，构造钥匙操作相关错误异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		/// <param name="nErrCode_">错误码（参考SHErrorCode）</param>
		public SHKeyException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 加解密（Crypt）相关错误异常，继承自SHException
	/// </summary>
	public class SHCryptException :  SHException
	{
	
		/// <summary>
		/// 构造函数，构造加解密（对称、非对称以及散列）操行相关错误异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		/// <param name="nErrCode_">错误码（参考SHErrorCode）</param>
		public SHCryptException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 过滤文件驱动（CreFCtrl）相关错误异常，继承自SHException
	/// </summary>
	public class SHFCtrlException :  SHException
	{
	
		/// <summary>
		/// 构造函数，构造加密文件系统操作相关错误异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		/// <param name="nErrCode_">错误码（参考SHErrorCode）</param>
        public SHFCtrlException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// 设备管理（Device）相关错误异常，继承自SHException
	/// </summary>
	public class SHDevException :  SHException
	{
	
		/// <summary>
		/// 构造函数，构造设备管理相关错误异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		/// <param name="nErrCode_">错误码（参考SHErrorCode）</param>
		public SHDevException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// U盘（UDisk）相关错误异常，继承自SHException
	/// </summary>
	public class SHUDiskException :  SHException
	{
	
		/// <summary>
		/// 构造函数，构造U盘相关错误异常
		/// </summary>
		/// <param name="strMsg_">错误说明</param>
		/// <param name="nErrCode_">错误码（参考SHErrorCode）</param>
		public SHUDiskException(string strMsg_, int nErrCode_)
            : base(strMsg_, nErrCode_)
		{
		}
	}; 
    #endregion
}
