🚗 AI Car Training with Genetic Algorithms & Neural Networks
Complete Project Documentation
________________________________________
Table of Contents
1.	Project Overview
2.	Features
3.	Installation
4.	Quick Start
5.	How It Works
6.	Training Guide
7.	Testing Mode
8.	Advanced Configuration
9.	Troubleshooting
10.	Next Steps After 600 Generations
________________________________________
Project Overview
A Unity-based simulation that trains autonomous cars to navigate obstacle courses using feedforward neural networks evolved through genetic algorithms. After 600+ generations, cars learn to avoid obstacles, maintain speed, and navigate complex tracks without any manual programming.
Key Achievements
•	✅ Cars successfully navigate complex obstacle courses
•	✅ Learn smooth driving patterns with minimal jerky movements

•	✅ Maintain optimal speeds while avoiding collisions
•	✅ Reach checkpoints efficiently
•	✅ Generalize to new, unseen track layouts
•	✅ Auto-save best weights every generation
•	✅ Real-time performance metrics and visualization
Tech Stack
•	Engine: Unity 2021.3+
•	Language: C# 9.0
•	Physics: Built-in Rigidbody
•	UI: TextMeshPro + Canvas
________________________________________
Features
🧠 Neural Network
•	Feedforward architecture with configurable layers (default: 7→8→4→2)
•	Multiple activation functions: Tanh, ReLU, LeakyReLU, Sigmoid
•	Xavier/He weight initialization for optimal training start
•	Biases for each neuron layer
•	Real-time inference for obstacle avoidance
•	Weight serialization (JSON format)
🧬 Genetic Algorithm
•	Population-based evolution (default: 20 cars per generation)
•	Elitism preserves top performers across generations
•	Three crossover methods:
o	Single-point crossover
o	Two-point crossover
o	Blend crossover (BLX-α)
•	Adaptive mutation rate that decreases over generations (15% → 2%)
•	Tournament selection for parent choosing (tournament size: 3)
•	Diversity tracking to prevent premature convergence
•	Stagnation detection with automatic mutation adjustment
🚙 Car Controller
•	5 raycast sensors detecting obstacles at multiple angles (±60°)
•	Velocity awareness: Speed and angular velocity as inputs
•	Enhanced fitness function with multiple components:
o	Distance traveled (primary metric)
o	Time survived (0.5x multiplier)
o	Speed bonus (0.3x multiplier)
o	Idle penalty (0.5x multiplier)
o	Checkpoint bonus (50 points each)
•	Checkpoint system for progressive learning
•	Physics-based movement using Rigidbody
•	Smooth steering with configurable turn speed
📊 Training System
•	Automatic generation management (configurable time limits)
•	Multi-threaded fitness evaluation for performance
•	Auto-save best weights every generation
•	CSV export of training metrics (generation, best/avg fitness, diversity)
•	Stagnation detection with automatic mutation adjustment
•	Real-time statistics display
o	Current generation
o	Time remaining in generation
o	Alive cars / Total population
o	Best fitness / Average fitness
o	Population diversity score
o	Current mutation rate
o	Generations without improvement
🎨 UI & Visualization
•	Training dashboard with live stats
•	Neural network output visualization for best car
•	Mutation rate/strength sliders for live parameter tuning
•	Time scale control (1x-10x speed)
•	Save/Load buttons for weight management
•	Scene view gizmos:
o	Raycast visualization (red=obstacle, green=clear)
o	Forward direction indicator (blue ray)
o	Fitness labels above each car
o	Car status indicator (green=alive, red=dead)
________________________________________
Installation
Prerequisites
•	Unity 2021.3 or higher
•	TextMeshPro package (usually included)
•	Minimum: 4GB RAM, modern GPU
Step-by-Step Setup
1.	Clone the repository
git clone https://github.com/yourusername/ai-car-training.git
cd ai-car-training
2.	Open in Unity
o	Open Unity Hub
o	Click “Add” → Select project folder
o	Select project and open
o	Wait for scripts to compile
3.	Scene Configuration
o	Open Scenes/TrainingScene
o	In Hierarchy, select GameManager
o	Verify TrainingManager component has:
	Car Prefab assigned
	Spawn Point assigned
	All other settings configured
4.	Create Training Obstacles
o	Create Cubes as obstacles
o	Tag them as “Obstacle” (or configure in Inspector)
o	Place on your track
5.	Configure Layer Mask
o	In Inspector, expand obstacleLayer dropdown
o	Select the layer your obstacles use
o	This enables raycast detection
6.	Ready to Train!
o	Press Play
o	Cars will spawn and begin training
________________________________________
Quick Start
Start Training (5 Minutes)
1.	Open TrainingScene in Unity
2.	Press Play ▶️
3.	Watch cars learn for a few generations
4.	Adjust settings as needed:
o	Time Scale: 5x for faster training
o	Population Size: 20 (or 50 for more diversity)
o	Generation Time: 30s (or 60s for slower evaluation)
5.	Monitor progress in real-time UI
Test Single Car (2 Minutes)
Quick Method: 1. Select GameManager in Hierarchy 2. In Inspector, check ✅ Test Mode on TrainingManager 3. Press Play 4. Your best trained car will load automatically 5. Press R to restart, Space to pause
Advanced Method: 1. Duplicate scene as TestScene 2. Delete TrainingManager component 3. Create new GameObject called TestManager 4. Attach SingleCarTest.cs script 5. Configure car prefab and spawn point 6. Press Play
________________________________________
How It Works
Neural Network Architecture
Input Layer (7 neurons):
├─ Front distance sensor
├─ Front-left distance sensor (±30°)
├─ Front-right distance sensor (±30°)
├─ Left distance sensor (-90°)
├─ Right distance sensor (+90°)
├─ Normalized current speed
└─ Normalized angular velocity

Hidden Layer 1 (8 neurons):
└─ LeakyReLU activation

Hidden Layer 2 (4 neurons):
└─ LeakyReLU activation

Output Layer (2 neurons):
├─ Acceleration (-1 to 1)
└─ Steering (-1 to 1)
Sensor System
Sensor Coverage:
     Front (-30°)
        ↑
    ╱       ╲
   /         \
Left         Right
(-90°)      (+90°)
   \         /
    ╲       ╱
   Front-right
   Front-left
      ↓
Each sensor returns normalized distance: - 1.0 = Obstacle very close (danger) - 0.5 = Obstacle at mid-range - 0.0 = Clear path (far or no obstacle)
Evolution Process
Generation N:
├─ Evaluate: Run all 20 cars for 30 seconds
├─ Calculate: Fitness for each car
├─ Select: Top 20% become parents
├─ Crossover: Combine parent genes
│   ├─ 33% Single-point crossover
│   ├─ 33% Two-point crossover
│   └─ 33% Blend crossover
├─ Mutate: Random weight adjustments
│   ├─ Mutation rate: 15% (adaptive, decreases over time)
│   ├─ Mutation strength: ±0.5 (configurable)
│   └─ Clamp to [-10, 10] range
├─ Elitism: Keep top 2 cars unchanged
└─ Repeat: Generation N+1 with new weights
Fitness Calculation
Fitness = Distance + Time + Speed Bonus - Idle Penalty + Checkpoint Bonus

Where:
├─ Distance: Total meters traveled (primary metric)
├─ Time: Seconds survived × 0.5 (secondary metric)
├─ Speed Bonus: Average speed × 0.3 (encourages fast driving)
├─ Idle Penalty: Idle time × 0.5 (penalizes standing still)
└─ Checkpoint Bonus: Checkpoints reached × 50 (encourages goal seeking)
________________________________________
Training Guide
Initial Setup (Gen 0-10)
Expected Behavior: - Cars move randomly - Frequent collisions - Fitness scores near 0
What’s Normal: - Don’t expect intelligent driving yet - Mutation rate is high (15%) - Population is very diverse
Actions: - Just let it run - Verify raycasts are working (watch Console for errors) - Ensure obstacle layer is correct
Early Learning (Gen 10-100)
Expected Behavior: - Some cars begin avoiding obstacles - Fitness slowly increases - Random crashes still common
What’s Normal: - Best fitness increases from 0 → 50 → 200 - Average fitness lags behind best - Diversity decreases slightly
Actions: - Monitor “No Improve” counter (should stay low) - Adjust mutation strength if progress too slow - Consider using 2x time scale to speed up
Mid-Training (Gen 100-300)
Expected Behavior: - Most cars avoid simple obstacles - Consistent lane following emerges - Some cars reach checkpoints
What’s Normal: - Best fitness 500-2000 - Clear learning curves in graphs - Diversity stabilizing
Actions: - Watch training log for patterns - Increase difficulty if plateau detected - Use 5x time scale safely
Advanced Learning (Gen 300-600)
Expected Behavior: - Sophisticated obstacle avoidance - Smooth turning and speed control - Checkpoint sequences completed
What’s Normal: - Best fitness 2000-5000+ - Diminishing returns (slower improvement) - High diversity with clear winners
Actions: - Consider track complexity increase - Monitor for overfitting (test on new tracks) - Prepare for deployment
Late-Stage Training (Gen 600+)
Expected Behavior: - Mastery of training track - Smooth, human-like driving - Potential for generalization
What’s Normal: - Fitness improvements may plateau - Population very homogeneous - “No Improve” counter increasing
Actions: - Evaluate performance on new tracks - Consider curriculum learning - Export weights for production
Monitoring Training
Use this table to assess progress:
Metric	Gen 0-50	Gen 50-200	Gen 200-400	Gen 400-600
Best Fitness	0-50	50-500	500-2000	2000+
Alive %	10-20%	30-50%	60-80%	80%+
Avg/Best Ratio	<0.1	0.1-0.3	0.3-0.6	0.6+
Diversity	High	High	Medium	Low
________________________________________
Testing Mode
Single Car Test (Method 1: Quick Toggle)
Setup (30 seconds):
1. Open TrainingManager.cs
2. Add this at the top of Start():
   if (testMode)
   {
       TestSingleCar();
   }
   else
   {
       InitializePopulation();
   }
3. In Inspector, check: testMode = true
4. Press Play
Controls: - R - Restart test car - Space - Pause/Resume - ESC - Quit
Displays: - Current fitness - Distance traveled - Time survived - Checkpoints reached - Crash indicator
Single Car Test (Method 2: Dedicated Script)
Setup (2 minutes):
1.	Create SingleCarTest.cs (provided in documentation)
2.	Create empty GameObject: TestManager
3.	Attach SingleCarTest component
4.	Assign:
o	Car Prefab
o	Spawn Point
o	Weights File Name: best_weights.json
5.	(Optional) Assign camera for auto-follow
6.	Press Play
Features: - Automatic camera follow - Detailed UI overlay - Smooth restarts - Pause capability
Finding Your Weights File
Via UI:
// Add temporary button to see folder path
if (GUI.Button(new Rect(10, 10, 200, 30), "Open Save Folder"))
{
    Application.OpenURL(Application.persistentDataPath);
}
Manual Navigation: - Windows: C:\Users\[YourName]\AppData\LocalLow\[Company]\[Project]\ - Mac: ~/Library/Application Support/[Company]/[Project]/ - Linux: ~/.config/unity3d/[Company]/[Project]/
Files Generated: - best_weights.json - Overall best car - gen_XXX_best.json - Best car per generation - training_log.csv - Performance metrics over time
Performance Evaluation
Test on 3 track types: 1. Training track (known) - Should get 90%+ fitness 2. Similar new track - Should get 70%+ fitness (slight variation) 3. Different track - Should get 40-60% fitness (true test of generalization)
Success Criteria: - ✅ Completes track without crashing - ✅ Smooth, non-erratic movements - ✅ Maintains reasonable speed - ✅ Works on unseen obstacles (generalization) - ✅ Recovery from near-collisions
________________________________________
Advanced Configuration
Network Architecture Modification
// Default architecture
new int[] { 7, 8, 4, 2 }

// Deeper network (more learning capacity)
new int[] { 7, 16, 8, 4, 2 }

// Wider network (faster training)
new int[] { 7, 12, 4, 2 }

// Minimal network (faster inference)
new int[] { 7, 6, 2 }
Recommendations: - Use default for most cases - Use deeper for complex tracks - Use wider for faster exploration - Use minimal for deployment (inference speed)
Activation Function Selection
brain.activationType = NeuralNetwork.ActivationType.LeakyReLU;
Options: - LeakyReLU (default) - Best for car control, fast learning - ReLU - Similar to LeakyReLU, slightly faster - Tanh - Smoother, good for stability - Sigmoid - Use only for output layer normally
Genetic Algorithm Parameters
// In TrainingManager
mutationRate = 0.15f;           // 15% chance per gene
mutationStrength = 0.5f;        // ±0.5 change per mutation
eliteCount = 2;                 // Keep top 2 unchanged
populationSize = 20;            // 20 cars per generation
generationTime = 30f;           // 30 seconds per generation
Tuning Guide:
Parameter	Low	Medium	High	Effect
Mutation Rate	0.05	0.15	0.30	Low=stable, High=exploratory
Mutation Strength	0.1	0.5	1.0	Low=fine-tune, High=explore
Population Size	10	20	50	Low=fast, High=diverse
Elite Count	1	2	5	Low=variable, High=stable
Fitness Function Customization
// In CarController.FixedUpdate()

// Base formula
fitness = distanceTraveled 
        + (timeSurvived * 0.5f) 
        + speedBonus 
        - idlePenalty;

// Add checkpoint focus
fitness += checkpointsReached * 50f;

// Add speed accuracy (target speed = 8 m/s)
float speedAccuracy = 1f - Mathf.Abs(currentSpeed - 8f) / maxSpeed;
fitness += speedAccuracy * 10f;

// Add smoothness penalty
fitness -= Mathf.Abs(steeringInput) * 5f;
Track Configuration
Obstacle Placement: - Use Cube primitives or custom meshes - Tag as “Obstacle” for detection - Ensure Collider component exists - Add to correct layer for raycast
Checkpoint Setup:
// Create empty GameObject with sphere collider
// Set as trigger: Collider > Is Trigger = ON
// Tag as "Checkpoint"
// Cars will automatically detect and count
________________________________________
Troubleshooting
🚗 Cars Driving Backwards
Problem: Cars move in reverse direction
Solution: Uses transform.forward (fixed in latest version) - Verify: Raycasts should cast forward (blue ray in Scene) - Check: CarController line with transform.forward
❌ Cars Not Moving
Problem: Cars spawn but don’t move
Checklist: - [ ] Time.timeScale = 1f (not 0) - [ ] Rigidbody exists and isn’t kinematic - [ ] Brain initialized correctly (check Console) - [ ] Acceleration > 0 in outputs
Debug:
// Add to FixedUpdate temporarily
Debug.Log($"Speed: {currentSpeed}, Accel Output: {outputs[0]}");
📁 Weights File Not Found
Problem: “File not found” error when loading weights
Solutions: 1. Check file exists: Open save folder with UI button 2. Check filename matches exactly (case-sensitive) 3. Verify Application.persistentDataPath is correct 4. Re-save weights in test mode
Manual Path:
// Print correct path
Debug.Log(Application.persistentDataPath);
🧠 Training Plateaus Early
Problem: Fitness stops improving around generation 50
Causes & Fixes: - Track too simple: Add obstacles - Mutation too low: Increase to 0.2 - Population too small: Increase to 30-50 - Generation time too short: Increase to 60s
🎯 Poor Generalization
Problem: Car works on training track but fails on new tracks
Causes & Fixes: - Overfitting: Add track randomization each generation - Sensor dependency: Add noise to sensors (±0.05) - Limited training variety: Create 3-5 different tracks - Curriculum too easy: Increase difficulty gradually
🐢 Training Too Slow
Problem: Generations take forever
Solutions: - [ ] Increase Time Scale: 5x or 10x - [ ] Reduce Population Size: 10 instead of 20 - [ ] Shorter generation time: 20s instead of 30s - [ ] Build instead of Editor (2-3x faster) - [ ] Reduce car/obstacle count on track
🔴 Raycasts Not Working
Problem: Sensors always return 0, or always max
Debug:
// Add to UpdateSensors()
Debug.Log($"Sensor {i}: {sensorInputs[i]}");

// Check raycast lines
// Red = hit obstacle, Green = clear
Fixes: - [ ] Verify obstacleLayer is set in Inspector - [ ] Confirm obstacles are on correct layer - [ ] Check obstacles have Collider components - [ ] Raycasts emit from rayHeight = 0.5 (above ground)
________________________________________
Next Steps After 600 Generations
Evaluate Performance
// Create test suite
Test 1: Training track - expect 90%+ of best fitness
Test 2: Similar track - expect 70-80% of best fitness
Test 3: Completely new track - expect 40-60% of best fitness
If Training Plateaus
Option A: Increase Difficulty
// Create harder tracks
Track 1 (Easy): Wide spaces, few obstacles
Track 2 (Medium): Narrow corridors, moderate obstacles  
Track 3 (Hard): Tight turns, many obstacles
Track 4 (Expert): Multi-path maze, moving obstacles
Option B: Progressive Difficulty
if (currentGeneration % 200 == 0)
{
    currentTrackIndex++;
    LoadNewTrack(trackPrefabs[currentTrackIndex]);
}
Option C: New Objectives - Add: Specific speed targets - Add: Time trials (complete in X seconds) - Add: Precision driving (minimal steering) - Add: Fuel efficiency
Export for Production
// AIDriver.cs - Use in your game/app
public class AIDriver : MonoBehaviour
{
    private NeuralNetwork brain;
    
    void Start()
    {
        brain = new NeuralNetwork(new int[] { 7, 8, 4, 2 });
        float[] weights = LoadWeights("best_weights.json");
        brain.SetWeights(weights);
    }
    
    void FixedUpdate()
    {
        float[] inputs = GetSensors();
        float[] outputs = brain.FeedForward(inputs);
        // Use outputs for car control
    }
}
Advanced Training Techniques
Multi-objective optimization: - Fitness = Speed + Efficiency + Safety + Checkpoint Progress
Transfer learning: - Train on simple track - Fine-tune on complex track - Reduce mutation rate for fine-tuning
Ensemble methods: - Train multiple networks - Combine predictions - More robust decisions
________________________________________
Performance Benchmarks
Hardware Requirements
Minimum: - CPU: Modern dual-core - RAM: 4GB - Storage: 5GB (with logs)
Recommended: - CPU: Quad-core or better - RAM: 8GB - SSD: 10GB free space - GPU: Any modern GPU (Editor 2x faster)
Training Speed
Setup	Gen/Hour	Notes
Editor 1x	120	Baseline
Editor 5x	600	5x time scale
Editor 10x	1200	Maximum time scale
Build 1x	240	~2x faster than Editor
Build 5x	1200	Best for fast training
Memory Usage
•	Per car: ~0.5MB (neural network only)
•	Training scene: ~200-500MB total
•	Build size: ~500MB (with all assets)
________________________________________
File Structure
Project Root/
├── Assets/
│   ├── Scripts/
│   │   ├── NeuralNetwork.cs
│   │   ├── GeneticAlgorithm.cs
│   │   ├── CarController.cs
│   │   ├── TrainingManager.cs
│   │   ├── UIController.cs
│   │   └── SingleCarTest.cs
│   ├── Prefabs/
│   │   ├── Car.prefab
│   │   └── Obstacle.prefab
│   ├── Scenes/
│   │   ├── TrainingScene.unity
│   │   └── TestScene.unity
│   └── Materials/
│       └── CarMaterial.mat
├── README.md
├── LICENSE
└── .gitignore
________________________________________
API Reference
NeuralNetwork
// Create network
NeuralNetwork nn = new NeuralNetwork(new int[] { 7, 8, 4, 2 });

// Set activation type
nn.activationType = NeuralNetwork.ActivationType.LeakyReLU;

// Forward pass
float[] outputs = nn.FeedForward(inputs);

// Save/Load
float[] weights = nn.GetWeights();
nn.SetWeights(weights);

// Utility
int count = nn.GetWeightsCount();
NeuralNetwork copy = nn.Copy();
GeneticAlgorithm
// Create generation
List<float[]> newGen = GeneticAlgorithm.CreateNewGeneration(
    population,      // List<float[]>
    fitnesses,       // List<float>
    eliteCount: 2    // int
);

// Crossover methods
float[] child1 = GeneticAlgorithm.Crossover(parent1, parent2);
float[] child2 = GeneticAlgorithm.TwoPointCrossover(parent1, parent2);
float[] child3 = GeneticAlgorithm.BlendCrossover(parent1, parent2);

// Utility
float distance = GeneticAlgorithm.GeneticDistance(genes1, genes2);
float rate = GeneticAlgorithm.GetAdaptiveMutationRate(gen, 0.15f, 0.02f, 150);
CarController
// Movement
car.maxSpeed = 10f;
car.acceleration = 5f;
car.turnSpeed = 100f;

// Sensors
car.numberOfRays = 5;
car.rayDistance = 10f;
car.rayAngle = 60f;

// Fitness info
float fitness = car.fitness;
float distance = car.distanceTraveled;
float time = car.timeSurvived;

// Control
car.Die();
car.ResetCar(position, rotation);
bool dead = car.IsDead();
________________________________________
Glossary
Fitness: Score measuring how well a car performs (higher = better)
Generation: One cycle of evolution (create, evaluate, select, reproduce)
Mutation: Random change to weights (explores new solutions)
Crossover: Combining parent genes to create offspring
Elitism: Keeping best solutions unchanged in next generation
Activation Function: Non-linear transformation in neural network layers
Neural Network: Brain of the car (computes control outputs from sensors)
Genetic Algorithm: Evolutionary optimization (simulates natural selection)
Tournament Selection: Pick best from random subset (faster than sorting all)
Convergence: Population improving and becoming more similar over time
________________________________________
Support & Resources
Unity Documentation
•	Physics.Raycast
•	Rigidbody
•	JsonUtility
Genetic Algorithm Resources
•	“Genetic Algorithms in Search, Optimization, and Machine Learning”
•	Wikipedia: Genetic Algorithm
•	Scholarpedia: Crossover and Mutation
Neural Network Resources
•	“The Nature of Code - Chapter on Neural Networks”
•	“Activation Functions: Tanh, ReLU, LeakyReLU, Sigmoid”
________________________________________
License
MIT License - Use freely in personal and commercial projects!
________________________________________
Version History
v1.0 (December 2025)
•	Initial release
•	Neural network with multiple activations
•	Genetic algorithm with 3 crossover methods
•	Car controller with 5 sensors
•	Training manager with auto-save
•	Complete UI system
•	Comprehensive documentation
________________________________________
Contributing
Found a bug? Have an improvement? Open an issue or PR!
Areas for contribution: - Multi-agent racing scenarios - Reinforcement learning integration - Improved sensor systems - Dynamic obstacles - Better visualization tools
________________________________________
Acknowledgments
Built with inspiration from neuroevolution techniques and genetic algorithms. Uses raycasting for realistic sensor simulation.
________________________________________
Last Updated: December 25, 2025
Author: Muhammad Shayan
Status: Active Development
For questions or support, open an issue on GitHub! ⭐

All generated with AI, felt like using, dont mind
