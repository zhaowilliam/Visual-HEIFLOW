// THIS FILE IS PART OF Visual HEIFLOW
// THIS PROGRAM IS NOT FREE SOFTWARE. 
// Copyright (c) 2015-2017 Yong Tian, SUSTech, Shenzhen, China. All rights reserved.
// Email: tiany@sustc.edu.cn
// Web: http://ese.sustc.edu.cn/homepage/index.aspx?lid=100000005794726
namespace  Heiflow.AI.Math.Random
{
    using System;

    /// <summary>
    /// Standard random numbers generator.
    /// </summary>
    /// 
    /// <remarks><para>The random number generator generates gaussian
    /// random numbers with zero mean and standard deviation of one. The generator
    /// implements polar form of the Box-Muller transformation.</para>
    /// 
    /// <para>The generator uses <see cref="UniformOneGenerator"/> generator as a base
    /// to generate random numbers.</para>
    /// 
    /// <para>Sample usage:</para>
    /// <code>
    /// // create instance of random generator
    /// IRandomNumberGenerator generator = new StandardGenerator( );
    /// // generate random number
    /// double randomNumber = generator.Next( );
    /// </code>
    /// </remarks>
    /// 
    public class StandardGenerator : IRandomNumberGenerator
    {
        private UniformOneGenerator rand = null;

        private double  secondValue;
        private bool    useSecond = false;

        /// <summary>
        /// Mean value of the generator.
        /// </summary>
        /// 
        public double Mean
        {
            get { return 0; }
        }

        /// <summary>
        /// Variance value of the generator.
        /// </summary>
        ///
        public double Variance
        {
            get { return 1; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardGenerator"/> class.
        /// </summary>
        /// 
        public StandardGenerator( )
        {
            rand = new UniformOneGenerator( );
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StandardGenerator"/> class.
        /// </summary>
        /// 
        /// <param name="seed">Seed value to initialize random numbers generator.</param>
        /// 
        public StandardGenerator( int seed )
        {
            rand = new UniformOneGenerator( seed );
        }

        /// <summary>
        /// Generate next random number.
        /// </summary>
        /// 
        /// <returns>Returns next random number.</returns>
        /// 
        public double Next( )
        {
            // check if we can use second value
            if ( useSecond )
            {
                // return the second number
                useSecond = false;
                return secondValue;
            }

            double x1, x2, w, firstValue;

            // generate new numbers
            do
            {
                x1  = rand.Next( ) * 2.0 - 1.0;
                x2  = rand.Next( ) * 2.0 - 1.0;
                w   = x1 * x1 + x2 * x2;
            }
            while ( w >= 1.0 );

            w = Math.Sqrt( ( -2.0 * Math.Log( w ) ) / w );

            // get two standard random numbers
            firstValue  = x1 * w;
            secondValue = x2 * w;

            useSecond = true;

            // return the first number
            return firstValue;
        }

        /// <summary>
        /// Set seed of the random numbers generator.
        /// </summary>
        /// 
        /// <param name="seed">Seed value.</param>
        /// 
        /// <remarks>Resets random numbers generator initializing it with
        /// specified seed value.</remarks>
        /// 
        public void SetSeed( int seed )
        {
            rand = new UniformOneGenerator( seed );
            useSecond = false;
        }
    }
}
