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
    public partial class AIThing : Thing
    {
        #region Clock
        /// <summary>
        /// 获取采样率
        /// </summary>
        /// <param name="shotNo"></param>
        /// <returns></returns>
        [Cfet2Config(ConfigActions = ConfigAction.Get, Name = "SampleRate")]
        public double SampleRate(int shotNo = -1)
        {
            //-1代表当前配置文件中的status配置
            if (shotNo == -1)
            {
                return basicAI.StaticConfig.ClockConfig.SampleRate;
            }
            //其余参数是从已保存的文件中读取
            return tryUpdateStaticConfigDic(shotNo).ClockConfig.SampleRate;
        }

        /// <summary>
        /// 设置采样率
        /// </summary>
        /// <param name="sampleRate"></param>
        [Cfet2Config(ConfigActions = ConfigAction.Set, Name = "SampleRate")]
        public void SampleRateSet(double sampleRate)
        {
            basicAI.StaticConfig.ClockConfig.SampleRate = sampleRate;

            //这里需要把ReadSamplePerTime也设置一下
            int gcd = Caculator.GCD((int)sampleRate, basicAI.StaticConfig.ClockConfig.TotalSampleLengthPerChannel);
            if(gcd <= 20000)
            {
                basicAI.StaticConfig.ClockConfig.ReadSamplePerTime = gcd;
            }
            else
            {
                basicAI.StaticConfig.ClockConfig.ReadSamplePerTime = Caculator.GCD(gcd, 20000);
            }
            
            basicAI.ChangeStaticConfig(basicAI.StaticConfig);
        }

        /// <summary>
        /// 获取每次采样点
        /// </summary>
        /// <param name="shotNo"></param>
        /// <returns></returns>
        [Cfet2Config(ConfigActions = ConfigAction.Get, Name = "SamplePerTime")]
        public double SamplePerTimeGet(int shotNo = -1)
        {
            //-1代表当前配置文件中的status配置
            if (shotNo == -1)
            {
                return basicAI.StaticConfig.ClockConfig.ReadSamplePerTime;
            }
            //其余参数是从已保存的文件中读取
            return tryUpdateStaticConfigDic(shotNo).ClockConfig.ReadSamplePerTime;
        }

        /// <summary>
        /// 设置每次采样点
        /// </summary>
        /// <param name="sampleRate"></param>
        [Cfet2Config(ConfigActions = ConfigAction.Set, Name = "SamplePerTime")]
        public void SamplePerTimeSet(int samplePerTime)
        {
            basicAI.StaticConfig.ClockConfig.ReadSamplePerTime = samplePerTime;

            basicAI.ChangeStaticConfig(basicAI.StaticConfig);
        }

        /// <summary>
        /// 获取采样点数
        /// </summary>
        /// <param name="shotNo"></param>
        /// <returns></returns>
        [Cfet2Config(ConfigActions = ConfigAction.Get, Name = "Length")]
        public int Length(int shotNo = -1)
        {
            //-1代表当前配置文件中的status配置
            if (shotNo == -1)
            {
                return basicAI.StaticConfig.ClockConfig.TotalSampleLengthPerChannel;
            }
            //其余参数是从已保存的文件中读取
            return tryUpdateStaticConfigDic(shotNo).ClockConfig.TotalSampleLengthPerChannel;
        }

        /// <summary>
        /// 设置采样点数
        /// </summary>
        /// <param name="length"></param>
        [Cfet2Config(ConfigActions = ConfigAction.Set, Name = "Length")]
        public void LengthSet(int length)
        {
            basicAI.StaticConfig.ClockConfig.TotalSampleLengthPerChannel = length;

            //这里需要把ReadSamplePerTime也设置一下
            int gcd = Caculator.GCD((int)length, (int)basicAI.StaticConfig.ClockConfig.SampleRate);
            if (gcd <= 20000)
            {
                basicAI.StaticConfig.ClockConfig.ReadSamplePerTime = gcd;
            }
            else
            {
                basicAI.StaticConfig.ClockConfig.ReadSamplePerTime = Caculator.GCD(gcd, 20000);
            }

            basicAI.ChangeStaticConfig(basicAI.StaticConfig);
        }

        /// <summary>
        /// 获取采样模式
        /// </summary>
        /// <param name="shotNo"></param>
        /// <returns></returns>
        [Cfet2Config(ConfigActions = ConfigAction.Get, Name = "SampleModel")]
        public AISamplesMode SampleModelGet(int shotNo = -1)
        {
            //-1代表当前配置文件中的status配置
            if (shotNo == -1)
            {
                return basicAI.StaticConfig.ClockConfig.SampleQuantityMode;
            }
            //其余参数是从已保存的文件中读取
            return tryUpdateStaticConfigDic(shotNo).ClockConfig.SampleQuantityMode;
        }

        /// <summary>
        /// 设置采样模式
        /// </summary>
        /// <param name="sampleRate"></param>
        [Cfet2Config(ConfigActions = ConfigAction.Set, Name = "SampleModel")]
        public void SampleModelSet(int sampleMode)
        {
            if ((AISamplesMode)sampleMode == AISamplesMode.ContinuousSamples)
            {
                basicAI.StaticConfig.AutoWriteDataToFile = false;
            }
            else if((AISamplesMode)sampleMode == AISamplesMode.FiniteSamples)
            {
                basicAI.StaticConfig.AutoWriteDataToFile = true;
            }
            basicAI.StaticConfig.ClockConfig.SampleQuantityMode = (AISamplesMode)sampleMode;

            basicAI.ChangeStaticConfig(basicAI.StaticConfig);
        }
        #endregion

        #region Trigger
        /// <summary>
        /// 获取同步方式
        /// </summary>
        /// <returns></returns>
        [Cfet2Config(ConfigActions = ConfigAction.Get, Name = "SyncType")]
        public AITriggerMasterOrSlave SyncType()
        {
            return basicAI.StaticConfig.TriggerConfig.MasterOrSlave;
        }

        /// <summary>
        /// 设置同步方式
        /// </summary>
        /// <param name="type"></param>
        [Cfet2Config(ConfigActions = ConfigAction.Set, Name = "SyncType")]
        public void SyncTypeSet(int type)
        {
            basicAI.StaticConfig.TriggerConfig.MasterOrSlave = (AITriggerMasterOrSlave)type;
            basicAI.ChangeStaticConfig(basicAI.StaticConfig);
        }

        /// <summary>
        /// 获取触发方式
        /// </summary>
        /// <returns></returns>
        [Cfet2Config(ConfigActions = ConfigAction.Get, Name = "TriggerType")]
        public AITriggerType TriggerType()
        {
            return basicAI.StaticConfig.TriggerConfig.TriggerType;
        }

        /// <summary>
        /// 设置触发方式
        /// </summary>
        /// <returns></returns>
        [Cfet2Config(ConfigActions = ConfigAction.Set, Name = "TriggerType")]
        public void TriggerTypeSet(int type)
        {
            basicAI.StaticConfig.TriggerConfig.TriggerType = (AITriggerType)type;
            basicAI.ChangeStaticConfig(basicAI.StaticConfig);
        }

        /// <summary>
        /// 获取触发通道
        /// </summary>
        [Cfet2Config(ConfigActions = ConfigAction.Get, Name = "TriggerSource")]
        public object TriggerSource()
        {
            return basicAI.StaticConfig.TriggerConfig.TriggerSource;
        }

        /// <summary>
        /// 设置触发通道
        /// </summary>
        [Cfet2Config(ConfigActions = ConfigAction.Set, Name = "TriggerSource")]
        public void TriggerSourceSet(string source)
        {
            basicAI.StaticConfig.TriggerConfig.TriggerSource = source;
            basicAI.ChangeStaticConfig(basicAI.StaticConfig);
        }

        /// <summary>
        /// 获取触发后延时
        /// </summary>
        /// <returns></returns>
        [Cfet2Config(ConfigActions = ConfigAction.Get, Name = "Delay")]
        public double Delay()
        {
            return basicAI.StaticConfig.TriggerConfig.Delay;
        }

        /// <summary>
        /// 设置触发后延时
        /// </summary>
        /// <param name="delay"></param>
        [Cfet2Config(ConfigActions = ConfigAction.Set, Name = "Delay")]
        public void DelaySet(double delay)
        {
            basicAI.StaticConfig.TriggerConfig.Delay = delay;
            basicAI.ChangeStaticConfig(basicAI.StaticConfig);
        }

        #endregion

        #region Channel
        /// <summary>
        /// 获取通道名
        /// </summary>
        /// <returns></returns>
        [Cfet2Config(ConfigActions = ConfigAction.Get, Name = "ChannelName")]
        public string ChannelName()
        {
            return basicAI.StaticConfig.ChannelConfig.ChannelName;
        }

        [Cfet2Config(ConfigActions = ConfigAction.Set, Name = "ChannelName")]
        public void ChannelNameSet(string channelName)
        {
            //注意这里的替换行为
            channelName = channelName.Replace(".", @"/");
            basicAI.StaticConfig.ChannelConfig.ChannelName = channelName;
            basicAI.ChangeStaticConfig(basicAI.StaticConfig);
        }

        /// <summary>
        /// 获取Channel个数
        /// </summary>
        /// <param name="shotNo"></param>
        /// <returns></returns>
        [Cfet2Config(ConfigActions = ConfigAction.Get, Name = "ChannelCount")]
        public int ChannelCount(int shotNo = -1)
        {
            //0代表当前配置文件中的status配置
            if (shotNo == -1)
            {
                return basicAI.StaticConfig.ChannelCount;
            }
            //其余参数是从已保存的文件中读取
            return tryUpdateStaticConfigDic(shotNo).ChannelCount;
        }

        /// <summary>
        /// 设置Channel个数
        /// </summary>
        /// <param name="channelCount"></param>
        [Cfet2Config(ConfigActions = ConfigAction.Set, Name = "ChannelCount")]
        public void ChannelCountSet(int channelCount)
        {
            basicAI.StaticConfig.ChannelCount = channelCount;
            basicAI.ChangeStaticConfig(basicAI.StaticConfig);
        }

        #endregion

        #region Other
        /// <summary>
        /// 获取设备是否在用
        /// </summary>
        /// <returns></returns>
        [Cfet2Config(ConfigActions = ConfigAction.Get, Name = "IsOn")]
        public bool IsOn()
        {
            return basicAI.StaticConfig.IsOn;
        }

        /// <summary>
        /// 设置设备是否在用
        /// </summary>
        /// <param name="on"></param>
        [Cfet2Config(ConfigActions = ConfigAction.Set, Name = "IsOn")]
        public void IsOnSet(bool on)
        {
            basicAI.StaticConfig.IsOn = on;
            basicAI.ChangeStaticConfig(basicAI.StaticConfig);
        }

        #endregion

    }
}
