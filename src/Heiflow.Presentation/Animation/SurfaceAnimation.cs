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
// Note:  The software also contains contributed files, which may have their own 
// copyright notices. If not, the GNU General Public License holds for them, too, 
// but so that the author(s) of the file have the Copyright.
//

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Heiflow.Models.Subsurface;
using ILNumerics;
using Heiflow.Models.Generic;
using Heiflow.Core.Animation;
using Heiflow.Core.Data;
using Heiflow.Presentation.Controls;
using Heiflow.Models.Generic.Project;

namespace Heiflow.Presentation.Animation
{
    public class SurfaceAnimation : DataCubeAnimation
    {
        private ISurfacePlotView _Plot;

        public SurfaceAnimation( )
        {
            _Name = "3D Animation";
        }


        public ISurfacePlotView SurfacePlot
        {
            get
            {
                return _Plot;
            }
            set
            {
                _Plot = value;
            }
        }

        protected override void Plot(int time_index)
        {
            var mat = _DataSource.ToILBaseArray(_DataSource.SelectedVariableIndex, time_index) as ILArray<float>;
            if (mat != null)
            {
                mat.Name = string.Format("{0}[{1}]", _DataSource.Name, _DataSource.Variables[_DataSource.SelectedVariableIndex]);
                _Plot.PlotSurface(mat);
                _Current++;
                if (Current >= Maximum)
                    Pause();
            }
        }
    }
}