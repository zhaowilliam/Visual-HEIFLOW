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
using System.Security.Cryptography;

namespace System.Threading
{
    /// <summary>
    /// Represents a thread-safe, pseudo-random number generator.
    /// </summary>
    public class ThreadSafeRandom : Random
    {
        /// <summary>Seed provider.</summary>
        private static readonly RNGCryptoServiceProvider _global = new RNGCryptoServiceProvider();
        /// <summary>The underlyin provider of randomness, one instance per thread, initialized with _global.</summary>
        private ThreadLocal<Random> _local = new ThreadLocal<Random>(() =>
        {
            byte[] buffer = new byte[4];
            _global.GetBytes(buffer); // RNGCryptoServiceProvider is thread-safe for use in this manner
            return new Random(BitConverter.ToInt32(buffer, 0));
        });

        /// <summary>Returns a nonnegative random number.</summary>
        /// <returns>A 32-bit signed integer greater than or equal to zero and less than MaxValue.</returns>
        public override int Next()
        {
            return _local.Value.Next();
        }

        /// <summary>Returns a nonnegative random number less than the specified maximum.</summary>
        /// <param name="maxValue">
        /// The exclusive upper bound of the random number to be generated. maxValue must be greater than or equal to zero. 
        /// </param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to zero, and less than maxValue; 
        /// that is, the range of return values ordinarily includes zero but not maxValue. However, 
        /// if maxValue equals zero, maxValue is returned.
        /// </returns>
        public override int Next(int maxValue)
        {
            return _local.Value.Next(maxValue);
        }

        /// <summary>Returns a random number within a specified range.</summary>
        /// <param name="minValue">The inclusive lower bound of the random number returned.</param>
        /// <param name="maxValue">The exclusive upper bound of the random number returned. maxValue must be greater than or equal to minValue.</param>
        /// <returns>
        /// A 32-bit signed integer greater than or equal to minValue and less than maxValue; 
        /// that is, the range of return values includes minValue but not maxValue. 
        /// If minValue equals maxValue, minValue is returned.
        /// </returns>
        public override int Next(int minValue, int maxValue)
        {
            return _local.Value.Next(minValue, maxValue);
        }
        /// <summary>Fills the elements of a specified array of bytes with random numbers.</summary>
        /// <param name="buffer">An array of bytes to contain random numbers.</param>
        public override void NextBytes(byte[] buffer)
        {
            _local.Value.NextBytes(buffer);
        }
        /// <summary>Returns a random number between 0.0 and 1.0.</summary>
        /// <returns>A double-precision floating point number greater than or equal to 0.0, and less than 1.0.</returns>
        public override double NextDouble()
        {
            return _local.Value.NextDouble();
        }
        public double NextDouble(double minValue, double maxValue)
        {
            return minValue + _local.Value.NextDouble() * (maxValue - minValue);
        }


    }
}
/*
// Copyright 2006-2009 Bahrudin Hrnjica (bhrnjica@hotmail.com)
// gpNETLib 
using System;
using NPack;
namespace  Heiflow.AI.GeneticProgramming
{
    ///Based on implementation of Random ThreadSafe http://blogs.msdn.com/pfxteam/archive/2009/02/19/9434171.aspx
    /// <summary>
    /// Thread Safe Generator slucajnih brojeva. Obzirom da klasa 
    /// Random nije ThreadSafe potrebno je zakljucati generiranje 
    /// slucajnog broja prije samog generiranja
    /// </summary>
    [Serializable]
    public class ThreadSafeRandom
    {
        private static MersenneTwister _global = new MersenneTwister();
        [ThreadStatic]
        private static MersenneTwister _local; 

        public ThreadSafeRandom()
        {
        }

        public int Next()
        {
            MersenneTwister inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new MersenneTwister(seed);
            }
            return inst.Next(); 
        }
        public int Next(int maxValue)
        {
            MersenneTwister inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new MersenneTwister(seed);
            }
            return inst.Next(maxValue); 
        }
        public int Next(int minValue, int maxValue)
        {
            MersenneTwister inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new MersenneTwister(seed);
            }
            return inst.Next(minValue, maxValue);
        }
        public void NextBytes(byte[] buffer)
        {
            MersenneTwister inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new MersenneTwister(seed);
            }
            inst.NextBytes(buffer);
        }
        public double NextDouble()
        {
            MersenneTwister inst = _local;
            if (inst == null)
            {
                int seed;
                lock (_global) seed = _global.Next();
                _local = inst = new MersenneTwister(seed);
            }
            return inst.NextDouble();
        }

    }
}
*/