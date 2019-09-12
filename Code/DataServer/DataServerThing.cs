using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Attributes;
using JTextDAQDataFileOperator.Interface;
using Newtonsoft.Json;

namespace DataServer
{
    public class DataServerThing : Thing
    {
        public IDataFileFactoty dataFileFactoty;
        public BasePathConfig myConfig;

        public DataServerThing(string configPath)
        {
            myConfig = new BasePathConfig(configPath);
        }

        public IDataReader GetDataReader(string dataFilePath, out int nChannel)
        {
            ParamsRebuilder filepathParse = new ParamsRebuilder();
            if (dataFilePath.Substring(0, 1) == System.IO.Path.DirectorySeparatorChar.ToString()) dataFilePath = dataFilePath.Substring(1);
            dataFilePath = myConfig.BasePath + dataFilePath;
            string dataFileDirectory = null;
            string dataFileName = null;
            filepathParse.SetParams(dataFilePath, ref dataFileDirectory, ref dataFileName, out nChannel);
            return dataFileFactoty.GetReader(dataFileDirectory, dataFileName);
        }

        /// <summary>
        /// 根据起始点、结束点以及模糊步长获取数据
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="stepD"></param>
        /// <returns></returns>
        [Cfet2Status]
        public double[] DataByStepD(string dataFilePath, int start, int end, double stepD)
        {
            if (end > Length(dataFilePath) || start < 0) return null;
            int stride = (int)Math.Ceiling(stepD);
            int count = (end - start) / stride;
            int nChannel;
            return GetDataReader(dataFilePath, out nChannel).LoadDataFromFileComplexRam(nChannel, (ulong)start, (ulong)stride, (ulong)count, 1);
        }

        /// <summary>
        /// 获取详细取样的数据，如果正确结果数据长度应该可以准确计算
        /// </summary>
        /// <param name="dataFilePath">通道路径</param>
        /// <param name="start">开始点</param>
        /// <param name="stride">步长</param>
        /// <param name="count">取样次数</param>
        /// <param name="block">每次取样连续长度</param>
        /// <returns></returns>
        [Cfet2Status]
        public double[] DataComplex(string dataFilePath, ulong start, ulong stride, ulong count, ulong block = 1)
        {
            int nChannel;
            return GetDataReader(dataFilePath, out nChannel).LoadDataFromFileComplexRam(nChannel, start, stride, count, block);
        }

        /// <summary>
        /// DataComplex的时间轴
        /// </summary>
        /// <returns></returns>
        [Cfet2Status]
        public double[] DataComplexTimeAxis(string dataFilePath, ulong start, ulong stride, ulong count, ulong block = 1)
        {
            int nChannel;
            return GetDataReader(dataFilePath, out nChannel).LoadDataFromFileComplexRam_TimeAxis(nChannel, start, stride, count, block);
        }

        /// <summary>
        /// 获取连续数据，如果正确结果数据长度应该可以准确计算
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <param name="start">开始点</param>
        /// <param name="length">长度</param>
        /// <returns></returns>
        [Cfet2Status]
        public double[] Data(string dataFilePath, ulong start = 0, ulong length = 0)
        {
            int nChannel;
            return GetDataReader(dataFilePath, out nChannel).LoadDataFromFile(nChannel, start, length);
        }

        /// <summary>
        /// Data的时间轴
        /// </summary>
        /// <returns></returns>
        [Cfet2Status]
        public double[] DataTimeAxis(string dataFilePath, ulong start = 0, ulong length = 0)
        {
            int nChannel;
            return GetDataReader(dataFilePath, out nChannel).LoadDataFromFile_TimeAxis(nChannel, start, length);
        }

        /// <summary>
        /// 根据时间获取数据
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="stride">步长</param>
        /// <returns></returns>
        [Cfet2Status]
        public double[] DataByTime(string dataFilePath, double startTime, double endTime, ulong stride)
        {
            int nChannel;
            return GetDataReader(dataFilePath, out nChannel).LoadDataFromFileByTime(nChannel, startTime, endTime, stride);
        }

        /// <summary>
        /// DataByTime的时间轴
        /// </summary>
        /// <returns></returns>
        [Cfet2Status]
        public double[] DataByTimeTimeAxis(string dataFilePath, double startTime, double endTime, ulong stride)
        {
            int nChannel;
            return GetDataReader(dataFilePath, out nChannel).LoadDataFromFileByTime_TimeAxis(nChannel, startTime, endTime, stride);
        }

        /// <summary>
        /// 根据时间获取数据，长度大于等于count
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <param name="startTime">开始时间</param>
        /// <param name="endTime">结束时间</param>
        /// <param name="count">预期想要得到的数据量</param>
        /// <returns></returns>
        [Cfet2Status]
        public double[] DataByTimeFuzzy(string dataFilePath, double startTime, double endTime, ulong count)
        {
            int nChannel;
            return GetDataReader(dataFilePath, out nChannel).LoadDataFromFileByTimeFuzzy(nChannel, startTime, endTime, count);
        }

        /// <summary>
        /// DataByTimeFuzzy的时间轴
        /// </summary>
        /// <returns></returns>
        [Cfet2Status]
        public double[] DataByTimeFuzzyTimeAxis(string dataFilePath, double startTime, double endTime, ulong count)
        {
            int nChannel;
            return GetDataReader(dataFilePath, out nChannel).LoadDataFromFileByTimeFuzzy_TimeAxis(nChannel, startTime, endTime, count);
        }

        /// <summary>
        /// 总数据长度
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        [Cfet2Status]
        public int Length(string dataFilePath)
        {
            int nChannel;
            return GetDataReader(dataFilePath, out nChannel).SampleCount();
        }

        /// <summary>
        /// 采样率
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        [Cfet2Status]
        public double SampleRate(string dataFilePath)
        {
            int nChannel;
            return GetDataReader(dataFilePath, out nChannel).SampleRate();
        }

        /// <summary>
        /// 采集开始时间
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        [Cfet2Status]
        public double StartTime(string dataFilePath)
        {
            int nChannel;
            return GetDataReader(dataFilePath, out nChannel).StartTime();
        }

        /// <summary>
        /// 采集文件创建时间
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        [Cfet2Status]
        public double CreateTime(string dataFilePath)
        {
            int nChannel;
            return GetDataReader(dataFilePath, out nChannel).CreateTime();
        }

        /// <summary>
        /// 将所有metadata序列化成JSON
        /// </summary>
        /// <param name="dataFilePath"></param>
        /// <returns></returns>
        [Cfet2Status]
        public string MetadataJson(string dataFilePath)
        {
            int nChannel;
            return JsonConvert.SerializeObject(GetDataReader(dataFilePath, out nChannel).Metadata(), Formatting.Indented);
        }

        /// <summary>
        /// 采集文件根目录
        /// </summary>
        /// <returns></returns>
        [Cfet2Status]
        public string BasePath()
        {
            return myConfig.BasePath;
        }
    }
}
