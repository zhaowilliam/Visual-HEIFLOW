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

namespace  Heiflow.AI.GeneticProgramming
{
    /// <summary>
    ///GPdotNET 4.0 implements the Relative Squared Error (RSE) fitness. The RSE fitness is
    ///based on the standard relative squared error, which is based on the absolute error.The relative squared 
    ///error is relative to what it would have been if a simple predictor had been used. More specifically, this simple 
    ///predictor is just the average of the actual values. The relative squared error takes the total squared error
    ///and normalizes it by dividing by the total squared error of the simple predictor.
    /// </summary>
    [Serializable]
    public class RSEFitness:IFitnessFunction
    {
        #region IFitnessFunction Members

        public void Evaluate(List<int> lst, GPFunctionSet gpFunctionSet, GPTerminalSet gpTerminalSet, GPChromosome c)
        {
            c.Fitness = 0;
            double rowFitness = 0.0;
            double SS_tot = 0;
            double y;
           

            //Translate chromosome to list expressions
            int indexOutput = gpTerminalSet.NumConstants + gpTerminalSet.NumVariables;
            for (int i = 0; i < gpTerminalSet.RowCount; i++)
            {
                // evalue the function
                y = gpFunctionSet.Evaluate(lst, gpTerminalSet, i);
                // check for correct numeric value
                if (double.IsNaN(y) || double.IsInfinity(y))
                    y = 0;

                //Calculate square error
                rowFitness += System.Math.Pow(y - gpTerminalSet.TrainingData[i][indexOutput], 2);
                SS_tot += System.Math.Pow(gpTerminalSet.TrainingData[i][indexOutput] - gpTerminalSet.AverageValue, 2);
            }

            rowFitness = rowFitness / SS_tot;

            if (double.IsNaN(rowFitness) || double.IsInfinity(rowFitness))
            {
                //if output is not a number return zero fitness
                c.Fitness = 0;
                return;
            }

            //Fitness
            c.Fitness = (float)((1.0 / (1.0 + rowFitness / gpTerminalSet.RowCount)) * 1000.0);
        }
        #endregion
    }
}
