using System;
using System.Collections;
using System.Text;

namespace LiuxiurongRobot
{
    public class BuildPhoneNum
    {
        public static string MackPhoneNum()
        {
            StringBuilder phoneNum = new StringBuilder();
            StringBuilder suffix = new StringBuilder();
            var suffixR = new Random();
            for (int i = 0; i < 8; i++)
                suffix.Append(suffixR.Next(0, 9));
            phoneNum.Append(GetPrefix() + suffix);
            return phoneNum.ToString();
        }

        private static string GetPrefix()
        {
            var array = new ArrayList()
            {
                "134","135","136","137","138","139",
                "150","151","152","158",
                "182","183","184",
                "178","174",
                "130","131","132","155","156",
                "176","185",
                "133","153","180","181","189",
                "177"
            };
            var prefixR = new Random().Next(0, 27);
            return array[prefixR].ToString();
        }
    }
}
