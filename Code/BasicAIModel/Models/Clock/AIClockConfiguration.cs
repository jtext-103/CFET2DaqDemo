using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.BasicAIModel
{
    /// <summary>
    /// 关于时钟的配置
    /// </summary>
    public class AIClockConfiguration
    {
        /// <summary>
        /// 时钟源
        /// </summary>
        public object ClkSource { get; set; }

        /// <summary>
        /// 采样方式（有限、无限、单点）
        /// </summary>
        public AISamplesMode SampleQuantityMode { get; set; }

        /// <summary>
        /// 采样率
        /// </summary>
        public double SampleRate { get; set; }

        /// <summary>
        /// 时钟边沿
        /// </summary>
        public Edge ClkActiveEdge { get; set; }

        /// <summary>
        /// 每通道采样数（仅用于有限采样）
        /// </summary>
        public int TotalSampleLengthPerChannel { get; set; }

        /// <summary>
        /// 每次读取每通道数据个数，保证该值能被“每通道采样数”整除
        /// 取值范围一般为采样率的1/4到1/2
        /// </summary>
        public int ReadSamplePerTime { get; set; }
    }
}
