﻿//
// The Visual HEIFLOW License
//
// Copyright (c) 2015-2018 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//
// Note: only part of the files distributed in the software belong to the Visual HEIFLOW. 
// The software also contains contributed files, which may have their own copyright notices.
//  If not, the GNU General Public License holds for them, too, but so that the author(s) 
// of the file have the Copyright.

using DotSpatial.Data;
using Heiflow.Models.Generic;
using Heiflow.Models.Generic.Attributes;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Models.Generic.Parameters
{
    public interface IPackageCoverage
    {
        System.Collections.Generic.List<ArealPropertyInfo> ArealProperties { get; set; }
        string PackageName { get; set; }
        IPackage Package { get; set; }
        int GridLayer { get; set; }
        DataTable LookupTable { get; set; }
        IFeatureSet TargetFeatureSet { get; set; }
        /// <summary>
        /// Full file name with path, extension of the coverage layer
        /// </summary>
        string FullCoverageFileName { get; }
        /// <summary>
        /// Relative file name of the coverage layer that will be saved in the vhfx project
        /// </summary>
        string CoverageFilePath { get; set; }
         string FullLookupTableFileName { get;  }
         string LookupTableFilePath { get; set; }
         string LegendText { get; set; }
         string ID { get; set; }
         ModelObjectState State { get; set; }
        void SaveLookupTable();
        void Clear();
    }
}