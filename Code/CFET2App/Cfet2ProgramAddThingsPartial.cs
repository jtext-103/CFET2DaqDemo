using Jtext103.CFET2.CFET2App.ExampleThings;
using Jtext103.CFET2.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Things.DAQAIThing;
using Jtext103.CFET2.Things.NiAiLib;
using Jtext103.CFET2.Things.DAQDataUploadThing;
using JTextDAQDataFileOperator.HDF5;
using Jtext103.CFET2.NancyHttpCommunicationModule;
using Jtext103.CFET2.Things.DicServer;
using DataServer;
using Jtext103.CFET2.Core.BasicThings;
using Jtext103.CFET2.Core.Middleware.Basic;

namespace Jtext103.CFET2.CFET2App
{
    public partial class Cfet2Program : CFET2Host
    {
        private void AddThings()
        {
            //If you don't want dynamic load things, please comment out the line below
            //var loader = new DynamicThingsLoader(this);

            //------------------------------Pipeline------------------------------//
            MyHub.Pipeline.AddMiddleware(new ResourceInfoMidware());
            MyHub.Pipeline.AddMiddleware(new NavigationMidware());


            //------------------------------Nancy HTTP通信模块------------------------------//
            var nancyCM = new NancyCommunicationModule(new Uri("http://localhost:8001"));
            MyHub.TryAddCommunicationModule(nancyCM);

            //you can add Thing by coding here

            //------------------------------Custom View------------------------------//
            var customView = new CustomViewThing();
            MyHub.TryAddThing(customView, "/", "customView", "./CustomView");

            //------------------------------模拟采集卡------------------------------//
            var fakeAI = new FakeAIThing();
            MyHub.TryAddThing(fakeAI, "/", "fakeCard", 16);


            //------------------------------NI采集卡------------------------------//
            var niNonSync = new AIThing();
            niNonSync.basicAI = new NIAI();
            niNonSync.DataFileFactory = new HDF5DataFileFactory();
            MyHub.TryAddThing(niNonSync, @"/", "Card0",
                new { ConfigFilePath = @".\ConfigFile\niNonSync.json", DataFileParentDirectory = @"D:\Data\ni\Card0" });


            ////------------------------------自动 Arm 采集卡与发布上传事件------------------------------//
            //var aiManagement = new AIManagementThing();
            //MyHub.TryAddThing(aiManagement, @"/", "aimanagement", @".\ConfigFile\AIManagement.json");

            ////------------------------------上传文件的------------------------------//
            //var uploader = new DataUpLoadThing();
            //MyHub.TryAddThing(uploader, @"/", "uploader", new string[] {
            //                    @".\ConfigFile\DataUploadConfig.json",
            //                    @".\ConfigFile\DataUpload.csv"
            //            });

            ////------------------------------上传炮号提供者------------------------------//
            //var dic = new DicServerThing();
            //MyHub.TryAddThing(dic, "/", "dicServer", @".\ConfigFile\Dic.json");

            //------------------------------数据服务------------------------------//
            var dataServer = new DataServerThing(@".\ConfigFile\BasePath.json");
            dataServer.dataFileFactoty = new HDF5DataFileFactory();
            MyHub.TryAddThing(dataServer, "/", "dataServer");

        }
    }
}
