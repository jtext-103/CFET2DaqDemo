using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.DAQDataUploadThing
{
    public class UploadConfig
    {
        /// <summary>
        /// 监听的AIThings
        /// </summary>
        public string[] AIThings { get; set; }

        /// <summary>
        /// 本地要上传的所有数据文件夹路径
        /// </summary>
        public string StatusOfAIThing { get; set; }

        /// <summary>
        /// 上传到的服务器的路径，需要跟 Local 的一一对应
        /// </summary>
        public string[] ServerDataDirectories { get; set; }

        /// <summary>
        /// 以上3个数组的长度，要求是一样的
        /// </summary>
        public int PathCount { get; set; }

        /// <summary>
        /// 上传炮号从 CFET 的获取路径
        /// </summary>
        public string ShotNoSource { get; set; }

        /// <summary>
        /// 监听的事件路径
        /// </summary>
        public string[] EventPaths { get; set; }

        /// <summary>
        /// 监听的事件类型，需要和上面一一匹配
        /// </summary>
        public string[] EventKinds { get; set; }

        /// <summary>
        /// 上传设置，如果目标下有同名文件，是否重命名原来的文件并拷贝新的，false表示直接放弃本次拷贝
        /// </summary>
        public Behavior UploadBehavior { get; set; }

        public UploadConfig() { }

        /// <summary>
        /// 配置上述参数
        /// </summary>
        /// <param name="path">配置文件(.txt)路径</param>
        public UploadConfig(string[] path)
        {
            //全部 public 参数反序列化
            JsonConvert.PopulateObject(File.ReadAllText(path[0], Encoding.Default), this);

            var config = CSVReader.SetThingsAndDirs(path[1]);

            AIThings = config.AIThings;
            ServerDataDirectories = config.ServerDataDirectories;
            PathCount = AIThings.Length;
            if (ServerDataDirectories.Length != PathCount)
            {
                throw new Exception("本地文件目录数与上传目录数不同!");
            }
        }
    }
}
