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
using System.Runtime.Serialization;
using  Heiflow.AI.NeuronDotNet.Core.LearningRateFunctions;

namespace  Heiflow.AI.NeuronDotNet.Core
{
    /// <summary>
    /// Layer is an abstract container for similar neurons. No two neurons within a layer can be
    /// connected to each other.
    /// </summary>
    /// <typeparam name="TNeuron">Type of Neurons in the layer</typeparam>
    [Serializable]
    public abstract class Layer<TNeuron> : ILayer where TNeuron : INeuron
    {
        /// <summary>
        /// Array of neurons in the layer. This is never <c>null</c>.
        /// </summary>
        protected readonly TNeuron[] neurons;

        /// <summary>
        /// List of source connectors. This read-only value is never <c>null</c>.
        /// </summary>
        protected readonly List<IConnector> sourceConnectors = new List<IConnector>();

        /// <summary>
        /// List of target connectors. This read-only value is never <c>null</c>.
        /// </summary>
        protected readonly List<IConnector> targetConnectors = new List<IConnector>();

        /// <summary>
        /// Learning Rate Function
        /// </summary>
        protected ILearningRateFunction learningRateFunction;

        /// <summary>
        /// Initializer used to initialize the layer. If this value is <c>null</c>, neurons will have
        /// default values for initializable parameters (usually zero)
        /// </summary>
        protected IInitializer initializer = null;

        /// <summary>
        /// Gets the neuron count
        /// </summary>
        /// <value>
        /// Number of neurons in the layer. It is always positive.
        /// </value>
        public int NeuronCount
        {
            get { return neurons.Length; }
        }

        /// <summary>
        /// Exposes an enumerator to iterate over all neurons in the layer
        /// </summary>
        /// <value>
        /// Neurons Enumerator. No neuron enumerated can be <c>null</c>.
        /// </value>
        public IEnumerable<TNeuron> Neurons
        {
            get
            {
                for (int i = 0; i < neurons.Length; i++)
                {
                    yield return neurons[i];
                }
            }
        }

        /// <summary>
        /// Neuron Indexer
        /// </summary>
        /// <param name="index">
        /// Index
        /// </param>
        /// <returns>
        /// Neuron at the given index
        /// </returns>
        /// <exception cref="IndexOutOfRangeException">
        /// If index is out of range
        /// </exception>
        public TNeuron this[int index]
        {
            get { return neurons[index]; }
        }

        IEnumerable<INeuron> ILayer.Neurons
        {
            get
            {
                for (int i = 0; i < neurons.Length; i++)
                {
                    yield return neurons[i];
                }
            }
        }

        INeuron ILayer.this[int index]
        {
            get { return neurons[index]; }
        }

        /// <summary>
        /// Gets the list of source connectors
        /// </summary>
        /// <value>
        /// The list of source connectors associated with this layer. It is neither <c>null</c>,
        /// nor contains <c>null</c> elements.
        /// </value>
        public IList<IConnector> SourceConnectors
        {
            get { return sourceConnectors; }
        }

        /// <summary>
        /// Gets the list of target connectors
        /// </summary>
        /// <value>
        /// The list of target connectors associated with this layer. It is neither <c>null</c>,
        /// nor contains <c>null</c> elements.
        /// </value>
        public IList<IConnector> TargetConnectors
        {
            get { return targetConnectors; }
        }

        /// <summary>
        /// Gets or sets the Initializer used to initialize the layer
        /// </summary>
        /// <value>
        /// Initializer used to initialize the layer. If this value is <c>null</c>, initialization
        /// is NOT performed.
        /// </value>
        public IInitializer Initializer
        {
            get { return initializer; }
            set { initializer = value; }
        }

        /// <summary>
        /// Gets the initial value of learning rate
        /// </summary>
        /// <value>
        /// Initial value of learning rate.
        /// </value>
        public double LearningRate
        {
            get { return learningRateFunction.InitialLearningRate; }
        }

        /// <summary>
        /// Gets the learning rate function
        /// </summary>
        /// <value>
        /// Learning Rate Function used while training. It is never <c>null</c>
        /// </value>
        public ILearningRateFunction LearningRateFunction
        {
            get { return learningRateFunction; }
        }

        /// <summary>
        /// Creates a new layer
        /// </summary>
        /// <param name="neuronCount">
        /// Number of neurons in the layer
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <c>neuronCount</c> is not positive
        /// </exception>
        protected Layer(int neuronCount)
        {
            Helper.ValidatePositive(neuronCount, "neuronCount");
            
            this.neurons = new TNeuron[neuronCount];
            this.learningRateFunction = new LinearFunction(0.3d, 0.05d);
        }

        /// <summary>
        /// Deserialization Constructor
        /// </summary>
        /// <param name="info">
        /// Serialization information to deserialize and obtain the data
        /// </param>
        /// <param name="context">
        /// Serialization context to use
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>info</c> is <c>null</c>
        /// </exception>
        protected Layer(SerializationInfo info, StreamingContext context)
        {
            // Validate
            Helper.ValidateNotNull(info, "info");

            // Deserialize
            int neuronCount = info.GetInt32("neuronCount");
            this.neurons = new TNeuron[neuronCount];

            this.initializer = info.GetValue("initializer", typeof(IInitializer)) as IInitializer;
            this.learningRateFunction = info.GetValue("learningRateFunction", typeof(ILearningRateFunction)) as ILearningRateFunction;
        }

        /// <summary>
        /// Populates the serialization info with the data needed to serialize the layer
        /// </summary>
        /// <param name="info">
        /// The serialization info to populate the data with
        /// </param>
        /// <param name="context">
        /// The serialization context to use
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>info</c> is <c>null</c>
        /// </exception>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            // Validate
            Helper.ValidateNotNull(info, "info");

            // Populate
            info.AddValue("neuronCount", neurons.Length);
            info.AddValue("initializer", initializer, typeof(IInitializer));
            info.AddValue("learningRateFunction", learningRateFunction, typeof(ILearningRateFunction));
        }

        /// <summary>
        /// Sets neuron inputs to the values specified by the given array
        /// </summary>
        /// <param name="input">
        /// the input array
        /// </param>
        /// <exception cref="ArgumentNullException">
        /// If <c>input</c> is <c>null</c>
        /// </exception>
        /// <exception cref="ArgumentException">
        /// If length of <c>input</c> array is different from number of neurons
        /// </exception>
        public virtual void SetInput(double[] input)
        {
            // Validate
            Helper.ValidateNotNull(input, "input");

            if (neurons.Length != input.Length)
            {
                throw new ArgumentException("Length of input array should be same as neuron count", "input");
            }

            // Bind inputs
            for (int i = 0; i < neurons.Length; i++)
            {
                neurons[i].Input = input[i];
            }
        }

        /// <summary>
        /// Gets the neuron outputs as an array
        /// </summary>
        /// <returns>
        /// An Array of double values representing neuron outputs
        /// </returns>
        public virtual double[] GetOutput()
        {
            double[] output = new double[neurons.Length];
            for (int i = 0; i < neurons.Length; i++)
            {
                output[i] = neurons[i].Output;
            }
            return output;
        }

        /// <summary>
        /// Sets the learning rate to the given value. The layer will use this constant value
        /// as learning rate throughout the learning process
        /// </summary>
        /// <param name="learningRate">
        /// The learning rate
        /// </param>
        public void SetLearningRate(double learningRate)
        {
            SetLearningRate(learningRate, learningRate);
        }

        /// <summary>
        /// Sets the initial and final values for learning rate. During the learning process, the
        /// effective learning rate uniformly changes from its initial value to final value
        /// </summary>
        /// <param name="initialLearningRate">
        /// Initial value of learning rate
        /// </param>
        /// <param name="finalLearningRate">
        /// Final value of learning rate
        /// </param>
        public void SetLearningRate(double initialLearningRate, double finalLearningRate)
        {
            SetLearningRate(new LinearFunction(initialLearningRate, finalLearningRate));
        }

        /// <summary>
        /// Sets the learning rate function.
        /// </summary>
        /// <param name="learningRateFunction">
        /// Learning rate function to use.
        /// </param>
        /// <exception cref="System.ArgumentNullException">
        /// If <c>learningRateFunction</c> is <c>null</c>
        /// </exception>
        public void SetLearningRate(ILearningRateFunction learningRateFunction)
        {
            Helper.ValidateNotNull(learningRateFunction, "learningRateFunction");
            this.learningRateFunction = learningRateFunction;
        }

        /// <summary>
        /// Initializes all neurons and makes them ready to undergo training freshly.
        /// </summary>
        public abstract void Initialize();

        /// <summary>
        /// Runs all neurons in the layer.
        /// </summary>
        public virtual void Run()
        {
            for (int i = 0; i < neurons.Length; i++)
            {
                neurons[i].Run();
            }
        }

        /// <summary>
        /// All neurons and their are source connectors are allowed to learn. This method assumes a
        /// learning environment where inputs, outputs and other parameters (if any) are appropriate.
        /// </summary>
        /// <param name="currentIteration">
        /// Current learning iteration
        /// </param>
        /// <param name="trainingEpochs">
        /// Total number of training epochs
        /// </param>
        /// <exception cref="ArgumentException">
        /// If <c>trainingEpochs</c> is zero or negative
        /// </exception>
        /// <exception cref="ArgumentOutOfRangeException">
        /// If <c>currentIteration</c> is negative or, if it is greater than <c>trainingEpochs</c>
        /// </exception>
        public virtual void Learn(int currentIteration, int trainingEpochs)
        {
            // Validation delegated
            double effectiveRate = learningRateFunction.GetLearningRate(currentIteration, trainingEpochs);
            for (int i = 0; i < neurons.Length; i++)
            {
                neurons[i].Learn(effectiveRate);
            }
        }
    }
}