using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.BasicAIModel
{
    public class DataAndConfigPath
    {
        /// <summary>
        /// 存放数据文件夹的父文件夹地址
        /// </summary>
        public string DataFileParentDirectory { get; set; }

        /// <summary>
        /// 配置文件完整路径
        /// </summary>
        public string ConfigFilePath { get; set; }

        public DataAndConfigPath(string dataFileParentDirectory, string configFilePath)
        {
            DataFileParentDirectory = dataFileParentDirectory;
            ConfigFilePath = configFilePath;
        }
    }
}
