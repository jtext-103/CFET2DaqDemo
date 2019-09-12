using Jtext103.CFET2.Things.BasicAIModel;
using NationalInstruments.DAQmx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Jtext103.CFET2.Things.NiAiLib
{
    /// <summary>
    /// 从配置文件中读取属性，配置NI AI任务
    /// </summary>
    public static class NIAIConfigMapper
    {
        /// <summary>
        /// 使用AIChannelConfiguration进行NI采集卡通道配置
        /// </summary>
        /// <param name="niTask"></param>
        /// <param name="channelConfiguration"></param>
        public static void MapAndConfigChannel(NationalInstruments.DAQmx.Task niTask, AIChannelConfiguration channelConfiguration)
        {
            AITerminalConfiguration niTerminalConfig;
            switch (channelConfiguration.TerminalConfigType)
            {
                case AITerminalType.NRSE:
                    niTerminalConfig = AITerminalConfiguration.Nrse;
                    break;
                case AITerminalType.RSE:
                    niTerminalConfig = AITerminalConfiguration.Rse;
                    break;
                case AITerminalType.Differential:
                    niTerminalConfig = AITerminalConfiguration.Differential;
                    break;
                case AITerminalType.Pseudodifferential:
                    niTerminalConfig = AITerminalConfiguration.Pseudodifferential;
                    break;
                default:
                    throw new Exception("终端输入方式（差分、单端等）配置错误！");
            }
            //channelName是NI驱动规定格式的字符串，单位默认是伏特
            niTask.AIChannels.CreateVoltageChannel((string)channelConfiguration.ChannelName, "", niTerminalConfig, channelConfiguration.MinimumValue, channelConfiguration.MaximumValue, AIVoltageUnits.Volts);
        }

        /// <summary>
        /// 使用AIClockConfiguration进行NI采集卡时钟配置
        /// </summary>
        /// <param name="niTask"></param>
        /// <param name="clockConfiguration"></param>
        public static void MapAndConfigClock(NationalInstruments.DAQmx.Task niTask, AIClockConfiguration clockConfiguration)
        {
            SampleQuantityMode samplesQuantityMode;
            SampleClockActiveEdge clkActiveEdge;
            switch (clockConfiguration.ClkActiveEdge)
            {
                case Edge.Falling:
                    clkActiveEdge = SampleClockActiveEdge.Falling;
                    break;
                case Edge.Rising:
                    clkActiveEdge = SampleClockActiveEdge.Rising;
                    break;
                default:
                    throw new Exception("时钟边沿配置错误！");
            }
            //经过测试，对于aiTask.Timing.ConfigureSampleClock
            //有限采样时，采样SamplesPerChannel个点
            //连续采样时，一直采集，直到手动停止
            switch (clockConfiguration.SampleQuantityMode)
            {
                case AISamplesMode.ContinuousSamples:
                    samplesQuantityMode = SampleQuantityMode.ContinuousSamples;
                    niTask.Timing.ConfigureSampleClock((string)clockConfiguration.ClkSource, clockConfiguration.SampleRate, clkActiveEdge, samplesQuantityMode);
                    break;
                case AISamplesMode.FiniteSamples:
                    samplesQuantityMode = SampleQuantityMode.FiniteSamples;
                    niTask.Timing.ConfigureSampleClock((string)clockConfiguration.ClkSource, clockConfiguration.SampleRate, clkActiveEdge, samplesQuantityMode, clockConfiguration.TotalSampleLengthPerChannel);
                    break;
                case AISamplesMode.HardwareTimedSinglePoint:
                    samplesQuantityMode = SampleQuantityMode.HardwareTimedSinglePoint;
                    break;
                default:
                    throw new Exception("采样方式（有限、无限、单点）配置错误！");
            }

        }

        /// <summary>
        /// 使用AITriggerConfiguration进行NI采集卡触发及多卡同步配置
        /// </summary>
        /// <param name="niTask"></param>
        /// <param name="triggerConfiguration"></param>
        public static void MapAndConfigTrigger(NationalInstruments.DAQmx.Task niTask, AITriggerConfiguration triggerConfiguration)
        {
            switch (triggerConfiguration.TriggerType)
            {
                case AITriggerType.Immediate:
                    //没触发，无需配置
                    break;
                case AITriggerType.DigitalTrigger:
                    DigitalEdgeStartTriggerEdge digitalTriggerEdge;
                    switch (triggerConfiguration.TriggerEdge)
                    {
                        case Edge.Falling:
                            digitalTriggerEdge = DigitalEdgeStartTriggerEdge.Falling;
                            break;
                        case Edge.Rising:
                            digitalTriggerEdge = DigitalEdgeStartTriggerEdge.Rising;
                            break;
                        default:
                            throw new Exception("触发边沿配置错误！");
                    }
                    niTask.Triggers.StartTrigger.ConfigureDigitalEdgeTrigger((string)triggerConfiguration.TriggerSource, digitalTriggerEdge);
                    //设置为0 NI报错
                    if(triggerConfiguration.Delay != 0)
                    {
                        niTask.Triggers.StartTrigger.DelayUnits = StartTriggerDelayUnits.Seconds;
                        niTask.Triggers.StartTrigger.Delay = triggerConfiguration.Delay;
                    }  
                    break;
                case AITriggerType.AnalogTrigger:
                    AnalogEdgeStartTriggerSlope analogTriggerEdge;
                    switch (triggerConfiguration.TriggerEdge)
                    {
                        case Edge.Falling:
                            analogTriggerEdge = AnalogEdgeStartTriggerSlope.Falling;
                            break;
                        case Edge.Rising:
                            analogTriggerEdge = AnalogEdgeStartTriggerSlope.Rising;
                            break;
                        default:
                            throw new Exception("触发边沿配置错误！");
                    }                 
                    //默认触发电平2.5v
                    niTask.Triggers.StartTrigger.ConfigureAnalogEdgeTrigger((string)triggerConfiguration.TriggerSource, analogTriggerEdge, 2.5);
                    if (triggerConfiguration.Delay != 0)
                    {
                        niTask.Triggers.StartTrigger.DelayUnits = StartTriggerDelayUnits.Seconds;
                        niTask.Triggers.StartTrigger.Delay = triggerConfiguration.Delay;
                    }
                    break;
                default:
                    throw new Exception("触发方式配置错误！");
            }

        }

        /// <summary>
        /// 配置NI采集卡AI任务触发、同步、通道、时钟等各项属性
        /// </summary>
        /// <param name="niTask"></param>
        /// <param name="basicAIConifg"></param>
        public static void MapAndConfigAll(NationalInstruments.DAQmx.Task niTask, BasicAIStaticConfig basicAIConifg)
        {
            MapAndConfigChannel(niTask, basicAIConifg.ChannelConfig);
            MapAndConfigClock(niTask, basicAIConifg.ClockConfig);
            MapAndConfigTrigger(niTask, basicAIConifg.TriggerConfig);
        }
    }
}
