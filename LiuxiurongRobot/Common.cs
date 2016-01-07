using System;
using System.Net;
using System.Net.Mime;
using System.Runtime.CompilerServices;
using System.Threading;
using FSLib.Network.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace LiuxiurongRobot
{
    public class Common
    {
        /// <summary>
        /// 获取IP代理地址
        /// </summary>
        /// <returns></returns>
        public static string GetIp()
        {
            int iptimes = 1;
            while (true)
            {
                if (iptimes >= 1000)
                {
                    Console.WriteLine("线程被终止了！");
                    Thread.CurrentThread.Abort();
                }
                Console.WriteLine(Thread.CurrentThread.ManagedThreadId + "尝试获取代理中，第" + iptimes++ + "次");
                var hashformate = new HttpClient().Create<string>(HttpMethod.Get, "http://vxer.daili666.com/ip/?tid=557541152620047&num=1&delay=5&category=2&sortby=time&foreign=none").Send();
                if (hashformate.IsValid() && IpAddress(hashformate.Result))
                {
                    return hashformate.Result;
                }
                Thread.Sleep(1000);
            }
        }

        /// <summary>
        /// 查询当前请求IP地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static bool IpAddress(string ip)
        {
            //const string searchIp = "http://1111.ip138.com/ic.asp";
            const string searchIp = "https://www.baidu.com/";
            var http = new HttpClient();
            http.Setting.Timeout = 1000 * 3;
            http.Setting.ReadWriteTimeout = 1000 * 3;
            http.Setting.Proxy = new WebProxy(ip);
            http.Setting.ReadWriteTimeout = 3 * 1000;
            var hashformate = http.Create<string>(HttpMethod.Get, searchIp).Send();
            if (hashformate.IsValid())
            {
                //HtmlDocument doc = new HtmlDocument();
                //doc.LoadHtml(hashformate.Result);
                //var ipAddress = doc.DocumentNode.SelectSingleNode("//center").InnerText;
                //OutLog(ipAddress);

                OutLog(hashformate.Status);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 查询IP地址的物理位置
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        public static dynamic IpPhysicsAddress(string ip)
        {
            ip = ip.Split(':')[0].Trim();
            var http = new HttpClient();
            var hashformate = http.Create<string>(HttpMethod.Get, "http://www.133ip.com/gn/jk.php", data: new { an = 1, q = ip }).Send();
            if (hashformate.IsValid())
            {
                try
                {
                    var json = JsonConvert.DeserializeObject<dynamic>(hashformate.Result);
                    return json.s1;
                }
                catch (Exception)
                {
                    return "地址解析错误";
                }
            }
            else
            {
                return "地址解析错误";
            }
        }

        public static void OutLog(object obj = null)
        {
            Console.WriteLine(obj);
        }
    }
}
