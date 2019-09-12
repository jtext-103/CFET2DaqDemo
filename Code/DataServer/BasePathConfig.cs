using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.IO;

namespace DataServer
{
    public class BasePathConfig
    {
        public string BasePath { get; set; }

        public BasePathConfig(string configFilePath)
        {
            JsonConvert.PopulateObject(File.ReadAllText(configFilePath, Encoding.Default), this);
        }
    }
}
