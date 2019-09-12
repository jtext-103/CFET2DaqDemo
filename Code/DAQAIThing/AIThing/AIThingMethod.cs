using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Attributes;
using Jtext103.CFET2.Core.Extension;
using System;
using System.Collections.Generic;
using Jtext103.CFET2.Things.BasicAIModel;
using Jtext103.CFET2.Things.ShotDirOperate;
using Jtext103.CFET2.Core.Log;
using System.Threading;
using JTextDAQDataFileOperator.Interface;

namespace Jtext103.CFET2.Things.DAQAIThing
{
    public partial class  AIThing : Thing
    {
        /// <summary>
        /// 启动AI采集任务
        /// </summary>
        [Cfet2Method]
        public void TryArm()
        {
            if (basicAI.StaticConfig.IsOn == false) return;

            System.Diagnostics.Debug.WriteLine("TryArm  Thread no: " + Thread.CurrentThread.ManagedThreadId.ToString() + " " + " " + Path);

            //初始化latestData
            latestData = new double[basicAI.StaticConfig.ChannelCount];

            basicAI.TryArmTask();  
        }

        /// <summary>
        /// 停止任务
        /// </summary>
        /// <returns></returns>
        [Cfet2Method]
        public bool TryStop()
        {
            return basicAI.TryStopTask();
        }

        [Cfet2Method]
        public bool SaveConfig()
        {
            return basicAI.SaveStaticConfig();
        }
    }
}
