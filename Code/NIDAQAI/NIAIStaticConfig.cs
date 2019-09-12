using Jtext103.CFET2.Things.BasicAIModel;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.NiAiLib
{
    public class NIAIStaticConfig : BasicAIStaticConfig
    {
        /// <summary>
        /// 使用默认参数初始化配置文件属性
        /// 部署到全新系统时使用该方法产生默认配置文件，并可在此基础上手动修改
        /// 其余情况务必使用有参构造函数通过配置文件构造该类实例
        /// </summary>
        public NIAIStaticConfig()
        {
            //没有配置文件路径时，初始化生成的模板
            TriggerConfig = new AITriggerConfiguration()
            {
                TriggerType = AITriggerType.DigitalTrigger,
                TriggerSource = "/PXI1Slot3/ai/StartTrigger",
                TriggerEdge = Edge.Rising,
                MasterOrSlave = AITriggerMasterOrSlave.Slave
            };
            ClockConfig = new AIClockConfiguration()
            {
                ClkSource = "",
                SampleQuantityMode = AISamplesMode.FiniteSamples,
                SampleRate = 1000,
                ClkActiveEdge = Edge.Rising,
                TotalSampleLengthPerChannel = 1000,
                ReadSamplePerTime = 500
            };
            ChannelConfig = new AIChannelConfiguration()
            {
                ChannelName = "PXI1Slot4/ai0:3",
                TerminalConfigType = AITerminalType.Differential,
                MinimumValue = 0,
                MaximumValue = 10
            };
            StartTime = 0.5;
            AutoWriteDataToFile = true;
            ChannelCount = 4;
            RemainShotsMax = 30;
            RemainShotsMin = 20;
        }

        /// <summary>
        /// 通过配置文件构造实例
        /// </summary>
        /// <param name="filePath"></param>
        public NIAIStaticConfig(string filePath)
        {
            NIAIStaticConfig config = (NIAIStaticConfig)InitFromConfigFile(filePath);
            TriggerConfig = config.TriggerConfig;
            ClockConfig = config.ClockConfig;
            ChannelConfig = config.ChannelConfig;
            StartTime = config.StartTime;
            AutoWriteDataToFile = config.AutoWriteDataToFile;
            ChannelCount = config.ChannelCount;
            RemainShotsMax = config.RemainShotsMax;
            RemainShotsMin = config.RemainShotsMin;
            IsOn = config.IsOn;
            CardType = config.CardType;
        }
    }
}
