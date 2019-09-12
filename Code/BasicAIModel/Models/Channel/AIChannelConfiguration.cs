using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.BasicAIModel
{
    /// <summary>
    /// 关于采集通道的配置
    /// </summary>
    public class AIChannelConfiguration
    {
        /// <summary>
        /// 采集通道
        /// </summary>
        public string ChannelName { get; set; }

        /// <summary>
        /// 配置终端输入方式（差分、单端）
        /// </summary>
        public AITerminalType TerminalConfigType { get; set; }

        /// <summary>
        /// 输入电压最小值
        /// </summary>
        public double MinimumValue { get; set; }

        /// <summary>
        /// 输入电压最大值
        /// </summary>
        public double MaximumValue { get; set; }

    }
}
