using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.BasicAIModel
{
    /// <summary>
    /// 新数据到来事件参数
    /// </summary>
    public class DataArrivalEventArgs : EventArgs
    {
        private double[,] newData;

        /// <summary>
        /// 用二维数组表示的多个通道的数据
        /// </summary>
        public double[,] NewData
        {
            get
            {
                return newData;
            }
        }

        /// <summary>
        /// 新数据到来事件参数
        /// </summary>
        /// <param name="data">数据</param>
        public DataArrivalEventArgs(double[,] data)
        {
            newData = data;
        }
    }
}