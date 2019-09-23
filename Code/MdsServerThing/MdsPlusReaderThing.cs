using Jtext103.CFET2.Core;
using Jtext103.CFET2.Core.Attributes;
using Jtext103.CFET2.Core.Extension;
using System;
using System.Collections.Generic;
using Jtext103.CFET2.Core.Log;
using System.Threading;
using MDSplusVBC;

namespace MdsServerThing
{
    public class MdsPlusReaderThing : Thing
    {
        private MdsConfig myConfig;
        private MDSplus mds;

        [Cfet2Status]
        public double[] Data(string tagName, int shotNo)
        {
            return read(tagName, shotNo, false);
        }

        [Cfet2Status]
        public double[] DataTimeAxis(string tagName, int shotNo)
        {
            return read(tagName, shotNo, true);
        }

        private double[] read(string tagName, int shotNo, bool isTimeAxis)
        {
            try
            {
                mds.Connect(myConfig.Host);
                mds.MdsOpen(myConfig.Tree, shotNo);
            }
            catch
            {
                throw new Exception("Failed to connect to Mds Server : " + myConfig.Host + "," + myConfig.Tree + "," + shotNo);
            }

            int status = 0;
            double[] result;
            try
            {
                if(isTimeAxis)
                {
                    result = (double[])mds.MdsValue(@"DIM_OF(BUILD_PATH(\" + tagName + "))", ref status);
                }
                else
                {
                    result = (double[])mds.MdsValue(@"\" + tagName, ref status);
                }    
            }
            catch
            {
                mds.DisConnect();
                throw new Exception("Failed to read tagName : " + tagName);
            }

            mds.DisConnect();
            return result;
        }

        public override void TryInit(object configFilePath)
        {
            myConfig = new MdsConfig((string)configFilePath);
            mds = new MDSplus();
        }
    }
}
