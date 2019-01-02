using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using SHCre.Xugd.CFile;

namespace SHCre.Xugd.Net.Xmpp
{
    /// <summary>
    /// 登录信息配置
    /// </summary>
    public class XmppLoginInfoConfig{
        /// <summary>
        /// Openfire服务器地址
        /// </summary>
        [XmlAttribute]
        public string Address { get; set; }

        /// <summary>
        /// Openfire服务器端口
        /// </summary>
        [XmlAttribute]
        public int Port { get; set; }

        /// <summary>
        /// 域名（Openfire服务器的域名）
        /// </summary>
        [XmlAttribute]
        public string Domain { get; set; }

        /// <summary>
        /// 登录Openfire服务器的用户名
        /// </summary>
        [XmlAttribute]
        public string UserName { get; set; }

        /// <summary>
        /// 密码
        /// </summary>
        [XmlAttribute]
        public string Password { get; set; }

        /// <summary>
        /// {IP}:{Port}
        /// </summary>
        /// <returns></returns>
        public string PrintAddr()
        {
            return string.Format("{0}:{1}", Address, Port);
        }

        /// <summary>
        /// {IP}:{Port}[{Domain}]
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("{0}:{1}[{2}]", Address, Port, Domain);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strAddr_"></param>
        /// <param name="strDomain_"></param>
        /// <param name="strUser_"></param>
        /// <param name="strPsw_"></param>
        public void Init(string strAddr_ = "127.0.0.1", string strDomain_ = "xugd.cti", string strUser_ = "creclient", string strPsw_ = "xgdxgd")
        {
            Address = strAddr_;
            Port = 5222;
            Domain = strDomain_;
            UserName = strUser_;
            Password = strPsw_;
        }

        /// <summary>
        /// 设定登录信息
        /// </summary>
        /// <param name="strAddr_"></param>
        /// <param name="strDomain_"></param>
        /// <param name="strName_"></param>
        /// <param name="strPsw_"></param>
        /// <param name="nPort_"></param>
        public void Set(string strAddr_, string strDomain_, string strName_, string strPsw_, int nPort_=5222){
            Address = strAddr_;
            Port = nPort_;
            Domain = strDomain_;
            UserName = strName_;
            Password = strPsw_;
        }

        /// <summary>
        /// 获取用户的Jid（name@domain)
        /// </summary>
        /// <returns></returns>
        public string GetUserJid(){
            return GetUserJid(UserName);
        }

        /// <summary>
        /// 获取用户的Jid（name@domain)
        /// </summary>
        /// <param name="strName_"></param>
        /// <returns></returns>
        public string GetUserJid(string strName_){
            return string.Format("{0}@{1}", strName_, Domain);
        }
    };

    /// <summary>
    /// 通讯（IM-Openfire）相关配置
    /// </summary>
    public class XmppConfig
    {
        /// <summary>
        /// 登录信息
        /// </summary>
        public XmppLoginInfoConfig LoginInfo { get; set; }

        /// <summary>
        /// 底层日志记录配置
        /// </summary>
        public XLogConfig LogConf;

        /// <summary>
        /// 
        /// </summary>
        public XmppConfig(){
            LoginInfo = new XmppLoginInfoConfig();
            LogConf = new XLogConfig();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strJid_"></param>
        /// <param name="strPsw_"></param>
        /// <param name="strLogName_"></param>
        public void Init(string strJid_ = null, string strPsw_ = null, string strLogName_=null)
        {
            var strUser = "cctest";
            var strDomain = "cti.net";
            if (!string.IsNullOrEmpty(strJid_))
            {
                var aryUsers = strJid_.Split('@');
                if (aryUsers.Length > 1)
                    strDomain = aryUsers[1];
                strUser = aryUsers[0];
            }
            LoginInfo.Init("127.0.0.1", strDomain, strUser, strPsw_);
            if (string.IsNullOrEmpty(strLogName_))
                LogConf.InitDayLog("glXmpp");
            else
                LogConf.InitDayLog(strLogName_);
        }
    }
}
