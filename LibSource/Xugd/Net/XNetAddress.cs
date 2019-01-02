using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Xml.Serialization;
using SHCre.Xugd.Config;

namespace SHCre.Xugd.Net
{
    /// <summary>
    /// 服务器地址（包括服务器标识、地址与端口）
    /// </summary>
    public class XNetAddrConfig
    {
        /// <summary>
        /// 服务器标识
        /// </summary>
        [XmlAttribute]
        public string SrvId { get; set; }

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
        /// 
        /// </summary>
        /// <param name="strIP_"></param>
        /// <param name="nPort_"></param>
        public void Init(string strIP_, int nPort_)
        {
            SrvId = string.Empty;
            Address = strIP_;
            Port = nPort_;
        }

        /// <summary>
        /// {IP}:{Port}
        /// </summary>
        /// <returns></returns>
        public string PrintAddr()
        {
            return string.Format("{0}:{1}", Address, Port);
        }

        /// <summary>
        /// 获取侦听地址：如果地址为空，则使用本机IPAddress.Any
        /// </summary>
        public IPEndPoint ListenIPEndPoint()
        {
            return XNetAddress.GetListenIPEndPoint(Port, Address);
        }

        /// <summary>
        /// 获取连接地址：如果地址为空，则使用本机回环地址
        /// </summary>
        public IPEndPoint ConnectIPEndPoint()
        {
            return XNetAddress.GetConnectIPEndPoint(Port, Address);
        }
    }

    /// <summary>
    /// 有关IP地址相关类
    /// </summary>
    public static class XNetAddress
    {
        /// <summary>
        /// 获取本地IP地址列表
        /// </summary>
        /// <returns>本地IP地址列表</returns>
        public static IPAddress[] GetLocalIPs()
        {
            return Dns.GetHostEntry(Dns.GetHostName()).AddressList;
        }

        /// <summary>
        /// 获取本地IPv4地址列表
        /// </summary>
        /// <returns>本地IPv4地址列表</returns>
        public static IPAddress[] GetLocalIPv4()
        {
            return Array.FindAll(GetLocalIPs(), z => z.AddressFamily == AddressFamily.InterNetwork);
        }

        /// <summary>
        /// 获取本地IPv6地址列表
        /// </summary>
        /// <returns>本地IPv6地址列表</returns>
        public static IPAddress[] GetLocalIPv6()
        {
            return Array.FindAll(GetLocalIPs(), z => z.AddressFamily == AddressFamily.InterNetworkV6);
        }

        /// <summary>
        /// 判断指定的IP地址是否为本地IP地址
        /// </summary>
        /// <param name="strIp_">要判断的IP地址</param>
        /// <returns>是本地地址，返回true；否则，返回false</returns>
        public static bool IsLocalIp(string strIp_)
        {
            return IsLocalIp(IPAddress.Parse(strIp_));
        }

        /// <summary>
        /// 判断指定的IP地址是否为本地IP地址(回环地址127.0.0.1认为是本机地址)
        /// </summary>
        /// <param name="ipAddr_">要判断的IP地址</param>
        /// <returns>是本地地址，返回true；否则，返回false</returns>
        public static bool IsLocalIp(IPAddress ipAddr_)
        {
            if (ipAddr_.ToString() == IPAddress.Loopback.ToString()
                || ipAddr_.ToString() == IPAddress.IPv6Loopback.ToString())
                return true;

            return GetLocalIPs().FirstOrDefault(z => z.ToString() == ipAddr_.ToString()) != null;
        }

        /// <summary>
        /// 获取本地侦听的网络端点
        /// </summary>
        /// <param name="nPort_">侦听端口号</param>
        /// <param name="strIp_">侦听的IP地址</param>
        /// <returns>侦听的网络端点</returns>
        public static IPEndPoint GetListenIPEndPoint(int nPort_, string strIp_ = null)
        {
            return GetListenIPEndPoint(nPort_, string.IsNullOrEmpty(strIp_) ? null : IPAddress.Parse(strIp_));
        }

        /// <summary>
        /// 获取本地侦听的网络端点
        /// </summary>
        /// <param name="nPort_">侦听端口号</param>
        /// <param name="ipAddr_">侦听的IP地址</param>
        /// <returns>侦听的网络端点</returns>
        public static IPEndPoint GetListenIPEndPoint(int nPort_, IPAddress ipAddr_ = null)
        {
            if (ipAddr_ == null)
                ipAddr_ = IPAddress.Any;

            return new IPEndPoint(ipAddr_, nPort_);
        }

        /// <summary>
        /// 获取连接的网络端点：如果IP地址为空，则使用本机回环地址
        /// </summary>
        /// <param name="nPort_">侦听端口</param>
        /// <param name="strIp_">要连接的地址</param>
        /// <returns>连接的网络端点</returns>
        public static IPEndPoint GetConnectIPEndPoint(int nPort_, string strIp_ = null)
        {
            return GetConnectIPEndPoint(nPort_, string.IsNullOrEmpty(strIp_) ? null : IPAddress.Parse(strIp_));
        }

        /// <summary>
        ///  获取连接的网络端点：如果IP地址为空，则使用本机回环地址
        /// </summary>
        /// <param name="nPort_">侦听端口</param>
        /// <param name="ipAddr_">要连接的地址</param>
        /// <returns>连接的网络端点</returns>
        public static IPEndPoint GetConnectIPEndPoint(int nPort_, IPAddress ipAddr_ = null)
        {
            if (ipAddr_ == null)
                ipAddr_ = IPAddress.Loopback;

            return new IPEndPoint(ipAddr_, nPort_);
        }

        /// <summary>
        /// 返回本地的服务器地址：
        /// 如果地址列表中只有一项，且地址（Address）为空，则返回第一项（说明使用IPAddress.Any作为侦听地址）
        /// </summary>
        /// <param name="srvAddresses_">服务器地址列表</param>
        /// <returns>如果有本机地址，则返回；否则，返回null</returns>
        public static XNetAddrConfig GetLocalServerAddress(List<XNetAddrConfig> srvAddresses_)
        {
            if (srvAddresses_.Count == 1 && string.IsNullOrEmpty(srvAddresses_[0].Address))
                return srvAddresses_[0];

            foreach (var addr in srvAddresses_)
            {
                if (IsLocalIp(addr.Address))
                    return addr;
            }

            return null;
        }

        /// <summary>
        /// 返回本地的服务器地址：
        /// 如果地址列表中只有一项，且地址（Address）为空，则返回第一项（说明使用IPAddress.Any作为侦听地址）
        /// </summary>
        /// <param name="srvAddresses_">服务器地址列表</param>
        /// <returns>如果有本机地址，则返回；否则，返回null</returns>
        public static XNetAddrConfig GetLocalServerAddress(params XNetAddrConfig[] srvAddresses_)
        {
            return GetLocalServerAddress(srvAddresses_.ToList());
        }
    }
}
