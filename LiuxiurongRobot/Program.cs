using System;
using System.Threading;
using FSLib.Network.Http;
using HtmlAgilityPack;

namespace LiuxiurongRobot
{
    class Program
    {
        /// <summary>
        /// 投票次数
        /// </summary>
        public static int CountTims { set; get; }
        public static int CountThread { set; get; } = 4;
        public static bool IsSleep { set; get; }

        static void Main(string[] args)
        {
            if (DateTime.Now > DateTime.Parse("2016/1/11"))
            {
                Console.WriteLine("刷票时间已经过了，本程序自动销毁~");
                Console.ReadKey();
                return;
            }
            GetNowNum();
            Console.WriteLine("请输入投票次数(数字大于零，无限大)：");
            var x = Console.ReadLine().ToInt32();
            IsSleep = false;
            RunThread(x);
            new Thread(() =>
            {
                while (true)
                {
                    Thread.Sleep(1000 * 60 * 4);
                    IsSleep = true;
                    Thread.Sleep(1000 * 30);
                }
            })
            { IsBackground = true }.Start();
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
                        string ipAddress = Common.GetIp();
                        Console.WriteLine("线程{0}启动", Thread.CurrentThread.ManagedThreadId);
                        while (true)
                        {
                            if (IsSleep)
                            {
                                var sleepMin = new Random().Next(1, 2);
                                Console.WriteLine("线程{0}开始休眠，{1}分钟", Thread.CurrentThread.ManagedThreadId, sleepMin);
                                Thread.Sleep(1000 * 60);// *sleepMin
                                IsSleep = false;
                            }

                            countTimes--;
                            if (countTimes < 0)
                                break;
                            Console.WriteLine("当 前 IP:" + (ipAddress == null ? "本地IP" : Common.IpPhysicsAddress(ipAddress)));
                            ipAddress = new ForgeRobot().RunRobot(ipAddress);
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
            var hashformate = client.Create<string>(HttpMethod.Get, Settion.RootUrl, data: new { id = "tom_weixin_vote", mod = "info", vid = 16, tid = Settion.Tid }).Send();
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
