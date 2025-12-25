using System;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Enhanced feedforward neural network with multiple activation functions
/// </summary>
[System.Serializable]
public class NeuralNetwork
{
    public enum ActivationType { Tanh, ReLU, LeakyReLU, Sigmoid }

    private int[] layers;
    private float[][] neurons;
    private float[][][] weights;
    private float[][] biases;

    public ActivationType activationType = ActivationType.LeakyReLU;

    public NeuralNetwork(int[] layers)
    {
        this.layers = new int[layers.Length];
        for (int i = 0; i < layers.Length; i++)
        {
            this.layers[i] = layers[i];
        }

        InitializeNeurons();
        InitializeWeights();
    }

    private void InitializeNeurons()
    {
        List<float[]> neuronsList = new List<float[]>();
        for (int i = 0; i < layers.Length; i++)
        {
            neuronsList.Add(new float[layers[i]]);
        }
        neurons = neuronsList.ToArray();
    }

    private void InitializeWeights()
    {
        List<float[][]> weightsList = new List<float[][]>();
        List<float[]> biasesList = new List<float[]>();

        for (int i = 1; i < layers.Length; i++)
        {
            float[][] layerWeights = new float[neurons[i].Length][];
            float[] layerBiases = new float[neurons[i].Length];

            for (int j = 0; j < neurons[i].Length; j++)
            {
                layerWeights[j] = new float[neurons[i - 1].Length];

                // Xavier/He initialization for better training
                float scale = activationType == ActivationType.ReLU || activationType == ActivationType.LeakyReLU
                    ? Mathf.Sqrt(2f / neurons[i - 1].Length) // He initialization
                    : Mathf.Sqrt(1f / neurons[i - 1].Length); // Xavier initialization

                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    layerWeights[j][k] = UnityEngine.Random.Range(-scale, scale);
                }

                layerBiases[j] = UnityEngine.Random.Range(-0.1f, 0.1f);
            }

            weightsList.Add(layerWeights);
            biasesList.Add(layerBiases);
        }

        weights = weightsList.ToArray();
        biases = biasesList.ToArray();
    }

    public float[] FeedForward(float[] inputs)
    {
        // Set input layer
        for (int i = 0; i < inputs.Length; i++)
        {
            neurons[0][i] = inputs[i];
        }

        // Propagate through hidden and output layers
        for (int i = 1; i < layers.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float value = 0f;

                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    value += weights[i - 1][j][k] * neurons[i - 1][k];
                }

                // Use Tanh for output layer (bounded -1 to 1), chosen activation for hidden
                bool isOutputLayer = i == layers.Length - 1;
                neurons[i][j] = Activate(value + biases[i - 1][j], isOutputLayer);
            }
        }

        return neurons[neurons.Length - 1];
    }

    private float Activate(float value, bool isOutputLayer = false)
    {
        // Always use Tanh for output layer (bounded -1 to 1)
        if (isOutputLayer)
            return (float)Math.Tanh(value);

        switch (activationType)
        {
            case ActivationType.ReLU:
                return Mathf.Max(0, value);

            case ActivationType.LeakyReLU:
                return value > 0 ? value : 0.01f * value;

            case ActivationType.Sigmoid:
                return 1f / (1f + Mathf.Exp(-value));

            case ActivationType.Tanh:
            default:
                return (float)Math.Tanh(value);
        }
    }

    public void SetWeights(float[] flatWeights)
    {
        int index = 0;

        // Set weights
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    weights[i][j][k] = flatWeights[index++];
                }
            }
        }

        // Set biases
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                biases[i][j] = flatWeights[index++];
            }
        }
    }

    public float[] GetWeights()
    {
        List<float> flatWeights = new List<float>();

        // Get weights
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    flatWeights.Add(weights[i][j][k]);
                }
            }
        }

        // Get biases
        for (int i = 0; i < biases.Length; i++)
        {
            for (int j = 0; j < biases[i].Length; j++)
            {
                flatWeights.Add(biases[i][j]);
            }
        }

        return flatWeights.ToArray();
    }

    public int GetWeightsCount()
    {
        int count = 0;

        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                count += weights[i][j].Length;
            }
        }

        for (int i = 0; i < biases.Length; i++)
        {
            count += biases[i].Length;
        }

        return count;
    }

    public NeuralNetwork Copy()
    {
        NeuralNetwork copy = new NeuralNetwork(layers);
        copy.activationType = this.activationType;
        copy.SetWeights(GetWeights());
        return copy;
    }
}
