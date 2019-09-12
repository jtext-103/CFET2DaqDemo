using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.BasicAIModel
{
    /// <summary>
    /// 触发主从卡设置
    /// </summary>
    public enum AITriggerMasterOrSlave
    {
        /// <summary>
        /// 只有一张卡时，不需要设置主从
        /// </summary>
        NonSync = 0,

        /// <summary>
        /// 主卡
        /// </summary>
        Master = 1,

        /// <summary>
        /// 从卡
        /// </summary>
        Slave = 2
    }
}
