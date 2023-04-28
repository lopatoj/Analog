using System;
using System.Collections.Generic;
using System.Text;
using MathNet.Numerics.LinearAlgebra;

namespace Analog.Models
{
    public class LinearLayer
    {
        private Matrix<byte> weights;
        private Vector<byte> biases;

        public LinearLayer(int inputSize, int outputSize)
        {
            weights = Matrix<byte>.Build.Random(inputSize, outputSize);
            biases = Vector<byte>.Build.Dense(outputSize);
        }

        public Vector<byte> Forward(Vector<byte> input)
        {
            var output = input * weights + biases;
            return output;
        }
    }
}
