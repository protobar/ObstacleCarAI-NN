using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Linq;

/// <summary>
/// Enhanced UI controller with neural network output visualization
/// </summary>
public class UIController : MonoBehaviour
{
    public TrainingManager trainingManager;

    [Header("UI Elements")]
    public Button saveButton;
    public Button loadButton;
    public Button pauseButton;
    public Slider mutationRateSlider;
    public Slider mutationStrengthSlider;
    public Slider timeScaleSlider;
    public TMP_Text mutationRateText;
    public TMP_Text mutationStrengthText;
    public TMP_Text timeScaleText;

    [Header("Neural Network Visualization")]
    public TMP_Text networkOutputText;
    public TMP_Text bestCarInfoText;

    void Start()
    {
        if (trainingManager == null)
        {
            trainingManager = FindObjectOfType<TrainingManager>();
        }

        // Setup button listeners
        if (saveButton != null)
            saveButton.onClick.AddListener(OnSaveClicked);
        if (loadButton != null)
            loadButton.onClick.AddListener(OnLoadClicked);
        if (pauseButton != null)
            pauseButton.onClick.AddListener(OnPauseClicked);

        // Setup slider listeners
        if (mutationRateSlider != null)
        {
            mutationRateSlider.value = trainingManager.mutationRate;
            mutationRateSlider.onValueChanged.AddListener(OnMutationRateChanged);
        }

        if (mutationStrengthSlider != null)
        {
            mutationStrengthSlider.value = trainingManager.mutationStrength;
            mutationStrengthSlider.onValueChanged.AddListener(OnMutationStrengthChanged);
        }

        if (timeScaleSlider != null)
        {
            timeScaleSlider.minValue = 1f;
            timeScaleSlider.maxValue = 10f;
            timeScaleSlider.value = Time.timeScale;
            timeScaleSlider.onValueChanged.AddListener(OnTimeScaleChanged);
        }

        UpdateUI();
    }

    void Update()
    {
        UpdateNetworkVisualization();
    }

    void OnSaveClicked()
    {
        trainingManager.SaveBestWeights();
        Debug.Log("Weights saved!");
    }

    void OnLoadClicked()
    {
        trainingManager.LoadBestWeights();
        Debug.Log("Weights loaded!");
    }

    void OnPauseClicked()
    {
        trainingManager.PauseTraining();
    }

    void OnMutationRateChanged(float value)
    {
        trainingManager.mutationRate = value;
        UpdateUI();
    }

    void OnMutationStrengthChanged(float value)
    {
        trainingManager.mutationStrength = value;
        UpdateUI();
    }

    void OnTimeScaleChanged(float value)
    {
        Time.timeScale = value;
        UpdateUI();
    }

    void UpdateUI()
    {
        if (mutationRateText != null)
            mutationRateText.text = $"Mutation Rate: {trainingManager.mutationRate:F2}";
        if (mutationStrengthText != null)
            mutationStrengthText.text = $"Mutation Strength: {trainingManager.mutationStrength:F2}";
        if (timeScaleText != null)
            timeScaleText.text = $"Time Scale: {Time.timeScale:F1}x";
    }

    void UpdateNetworkVisualization()
    {
        if (trainingManager != null && trainingManager.cars.Count > 0)
        {
            var bestCar = trainingManager.cars.OrderByDescending(c => c.fitness).FirstOrDefault();

            if (bestCar != null && bestCar.brain != null && !bestCar.IsDead())
            {
                float[] outputs = bestCar.brain.FeedForward(bestCar.sensorInputs);

                if (networkOutputText != null)
                {
                    networkOutputText.text = $"Best Car NN Output:\n" +
                                           $"Acceleration: {outputs[0]:F2}\n" +
                                           $"Steering: {outputs[1]:F2}";
                }

                if (bestCarInfoText != null)
                {
                    bestCarInfoText.text = $"Best Car Stats:\n" +
                                         $"Fitness: {bestCar.fitness:F1}\n" +
                                         $"Distance: {bestCar.distanceTraveled:F1}\n" +
                                         $"Time: {bestCar.timeSurvived:F1}s\n" +
                                         $"Checkpoints: {bestCar.checkpointsReached}";
                }
            }
        }
    }
}
