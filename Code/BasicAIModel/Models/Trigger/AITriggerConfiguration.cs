using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.BasicAIModel
{
    public class AITriggerConfiguration
    {
        /// <summary>
        /// 触发方式(立即开始、数字触发、模拟触发)
        /// </summary>
        public AITriggerType TriggerType { get; set; }

        /// <summary>
        /// 触发通道（仅用于外部触发）
        /// </summary>
        public object TriggerSource { get; set; }

        /// <summary>
        /// 触发边沿
        /// </summary>
        public Edge TriggerEdge { get; set; }

        /// <summary>
        /// 触发后采集开始延时
        /// </summary>
        public double Delay { get; set; }

        /// <summary>
        /// 触发主从卡设置
        /// </summary>
        public AITriggerMasterOrSlave MasterOrSlave { get; set; }
                
    }
}
