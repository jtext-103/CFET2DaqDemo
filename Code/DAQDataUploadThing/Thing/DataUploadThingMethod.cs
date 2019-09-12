using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Attributes;

namespace Jtext103.CFET2.Things.DAQDataUploadThing
{
    public partial class DataUpLoadThing : Thing
    {
        /// <summary>
        /// 上传所有路径下的数据，如果任务不能开始返回-1，成功开始返回0
        /// </summary>
        /// <param name="localPath">源</param>
        /// <param name="serverPath">目标</param>
        /// <returns></returns>
        [Cfet2Method]
        public int UploadAll()
        {
            lock (myStateLock)
            {
                if (State != Status.Idle)
                {
                    return -1;
                }
                State = Status.Running;

                Task.Run(() => Upload(myConfig.ServerDataDirectories, myConfig.AIThings));
                return 0;
            }
        }

        // 最基础的上传的方法，将本地路径中的所有文件复制到服务器路径下
        private void Upload(string[] serverDirectories, string[] aIThings)
        {
            for (int i = 0; i < LocalDataFileNames.Count; i++)
            {
                string realServerDirectory = SetServerFileDirectory(serverDirectories[i]);

                switch (myConfig.UploadBehavior)
                {
                    case Behavior.KeepOriginal:
                        {

                        }
                        try
                        {
                            foreach(var p in LocalDataFileNames[i])
                            {
                                File.Copy(p, realServerDirectory + GetServerFilename(p));
                            }
                            logger.Info("KeepOriginal上传成功");
                        }
                        catch (Exception e)
                        {
                            logger.Error("不覆盖拷贝错误! Error Message : {0}" + e.ToString());
                            Console.WriteLine("不覆盖拷贝错误! Error Message : {0}", e);
                        }
                        break;
                    case Behavior.RenameOriginal:
                        try
                        {
                            foreach (var p in LocalDataFileNames[i])
                            {
                                FileOperator.RenameExistedFile(GetServerFilename(p), realServerDirectory);
                                File.Copy(p, realServerDirectory + GetServerFilename(p));
                            }
                            logger.Info("RenameOriginal上传成功");
                        }
                        catch (Exception e)
                        {
                            logger.Error("重命名原文件拷贝错误! Error Message : {0}" + e.ToString());
                            Console.WriteLine("重命名原文件拷贝错误! Error Message : {0}", e);
                        }
                        break;
                    case Behavior.Overwrite:
                        try
                        {
                            foreach (var p in LocalDataFileNames[i])
                            {
                                if (File.Exists(realServerDirectory + GetServerFilename(p)))
                                {
                                    logger.Info("正覆盖原有数据文件:" + GetServerFilename(p));
                                    Console.WriteLine("警告！正覆盖原有数据文件！");
                                }
                                File.Copy(p, realServerDirectory + GetServerFilename(p), true);
                            }
                            logger.Info("Overwrite上传成功");
                        }
                        catch (Exception e)
                        {
                            logger.Error("覆盖拷贝错误! Error Message : {0}" + e.ToString());
                            Console.WriteLine("覆盖拷贝错误! Error Message : {0}", e);
                        }
                        break;
                    default:
                        throw new Exception("UploadBehavior设置错误！");
                }
            }
            //以免上传太快捕获不了 Uploading 状态
            Thread.Sleep(2000);
            //上传完毕
            lock (myStateLock)
            {
                State = Status.Idle;
            }
        }

        //通过本地采集电脑上配置的ServerDirectories和ShotServer获取最后要上传到Server上的文件夹路径，不带文件名
        //如果Server上的文件夹路径不存在，则在这里自动创建
        private string SetServerFileDirectory(string serverParentDirectory)
        {
            //int shotNo = 233333333;
            string shotNoS = (string)MyHub.TryGetResourceSampleWithUri(myConfig.ShotNoSource).ObjectVal;
            int shotNo;
            try
            {
                shotNo = int.Parse(shotNoS);
            }
            catch
            {
                shotNo = 0;
            }
            

            string nowDirectory = serverParentDirectory;
            string newDir;

            //每100分一级文件夹，最多分两级
            int firstLevel = shotNo / 10000;
            if (firstLevel >= 1)
            {
                newDir = nowDirectory + firstLevel + "xxxx";
                if (!Directory.Exists(newDir))
                {
                    Directory.CreateDirectory(newDir);
                }
                nowDirectory = newDir + @"\";
            }

            int secondLevel = shotNo / 100;
            if (secondLevel >= 1)
            {
                newDir = nowDirectory + secondLevel + "xx";
            }
            else
            {
                newDir = nowDirectory + "xx";
            }
            if (!Directory.Exists(newDir))
            {
                Directory.CreateDirectory(newDir);
            }
            nowDirectory = newDir + @"\";
            return nowDirectory;
        }

        //通过ShotServerThing获得最后要存到Server上的文件名，不带路径，带后缀
        private string GetServerFilename(string localName)
        {
            //int shotNo = 233333333;
            string shotNoS = (string)MyHub.TryGetResourceSampleWithUri(myConfig.ShotNoSource).ObjectVal;
            int shotNo;
            try
            {
                shotNo = int.Parse(shotNoS);
            }
            catch
            {
                shotNo = 0;
            }
            return shotNo.ToString() + localName.Substring(localName.LastIndexOf('.'), localName.Length - localName.LastIndexOf('.'));
        }
    }
}
