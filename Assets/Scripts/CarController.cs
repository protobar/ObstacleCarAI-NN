using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// AI-controlled car using neural network - FIXED FORWARD MOVEMENT
/// </summary>
public class CarController : MonoBehaviour
{
    [Header("Neural Network")]
    public NeuralNetwork brain;

    [Header("Sensors")]
    public int numberOfRays = 5;
    public float rayDistance = 10f;
    public float rayAngle = 60f;
    public LayerMask obstacleLayer;
    public float rayHeight = 0.5f; // Height offset for raycasts

    [Header("Movement")]
    public float maxSpeed = 10f;
    public float acceleration = 5f;
    public float turnSpeed = 100f;

    [Header("Fitness")]
    public float fitness = 0f;
    public float distanceTraveled = 0f;
    public float timeSurvived = 0f;
    public int checkpointsReached = 0;

    private Rigidbody rb;
    private Vector3 lastPosition;
    private bool isDead = false;
    private float currentSpeed = 0f;
    public float[] sensorInputs; // Made public for UI visualization

    // Fitness shaping variables
    private float avgSpeedSum = 0f;
    private int speedSampleCount = 0;
    private float idleTime = 0f;
    private HashSet<int> reachedCheckpoints = new HashSet<int>();

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        lastPosition = transform.position;

        // Initialize neural network with appropriate layer sizes
        // Input: ray sensors + speed + angular velocity
        if (brain == null)
        {
            brain = new NeuralNetwork(new int[] { numberOfRays + 2, 8, 4, 2 });
            brain.activationType = NeuralNetwork.ActivationType.LeakyReLU;
        }

        sensorInputs = new float[numberOfRays + 2];
    }

    void FixedUpdate()
    {
        if (isDead) return;

        // Update sensors
        UpdateSensors();

        // Get decision from neural network
        float[] outputs = brain.FeedForward(sensorInputs);

        // Apply outputs to movement (outputs are -1 to 1)
        float accelerationInput = outputs[0];
        float steeringInput = outputs[1];

        // Update speed
        currentSpeed += accelerationInput * acceleration * Time.fixedDeltaTime;
        currentSpeed = Mathf.Clamp(currentSpeed, 0f, maxSpeed);

        // FIXED: Move in the actual forward direction of the car
        Vector3 movement = transform.forward * currentSpeed * Time.fixedDeltaTime;
        rb.MovePosition(rb.position + movement);

        // Apply steering
        float turnAmount = steeringInput * turnSpeed * Time.fixedDeltaTime;
        Quaternion turnRotation = Quaternion.Euler(0f, turnAmount, 0f);
        rb.MoveRotation(rb.rotation * turnRotation);

        // Update fitness tracking
        timeSurvived += Time.fixedDeltaTime;
        float distanceThisFrame = Vector3.Distance(transform.position, lastPosition);
        distanceTraveled += distanceThisFrame;
        lastPosition = transform.position;

        // Track average speed
        avgSpeedSum += currentSpeed;
        speedSampleCount++;

        // Penalize staying idle
        if (currentSpeed < 0.1f)
            idleTime += Time.fixedDeltaTime;

        // Enhanced fitness calculation
        float avgSpeed = speedSampleCount > 0 ? avgSpeedSum / speedSampleCount : 0f;
        float speedBonus = avgSpeed * 0.3f;
        float idlePenalty = idleTime * 0.5f;
        float checkpointBonus = checkpointsReached * 50f;

        fitness = distanceTraveled + (timeSurvived * 0.5f) + speedBonus - idlePenalty + checkpointBonus;
    }

    void UpdateSensors()
    {
        float angleStep = rayAngle / (numberOfRays - 1);
        float startAngle = -rayAngle / 2f;

        for (int i = 0; i < numberOfRays; i++)
        {
            float angle = startAngle + (angleStep * i);
            // FIXED: Cast rays in the FORWARD direction relative to car
            Vector3 direction = Quaternion.Euler(0, angle, 0) * transform.forward;
            Vector3 rayOrigin = transform.position + Vector3.up * rayHeight;

            RaycastHit hit;
            if (Physics.Raycast(rayOrigin, direction, out hit, rayDistance, obstacleLayer))
            {
                // Normalize distance (0 = far, 1 = close)
                sensorInputs[i] = 1f - (hit.distance / rayDistance);
                Debug.DrawRay(rayOrigin, direction * hit.distance, Color.red);
            }
            else
            {
                sensorInputs[i] = 0f;
                Debug.DrawRay(rayOrigin, direction * rayDistance, Color.green);
            }
        }

        // Add normalized speed and angular velocity
        sensorInputs[numberOfRays] = currentSpeed / maxSpeed;
        sensorInputs[numberOfRays + 1] = Mathf.Clamp(rb.angularVelocity.y / 100f, -1f, 1f);
    }

    public void Die()
    {
        isDead = true;
        currentSpeed = 0f;
    }

    public bool IsDead()
    {
        return isDead;
    }

    public void ResetCar(Vector3 position, Quaternion rotation)
    {
        isDead = false;
        fitness = 0f;
        distanceTraveled = 0f;
        timeSurvived = 0f;
        currentSpeed = 0f;
        avgSpeedSum = 0f;
        speedSampleCount = 0;
        idleTime = 0f;
        checkpointsReached = 0;
        reachedCheckpoints.Clear();

        transform.position = position;
        transform.rotation = rotation;
        lastPosition = position;

        if (rb != null)
        {
            rb.velocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (((1 << collision.gameObject.layer) & obstacleLayer) != 0)
        {
            Die();
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Checkpoint"))
        {
            int checkpointID = other.GetInstanceID();
            if (!reachedCheckpoints.Contains(checkpointID))
            {
                reachedCheckpoints.Add(checkpointID);
                checkpointsReached++;
            }
        }
    }

    // Visual debugging in Scene view
    void OnDrawGizmos()
    {
        if (!Application.isPlaying) return;

        // Draw car status
        Gizmos.color = isDead ? Color.red : Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.5f);

        // Draw forward direction indicator
        Gizmos.color = Color.blue;
        Gizmos.DrawRay(transform.position, transform.forward * 2f);

#if UNITY_EDITOR
        // Draw fitness label
        UnityEditor.Handles.Label(transform.position + Vector3.up * 2, $"Fit: {fitness:F1}");
#endif
    }
}
