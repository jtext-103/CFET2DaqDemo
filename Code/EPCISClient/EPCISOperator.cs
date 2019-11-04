using JtextEpcisPvClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.EPCISClient
{
    public class EPCISOperator
    {      
        JtextEpicsClient configEpcisClient;
        JtextEpicsClient epcisClient;

        bool isConfigOnline;

        public EPCISConfig Config { get; set; }

        public EPCISOperator(string configFilePath)
        {
            Config = new EPCISConfig(configFilePath);  
            try
            {
                configEpcisClient = new JtextEpicsClient(Config.PvNames, null);
                isConfigOnline = true;
            }
            catch
            {
                isConfigOnline = false;
            }
        }

        public object TryGetPV(string pvName)
        {
            if(Config.PvNames.Contains(pvName) && isConfigOnline)
            {
                try
                {
                    return configEpcisClient.GetPV(pvName);
                }
                catch (Exception e)
                {
                    return "Exception! Error Message: "+ e.ToString();
                }
            }
            else if(Config.PvNames.Contains(pvName) && !isConfigOnline)
            {
                try
                {
                    configEpcisClient = new JtextEpicsClient(Config.PvNames, null);
                    isConfigOnline = true;
                    return configEpcisClient.GetPV(pvName);
                }
                catch (Exception e)
                {
                    return "Exception! Error Message: " + e.ToString();
                }
            }
            else
            {
                string[] name = new string[] { pvName };
                try
                {
                    epcisClient = new JtextEpicsClient(name);
                    return epcisClient.GetPV(pvName);
                }
                catch (Exception e)
                {
                    return "Exception! Error Message: " + e.ToString();
                }
            } 
        }

        public int TrySetPV(string pvName, int value)
        {
            if (Config.PvNames.Contains(pvName) && isConfigOnline)
            {
                try
                {
                    configEpcisClient[pvName] = value;
                    return 0;
                }
                catch (Exception e)
                {
                    return -1;
                }
            }
            else if (Config.PvNames.Contains(pvName) && !isConfigOnline)
            {
                try
                {
                    configEpcisClient = new JtextEpicsClient(Config.PvNames, null);
                    configEpcisClient[pvName] = value;
                    isConfigOnline = true;
                    return 0;
                }
                catch (Exception e)
                {
                    return -1;
                }
            }
            else
            {
                string[] name = new string[] { pvName };
                try
                {
                    epcisClient = new JtextEpicsClient(name);
                    epcisClient[pvName] = value;
                    return 0;
                }
                catch
                {
                    return -1;
                }
            }   
        }
    }
}
