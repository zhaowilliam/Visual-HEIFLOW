﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic.Project;
using Heiflow.Models.Integration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Heiflow.Models.Generic.Project
{
    public class OpenHeiflowProjectFileProvider : IOpenProjectFileProvider
    {
        public OpenHeiflowProjectFileProvider()
        {

        }

        public  virtual string FileTypeDescription
        {
            get 
            {
                return "HEIFLOW Project File";
            }
        }

        public virtual string Extension
        {
            get 
            {
                return ".vhfx";
            }
        }


        public string FileName
        {
            get;
            set;
        }


        public string ProviderName
        {
            get { return "HeiflowProject"; }
        }

        public virtual IProject Open(string fileName)
        {
            FileName = fileName;
            XmlSerializer xs = new XmlSerializer(typeof(HeiflowProject));
            Stream stream = new FileStream(fileName, FileMode.Open, FileAccess.ReadWrite, FileShare.Read);
            var project = (HeiflowProject)xs.Deserialize(stream);
            return project;
        }
    }
}
