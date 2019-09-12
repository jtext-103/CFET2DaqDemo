using System;

namespace Jtext103.CFET2.Things.BasicAIModel
{
    /// <summary>
    /// AI需要实现该接口
    /// </summary>
    public interface IBasicAI
    {
        /// <summary>
        /// AI任务状态
        /// </summary>
        Status AIState { get; }

        /// <summary>
        /// 最新一炮开始时间
        /// </summary>
        DateTime LastShotTime { get; }

        /// <summary>
        /// 存数据时是否需要将原始数据转置
        /// 如果数据格式为“data[通道个数][每通道数据个数]”则不需要转置；
        /// 如果数据格式为“data[每通道数据个数][通道个数]”则需要转置
        /// </summary>
        bool DataNeedTransposeWhenSaving { get; }

        /// <summary>
        /// AI配置文件，AI基本属性均从该配置文件中获取
        /// </summary>
        BasicAIStaticConfig StaticConfig { get; }

        /// <summary>
        /// 配置文件完整路径
        /// </summary>
        string ConfigFilePath { get; }

        /// <summary>
        /// 任务停止事件
        /// </summary>
        event EventHandler RaiseAITaskStopEvent;

        /// <summary>
        /// 任务状态改变事件
        /// </summary>
        event EventHandler RaiseStatusChangeEvent;

        /// <summary>
        /// 采集到新数据事件
        /// </summary>
        event EventHandler<DataArrivalEventArgs> RaiseDataArrivalEvent;

        /// <summary>
        /// 从配置文件中初始化AI任务的属性
        /// </summary>
        /// <param name="configFilePath">配置文件完整路径</param>
        void InitAI(string configFilePath);

        /// <summary>
        /// 启动采集任务
        /// </summary>
        void TryArmTask();
        
        /// <summary>
        /// 停止采集任务
        /// </summary>
        /// <returns></returns>
        bool TryStopTask();

        /// <summary>
        /// 从配置文件中读取配置,只会返回一个config对象，并不会改变AI本身的设置
        /// </summary>
        /// <param name="configFilePath"></param>
        /// <returns></returns>
        BasicAIStaticConfig LoadStaticConfig(string configFilePath);

        /// <summary>
        /// 修改配置
        /// </summary>
        void ChangeStaticConfig(BasicAIStaticConfig basicAIStaticConfig);

        /// <summary>
        /// 将配置作为新文件保存，返回true保存成功
        /// </summary>
        /// <param name="configFilePath">需要保存的路径</param>
        /// <returns></returns>
        bool SaveStaticConfig();
    }
}
