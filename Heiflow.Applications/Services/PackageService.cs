﻿// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
using Heiflow.Models.Generic;
using Heiflow.Models.Subsurface;
using Heiflow.Presentation.Services;
using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Applications.Services
{
    [Export(typeof(IPackageService))]
    public  class PackageService : IPackageService
    {
        public PackageService()
        {

        }

        [ImportMany(typeof(IMFPackage))]
        public  IEnumerable<IMFPackage> SupportedMFPackages
        {
            get;
            set;
        }
    }
}
