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

namespace  Heiflow.AI.Math.Metrics
{
    using System;

    /// <summary>
    /// Euclidean similarity metric. 
    /// </summary>
    /// 
    /// <remarks><para>This class represents the 
    /// <a href="http://en.wikipedia.org/wiki/Euclidean_distance">Euclidean Similarity metric</a>,
    /// which is calculated as 1.0 / ( 1.0 + EuclideanDistance ).</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // instantiate new similarity class
    /// EuclideanSimilarity sim = new EuclideanSimilarity( ); 
    /// // create two vectors for inputs
    /// double[] p = new double[] { 2.5, 3.5, 3.0, 3.5, 2.5, 3.0 };
    /// double[] q = new double[] { 3.0, 3.5, 1.5, 5.0, 3.5, 3.0 };
    /// // get simirarity between the two vectors
    /// double similarityScore = sim.GetSimilarityScore( p, q );
    /// </code>    
    /// </remarks>
    /// 
    public sealed class EuclideanSimilarity : ISimilarity
    {
        /// <summary>
        /// Returns similarity score for two N-dimensional double vectors. 
        /// </summary>
        /// 
        /// <param name="p">1st point vector.</param>
        /// <param name="q">2nd point vector.</param>
        /// 
        /// <returns>Returns Euclidean similarity between two supplied vectors.</returns>
        /// 
        /// <exception cref="ArgumentException">Thrown if the two vectors are of different dimensions (if specified
        /// array have different length).</exception>
        /// 
        public double GetSimilarityScore( double[] p, double[] q )
        {
            double distance = 0;

            EuclideanDistance dist = new EuclideanDistance( );
            distance = 1.0 / ( 1.0 + dist.GetDistance( p, q ) );

            return distance;
        }
    }
}
