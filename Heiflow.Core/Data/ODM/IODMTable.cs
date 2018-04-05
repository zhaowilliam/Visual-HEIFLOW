﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using System;
namespace Heiflow.Core.Data.ODM
{
    public interface IODMTable
    {
        event EventHandler<int> ProgressChanged;
        event EventHandler Started;
        event EventHandler Finished;

        string Message { get; }
        string TableName { get; }
        IODMExportSetting ExportSetting { get; }
        bool Check(System.Data.DataTable dt);
        bool Save(System.Data.DataTable dt);
        void Export(string filename, System.Data.DataTable source);
        void CustomExport(System.Data.DataTable source);
    }
}
