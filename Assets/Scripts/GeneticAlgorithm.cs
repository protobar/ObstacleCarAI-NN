using System.Collections.Generic;
using System.Linq;
using UnityEngine;

/// <summary>
/// Enhanced genetic algorithm with multiple crossover methods and diversity preservation
/// </summary>
public class GeneticAlgorithm
{
    public static float mutationRate = 0.1f;
    public static float mutationStrength = 0.5f;

    private class Individual
    {
        public float[] genes;
        public float fitness;

        public Individual(float[] genes, float fitness)
        {
            this.genes = genes;
            this.fitness = fitness;
        }
    }

    /// <summary>
    /// Create new generation with elitism, crossover, and mutation
    /// </summary>
    public static List<float[]> CreateNewGeneration(List<float[]> population, List<float> fitnesses, int eliteCount = 2)
    {
        List<float[]> newPopulation = new List<float[]>();

        // Create list of individuals with fitness
        List<Individual> individuals = new List<Individual>();
        for (int i = 0; i < population.Count; i++)
        {
            individuals.Add(new Individual(population[i], fitnesses[i]));
        }

        // Sort by fitness (descending)
        individuals = individuals.OrderByDescending(x => x.fitness).ToList();

        // Elitism: Keep best performers
        for (int i = 0; i < eliteCount && i < individuals.Count; i++)
        {
            newPopulation.Add((float[])individuals[i].genes.Clone());
        }

        // Fill rest with crossover and mutation
        while (newPopulation.Count < population.Count)
        {
            float[] parent1 = TournamentSelection(individuals);
            float[] parent2 = TournamentSelection(individuals);

            // Randomly choose crossover method
            float[] child;
            float crossoverType = Random.value;

            if (crossoverType < 0.33f)
                child = Crossover(parent1, parent2);
            else if (crossoverType < 0.66f)
                child = TwoPointCrossover(parent1, parent2);
            else
                child = BlendCrossover(parent1, parent2);

            child = Mutate(child);
            newPopulation.Add(child);
        }

        return newPopulation;
    }

    /// <summary>
    /// Tournament selection - pick best from random sample
    /// </summary>
    private static float[] TournamentSelection(List<Individual> individuals, int tournamentSize = 3)
    {
        float bestFitness = float.MinValue;
        float[] bestGenes = null;

        for (int i = 0; i < tournamentSize; i++)
        {
            int randomIndex = Random.Range(0, individuals.Count);
            if (individuals[randomIndex].fitness > bestFitness)
            {
                bestFitness = individuals[randomIndex].fitness;
                bestGenes = individuals[randomIndex].genes;
            }
        }

        return bestGenes;
    }

    /// <summary>
    /// Single-point crossover
    /// </summary>
    private static float[] Crossover(float[] parent1, float[] parent2)
    {
        float[] child = new float[parent1.Length];
        int crossoverPoint = Random.Range(0, parent1.Length);

        for (int i = 0; i < parent1.Length; i++)
        {
            child[i] = i < crossoverPoint ? parent1[i] : parent2[i];
        }

        return child;
    }

    /// <summary>
    /// Two-point crossover
    /// </summary>
    public static float[] TwoPointCrossover(float[] parent1, float[] parent2)
    {
        float[] child = new float[parent1.Length];
        int point1 = Random.Range(0, parent1.Length);
        int point2 = Random.Range(point1, parent1.Length);

        for (int i = 0; i < parent1.Length; i++)
        {
            if (i >= point1 && i < point2)
                child[i] = parent2[i];
            else
                child[i] = parent1[i];
        }

        return child;
    }

    /// <summary>
    /// Blend crossover (BLX-alpha)
    /// </summary>
    public static float[] BlendCrossover(float[] parent1, float[] parent2, float alpha = 0.5f)
    {
        float[] child = new float[parent1.Length];

        for (int i = 0; i < parent1.Length; i++)
        {
            float min = Mathf.Min(parent1[i], parent2[i]);
            float max = Mathf.Max(parent1[i], parent2[i]);
            float range = max - min;
            child[i] = Random.Range(min - alpha * range, max + alpha * range);
            child[i] = Mathf.Clamp(child[i], -10f, 10f);
        }

        return child;
    }

    /// <summary>
    /// Mutate genes with random changes
    /// </summary>
    private static float[] Mutate(float[] genes)
    {
        float[] mutated = (float[])genes.Clone();

        for (int i = 0; i < mutated.Length; i++)
        {
            if (Random.value < mutationRate)
            {
                mutated[i] += Random.Range(-mutationStrength, mutationStrength);
                mutated[i] = Mathf.Clamp(mutated[i], -10f, 10f);
            }
        }

        return mutated;
    }

    /// <summary>
    /// Uniform crossover
    /// </summary>
    public static float[] UniformCrossover(float[] parent1, float[] parent2)
    {
        float[] child = new float[parent1.Length];

        for (int i = 0; i < parent1.Length; i++)
        {
            child[i] = Random.value < 0.5f ? parent1[i] : parent2[i];
        }

        return child;
    }

    /// <summary>
    /// Calculate genetic distance between two individuals
    /// </summary>
    public static float GeneticDistance(float[] genes1, float[] genes2)
    {
        float sum = 0f;
        for (int i = 0; i < genes1.Length; i++)
        {
            sum += Mathf.Abs(genes1[i] - genes2[i]);
        }
        return sum / genes1.Length;
    }

    /// <summary>
    /// Calculate adaptive mutation rate that decreases over generations
    /// </summary>
    public static float GetAdaptiveMutationRate(int generation, float initialRate, float finalRate = 0.01f, int decayGenerations = 100)
    {
        float progress = Mathf.Clamp01((float)generation / decayGenerations);
        return Mathf.Lerp(initialRate, finalRate, progress);
    }
}
