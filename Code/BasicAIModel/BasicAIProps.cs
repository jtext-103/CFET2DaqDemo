using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.NiAiLib
{
    /// <summary>
    /// AI任务需要的基本属性
    /// </summary>
    public class BasicAIProps
    {
        /// <summary>
        /// 触发方式
        /// </summary>
        public TriggerType AITriggerType { get; set; }

        /// <summary>
        /// 配置终端输入方式（差分、单端）
        /// </summary>
        public TerminalConfiguration TerminalConfig { get; set; }
        /// <summary>
        /// 输入电压最小值
        /// </summary>
        public double MinimumVolt { get; set; }

        /// <summary>
        /// 输入电压最大值
        /// </summary>
        public double MaximumVolt { get; set; }

        /// <summary>
        /// 采样率
        /// </summary>
        public int SampleRate { get; set; }

        /// <summary>
        /// 触发通道（仅用于外部触发）
        /// </summary>
        public string TriggerSource { get; set; }

        /// <summary>
        /// 触发边沿
        /// </summary>
        public Edge AITriggerEdge { get; set; }

        /// <summary>
        /// 触发电平（仅用于外部模拟触发）
        /// </summary>
        public double AnalogTriggerLevel { get; set; }

        /// <summary>
        /// 时钟源（使用内部时钟时，为空字符串）
        /// </summary>
        public string ClkSource { get; set; }

        /// <summary>
        /// 时钟边沿
        /// </summary>
        public Edge ClkActiveEdge { get; set; }

        /// <summary>
        /// 采样方式
        /// </summary>
        public SamplesMode AISamplesMode { get; set; }

        /// <summary>
        /// 每通道采样数（仅用于有限采样）
        /// </summary>
        public int SamplesPerChannel { get; set; }

        /// <summary>
        /// 每次读取数据个数，保证该值能被“每通道采样数”整除
        /// </summary>
        public int ReadSamplePerTime { get; set; }

        public BasicAIProps()
        {
            AITriggerType = TriggerType.SoftTrigger;
            TerminalConfig = TerminalConfiguration.Differential;
            MinimumVolt = 0;
            MaximumVolt = 10;
            SampleRate = 3000;
            TriggerSource = "";
            AITriggerEdge = Edge.Rising;
            AnalogTriggerLevel = 1;
            ClkSource = "";
            ClkActiveEdge = Edge.Rising;
            AISamplesMode = SamplesMode.ContinuousSamples;
            SamplesPerChannel = 3100;
            ReadSamplePerTime = 1000;
        }

    }
}
