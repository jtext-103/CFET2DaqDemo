using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Jtext103.CFET2.Things.EPCISClient
{
    public class EPCISConfig
    {
        public string[] PvNames { get; set; }

        public EPCISConfig(string configFilePath)
        {
            JsonConvert.PopulateObject(File.ReadAllText(configFilePath, Encoding.Default), this);
        }
    }
}
