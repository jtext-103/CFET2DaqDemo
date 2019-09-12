using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.DAQAIThing
{
    public class AIManagementConfig
    {
        public AllAndAutoArmAIThings AIThings { get; set; }

        public string MonitorSource { get; set; }

        public object MonitorValue { get; set; }

        public bool IsEqualToArm { get; set; }

        public int DelaySecondAfterFinish { get; set; }

        public AIManagementConfig(string path)
        {
            //默认延迟2秒
            DelaySecondAfterFinish = 2;
            JsonConvert.PopulateObject(File.ReadAllText(path, Encoding.Default), this);
        }
    }
}
