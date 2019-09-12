using NationalInstruments.DAQmx;
using System;
using System.Collections.Generic;
using Jtext103.CFET2.Things.BasicAIModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Threading;

namespace Jtext103.CFET2.Things.NiAiLib
{
    public class NIAI : IBasicAI, IDisposable
    {
        private NationalInstruments.DAQmx.Task aiTask;

        private Status _aiState;

        private NIAIStaticConfig _staticConfig;

        #region public AI status

        public Status AIState
        {
            get
            {
                return _aiState;
            }
            private set
            {
                if (_aiState != value)
                {
                    _aiState = value;
                    //状态改变时，产生OnStatusChanged事件
                    OnStatusChanged();
                }
            }
        }
        public DateTime LastShotTime { get; private set; }

        public BasicAIStaticConfig StaticConfig
        {
            get
            {
                return _staticConfig;
            }
        }

        public string ConfigFilePath { get; internal set; }

        /// <summary>
        /// NI的数据不需要转置
        /// </summary>
        public bool DataNeedTransposeWhenSaving
        {
            get
            {
                return false;
            }
        }
        
        #endregion

        #region event

        public event EventHandler RaiseAITaskStopEvent;

        public event EventHandler RaiseStatusChangeEvent;

        public event EventHandler<DataArrivalEventArgs> RaiseDataArrivalEvent;

        /// <summary>
        /// invoke RaiseAITaskStopEvent
        /// </summary>
        protected virtual void OnAITaskStopped()
        {
            LastShotTime = DateTime.UtcNow;
            RaiseAITaskStopEvent?.Invoke(this, new EventArgs());
            //相当于下面的
            //if (StopEventHandler != null)
            //{
            //    StopEventHandler(this, new EventArgs());
            //}
        }

        /// <summary>
        /// invoke RaiseStatusChangeEvent
        /// </summary>
        protected virtual void OnStatusChanged()
        {
            RaiseStatusChangeEvent?.Invoke(this, new EventArgs());
        }

        /// <summary>
        /// invoke RaiseDataArrivelEvent
        /// </summary>
        protected virtual void OnDataArrival(double[,] data)
        {
            //如果传过来了数据，产生事件
            if (data.GetLength(1) > 0)
            {
                RaiseDataArrivalEvent?.Invoke(this, new DataArrivalEventArgs(data));
            }
        }

        #endregion

        //通过比较totalTimes和times判断任务是否结束
        //totalTimes应该等于产生EveryNSamplesRead事件的次数
        private int totalTimes;
        //当前产生EveryNSamplesRead的次数
        private int times;
        //防止产生多次结束事件
        private bool hasFinishFlag;

        /// <summary>
        /// 用于NI采集卡AI读数据
        /// </summary>
        private AnalogMultiChannelReader reader;

        public NIAI()
        {
            LastShotTime = DateTime.UtcNow;
            _aiState = Status.Idle;
        }

        public void InitAI(string configFilePath)
        {
            _staticConfig = LoadStaticConfig(configFilePath) as NIAIStaticConfig;
        }

        public BasicAIStaticConfig LoadStaticConfig(string configFilePath)
        {      
            if (configFilePath == "" || configFilePath == null)
            {
                return new NIAIStaticConfig();
            }
            ConfigFilePath = configFilePath;
            return new NIAIStaticConfig(configFilePath);
        }

        public void ChangeStaticConfig(BasicAIStaticConfig basicAIStaticConfig)
        {
            _staticConfig = (NIAIStaticConfig)basicAIStaticConfig;
        }

        public bool SaveStaticConfig()
        {
            return _staticConfig.Save(ConfigFilePath);
        }

        /// <summary>
        /// 启动AI采集任务
        /// </summary>
        public void TryArmTask()
        {
            if (AIState != Status.Idle)
            {
                throw new Exception("If you want to arm, the AI state must be 'Idle'!");
            }
            else
            {
                if (aiTask == null)
                {
                    try
                    {
                        hasFinishFlag = false;
                        //新建任务
                        aiTask = new NationalInstruments.DAQmx.Task();

                        //aiTask.Stream.ReadAutoStart = true;

                        //配置任务
                        NIAIConfigMapper.MapAndConfigAll(aiTask, _staticConfig);

                        //获取并设置通道数
                        _staticConfig.ChannelCount = aiTask.AIChannels.Count;

                        //使用NI Task中的EveryNSamplesRead事件读取数据
                        aiTask.EveryNSamplesReadEventInterval = _staticConfig.ClockConfig.ReadSamplePerTime;
                        aiTask.EveryNSamplesRead += AiTask_EveryNSamplesRead;

                        //计算读取次数
                        times = 0;
                        totalTimes = _staticConfig.ClockConfig.TotalSampleLengthPerChannel / _staticConfig.ClockConfig.ReadSamplePerTime;

                        //Verify the Task
                        aiTask.Control(TaskAction.Verify);

                        //read stream
                        //使用reader读数据
                        reader = new AnalogMultiChannelReader(aiTask.Stream);

                        aiTask.SynchronizeCallbacks = true;

                        //开始任务
                        aiTask.Start();

                        //idle -> ready
                        AIState = Status.Ready;
                    }
                    catch (DaqException ex)
                    {
                        //ex.Message
                        goError();
                        throw ex;
                    }
                }
            }
        }

        private object lockObject = new object();

        //NI每一波采集数据到来的事件处理函数
        private void AiTask_EveryNSamplesRead(object sender, EveryNSamplesReadEventArgs e)
        {
            lock (lockObject)
            {
                if (AIState == Status.Ready)
                {
                    AIState = Status.Running;
                }

                double[,] readData = reader.ReadMultiSample(_staticConfig.ClockConfig.ReadSamplePerTime);

                times++;

                System.Diagnostics.Debug.WriteLine("Data arrival! NO." + times + " Thread no: " + Thread.CurrentThread.ManagedThreadId.ToString() + " " + StaticConfig.ChannelConfig.ChannelName);

                OnDataArrival(readData);

                if (aiTask.IsDone && times == totalTimes && !hasFinishFlag)
                {
                    //等待5s确保所有采集都完成
                    Thread.Sleep(5000);

                    hasFinishFlag = true;
                    System.Diagnostics.Debug.WriteLine("NIAI stop !!! Thread no: " + Thread.CurrentThread.ManagedThreadId.ToString() + " " + StaticConfig.ChannelConfig.ChannelName);
                    //任务结束
                    //产生任务结束事件
                    //在该事件处理函数中结束当前任务
                    OnAITaskStopped();
                }
            }
        }

        /// <summary>
        /// 停止任务，回到idle状态
        /// </summary>
        /// <returns></returns>
        public bool TryStopTask()
        {
            if (aiTask != null)
            {
                try
                {
                    aiTask.Stop();

                    //之前没有，发布停事件
                    //OnAITaskStopped();
                }
                catch (Exception ex)
                {
                    //目前啥也不用做
                }
                Thread.Sleep(1000);
                if(aiTask != null)
                {
                    aiTask.Dispose();
                }  
                aiTask = null;
                AIState = Status.Idle;
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 进入ERROR状态
        /// 状态改为ERROR并停止任务
        /// </summary>
        private void goError()
        {
            AIState = Status.Error;
        }

        #region IDisposable Support

        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    TryStopTask();
                    aiTask.Dispose();
                }
                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~AI() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        public void Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }

        #endregion
    }
}
