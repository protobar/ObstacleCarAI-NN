using UnityEngine;
using System.IO;

/// <summary>
/// Simple single-car test mode - attach to GameManager
/// </summary>
public class SingleCarTest : MonoBehaviour
{
    [Header("Test Settings")]
    public GameObject carPrefab;
    public Transform spawnPoint;
    public string weightsFileName = "best_weights.json";

    [Header("Optional: Camera Follow")]
    public Camera followCamera;
    public Vector3 cameraOffset = new Vector3(0, 5, -10);

    private CarController testCar;

    void Start()
    {
        LoadAndTestBestCar();
    }

    void LoadAndTestBestCar()
    {
        // Instantiate single car
        GameObject carObj = Instantiate(carPrefab, spawnPoint.position, spawnPoint.rotation);
        testCar = carObj.GetComponent<CarController>();

        // Initialize brain with same architecture as training
        testCar.brain = new NeuralNetwork(new int[] { testCar.numberOfRays + 2, 8, 4, 2 });
        testCar.brain.activationType = NeuralNetwork.ActivationType.LeakyReLU;

        // Load trained weights
        float[] weights = LoadWeights(weightsFileName);

        if (weights != null)
        {
            testCar.brain.SetWeights(weights);
            Debug.Log($"✅ Loaded {weights.Length} weights from {weightsFileName}");
            Debug.Log($"📍 Weights location: {Application.persistentDataPath}");
        }
        else
        {
            Debug.LogError($"❌ Failed to load {weightsFileName}!");
            Debug.Log($"Expected location: {Path.Combine(Application.persistentDataPath, weightsFileName)}");
        }

        // Set normal time scale
        Time.timeScale = 1f;

        Debug.Log("🚗 Test car ready! Press R to restart, Space to toggle pause");
    }

    void Update()
    {
        // Camera follow
        if (followCamera != null && testCar != null)
        {
            Vector3 desiredPosition = testCar.transform.position +
                                     testCar.transform.TransformDirection(cameraOffset);
            followCamera.transform.position = Vector3.Lerp(
                followCamera.transform.position,
                desiredPosition,
                Time.deltaTime * 5f
            );
            followCamera.transform.LookAt(testCar.transform.position + Vector3.up);
        }

        // Controls
        if (Input.GetKeyDown(KeyCode.R))
        {
            RestartTest();
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            Time.timeScale = Time.timeScale > 0 ? 0 : 1f;
            Debug.Log(Time.timeScale > 0 ? "▶️ Resumed" : "⏸️ Paused");
        }

        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }

    void RestartTest()
    {
        if (testCar != null)
        {
            testCar.ResetCar(spawnPoint.position, spawnPoint.rotation);
            Debug.Log("🔄 Test restarted");
        }
    }

    void OnGUI()
    {
        if (testCar != null)
        {
            GUI.skin.label.fontSize = 16;
            GUI.Label(new Rect(10, 10, 300, 30), $"Fitness: {testCar.fitness:F1}");
            GUI.Label(new Rect(10, 40, 300, 30), $"Distance: {testCar.distanceTraveled:F1}m");
            GUI.Label(new Rect(10, 70, 300, 30), $"Time: {testCar.timeSurvived:F1}s");
            GUI.Label(new Rect(10, 100, 300, 30), $"Checkpoints: {testCar.checkpointsReached}");

            GUI.Label(new Rect(10, 140, 300, 30), "Controls:");
            GUI.Label(new Rect(10, 170, 300, 30), "R - Restart | Space - Pause");

            if (testCar.IsDead())
            {
                GUI.skin.label.fontSize = 24;
                GUI.color = Color.red;
                GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2 - 50, 200, 50), "CRASHED!");
                GUI.Label(new Rect(Screen.width / 2 - 100, Screen.height / 2, 200, 50), "Press R to restart");
            }
        }
    }

    float[] LoadWeights(string filename)
    {
        string path = Path.Combine(Application.persistentDataPath, filename);

        if (File.Exists(path))
        {
            string json = File.ReadAllText(path);
            WeightData data = JsonUtility.FromJson<WeightData>(json);
            return data.weights;
        }
        else
        {
            Debug.LogWarning($"Weights file not found at: {path}");
            return null;
        }
    }
}
