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

using DotSpatial.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Heiflow.Core.Data
{
    public  class RasterEx
    {
        public static ISet<double> GetUniqueValues(IRaster raster, int maxCount, out bool overMaxCount)
        {
            overMaxCount = false;
            var result = new HashSet<double>();

            var totalPossibleCount = int.MaxValue;

            // Optimization for integer types
            if (raster.DataType == typeof(byte) ||
                raster.DataType == typeof(int) ||
                raster.DataType == typeof(sbyte) ||
                raster.DataType == typeof(uint) ||
                raster.DataType == typeof(short) ||
                raster.DataType == typeof(ushort))
            {
                totalPossibleCount = (int)(raster.Maximum - raster.Minimum + 1);
            }

            // NumRows and NumColumns - virtual properties, so copy them local variables for faster access
            var numRows = raster.NumRows;
            var numCols = raster.NumColumns;
            var valueGrid = raster.Value;

            for (var row = 0; row < numRows; row++)
                for (var col = 0; col < numCols; col++)
                {
                    double val = valueGrid[row, col];
                    if (val != raster.NoDataValue)
                    {
                        if (result.Add(val))
                        {
                            if (result.Count > maxCount)
                            {
                                overMaxCount = true;
                                goto fin;
                            }
                            if (result.Count == totalPossibleCount)
                                goto fin;
                        }
                    }
                }
        fin:
            return result;
        }
    }
}
