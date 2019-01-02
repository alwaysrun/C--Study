using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SHCre.Xugd.Net.Ftp
{
    /// <summary>
    /// Ftp服务器配置文件
    /// </summary>
    public class XFtpConfig
    {
        /// <summary>
        /// 服务器标识
        /// </summary>
        [XmlAttribute]
        public string FtpId { get; set; }

        /// <summary>
        /// 地址（IP地址或域名）
        /// </summary>
        [XmlAttribute]
        public string Address { get; set; }

        /// <summary>
        /// 端口号
        /// </summary>
        [XmlAttribute]
        public int Port { get; set; }

        /// <summary>
        /// 默认的根目录
        /// </summary>
        [XmlAttribute]
        public string RootPath {get;set;}

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

        /// <summary>
        /// 显示地址信息[Ftp]{Addr}:{Port}
        /// </summary>
        /// <returns></returns>
        public string PrintAddr()
        {
            return string.Format("[Ftp]{0}:{1}", Address, Port);
        }

        /// <summary>
        /// 设定默认值
        /// </summary>
        /// <param name="strAddr_"></param>
        /// <param name="strUser_"></param>
        /// <param name="strPsw_"></param>
        public void Init(string strAddr_ = "127.0.0.1", string strUser_ = "ncre", string strPsw_ = "xgdxgd")
        {
            FtpId = string.Empty;
            Address = strAddr_;
            Port = 21;
            RootPath = string.Empty;
            UserName = strUser_;
            Password = strPsw_;
        }
    }
}
