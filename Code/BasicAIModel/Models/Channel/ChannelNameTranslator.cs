using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.BasicAIModel
{
    public static class ChannelNameTranslator
    {
        /// <summary>
        /// 将形如"0,1,2,3,5"的字符串转换成obeject（数组）
        /// <param name="str"></param>
        /// <param name="breakMark">分隔符，默认为逗号</param>
        /// <returns></returns>
        public static List<int> StringToListInt(string str, char breakMark = ',')
        {
            List<int> lis = new List<int>();

            string nowString = null;
            int breakIndex = str.IndexOf(breakMark);
            while (breakIndex > 0)
            {
                nowString = str.Substring(0, breakIndex);
                lis.Add(int.Parse(nowString));
                str = str.Substring(breakIndex + 1, str.Length - breakIndex - 1);
                breakIndex = str.IndexOf(breakMark);
            }
            lis.Add(int.Parse(str));

            return lis;
        }
    }
}
