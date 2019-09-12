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

namespace Jtext103.CFET2.CFET2App
{
    partial  class Cfet2Program : CFET2Host
    {
        private void AddThings()
        {
            //------------------------------Nancy HTTP通信模块------------------------------//
            var nancyCM = new NancyCommunicationModule(new Uri("http://localhost:8001"));
            MyHub.TryAddCommunicationModule(nancyCM);

            //------------------------------模拟采集卡------------------------------//
            var fakeAI = new FakeAIThing();
            MyHub.TryAddThing(fakeAI, "/", "fakeCard", 16);


            ////------------------------------NI采集卡------------------------------//
            //var niNonSync = new AIThing();
            //niNonSync.basicAI = new NIAI();
            //niNonSync.DataFileFactory = new HDF5DataFileFactory();
            //MyHub.TryAddThing(niNonSync, @"/", "Card0",
            //    new { ConfigFilePath = @"C:\Users\wyx\Desktop\CfetDaqDemo\ConfigFile\niNonSync.json", DataFileParentDirectory = @"D:\Data\ni\Card0" });


            ////------------------------------自动 Arm 采集卡与发布上传事件------------------------------//
            //var aiManagement = new AIManagementThing();
            //MyHub.TryAddThing(aiManagement, @"/", "aimanagement", @"C:\Users\wyx\Desktop\CfetDaqDemo\ConfigFile\AIManagement.json");

            ////------------------------------上传文件的------------------------------//
            //var uploader = new DataUpLoadThing();
            //MyHub.TryAddThing(uploader, @"/", "uploader", new string[] {
            //                    @"C:\Users\wyx\Desktop\CfetDaqDemo\ConfigFile\DataUploadConfig.json",
            //                    @"C:\Users\wyx\Desktop\CfetDaqDemo\ConfigFile\DataUpload.csv"
            //            });

            ////------------------------------上传炮号提供者------------------------------//
            //var dic = new DicServerThing();
            //MyHub.TryAddThing(dic, "/", "dicServer", @"C:\Users\wyx\Desktop\CfetDaqDemo\ConfigFile\Dic.json");

            ////------------------------------数据服务------------------------------//
            //var dataServer = new DataServerThing(@"C:\Users\wyx\Desktop\CfetDaqDemo\ConfigFile\BasePath.json");
            //dataServer.dataFileFactoty = new HDF5DataFileFactory();
            //MyHub.TryAddThing(dataServer, "/", "dataServer");

        }
    }
}
