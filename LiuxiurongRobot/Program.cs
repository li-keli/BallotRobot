using System;
using System.Globalization;
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
        public static int CountThread { set; get; } = 1;
        public static string RootUrl { set; get; } = "http://art-work.com.cn/plugin.php";
        public static bool IsSleep { set; get; }

        static void Main(string[] args)
        {
            if (DateTime.Now > DateTime.Parse("2016/1/11"))
            {
                Console.WriteLine("刷票时间已经过了，本程序自动销毁~");
                Console.ReadKey();
                return;
            }
            //GetNowNum();

            var client = new HttpClient();
            client.Setting.Headers.Add("User-Agent",
                             "Mozilla/5.0 (iPhone; CPU iPhone OS 7_0 like Mac OS X; en-us) AppleWebKit/537.51.1 (KHTML, like Gecko) Version/7.0 Mobile/11A465 Safari/9537.53");
            var context = client.Create<string>(HttpMethod.Get, "http://www.cnblogs.com/").Send();
            var a = context.Result;
            var b = 1;
            //Console.WriteLine("请输入投票次数(数字大于零，无限大)：");
            //var x = Console.ReadLine().ToInt32();
            //IsSleep = false;
            //RunThread(x);
            //new Thread(() =>
            //{
            //    while (true)
            //    {
            //        Thread.Sleep(1000 * 60 * 4);
            //        IsSleep = true;
            //        Thread.Sleep(1000 * 30);
            //    }
            //})
            //{ IsBackground = true }.Start();
            //Console.ReadKey();
        }

        public static void RunThread(int countTimes)
        {
            string ipAddress = null; // GetIp();
            for (var i = 0; i < CountThread; i++)
            {
                new Thread(() =>
                {
                    try
                    {
                        Console.WriteLine("线程{0}启动", Thread.CurrentThread.ManagedThreadId);
                        while (true)
                        {
                            if (IsSleep)
                            {
                                var sleepMin = new Random().Next(1, 2);
                                Console.WriteLine("线程{0}开始休眠，{1}分钟", Thread.CurrentThread.ManagedThreadId, sleepMin);
                                Thread.Sleep(1000 * 60 * sleepMin);
                                IsSleep = false;
                            }

                            countTimes--;
                            if (countTimes < 0)
                                break;
                            Console.WriteLine("当 前 IP:" + (ipAddress == null ? "本地IP" : Common.IpPhysicsAddress(ipAddress)));
                            //ipAddress = RunRobot(new Random().Next(1000, 337339), string.IsNullOrWhiteSpace(ipAddress) ? "222.188.100.204:8086" : ipAddress);
                        }
                        GetNowNum();
                        Common.OutLog("任务完成！");
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("线程{0}出现致命错误:{1}", Thread.CurrentThread.ManagedThreadId, e.Message);
                    }

                })
                { IsBackground = true }.Start();
                Thread.Sleep(250);
            }
        }

        public static void GetNowNum()
        {
            Console.WriteLine("获取目前票况，请稍等...");
            var client = new HttpClient();
            var hashformate = client.Create<string>(HttpMethod.Get, RootUrl, data: new { id = "tom_weixin_vote", mod = "info", vid = 16, tid = 254 }).Send();//"id=tom_weixin_vote&mod=info&vid=16&tid=254"
            if (!hashformate.IsValid())
            {
                Console.WriteLine("数据拉取失败," + hashformate.State);
                return;
            }
            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(hashformate.Result);
            var num = doc.DocumentNode.SelectSingleNode("//div[@class='info']").SelectNodes("//p");
            for (int i = 0; i < 3; i++)
                Console.WriteLine(num[i].InnerText);
            Console.WriteLine();
        }

        
    }
}
