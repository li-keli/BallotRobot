using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LiuxiurongRobot
{
    public static class Settion
    {
        public static string RootUrl { set; get; } = "http://yes10.cjh-wang.com.cn/plugin.php";
        /// <summary>
        /// 类别编号
        /// </summary>
        public static int Vid { set; get; } = 1;
        /// <summary>
        /// 投票编号
        /// </summary>
        public static int Tid { set; get; } = 4;
    }
}
