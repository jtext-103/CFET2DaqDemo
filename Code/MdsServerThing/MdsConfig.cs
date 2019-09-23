using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MdsServerThing
{
    public class MdsConfig
    {
        public string Host { get; set; }

        public string Tree { get; set; }

        public MdsConfig(string configFilePath)
        {
            JsonConvert.PopulateObject(File.ReadAllText(configFilePath, Encoding.Default), this);
        }
    }
}
