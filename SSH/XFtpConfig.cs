using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace SHCre.Xugd.Ssh
{
    /// <summary>
    /// 登录Linux的SSH-Ftp相关配置
    /// </summary>
    public class XsftpConfig
    {
        /// <summary>
        /// 地址
        /// </summary>
        [XmlAttribute]
        public string Address { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public int Port { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public string User { get; set; }
        /// <summary>
        /// 
        /// </summary>
        [XmlAttribute]
        public string Psw { get; set; }
        /// <summary>
        /// 语音文件的存放路径（位置）
        /// </summary>
        [XmlAttribute]
        public string BasePath { get; set; }

        /// <summary>
        /// 显示地址信息[SFtp]{Addr}:{Port}
        /// </summary>
        /// <returns></returns>
        public string PrintAddr()
        {
            return string.Format("[SFtp]{0}:{1}", Address, Port);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="strAddr_"></param>
        /// <param name="strUser_"></param>
        /// <param name="strPsw_"></param>
        public void Init(string strAddr_, string strUser_ = "cre", string strPsw_ = "cre123456")
        {
            Address = strAddr_;
            Port = 0;
            User = strUser_;
            Psw = strPsw_;
            BasePath = "/home/fsdir/recordings/voices";
        }
    }
}
