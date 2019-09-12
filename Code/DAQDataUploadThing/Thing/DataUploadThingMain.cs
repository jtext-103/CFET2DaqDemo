using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Attributes;
using Jtext103.CFET2.Core.Event;
using Jtext103.CFET2.Core.Log;

namespace Jtext103.CFET2.Things.DAQDataUploadThing
{
    /// <summary>
    /// 将本地文件上传到服务器的Thing，用的方法就是简单的文件拷贝
    /// </summary>
    public partial class DataUpLoadThing : Thing
    {
        private UploadConfig myConfig;

        private ICfet2Logger logger;

        //事件成员
        private Token token;

        //锁，保证只有一个上传方法被调用
        object myStateLock = new object();

        public override void TryInit(object configFilePath)
        {
            myConfig = new UploadConfig((string[])configFilePath);
            if (myConfig != null)
            {
                State = Status.Idle;
            }
            else
            {
                throw new Exception("上传配置文件错误!");
            }
            logger = Cfet2LogManager.GetLogger("UploadLog");
        }

        public override void Start()
        {
            //订阅 CFET2 事件
            for (int i = 0; i < myConfig.EventPaths.Count(); i++)
            {
                token = MyHub.EventHub.Subscribe(new EventFilter(myConfig.EventPaths[i], myConfig.EventKinds[i]), DAQCompleteEventhandler);
            }
        }

        private void DAQCompleteEventhandler(EventArg e)
        {
            //这里也要锁住，防止意外
            lock (myStateLock)
            {
                if (State == Status.Idle)
                {
                    LocalDataFileNames = new List<string[]>();
                    //设置要上传的文件名
                    for (int i = 0; i < myConfig.AIThings.Length; i++)
                    {
                        LocalDataFileNames.Add((string[])MyHub.TryGetResourceSampleWithUri(myConfig.AIThings[i] + myConfig.StatusOfAIThing).ObjectVal);
                    }

                    //开始上传   
                    UploadAll();
                }
            }
        }
    }
}
