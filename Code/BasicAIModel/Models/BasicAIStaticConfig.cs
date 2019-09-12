using Newtonsoft.Json;
using System.IO;
using System.Text;

namespace Jtext103.CFET2.Things.BasicAIModel
{
    /// <summary>
    /// AI基本静态配置文件
    /// 因为有些属性是object，实际类型与具体板卡相关，对于具体板卡的配置文件需要继承该类，并实例化被继承的类
    /// </summary>
    public abstract class BasicAIStaticConfig
    {
        /// <summary>
        /// 采集卡类型，就是个名
        /// </summary>
        public string CardType { get; set; }

        /// <summary>
        /// 触发配置
        /// </summary>
        public AITriggerConfiguration TriggerConfig { get; set; }

        /// <summary>
        /// 时钟配置
        /// </summary>
        public AIClockConfiguration ClockConfig { get; set; }

        /// <summary>
        /// 通道配置
        /// </summary>
        public AIChannelConfiguration ChannelConfig { get; set; }

        /// <summary>
        /// 采集开始参考时间点
        /// </summary>
        public double StartTime { get; set; }

        /// <summary>
        /// 是否将采集到的数据自动存文件
        /// </summary>
        public bool AutoWriteDataToFile { get; set; }

        /// <summary>
        /// 总通道数（注意：必须在AI任务开始前设置！！！）
        /// 因为这个ChannelCount是初始化时设置好的（没实际用），Arm之后可能会变，因此单独拿出来
        /// </summary>
        public int ChannelCount { get; set; }

        /// <summary>
        /// 最多保存的炮数，多了就会删
        /// </summary>
        public uint RemainShotsMax { get; set; }

        /// <summary>
        /// 最少保存的炮数，每次删剩下这么多
        /// </summary>
        public uint RemainShotsMin { get; set; }

        /// <summary>
        /// 设备是否使用（不使用就一直Idle）
        /// </summary>
        public bool IsOn { get; set; }

        /// <summary>
        /// 从配置文件中创建对应配置文件的实例
        /// </summary>
        /// <param name="configFilePath"></param>
        public object InitFromConfigFile(string configFilePath)
        {
            return JsonConvert.DeserializeObject(File.ReadAllText(configFilePath, Encoding.Default), this.GetType());             
        }

        /// <summary>
        /// 将配置保存到文件
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public bool Save(string filePath)
        {
            try
            {
                string jsonData = JsonConvert.SerializeObject(this, Formatting.Indented);
                File.WriteAllText(filePath, jsonData);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
