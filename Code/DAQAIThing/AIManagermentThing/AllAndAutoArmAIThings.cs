using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.DAQAIThing
{
    /// <summary>
    /// 自动arm按列表倒序顺序arm（如果有master，需要把master放List中第一个，即最后一个arm）
    /// </summary>
    public class AllAndAutoArmAIThings
    {
        //所有的AIThing
        public string[] AllAIThingPaths { get; set; }
        //所有需要自动arm的AIThing
        public string[] AutoArmAIThingPaths { get; set; }                
    }
}
