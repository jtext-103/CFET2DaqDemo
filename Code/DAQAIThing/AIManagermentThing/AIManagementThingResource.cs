using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Attributes;
using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.Core.Extension;
using Jtext103.CFET2.Core.Log;
using Jtext103.CFET2.Things.BasicAIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.DAQAIThing
{
    /// <summary>
    /// CFET资源都在这里
    /// </summary>
    public partial class AIManagementThing : Thing, IDisposable
    {
        [Cfet2Status]
        public string ShowAll
        {
            get
            {
                string result = null;
                foreach (var s in config.AIThings.AllAIThingPaths)
                {
                    result += s + "\n";
                }
                return result.Substring(0, result.Length - 1);
            }
        }

        [Cfet2Status]
        public int AllCardsCount()
        {
            return config.AIThings.AllAIThingPaths.Count();
        }

        [Cfet2Status]
        public string ShowAuto
        {
            get
            {
                string result = null;
                foreach (var s in config.AIThings.AutoArmAIThingPaths)
                {
                    result += s.ToString() + "\n";
                }
                return result.Substring(0, result.Length - 1);
            }
        }

        [Cfet2Status]
        public int AutoCardsCount()
        {
            return config.AIThings.AutoArmAIThingPaths.Count();
        }

        [Cfet2Status]
        public string MonitorSource()
        {
            return config.MonitorSource;
        }

        [Cfet2Status]
        public object MonitorValue()
        {
            return config.MonitorValue;
        }

        [Cfet2Status]
        public bool IsEqualToArm()
        {
            return config.IsEqualToArm;
        }

        [Cfet2Status]
        public int DelaySecondAfterFinish()
        {
            return config.DelaySecondAfterFinish;
        }

        [Cfet2Status]
        public object TrueMonitorValue()
        {
            if (config.MonitorSource != null)
            {
                return MyHub.TryGetResourceSampleWithUri(config.MonitorSource).ObjectVal;
            }
            return null;
        }
    }
}
