using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Jtext103.CFET2.Things;
using Jtext103.CSV;

namespace Jtext103.CFET2.Things.DAQDataUploadThing
{
    public class CSVReader
    {
        public static UploadConfig SetThingsAndDirs(string filepath)
        {
            var result = CSVOperator.LoadCSVFile(filepath);

            var config = new UploadConfig();

            config.AIThings = new string[result.Count - 1];
            config.ServerDataDirectories = new string[result.Count - 1];
            for(int i = 1; i < result.Count; i++)
            {
                config.AIThings[i - 1] = result[i][0];
                config.ServerDataDirectories[i - 1] = result[i][1];
            }

            return config;
        }
    }
}
