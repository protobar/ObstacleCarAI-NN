using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;
using TMPro;
using System.Threading.Tasks;

/// <summary>
/// Enhanced training manager with adaptive mutation, diversity tracking, and performance optimizations
/// </summary>
public class TrainingManager : MonoBehaviour
{
    [Header("Population Settings")]
    public GameObject carPrefab;
    public int populationSize = 20;
    public Transform spawnPoint;

    [Header("Training Settings")]
    public float generationTime = 30f;
    public int eliteCount = 2;

    [Header("Genetic Algorithm")]
    [Range(0f, 1f)]
    public float mutationRate = 0.15f;
    [Range(0f, 2f)]
    public float mutationStrength = 0.5f;
    public bool useAdaptiveMutation = true;
    public int maxGenerationsWithoutImprovement = 20;

    [Header("Save/Load")]
    public string saveFileName = "best_weights.json";
    public bool autoSaveEveryGeneration = true;

    [Header("UI Info")]
    public TMP_Text statsText;

    public List<CarController> cars = new List<CarController>();
    private int currentGeneration = 0;
    private float generationTimer = 0f;
    private bool isTraining = true;

    // Performance tracking
    private List<float> bestFitnessHistory = new List<float>();
    private List<float> avgFitnessHistory = new List<float>();
    private List<float> diversityHistory = new List<float>();
    private int generationsWithoutImprovement = 0;

    void Start()
    {
        InitializePopulation();
    }

    void Update()
    {
        if (!isTraining) return;

        generationTimer += Time.deltaTime;

        // Check if all cars are dead or time is up
        bool allDead = cars.All(car => car.IsDead());

        if (allDead || generationTimer >= generationTime)
        {
            EvolvePopulation();
            generationTimer = 0f;
        }

        UpdateUI();
    }

    void InitializePopulation()
    {
        for (int i = 0; i < populationSize; i++)
        {
            Vector3 spawnPos = spawnPoint.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
            GameObject carObj = Instantiate(carPrefab, spawnPos, spawnPoint.rotation);
            CarController car = carObj.GetComponent<CarController>();

            // Initialize with correct input size: numberOfRays + 2 (speed + angular velocity)
            car.brain = new NeuralNetwork(new int[] { car.numberOfRays + 2, 8, 4, 2 });
            car.brain.activationType = NeuralNetwork.ActivationType.LeakyReLU;

            cars.Add(car);
        }

        Debug.Log($"Initialized population of {populationSize} cars");
    }

    void EvolvePopulation()
    {
        currentGeneration++;

        // Collect fitnesses (using parallel processing for speed)
        float[] fitnessArray = new float[cars.Count];
        Parallel.For(0, cars.Count, i =>
        {
            fitnessArray[i] = cars[i].fitness;
        });

        List<float> fitnesses = new List<float>(fitnessArray);
        List<float[]> weights = cars.Select(car => car.brain.GetWeights()).ToList();

        // Calculate statistics
        float bestFitness = fitnesses.Max();
        float avgFitness = fitnesses.Average();
        float diversity = CalculatePopulationDiversity();
        int bestIndex = fitnesses.IndexOf(bestFitness);

        // Track history
        bestFitnessHistory.Add(bestFitness);
        avgFitnessHistory.Add(avgFitness);
        diversityHistory.Add(diversity);

        // Check for improvement
        if (bestFitnessHistory.Count > 1 && bestFitness <= bestFitnessHistory[bestFitnessHistory.Count - 2])
        {
            generationsWithoutImprovement++;
        }
        else
        {
            generationsWithoutImprovement = 0;
        }

        Debug.Log($"Gen {currentGeneration}: Best={bestFitness:F2}, Avg={avgFitness:F2}, " +
                  $"Diversity={diversity:F2}, NoImprove={generationsWithoutImprovement}");

        // Save best weights
        if (autoSaveEveryGeneration)
        {
            SaveWeights(cars[bestIndex].brain.GetWeights(), $"gen_{currentGeneration}_best.json");
            SaveWeights(cars[bestIndex].brain.GetWeights(), saveFileName); // Also update best overall
        }

        // Adaptive mutation rate
        if (useAdaptiveMutation)
        {
            mutationRate = GeneticAlgorithm.GetAdaptiveMutationRate(currentGeneration, 0.15f, 0.02f, 150);

            // Increase mutation if stuck
            if (generationsWithoutImprovement >= maxGenerationsWithoutImprovement / 2)
            {
                mutationRate = Mathf.Min(mutationRate * 1.5f, 0.3f);
                Debug.Log($"Stagnation detected! Increasing mutation to {mutationRate:F3}");
            }
        }

        // Create new generation
        GeneticAlgorithm.mutationRate = mutationRate;
        GeneticAlgorithm.mutationStrength = mutationStrength;
        List<float[]> newWeights = GeneticAlgorithm.CreateNewGeneration(weights, fitnesses, eliteCount);

        // Apply new weights and reset cars
        for (int i = 0; i < cars.Count; i++)
        {
            cars[i].brain.SetWeights(newWeights[i]);
            Vector3 spawnPos = spawnPoint.position + new Vector3(Random.Range(-2f, 2f), 0, Random.Range(-2f, 2f));
            cars[i].ResetCar(spawnPos, spawnPoint.rotation);
        }
    }

    float CalculatePopulationDiversity()
    {
        if (cars.Count < 2) return 0f;

        float totalDifference = 0f;
        int comparisons = 0;

        for (int i = 0; i < cars.Count - 1; i++)
        {
            for (int j = i + 1; j < cars.Count; j++)
            {
                float[] genes1 = cars[i].brain.GetWeights();
                float[] genes2 = cars[j].brain.GetWeights();

                float diff = 0f;
                for (int k = 0; k < genes1.Length; k++)
                {
                    diff += Mathf.Abs(genes1[k] - genes2[k]);
                }
                totalDifference += diff / genes1.Length;
                comparisons++;
            }
        }

        return totalDifference / comparisons;
    }

    void UpdateUI()
    {
        if (statsText != null)
        {
            var aliveCars = cars.Where(car => !car.IsDead()).ToList();
            float bestFitness = cars.Max(car => car.fitness);
            float avgFitness = cars.Average(car => car.fitness);
            float diversity = diversityHistory.Count > 0 ? diversityHistory[diversityHistory.Count - 1] : 0f;

            statsText.text = $"Generation: {currentGeneration}\n" +
                           $"Time: {generationTimer:F1}s / {generationTime}s\n" +
                           $"Alive: {aliveCars.Count} / {populationSize}\n" +
                           $"Best Fitness: {bestFitness:F2}\n" +
                           $"Avg Fitness: {avgFitness:F2}\n" +
                           $"Diversity: {diversity:F2}\n" +
                           $"Mutation Rate: {mutationRate:F3}\n" +
                           $"No Improve: {generationsWithoutImprovement}";
        }
    }

    public void SaveWeights(float[] weights, string filename)
    {
        string path = Path.Combine(Application.persistentDataPath, filename);
        WeightData data = new WeightData { weights = weights };
        string json = JsonUtility.ToJson(data, true);
        File.WriteAllText(path, json);
        Debug.Log($"Weights saved to: {path}");
    }

    public float[] LoadWeights(string filename)
    {
        string path = Path.Combine(Application.persistentDataPath, filename);
        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            WeightData data = JsonUtility.FromJson<WeightData>(json);
            Debug.Log($"Weights loaded from: {path}");
            return data.weights;
        }
        else
        {
            Debug.LogWarning($"File not found: {path}");
            return null;
        }
    }

    public void SaveBestWeights()
    {
        float bestFitness = cars.Max(car => car.fitness);
        int bestIndex = cars.ToList().FindIndex(car => car.fitness == bestFitness);
        SaveWeights(cars[bestIndex].brain.GetWeights(), saveFileName);
    }

    public void LoadBestWeights()
    {
        float[] weights = LoadWeights(saveFileName);
        if (weights != null && cars.Count > 0)
        {
            cars[0].brain.SetWeights(weights);
            Debug.Log("Best weights loaded to first car");
        }
    }

    public void PauseTraining()
    {
        isTraining = !isTraining;
        Debug.Log($"Training {(isTraining ? "resumed" : "paused")}");
    }

    void OnApplicationQuit()
    {
        SaveBestWeights();

        // Export training data to CSV
        string csvPath = Path.Combine(Application.persistentDataPath, "training_log.csv");
        System.Text.StringBuilder sb = new System.Text.StringBuilder();
        sb.AppendLine("Generation,BestFitness,AvgFitness,Diversity");

        for (int i = 0; i < bestFitnessHistory.Count; i++)
        {
            float best = bestFitnessHistory[i];
            float avg = i < avgFitnessHistory.Count ? avgFitnessHistory[i] : 0f;
            float div = i < diversityHistory.Count ? diversityHistory[i] : 0f;
            sb.AppendLine($"{i + 1},{best:F2},{avg:F2},{div:F2}");
        }

        File.WriteAllText(csvPath, sb.ToString());
        Debug.Log($"Training log saved to: {csvPath}");
    }
}

[System.Serializable]
public class WeightData
{
    public float[] weights;
}
