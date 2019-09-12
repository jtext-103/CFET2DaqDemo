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

        #region public AI status

        private Status _aiState;

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

        private NIAIStaticConfig _staticConfig;
        
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

        /// <summary>
        /// 使用默认参数构造AI
        /// </summary>
        public NIAI()
        {
            AIState = Status.Idle;
            LastShotTime = DateTime.UtcNow;
            _aiState = Status.Idle;
        }

        public void InitAI(string configFilePath)
        {
            _staticConfig = LoadStaticConfig(configFilePath) as NIAIStaticConfig;
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
                        //新建任务
                        aiTask = new NationalInstruments.DAQmx.Task();

                        aiTask.Stream.ReadAutoStart = true;

                        //配置任务
                        NIAIConfigMapper.MapAndConfigAll(aiTask, _staticConfig.BasicAITaskConifg);

                        //获取并设置通道数
                        _staticConfig.ChannelCount = aiTask.AIChannels.Count;
                        
                        //Verify the Task
                        aiTask.Control(TaskAction.Verify);
                        
                        int channelCount = _staticConfig.ChannelCount;
                        int readSamplePerTime = _staticConfig.BasicAITaskConifg.ClockConfig.ReadSamplePerTime;
                        ReadData(aiTask, channelCount, readSamplePerTime);

                        //idle -> ready
                        AIState = Status.Ready;

                        //开始任务
                        aiTask.Start();

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

        private async System.Threading.Tasks.Task ReadData(NationalInstruments.DAQmx.Task aiTask, int channelCount, int readSamplePerTime)
        {
            await System.Threading.Tasks.Task.Run(() =>
            {
                AnalogMultiChannelReader reader = new AnalogMultiChannelReader(aiTask.Stream);
                int totalReadDataLength = 0;
                do
                {
                    double[,] readData = new double[channelCount, readSamplePerTime];
                    int actualLength;
                    reader.MemoryOptimizedReadMultiSample(readSamplePerTime, ref readData, ReallocationPolicy.DoNotReallocate, out actualLength);

                    System.Diagnostics.Debug.WriteLine("Data arrival!"+ actualLength + " Thread no: " + Thread.CurrentThread.ManagedThreadId.ToString());

                    OnDataArrival(readData);
                    totalReadDataLength += readSamplePerTime;
                    if (AIState == Status.Ready)
                    {
                        //ready -> running
                        AIState = Status.Running;
                    }
                    //当读够数据则停止
                    if (aiTask.IsDone)
                    {
                        OnAITaskStopped();
                        break;
                    }
                }
                while (true);
            });
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
                }
                catch (Exception ex)
                {
                    //目前啥也不用做
                }
                aiTask.Dispose();
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

        public BasicAIStaticConfig LoadStaticConfig(string filePath)
        {
            return new NIAIStaticConfig(filePath);
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
