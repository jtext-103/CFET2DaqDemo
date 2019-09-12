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
    /// 当所有AIThing都是idle时，自动ARM所有需要自动ARM的AIThing
    /// 当所有AIThing任务都结束时，产生采集完成的事件（可以用来触发上传）
    /// </summary>
    public partial class AIManagementThing : Thing, IDisposable
    {
        private ICfet2Logger logger;

        private Dictionary<string, Status> allAIThingStates;

        private Dictionary<string, bool> allAIFinishFlags;

        private AIManagementConfig config;

        /// <summary>
        /// 上一个状态
        /// </summary>
        public AllAIStatus AllAIStatePrevious { get; private set; }

        private AllAIStatus _allAIStateNow;

        /// <summary>
        /// 当前状态
        /// </summary>
        [Cfet2Status]
        public AllAIStatus AllAIStateNow
        {
            get
            {
                return _allAIStateNow;
            }
            private set
            {
                if (_allAIStateNow != value)
                {
                    _allAIStateNow = value;
                    MyHub.EventHub.Publish(Path, "AllAIStateChanged", _allAIStateNow);
                    logger.Info("All AI state changed event fired. All AI state change to " + _allAIStateNow.ToString("G") + ".");
                    System.Diagnostics.Debug.WriteLine("AllAIStateChanged = " + _allAIStateNow.ToString() + " time: " + DateTime.Now.ToLocalTime().ToString("HH:mm:ss.fff"));
                }
            }
        }

        /// <summary>
        /// initObj是一个AllAndAutoArmAIThings
        /// </summary>
        /// <param name="path"></param>
        public override void TryInit(object path)
        {
            config = new AIManagementConfig((string)path);

            //判断autoArmAIThings是否是allAIThings的子集
            if (!config.AIThings.AutoArmAIThingPaths.All(autoArm => config.AIThings.AllAIThingPaths.Any(every => every == autoArm)))
            {
                throw new Exception("需要自动arm的所有AIThing必须包含在所有AIThing中");
            }
            logger = Cfet2LogManager.GetLogger("AIManagement");
        }

        public override void Start()
        {
            var allAIThingPaths = config.AIThings.AllAIThingPaths;
            var autoArmAIThingPaths = config.AIThings.AutoArmAIThingPaths;

            allAIThingStates = new Dictionary<string, Status>();
            allAIFinishFlags = new Dictionary<string, bool>();

            foreach (var aiThingPath in allAIThingPaths)
            {
                allAIThingStates.Add(aiThingPath, Status.Idle);
                allAIFinishFlags.Add(aiThingPath, false);
            }

            AllAIStatePrevious = AllAIStatus.AllIdle;
            _allAIStateNow = AllAIStatus.AllIdle;

            //订阅每个aiThing的StateChanged和AITaskFinished事件
            foreach (var aiThingPath in allAIThingPaths)
            {
                MyHub.EventHub.Subscribe(new EventFilter(aiThingPath, "StateChanged"), stateChangedhandler);
                MyHub.EventHub.Subscribe(new EventFilter(aiThingPath, "AITaskFinished"), aiTaskFinishedhandler);
            }

            //第一次无需判断（默认认为程序启动时所有AIThing都是idle状态）
            //直接arm所有需要自动arm的AIThing
            StartArmTask();
        }

        object stateChangedLockObject = new object();

        private void stateChangedhandler(EventArg e)
        {
            lock (stateChangedLockObject)
            {
                string aiThing = e.Source;
                if (allAIThingStates.ContainsKey(aiThing))
                {
                    Status state = (Status)e.Sample.ObjectVal;
                    if (!allAIThingStates[aiThing].Equals(state))
                    {
                        allAIThingStates[aiThing] = state;
                        attemptToChangeAllAIState();
                    }
                }
            }
        }

        //当所有卡状态都变为一致时，AllAIState为该状态，否则AllAIState为Chaos
        //当所有卡状态都变为Idle时，arm所有需要自动arm的AIThing
        private void attemptToChangeAllAIState()
        {
            var tempState = allAIThingStates.Values.FirstOrDefault();
            foreach (var state in allAIThingStates.Values)
            {
                if (state != tempState)
                {
                    if (!(AllAIStateNow == AllAIStatus.Chaos))
                    {
                        AllAIStatePrevious = AllAIStateNow;
                        AllAIStateNow = AllAIStatus.Chaos;
                    }
                    return;
                }
            }
            //循环完还没return说明所有卡状态一致
            AllAIStatePrevious = AllAIStateNow;
            //tempState是AllAIStatus的子集，可以强制转换
            AllAIStateNow = (AllAIStatus)tempState;
            //当所有卡状态都变为Idle时，arm所有需要自动arm的AIThing
            if (AllAIStateNow == AllAIStatus.AllIdle)
            {
                Thread.Sleep(config.DelaySecondAfterFinish * 1000);
                StartArmTask();
            }
        }

        object aiTaskFinishedLockObject = new object();

        //是否所有aiTask都结束了，如果都结束，则将allAIFinishFlags重置，并产生AllAITaskFinished事件
        private void aiTaskFinishedhandler(EventArg e)
        {
            lock (aiTaskFinishedLockObject)
            {
                string aiThing = e.Source;
                if (allAIFinishFlags.ContainsKey(aiThing))
                {
                    allAIFinishFlags[aiThing] = true;
                    //判断是否所有aiTask都结束
                    foreach (var flag in allAIFinishFlags)
                    {
                        if (flag.Value == false && (bool)MyHub.TryGetResourceSampleWithUri(flag.Key + @"/ison").ObjectVal)
                        {
                            return;
                        }
                    }
                    var keys = allAIFinishFlags.Keys.ToArray();
                    //如果都结束，则将allAIFinishFlags重置
                    for (int i = 0; i < keys.Count(); i++)
                    {
                        allAIFinishFlags[keys[i]] = false;
                    }
                    //产生AllAITaskFinished事件
                    MyHub.EventHub.Publish(Path, "AllAITaskFinished", "AllAITaskFinished");
                    logger.Info("All AI task finished event fired!");
                    System.Diagnostics.Debug.WriteLine("AllAITaskFinished, time: " + DateTime.Now.ToLocalTime().ToString("HH:mm:ss.fff"));
                }
            }
        }

        private void StartArmTask()
        {
            Task.Run(() => MonitorAndArm());
        }

        //按倒序arm所有需要自动arm的AIThing
        //如果使用了带参构造函数，则增加判断
        private void MonitorAndArm()
        {
            if(config.MonitorSource != null)
            {
                while(true)
                {
                    object val = MyHub.TryGetResourceSampleWithUri(config.MonitorSource).ObjectVal;
                    if (config.IsEqualToArm)
                    {
                        if(val.ToString() == config.MonitorValue.ToString())
                        {
                            break;
                        }
                    }
                    else
                    {
                        if (val.ToString() != config.MonitorValue.ToString())
                        {
                            break;
                        }
                    }
                    Thread.Sleep(800);
                }
            }

            for (int i = config.AIThings.AutoArmAIThingPaths.Count() - 1; i >= 0; i--)
            {
                System.Diagnostics.Debug.WriteLine("Auto arm " + config.AIThings.AutoArmAIThingPaths[i]);
                MyHub.TryInvokeSampleResourceWithUri(config.AIThings.AutoArmAIThingPaths[i] + @"/tryarm");
            }
        }


        #region IDisposable Support
        private bool disposedValue = false; // 要检测冗余调用

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: 释放托管状态(托管对象)。
                    for (int i = config.AIThings.AutoArmAIThingPaths.Count() - 1; i >= 0; i--)
                    {
                        System.Diagnostics.Debug.WriteLine("Disposing " + config.AIThings.AutoArmAIThingPaths[i]);
                        MyHub.TryInvokeSampleResourceWithUri(config.AIThings.AutoArmAIThingPaths[i] + @"/trystop");
                    }
                }

                // TODO: 释放未托管的资源(未托管的对象)并在以下内容中替代终结器。
                // TODO: 将大型字段设置为 null。

                disposedValue = true;
            }
        }

        // TODO: 仅当以上 Dispose(bool disposing) 拥有用于释放未托管资源的代码时才替代终结器。
        // ~AIManagementThing() {
        //   // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
        //   Dispose(false);
        // }

        // 添加此代码以正确实现可处置模式。
        void IDisposable.Dispose()
        {
            // 请勿更改此代码。将清理代码放入以上 Dispose(bool disposing) 中。
            Dispose(true);
            // TODO: 如果在以上内容中替代了终结器，则取消注释以下行。
            // GC.SuppressFinalize(this);
        }
        #endregion
    }
}
