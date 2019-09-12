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
        public IBasicAI basicAI;

        private ICfet2Logger logger;

        ///// <summary>
        ///// 每次程序运行起来后，配置文件不能改变，用一个字段保存
        ///// </summary>
        //private BasicAIStaticConfig staticConfig;

        // 以炮号为key，存储需要的config配置文件，但是只有保存到文件中的部分信息
        // cfet status及部分字段均从该config中读，免得老是要从文件中读                                                                                                                                                                                                                                                                         g中读取
        private Dictionary<int, BasicAIStaticConfig> staticConfigDic;

        //用于存文件
        public IDataFileFactoty DataFileFactory;
        private IDataWriter dataWriter;
        private IDataReader dataReader;

        private string dataFileName;

        // 暂存最新数据（每个通道一个点）
        // 每次arm的时候初始化
        private double[] latestData;

        private Status _aiState;

        //用来计算当前状态持续时间
        long lastTicks;

        /// <summary>
        /// 需要在外层手动 new 对应AI实例再传进来
        /// </summary>
        public AIThing()
        {
            _aiState = Status.Idle;
            lastTicks = DateTime.Now.Ticks;
            staticConfigDic = new Dictionary<int, BasicAIStaticConfig>();
            dataFileName = "data";
        }

        public override void TryInit(object initObj)
        {
            var dataAndConfigPath = (DataAndConfigPath)initObj.TryConvertTo(typeof(DataAndConfigPath));
            DataFileParentDirectory = dataAndConfigPath.DataFileParentDirectory;

            //get a logger
            logger = Cfet2LogManager.GetLogger("Event-" + DataFileParentDirectory);

            basicAI.InitAI(dataAndConfigPath.ConfigFilePath);

            //订阅AI的event
            subscribeEvent();
        }

        public override void Start() { }

        /// <summary>
        /// 更新staticConfigDic，并返回指定炮号的配置文件
        /// 如果给定炮号已经在staticConfigDic中，则直接返回
        /// 否则将该炮的配置文件加入staticConfigDic中
        /// 如果炮号有误，将抛出异常
        /// </summary>
        /// <param name="shotNo"></param>
        private BasicAIStaticConfig tryUpdateStaticConfigDic(int shotNo)
        {
            if (shotNo < 0)
            {
                throw new Exception("从文件中读取数据时，炮号应为正数！");
            }
            int actualShotNo;
            if (shotNo == 0)
            {
                actualShotNo = CurrentShotNo;
            }
            else
            {
                actualShotNo = shotNo;
            }

            try
            {
                if (!staticConfigDic.ContainsKey(actualShotNo))
                {
                    string fileDirectory = DataFileParentDirectory + "\\" + actualShotNo.ToString() + "\\";
                    dataReader = DataFileFactory.GetReader(fileDirectory, dataFileName);
                    BasicAIStaticConfig temp = basicAI.LoadStaticConfig(null);

                    //添加 Status 中暴露的参数
                    temp.ChannelCount = dataReader.ChannelCount();
                    temp.ClockConfig.SampleRate = dataReader.SampleRate();
                    temp.ClockConfig.TotalSampleLengthPerChannel = dataReader.SampleCount();

                    staticConfigDic.Add(actualShotNo, temp);
                }
                return staticConfigDic[actualShotNo];
            }
            catch
            {
                throw new Exception("无法找到" + actualShotNo.ToString() + "的数据文件！可能未采集完全或已经删除。");
            }
        }

        private void subscribeEvent()
        {
            basicAI.RaiseDataArrivalEvent += DataArrivalHandler;
            basicAI.RaiseStatusChangeEvent += StatusChangedHandler;
            basicAI.RaiseAITaskStopEvent += AIStopHandler;
        }

        /// <summary>
        /// 数据到达事件处理函数
        /// 1.将数据传到dataWriter中为写文件做准备
        /// 2.更新最新数据latestData
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataArrivalHandler(object sender, DataArrivalEventArgs e)
        {
            //sender其实就是ai
            IBasicAI thisAI = (IBasicAI)sender;
            double[,] newData = e.NewData;
            //只有AutoWriteDataToFile == true，才存文件
            if (thisAI.StaticConfig.AutoWriteDataToFile)
            {
                //传数据到dataWriter
                dataWriter.AcceptNewData(newData);
            }

            //更新最新数据latestData
            int latestdataIndex;
            //如果数据格式为“data[通道个数][每通道数据个数]”则不需要转置；
            //如果数据格式为“data[每通道数据个数][通道个数]”则需要转置
            if (basicAI.DataNeedTransposeWhenSaving)
            {
                latestdataIndex = newData.GetUpperBound(0);
                for (int i = 0; i < ChannelCount(-1); i++)
                {
                    latestData[i] = newData[latestdataIndex, i];
                }
            }
            else
            {
                latestdataIndex = newData.GetUpperBound(1);
                for (int i = 0; i < ChannelCount(-1); i++)
                {
                    latestData[i] = newData[i, latestdataIndex];
                }
            }
        }

        /// <summary>
        /// AI任务状态变化事件处理
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StatusChangedHandler(object sender, EventArgs e)
        {
            //sender其实就是ai
            IBasicAI thisAI = (IBasicAI)sender;
            switch (thisAI.AIState)
            {
                case Status.Idle:
                    //自动删除多余数据
                    try
                    {
                        ShotDirOperator.DeleteRedundantDataDirectories(
                            DataFileParentDirectory, basicAI.StaticConfig.RemainShotsMax - basicAI.StaticConfig.RemainShotsMin, basicAI.StaticConfig.RemainShotsMin);
                    }
                    catch (Exception)
                    {
                        //删不掉就不删
                    }
                    AIState = Status.Idle;
                    break;
                case Status.Ready:
                    //只有AutoWriteDataToFile == true，才创建下一个文件夹
                    if (thisAI.StaticConfig.AutoWriteDataToFile)
                    {
                        //创建下一个文件夹
                        CurrentFileDirectiory = ShotDirOperator.CreateNextDirectory(DataFileParentDirectory);
                        //new DataWriter，缓存当前炮数据及将数据写入文件
                        string dataDirectory = CurrentFileDirectiory + @"\";
                        dataWriter = DataFileFactory.GetWriter(dataDirectory, dataFileName, ChannelCount(-1), SampleRate(-1), basicAI.DataNeedTransposeWhenSaving, basicAI.StaticConfig.StartTime);
                        FullDataFilePaths = dataWriter.FullPathOfAllDataFiles();
                    }
                    AIState = Status.Ready;
                    break;
                case Status.Running:
                    AIState = Status.Running;
                    break;
                case Status.Error:
                    thisAI.TryStopTask();
                    AIState = Status.Error;
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// AI任务结束
        /// 保存时间文件及配置文件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AIStopHandler(object sender, EventArgs e)
        {
            //sender其实就是ai
            IBasicAI thisAI = (IBasicAI)sender;

            System.Diagnostics.Debug.WriteLine("AIThing stop! Thread no: " + Thread.CurrentThread.ManagedThreadId.ToString() + " " + Path);

            //只有AutoWriteDataToFile == true，才写文件
            if (thisAI.StaticConfig.AutoWriteDataToFile)
            {
                //不再读数据，把数据文件写完
                dataWriter.FinishWrite();
            }

            //停止任务
            TryStop();

            //产生AITaskFinished事件
            MyHub.EventHub.Publish(Path, "AITaskFinished", "AITaskFinished");
        }
    }
}
