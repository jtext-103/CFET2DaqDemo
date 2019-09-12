using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.BasicAIModel
{
    /// <summary>
    /// 配置终端输入方式
    /// </summary>
    public enum AITerminalType
    {
        //差分
        Differential = 0,
        //参考单端
        RSE = 1,
        //非参考单端
        NRSE = 2,        
        //伪差分
        Pseudodifferential = 3
    }
}
