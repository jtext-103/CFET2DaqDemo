using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.BasicAIModel
{
    /// <summary>
    /// 触发方式
    /// </summary>
    public enum AITriggerType
    {
        //无触发，直接运行
        Immediate = 0,
        DigitalTrigger = 1,
        AnalogTrigger = 2
    }
}
