using Neko.Utility.Core.Data;
using Neko.Utility.Core.IO;
using Neko.Utility.Core.IO.Logging;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;

namespace Neko.Utility.Core.Net
{
    /// <summary>
    /// 网络相关的帮助类
    /// <para>可以获取本机的IP、查询一个站点的延迟、发送基于smtp协议的邮件</para>
    /// </summary>
    public sealed partial class NetUtil
    {
        /// <summary>
        /// 获取本机IP
        /// </summary>
        /// <param name="internetIp">获取公网IP(true)、获取局域网IP(false)</param>
        /// <returns></returns>
        public static string GetIP(bool internetIp = false)
        {
            if (!internetIp)
            {
                return GetLocalIP();
            }
            return GetNetIP();
        }

        /// <summary>
        /// 获取本机IP(IPv4)
        /// </summary>
        /// <returns></returns>
        private static string GetLocalIP()
        {
            TcpClient tcpClient = new TcpClient();
            string result = string.Empty;
            try
            {
                tcpClient.Connect("www.baidu.com", 80);
                result = ((IPEndPoint)tcpClient.Client.LocalEndPoint).Address.MapToIPv4().ToString();
            }
            catch (SocketException)
            {
                LogUtil.WriteWarning(null, "无法通过互联网获取本机IP，将采用本地Dns的形式进行获取!");
                result = GetLocalIPSafe();
            }
            finally
            {
                tcpClient.Close();
            }
            return result;
        }

        /// <summary>
        /// <inheritdoc cref="GetLocalIP"/>
        /// </summary>
        /// <returns></returns>
        private static string GetLocalIPSafe()
        {
            string result = Dns.GetHostEntry(Dns.GetHostName()).AddressList.Where(p => p.AddressFamily == AddressFamily.InterNetwork).Select(p => p.ToString()).FirstOrDefault();
            return result;
        }

        /// <summary>
        /// 获取本机公网IP
        /// </summary>
        /// <returns></returns>
        private static string GetNetIP()
        {
            string result = string.Empty;
            HttpWebRequest webRequest = (HttpWebRequest)HttpWebRequest.Create("http://www.net.cn/static/customercare/yourip.asp");
            using (StreamReader reader = new StreamReader(webRequest.GetResponse().GetResponseStream()))
            {
                string htmlContent = reader.ReadToEnd();
                result = RegularUtil.Get(htmlContent, RegularUtil.IPADDRESS);
            }
            return result;
        }

        /// <summary>
        /// 获取本地计算机与公网的延迟
        /// </summary>
        /// <returns></returns>
        public static int Ping()
        {
            return Ping("www.baidu.com");
        }

        /// <summary>
        /// 获取本地计算机与一个域名的延迟
        /// </summary>
        /// <param name="host">域名</param>
        /// <returns></returns>
        public static int Ping(string host)
        {
            PingReply pingReply = Ping(host, 30);
            return StringUtil.GetInt(pingReply.Status == IPStatus.Success ? pingReply.RoundtripTime : int.MaxValue);
        }

        /// <summary>
        /// <inheritdoc cref="Ping(string)"/>
        /// </summary>
        /// <param name="host">域名</param>
        /// <param name="timeout">超时时间(秒)</param>
        /// <returns></returns>
        public static PingReply Ping(string host, int timeout)
        {
            PingReply result = new Ping().Send("127.0.0.1");
            try
            {
                result = new Ping().Send(host, timeout);
            }
            catch (Exception ex)
            {
                LogUtil.WriteException(ex);
            }
            return result;
        }
    }
}
