using System;
using System.Net;
using System.Threading;
using FSLib.Network.Http;
using HtmlAgilityPack;
using Newtonsoft.Json;

namespace LiuxiurongRobot
{
    class Program
    {
        /// <summary>
        /// 投票次数
        /// </summary>
        public static int CountTims { set; get; }
        public static int CountThread { set; get; }

        static void Main(string[] args)
        {
            if (DateTime.Now > DateTime.Parse("2016/1/1"))
            {
                Console.WriteLine("刷票时间已经过了，本程序自动销毁~");
                Console.ReadKey();
                return;
            }
            GetNowNum();
            Console.WriteLine("请输入投票次数(数字大于零，无限大)：");
            var x = Console.ReadLine().ToInt32();
            CountThread = 1;
            RunThread(x);
            Console.ReadKey();
        }

        public static void RunThread(int countTimes)
        {
            for (var i = 0; i < CountThread; i++)
            {
                new Thread(() =>
                {
                    try
                    {
                        Console.WriteLine("线程{0}启动", Thread.CurrentThread.ManagedThreadId);
                        string ipAddress = "222.188.100.204:8086";
                        while (true)
                        {
                            countTimes--;
                            if (countTimes < 0)
                                break;
                            var name = new BuildName().MakeRealNames(1).Trim(',');
                            var phoneNum = BuildPhoneNum.MackPhoneNum();

                            //OutLog("当 前 IP:" + (ipAddress == null ? "本地IP" : IpPhysicsAddress(ipAddress)));
                            OutLog("伪造姓名:" + name);
                            OutLog("伪造电话:" + phoneNum);
                            ipAddress = RunRobot(name, phoneNum, ipAddress);
                        }
                        GetNowNum();
                        OutLog("任务完成！");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("线程{0}出现致命错误:{1}", Thread.CurrentThread.ManagedThreadId, e.Message);
                    }
                    
                }) { IsBackground = true }.Start();
                Thread.Sleep(250);
            }
        }

        public static string RunRobot(string name, string phoneNum, string ipProxy = null)
        {
            try
            {
                while (true)
                {
                    var client = new HttpClient();
                    var hashformate = client.Create<string>(HttpMethod.Get, "http://art-work.com.cn/plugin.php?id=tom_weixin_vote&mod=info&vid=12&tid=177").Send();
                    string formhash = "", tomhash = "";
                    if (hashformate.IsValid())
                    {
                        HtmlDocument doc = new HtmlDocument();
                        doc.LoadHtml(hashformate.Result);
                        formhash = doc.DocumentNode.SelectSingleNode("//input[@name='formhash']").Attributes["value"].Value;
                        tomhash = doc.DocumentNode.SelectSingleNode("//input[@name='tomhash']").Attributes["value"].Value;
                        OutLog("提取FORMHASH值：" + formhash);
                        OutLog("提取TOMHASH值：" + tomhash);
                    }
                    if (!string.IsNullOrWhiteSpace(ipProxy))
                        client.Setting.Proxy = new WebProxy(ipProxy);
                    var context = client.Create<string>(HttpMethod.Get, "http://art-work.com.cn/plugin.php", data: new
                    {
                        id = "tom_weixin_vote",
                        mod = "save",
                        vid = 12,
                        formhash = formhash,
                        tomhash = tomhash,
                        act = "tpadd",
                        tid = "177 or 1=1", //216 or tid=177
                        tpxm = name,
                        tptel = phoneNum
                    });
                    context.Send();
                    if (context.IsValid())
                    {
                        OutLog(context.Result);
                        var model = JsonConvert.DeserializeObject<dynamic>(context.Result);
                        if (model.status == "303")
                        {
                            OutLog("IP被限制，自动切换IP代理！");
                            ipProxy = GetIp();
                        }
                        else
                        {
                            Console.WriteLine(Thread.CurrentThread.ManagedThreadId + " 成功投一票！");
                            return ipProxy;
                        }
                    }
                    else
                    {
                        OutLog("错误！状态码：" + context.Response);
                        ipProxy = GetIp();
                    }
                }

            }
            catch (Exception e)
            {
                OutLog("出现一次错误，跳过本次" + e.Message);
            }
            OutLog();
            return null;
        }

        public static void GetNowNum()
        {
            Console.WriteLine("获取目前票况，请稍等...");
            var client = new HttpClient();
            var hashformate = client.Create<string>(HttpMethod.Get, "http://art-work.com.cn/plugin.php?id=tom_weixin_vote&mod=info&vid=12&tid=177").Send();
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(hashformate.Result);
            var num = doc.DocumentNode.SelectSingleNode("//div[@class='info']").SelectNodes("//p");
            for (int i = 0; i < 3; i++)
                Console.WriteLine(num[i].InnerText);
            Console.WriteLine();
        }

        /// <summary>
        /// 获取IP代理地址
        /// </summary>
        /// <returns></returns>
        private static string GetIp()
        {
            int iptimes = 1;
            while (true)
            {
                OutLog("尝试获取代理中，第" + iptimes++ + "次");
                var hashformate = new HttpClient().Create<string>(HttpMethod.Get, "http://vxer.daili666.com/ip/?tid=557541152620047&num=1&delay=5&category=2&sortby=time&foreign=none&filter=on").Send();
                if (hashformate.IsValid() && IpAddress(hashformate.Result))
                {
                    return hashformate.Result;
                }
                Thread.Sleep(250);
            }
        }

        /// <summary>
        /// 查询当前请求IP地址
        /// </summary>
        /// <param name="ip"></param>
        /// <returns></returns>
        private static bool IpAddress(string ip)
        {
            const string searchIp = "http://1111.ip138.com/ic.asp";
            var http = new HttpClient();
            http.Setting.Proxy = new WebProxy(ip);
            http.Setting.ReadWriteTimeout = 3 * 1000;
            var hashformate = http.Create<string>(HttpMethod.Get, searchIp).Send();
            if (hashformate.IsValid())
            {
                HtmlDocument doc = new HtmlDocument();
                doc.LoadHtml(hashformate.Result);
                var ipAddress = doc.DocumentNode.SelectSingleNode("//center").InnerText;
                OutLog(ipAddress);
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

        private static void OutLog(string obj = null)
        {
            Console.WriteLine(obj);
        }
    }
}
