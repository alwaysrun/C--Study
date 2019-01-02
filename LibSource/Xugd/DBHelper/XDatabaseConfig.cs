using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SHCre.Xugd.Common;

namespace SHCre.Xugd.DbHelper
{
    /// <summary>
    /// 数据库的类型
    /// </summary>
    public enum XDatabaseType
    {
        /// <summary>
        /// 无效
        /// </summary>
        Invalid = 0,
        /// <summary>
        /// SqlServer
        /// </summary>
        SqlServer,
        /// <summary>
        /// Oracle(Using Oracle.ManagedDataAccess.dll)
        /// </summary>
        Oracle,
        /// <summary>
        /// Oracle(Using System.Data.OracleClient), 
        /// should install the Oracle-Client
        /// </summary>
        Oracle_Net,
    }

    /// <summary>
    /// 数据库配置信息
    /// </summary>
    public class XDatabaseConfig
    {
        /// <summary>
        /// 用于区分不同配置的Id
        /// </summary>
        [XmlAttribute]
        public string Id {get;set;}

        /// <summary>
        /// 数据库的类型
        /// </summary>
        [XmlAttribute]
        public XDatabaseType Type { get; set; }

        /// <summary>
        /// 数据库类型的可选项
        /// </summary>
        [XmlAttribute]
        public string TypeChoice {get;set;}

        /// <summary>
        /// 数据库服务器的地址：
        /// 对于Oracle，如果为空则使用Oracle客户端进行连接（此时Type要为Oracle_Net，且ServiceOrDbName为客户端中‘网络服务名’）；
        /// 若为空，则ServiceOrDbName为完整的连接字符串（Data Source=后面内容）
        /// </summary>
        [XmlAttribute]
        public string Address { get; set; }

        /// <summary>
        /// 端口号：对于SqlServer2008，如果使用默认的端口号，则设为0。
        /// </summary>
        [XmlAttribute]
        public int Port { get; set; }

        /// <summary>
        /// Oracle：若Address非空，为数据库服务CONNECT_DATA=后面内容（如：(SERVICE_NAME=orcl) (可简写为orcl）；或者(SID=orcl)）；
        ///     若Address为空，则为Data Source=后面内容；
        /// SqlServer：要连接数据库的名称（如Mvp4Base）
        /// </summary>
        [XmlAttribute]
        public string ServiceOrDbName { get; set; }

        /// <summary>
        /// 用户名
        /// </summary>
        [XmlAttribute]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [XmlAttribute]
        public string Password { get; set; }

        ///// <summary>
        ///// 
        ///// </summary>
        //public XDatabaseConfig()
        //{
        //    Id = string.Empty;
        //    Type = XDatabaseType.Oracle;
        //    TypeChoice = string.Join(",", Enum.GetNames(typeof(XDatabaseType)));
        //    Port = 1521;
        //    Address = "192.168.1.202";
        //    ServiceOrDbName = "ORCL";
        //    UserName = "ncre";
        //    Password = "ncre";               
        //}

        /// <summary>
        /// 显示地址信息[Type]IP:Port；
        /// </summary>
        public string PrintAddr()
        {
            return string.Format("[{0}]{1}:{2}", Type, Address, Port);
        }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="strAddr_"></param>
        /// <param name="strUser_"></param>
        /// <param name="strPsw_"></param>
        /// <param name="euType"></param>
        public void Init(string strUser_, string strPsw_, string strAddr_, XDatabaseType euType = XDatabaseType.Oracle)
        {
            Id = string.Empty;
            Type = euType;
            TypeChoice = XString.GetEnumNames<XDatabaseType>();
            Address = strAddr_;
            UserName = strUser_;
            Password = strPsw_;
            if(Type == XDatabaseType.Oracle || Type == XDatabaseType.Oracle_Net)
            {
                Port = 1521;
                ServiceOrDbName = "ORCL";
            }
            else if(Type == XDatabaseType.SqlServer)
            {
                Port = 1433;
                ServiceOrDbName = "CreBase";
            }
            else
            {
                Port = 0;
                ServiceOrDbName = string.Empty;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="euType"></param>
        public void Init(XDatabaseType euType = XDatabaseType.Oracle)
        {
            Init("ncre", "ncre", "127.0.0.1", euType);
        }

        /// <summary>
        /// 获取连接字符串，如果是Oracle_Net，则需要手动
        /// 设定ConnectOracle.DbCreateFrom为Oracle_Net
        /// </summary>
        /// <returns></returns>
        public string GetConnectString()
        {
            switch(Type)
            {
                case XDatabaseType.Oracle:
                case XDatabaseType.Oracle_Net:
                    return XDapperORM.ConnectOracle.BuildConnectString(Address, Port, ServiceOrDbName, UserName, Password);

                case XDatabaseType.SqlServer:
                    return XDapperORM.ConnectSqlServer.BuildConnectString(Address, Port, ServiceOrDbName, UserName, Password);

                default:
                    return string.Empty;
            }
        }

        /// <summary>
        /// 根据配置获取连接
        /// </summary>
        /// <returns></returns>
        public XDapperORM.IConnectBase GetConnection()
        {
            switch (Type)
            {
                case XDatabaseType.Oracle:
                case XDatabaseType.Oracle_Net:
                    return new XDapperORM.ConnectOracle(this);

                case XDatabaseType.SqlServer:
                    return new XDapperORM.ConnectSqlServer(this);

                default:
                    throw new XDbException("Invalid DbType " + Type.ToString(), 0);
            }
        }
    }
}
