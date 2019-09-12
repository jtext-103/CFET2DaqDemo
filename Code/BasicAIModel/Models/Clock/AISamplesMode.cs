using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.BasicAIModel
{
    public enum AISamplesMode
    {
        //有限采样
        FiniteSamples = 0,
        
        //无限采样
        ContinuousSamples = 1,
        
        //硬件定时单点
        HardwareTimedSinglePoint = 2
    }
}
