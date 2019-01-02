using System;

namespace SHCre.Xugd.Common
{
    /// <summary>
    /// �쳣���࣬��ͨ��GetMessage��ȡ�����쳣��Ϣ
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
        /// ��ȡ��Ϣ����������)
        /// </summary>
        /// <returns></returns>
        public virtual string GetMessage()
        {
            return string.Format("{0}({1})", XReflex.GetTypeName(this, false), this.Message);
        }
    }

    /// <summary>
    /// ������ش���
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
        /// ���������
        /// </summary>
        public Type ReflectType { get; set; }
        
        /// <summary>
        /// ��ȡ��Ϣ����������)
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
    /// δ�ҵ�
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
        /// ��ȡ��Ϣ����������)
        /// </summary>
        /// <returns></returns>
        public override string GetMessage()
        {
            return string.Format("XNotFoundException({0})", this.Message);
        }
    }

    /// <summary>
    /// δ�ҵ�
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
        /// ��ȡ��Ϣ����������)
        /// </summary>
        /// <returns></returns>
        public override string GetMessage()
        {
            return string.Format("XExistedException({0})", this.Message);
        }
    }

    /// <summary>
    /// ���������ʧ��
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
        /// ��ȡ��Ϣ����������)
        /// </summary>
        /// <returns></returns>
        public override string GetMessage()
        {
            return string.Format("XOperateException({0})", this.Message);
        }
    }

    /// <summary>
    /// ȱ������
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
        /// ��ȡ��Ϣ����������)
        /// </summary>
        /// <returns></returns>
        public override string GetMessage()
        {
            return string.Format("XConditionException({0})", this.Message);
        }
    }

    /// <summary>
    /// ��ͻ
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
        /// ��ȡ��Ϣ����������)
        /// </summary>
        /// <returns></returns>
        public override string GetMessage()
        {
            return string.Format("XConflictException({0})", this.Message);
        }
    }

    /// <summary>
    /// ������ش���
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
    /// ����
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
        ///// ��ȡ��Ϣ����������)
        ///// </summary>
        ///// <returns></returns>
        //public override string GetMessage()
        //{
        //    return string.Format("XConnectException({0})", this.Message);
        //}
    }

    /// <summary>
    /// δ��¼����쳣
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
        /// ˭
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// ��ȡ��Ϣ����������)
        /// </summary>
        /// <returns></returns>
        public override string GetMessage()
        {
            return string.Format("XNotLoginException({0}: {1})", UserName, Message);
        }
    }

    #region "SHExcepions"
    /// <summary>
    /// ���ô����루��windows������ͳһ��
    /// </summary>
    public enum SHErrorCode
    {
        /// <summary>
        /// ��Ч�ĺ���(1)
        /// </summary>
        InvalidFunction = 1,
        /// <summary>
        /// �ļ�δ�ҵ�(2)
        /// </summary>
        FileNotFound = 2,
        /// <summary>
        /// ·��δ�ҵ�(3)
        /// </summary>
        PathNotFound = 3,
        /// <summary>
        /// ���ļ�����(4)
        /// </summary>
        TooManyOpenFiles = 4,
        /// <summary>
        /// �ܾ����ʣ�û��Ȩ�ޣ�(5)
        /// </summary>
        AccessDenied = 5,
        /// <summary>
        /// ��Ч�ľ��(6)
        /// </summary>
        InvalidHandle = 6,
        /// <summary>
        /// �ڴ治�㣨�����ڴ�ʧ�ܣ�(8)
        /// </summary>
        NotEnoughMemory = 8,
        /// <summary>
        /// ��Ч�Ŀ�(9)
        /// </summary>
        InvalidBlock = 9,
        /// <summary>
        /// ��ʽ����(11)
        /// </summary>
        BadFormat = 11,
        /// <summary>
        /// ������Ч(13)
        /// </summary>
        InvalidData = 13,
        /// <summary>
        /// �����ڴ�(14)
        /// </summary>
        OutOfMemory = 14,
        /// <summary>
        /// ��Ч������(15)
        /// </summary>
        InvalidDrive = 15,
        /// <summary>
        /// û�и�����ļ�(18)
        /// </summary>
        NoMoreFile = 18,
        /// <summary>
        /// д����(19)
        /// </summary>
        WriteProtect = 19,
        /// <summary>
        /// δ׼����(21)
        /// </summary>
        NotReady = 21,
        /// <summary>
        /// ���������(22)
        /// </summary>
        BadCommand = 22,
        /// <summary>
        /// У���루CRC����֤ʧ��(23)
        /// </summary>
        ErrorCrc = 23,
        /// <summary>
        /// ����ĳ���(24)
        /// </summary>
        BadLength = 24,
        /// <summary>
        /// дʧ��(29)
        /// </summary>
        WriteFault = 29,
        /// <summary>
        /// ��ʧ��(30)
        /// </summary>
        ReadFault = 30,
        /// <summary>
        /// ������(34)
        /// </summary>
        WrongDisk = 34,
        /// <summary>
        /// ���β���ﵽ�ļ�ĩβ��(38)
        /// </summary>
        HanleEOF = 38,
        /// <summary>
        /// ��֧��(50)
        /// </summary>
        NotSupported = 50,
        /// <summary>
        /// ����������(55)
        /// </summary>
        DevNotExist = 55,
        /// <summary>
        /// �ļ��Ѵ���(80)
        /// </summary>
        FileExists = 80,
        /// <summary>
        /// ���ܴ����ļ���Ŀ¼(82)
        /// </summary>
        CannotMake = 82,
        /// <summary>
        /// ��Ч�Ŀ���(86)
        /// </summary>
        InvalidPassword = 86,
        /// <summary>
        /// ��Ч�Ĳ���(87)
        /// </summary>
        InvalidParam = 87,
        /// <summary>
        /// ����������(108)
        /// </summary>
        DriveLocked = 108,
        /// <summary>
        /// ��ʧ��(110)
        /// </summary>
        OpenFailed = 110,
        /// <summary>
        /// ���������(111)
        /// </summary>
        BufferOverFlow = 111,
        /// <summary>
        /// ����δʵ��(120)
        /// </summary>
        CallNotImplemented = 120,
        /// <summary>
        /// ���������ڴ棩����(122)
        /// </summary>
        InsufficientBuffer = 122,
        /// <summary>
        /// ��Ч������(123)
        /// </summary>
        InvalidName = 123,
        /// <summary>
        /// ����δ�ҵ�(127)
        /// </summary>
        ProcNotFound = 127,
        /// <summary>
        /// ���Ǹ�Ŀ¼(144)
        /// </summary>
        DirNotRoot = 144,
        /// <summary>
        /// Ŀ¼�ǿ�(145)
        /// </summary>
        DirNotEmpty = 145,
        /// <summary>
        /// δ����(158)
        /// </summary>
        NotLocked = 158,
        /// <summary>
        /// �����·����(161)
        /// </summary>
        BadPathName = 161,
        /// <summary>
        /// ����ʧ��(164)
        /// </summary>
        LockFailed = 164,
        /// <summary>
        /// æµ(170)
        /// </summary>
        Busy = 170,
        /// <summary>
        /// �Ѿ�����(183)
        /// </summary>
        AlreadyExists = 183,
        /// <summary>
        /// ����(212)
        /// </summary>
        Locked = 212,
        /// <summary>
        /// ������ļ�����(222)
        /// </summary>
        BadFileType = 222,
        /// <summary>
        /// �ļ�̫�󣨳���2G��(223)
        /// </summary>
        FileTooLarge = 223,
        /// <summary>
        /// û������(232)
        /// </summary>
        NoData = 232,
        /// <summary>
        /// �и��������(234)
        /// </summary>
        MoreData = 234,
        /// <summary>
        /// �ȴ���һ��Ϊ�ź���أ���ʱ(258)
        /// </summary>
        WaitTimeOut = 258,
        /// <summary>
        /// û�и������(259)
        /// </summary>
        NoMoreItem = 259,
        /// <summary>
        /// ���ܸ���(266)
        /// </summary>
        CannotCopy = 266,
        /// <summary>
        /// ��Ŀ¼(267)
        /// </summary>
        Directory = 267,
        /// <summary>
        /// ���������ߣ�Owner��(288)
        /// </summary>
        NotOwner = 288,
        /// <summary>
        /// ��Ч�ĵ�ַ(487)
        /// </summary>
        InvalidAddress = 487,
        /// <summary>
        /// ��Ч����Ϣ(1002)
        /// </summary>
        InvalidMessage = 1002,
        /// <summary>
        /// ���ܣ��޷������(1003)
        /// </summary>
        CannotComplete = 1003,
        /// <summary>
        /// ��Ч�ı�־(1004)
        /// </summary>
        InvalidFlags = 1004,
        /// <summary>
        /// �ļ���Ч(1005)
        /// </summary>
        FileInvalid = 1006,
        /// <summary>
        /// û�����ƣ�Token��(1008)
        /// </summary>
        NoToken = 1008,
        /// <summary>
        /// �����ע������ݿ�(1009)
        /// </summary>
        BadDB = 1009,
        /// <summary>
        /// �����ע����(1010)
        /// </summary>
        BadKey = 1010,
        /// <summary>
        /// ������������(1065)
        /// </summary>
        ServiceAlreadyRunning = 1056,
        /// <summary>
        /// ���񱻽���(1058)
        /// </summary>
        ServiceDisabled = 1058,
        /// <summary>
        /// ���񲻴���(1060)
        /// </summary>
        ServiceNotExists = 1060,
        /// <summary>
        /// ����δ����(1062)
        /// </summary>
        ServiceNotActive = 1062,
        /// <summary>
        /// ���ݿⲻ����(1065)
        /// </summary>
        DatabaseNotExist = 1065,
        /// <summary>
        /// �����Ѵ���(1073)
        /// </summary>
        ServiceExists = 1073,
        /// <summary>
        /// �������Ѵ��ڣ�������(1078)
        /// </summary>
        ServiceNameSake = 1078,
        /// <summary>
        /// ��Ч�Ŀ鳤��(1106)
        /// </summary>
        InvalidBlockLen = 1106,
        /// <summary>
        /// ���ʼ��ʧ��(1114)
        /// </summary>
        DllInitFailed = 1114,
        /// <summary>
        /// Windows�汾̫��
        /// </summary>
        WinVerLow = 1150,
        /// <summary>
        /// ��Ч�Ŀ�(1154)
        /// </summary>
        InvalidDll = 1154,
        /// <summary>
        /// ��δ�ҵ�(1157)
        /// </summary>
        DllNotFound = 1157,
        /// <summary>
        /// δ�ҵ�(1168)
        /// </summary>
        NotFound = 1168,
        /// <summary>
        /// ��ƥ��(1169)
        /// </summary>
        NoMatch = 1169,
        /// <summary>
        /// ������豸(1200)
        /// </summary>
        DadDevice = 1200,
        /// <summary>
        /// ����Ĺ�Ӧ��(1204)
        /// </summary>
        BadProvider = 1204,
        /// <summary>
        /// ��Ч������(1209)
        /// </summary>
        InvalidGroupName = 1209,
        /// <summary>
        /// ��Ч������(1212)
        /// </summary>
        InvalidDomainName = 1212,
        /// <summary>
        /// ��Ч�ķ�����(1213)
        /// </summary>
        InvalidServiceName = 1213,
        /// <summary>
        /// ��Ч��������(1214)
        /// </summary>
        InvalidNetName = 1214,
        /// <summary>
        /// û������(1222)
        /// </summary>
        NoNetwork = 1222,
        /// <summary>
        /// ��ȡ��(1223)
        /// </summary>
        Cancelled = 1223,
        /// <summary>
        /// ��Ч������(1229)
        /// </summary>
        InvalidConnection = 1229,
        /// <summary>
        /// ���Ӽ���(1230)
        /// </summary>
        ConnectionActive = 1230,
        /// <summary>
        /// ����(1237)
        /// </summary>
        Retry = 1237,
        /// <summary>
        /// ����ĵ�ַ(1241)
        /// </summary>
        IncorrectAddress = 1241,
        /// <summary>
        /// ��ע��(1242)
        /// </summary>
        AlreadyRegistered = 1242,
        /// <summary>
        /// ����δ�ҵ�(1243)
        /// </summary>
        ServiceNotFound = 1243,
        /// <summary>
        /// δ��֤(1244)
        /// </summary>
        NotAuthenticated = 1244,
        /// <summary>
        /// û�е�¼(1245)
        /// </summary>
        NotLoggedOn = 1245,
        /// <summary>
        /// �ѳ�ʼ��(1247)
        /// </summary>
        AlreadyInitialized = 1247,
        /// <summary>
        /// û�и�����豸(1248)
        /// </summary>
        NoMoreDevice = 1248,
        /// <summary>
        /// �ָ�ʧ��(1279)
        /// </summary>
        RecoveryFailure = 1279,
        /// <summary>
        /// ��Ч�������ߣ�Owner��(1307)
        /// </summary>
        InvalidOwner = 1307,
        /// <summary>
        /// �û��Ѵ���(1316)
        /// </summary>
        UserExists = 1316,
        /// <summary>
        /// û��ָ���û�(1317)
        /// </summary>
        NoSuchUser = 1317,
        /// <summary>
        /// ���Ѵ���(1318)
        /// </summary>
        GroupExists = 1218,
        /// <summary>
        /// û��ָ������(1319)
        /// </summary>
        NoSuchGroup = 1319,
        /// <summary>
        /// ����Ŀ���(1323)
        /// </summary>
        WrongPassword = 1323,
        /// <summary>
        /// ��¼ʧ��(1326)
        /// </summary>
        LogonFailure = 1326,
        /// <summary>
        /// �������(1330)
        /// </summary>
        PasswordExpired = 1330,
        /// <summary>
        /// �˻���Ч(1331)
        /// </summary>
        AccountDisabled = 1331,
        /// <summary>
        /// û��ָ������(1355)
        /// </summary>
        NoSuchDomain = 1355,
        /// <summary>
        /// ���Ѵ���(1356)
        /// </summary>
        DomainExists = 1356,
        /// <summary>
        /// ���ƣ�Token������ʹ��(1375)
        /// </summary>
        TokenAlreadyInUse = 1375,
        /// <summary>
        /// �ļ����ƻ�(1392)
        /// </summary>
        FileCorrupt = 1392,
        /// <summary>
        /// �������ƻ�(1393)
        /// </summary>
        DiskCorrupt = 1393,
        /// <summary>
        /// ��Ч������(1413)
        /// </summary>
        InvalidIndex = 1413,
        /// <summary>
        /// ճ����δ��(1418)
        /// </summary>
        ClipboardNotOpen = 1418,
        /// <summary>
        /// ��ʱ(1460)
        /// </summary>
        TimeOut = 1460,
        /// <summary>
        /// ����Ĵ�С(1462)
        /// </summary>
        InCorrectSize = 1462,
        /// <summary>
        /// ���ݸ�ʽ��ƥ��(1429)
        /// </summary>
        DataTypeMismatch = 1629,
        /// <summary>
        /// ��֧�ֵ�����(1430)
        /// </summary>
        UnsupportType = 1630,
        /// <summary>
        /// ����ʧ��(1431)
        /// </summary>
        CreateFailed = 1631,
        /// <summary>
        /// ��Ʒ�汾����(1438)
        /// </summary>
        ProductVersion = 1638,
        /// <summary>
        /// �˻�����(1793)
        /// </summary>
        AccountExpired = 1793,
        /// <summary>
        /// δ֪�˿�(1796)
        /// </summary>
        UnknownPort = 1796,
        /// <summary>
        /// ��Ч����������(1804)
        /// </summary>
        InvalidDataType = 1804,
        /// <summary>
        /// ��Ч��ʱ��(1901)
        /// </summary>
        InvalidTime = 1901,
        /// <summary>
        /// ����������(1907)
        /// </summary>
        PasswordMustChange = 1907,
        /// <summary>
        /// �˻�������(1909)
        /// </summary>
        AccountLockedOut = 1909,
        /// <summary>
        /// ������û���(2202)
        /// </summary>
        BadUserName = 2202,
        /// <summary>
        /// δ����(2250)
        /// </summary>
        NotConnect = 2250,
        /// <summary>
        /// ���������(2402)
        /// </summary>
        ActiveConnections = 2402,
        /// <summary>
        /// �豸��ʹ��(2404)
        /// </summary>
        DeviceInUse = 2404,
        /// <summary>
        /// Ϊ��(4306)
        /// </summary>
        Empty = 4306,
        /// <summary>
        /// ��Ϊ��(4307)
        /// </summary>
        NotEmpty = 4307,
        /// <summary>
        /// ����δ�ҵ�(4312)
        /// </summary>
        ObjectNotFound = 4312,
        /// <summary>
        /// ���ݿ�ʧ��(4313)
        /// </summary>
        DatabaseFailure = 4313,
        /// <summary>
        /// ���ݿ�����(4314)
        /// </summary>
        DatabaseFull = 4314,
        /// <summary>
        /// ��Ч�Ĳ���(4317)
        /// </summary>
        InvalidOperation = 4317,
        /// <summary>
        /// �豸δ׼����(4319)
        /// </summary>
        DeviceNotAvailable = 4319,
        /// <summary>
        /// ��������(4320)
        /// </summary>
        RequestRefused = 4320,
        /// <summary>
        /// ����δ�ҵ�(5002)
        /// </summary>
        DependencyNotFound = 5002,
        /// <summary>
        /// �����Ѵ���(5003)
        /// </summary>
        DependencyAlreadExists = 5003,
        /// <summary>
        /// ��Դδ׼����(5006)
        /// </summary>
        ResourceNotAvailable = 5006,
        /// <summary>
        /// ��Դδ�ҵ�(5007)
        /// </summary>
        ResourceNotFound = 5007,
        /// <summary>
        /// �����Ѵ���(5010)
        /// </summary>
        ObjectAlreadyExists = 5010,
        /// <summary>
        /// ��Ч��״̬(5023)
        /// </summary>
        InvalidState = 5023,
        /// <summary>
        /// ����ʧ��(6000)
        /// </summary>
        EncryptionFailed = 6000,
        /// <summary>
        /// ����ʧ��(6001)
        /// </summary>
        DecryptionFailed = 6001,
        /// <summary>
        /// �ļ��Ѽ���(6002)
        /// </summary>
        FileEncrypted = 6002,
        /// <summary>
        /// û�м����ļ�ϵͳ(6004)
        /// </summary>
        NoEFS = 6004,
        /// <summary>
        /// ����ļ����ļ�ϵͳ(6005)
        /// </summary>
        WrongEFS = 6005,
        /// <summary>
        /// û���û�����UserKeys��(6006)
        /// </summary>
        NoUserKeys = 6006,
        /// <summary>
        /// �ļ�δ����(6007)
        /// </summary>
        FileNotEncrypt = 6007,
        /// <summary>
        /// ���ǵ����ĸ�ʽ(6008)
        /// </summary>
        NotExportFormat = 6008,
        /// <summary>
        /// �ļ�δֻ��(6009)
        /// </summary>
        FileReadOnly = 6009
    };

	/// <summary>
	/// ���Ƕ���������쳣���࣬
	/// ���������Լ���װ��C++���׳����쳣��Ϊ�����������
	/// </summary>
	public class SHException 
		:  ApplicationException
	{
		int m_ErrorCode; 
	
		/// <summary>
		/// ���캯���������쳣����
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		/// <param name="nErrCode_">�����루�ο�SHErrorCode��</param>
		public SHException(string strMsg_, int nErrCode_) 
			: base(strMsg_)
		{
			m_ErrorCode = nErrCode_;
		}

        /// <summary>
        /// ���캯���������쳣����
        /// </summary>
        /// <param name="strMsg_">����˵��</param>
        /// <param name="euErrCode_">������</param>
        public SHException(string strMsg_, SHErrorCode euErrCode_)
            : this(strMsg_, (int)euErrCode_)
        {
        }

		/// <summary>
		/// ��ȡ�����루�ο�SHErrorCode��
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
	/// �ڴ治�㣨����ʧ�ܣ��쳣���̳���SHException
	/// </summary>
	public class SHMemException :  SHException
	{
	
		/// <summary>
		/// ���캯�������������ΪNotEnoughMemory���ڴ��쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		public SHMemException(string strMsg_)
			: base(strMsg_, (int)SHErrorCode.NotEnoughMemory)
		{
		}
	};

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// �û���������쳣���̳���SHException
	/// </summary>
	public class SHUserException :  SHException
	{
	
		/// <summary>
		/// ���캯���������û�����쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		/// <param name="nErrCode_">�����루�ο�SHErrorCode��</param>
		public SHUserException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	};

	/// <summary>
	/// �û�ȡ����ǰ����ʱ���׳����쳣��һ�����ڻص������̳���SHUserException
	/// </summary>
	public class SHUserCancelException :  SHUserException
	{
	
		/// <summary>
		/// ���캯�������������ΪCancelled�Ĳ���ȡ���쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		public SHUserCancelException(string strMsg_)
			: base(strMsg_, (int)SHErrorCode.Cancelled)
		{
		}
	};

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Windows������ش����쳣���̳���SHException
	/// </summary>
	public class SHWinException :  SHException
	{
	
		/// <summary>
		/// ���캯��������Windows������ش����쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		/// <param name="nErrCode_">�����루�ο�SHErrorCode��</param>
		public SHWinException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	};

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// �ļ�������ش����쳣���̳���SHException
	/// </summary>
	public class SHFileException :  SHException
	{
	
		/// <summary>
		/// ���캯���������ļ�������ش����쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		/// <param name="nErrCode_">�����루�ο�SHErrorCode��</param>
		public SHFileException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	/// <summary>
	/// ע��������ش����쳣���̳���SHFileException
	/// </summary>
	public class SHRegistryException :  SHFileException
	{
	
		/// <summary>
		/// ���캯��������ע��������ش����쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		/// <param name="nErrCode_">�����루�ο�SHErrorCode��</param>
		public SHRegistryException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	/// <summary>
	/// ��̬����������صȣ�������ش����쳣���̳���SHFileException
	/// </summary>
	public class SHDllException :  SHFileException
	{	
		/// <summary>
		/// ���캯�������춯̬�⣨dll��������ش����쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		/// <param name="nErrCode_">�����루�ο�SHErrorCode��</param>
		public SHDllException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// �ź������ٽ��������⡢�¼��ȣ�������ش����쳣���̳���SHException
	/// </summary>
	public class SHSynException :  SHException
	{
	
		/// <summary>
		/// ���캯���������ź������ٽ��������⡢�¼��ȣ�������ش����쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		/// <param name="nErrCode_">�����루�ο�SHErrorCode��</param>
		public SHSynException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// ������ش����쳣���̳���SHException
	/// </summary>
	public class SHParamException :  SHException
	{
	
		/// <summary>
		/// ���캯�������������ش����쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		/// <param name="nErrCode_">�����루�ο�SHErrorCode��</param>
		public SHParamException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	/// <summary>
	/// ����ΪNULL�Ĵ����쳣���̳���SHParamException
	/// </summary>
	public class SHNullParamException :  SHParamException 
	{
	
		/// <summary>
		/// ���캯�����������Ϊ�գ�NULL�����쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		/// <param name="nErrCode_">�����루�ο�SHErrorCode��</param>
		public SHNullParamException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}

		/// <summary>
		/// ���캯�����������Ϊ�գ�NULL����������ΪInvalidParam���쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		public SHNullParamException(string strMsg_)
			: base(strMsg_, (int)SHErrorCode.InvalidParam)
		{
		}
	}; 

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// ���������ش����쳣���̳���SHException
	/// </summary>
	public class SHNetException :  SHException
	{
	
		/// <summary>
		/// ���캯�����������������ش����쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		/// <param name="nErrCode_">�����루�ο�SHErrorCode��</param>
		public SHNetException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// Կ�ף�CKey��������ش����쳣���̳���SHException
	/// </summary>
	public class SHKeyException :  SHException
	{
	
		/// <summary>
		/// ���캯��������Կ�ײ�����ش����쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		/// <param name="nErrCode_">�����루�ο�SHErrorCode��</param>
		public SHKeyException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// �ӽ��ܣ�Crypt����ش����쳣���̳���SHException
	/// </summary>
	public class SHCryptException :  SHException
	{
	
		/// <summary>
		/// ���캯��������ӽ��ܣ��Գơ��ǶԳ��Լ�ɢ�У�������ش����쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		/// <param name="nErrCode_">�����루�ο�SHErrorCode��</param>
		public SHCryptException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// �����ļ�������CreFCtrl����ش����쳣���̳���SHException
	/// </summary>
	public class SHFCtrlException :  SHException
	{
	
		/// <summary>
		/// ���캯������������ļ�ϵͳ������ش����쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		/// <param name="nErrCode_">�����루�ο�SHErrorCode��</param>
        public SHFCtrlException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// �豸����Device����ش����쳣���̳���SHException
	/// </summary>
	public class SHDevException :  SHException
	{
	
		/// <summary>
		/// ���캯���������豸������ش����쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		/// <param name="nErrCode_">�����루�ο�SHErrorCode��</param>
		public SHDevException(string strMsg_, int nErrCode_)
			: base(strMsg_, nErrCode_)
		{
		}
	}; 

	//////////////////////////////////////////////////////////////////////////
	/// <summary>
	/// U�̣�UDisk����ش����쳣���̳���SHException
	/// </summary>
	public class SHUDiskException :  SHException
	{
	
		/// <summary>
		/// ���캯��������U����ش����쳣
		/// </summary>
		/// <param name="strMsg_">����˵��</param>
		/// <param name="nErrCode_">�����루�ο�SHErrorCode��</param>
		public SHUDiskException(string strMsg_, int nErrCode_)
            : base(strMsg_, nErrCode_)
		{
		}
	}; 
    #endregion
}
