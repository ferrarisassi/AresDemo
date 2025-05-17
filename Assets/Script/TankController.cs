using UnityEngine;

/// <summary>
/// A tank controller for Unity
/// Implements differential steering for tank-like movement using treads
/// </summary>
public class TankController : MonoBehaviour
{
    [Header("Tank Specifications")]
    [Tooltip("Maximum speed of the tank in units per second")]
    [SerializeField] private float maxSpeed = 10.0f;
    
    [Tooltip("How quickly the tank accelerates")]
    [SerializeField] private float accelerationForce = 3.0f;
    
    [Tooltip("How quickly the tank decelerates when not accelerating")]
    [SerializeField] private float decelerationForce = 2.0f;
    
    [Tooltip("Maximum rotation speed in degrees per second")]
    [SerializeField] private float maxRotationSpeed = 60.0f;
    
    [Header("Tread Controls")]
    [Tooltip("Tread movement power multiplier")]
    [SerializeField] private float treadPower = 15.0f;
    
    [Tooltip("Maximum tread speed")]
    [SerializeField] private float maxTreadSpeed = 5.0f;
    
    // Variables not in inspector
    private Rigidbody tankRigidbody;
    private float currentSpeed = 0.0f;
    private float leftTreadSpeed = 0.0f;
    private float rightTreadSpeed = 0.0f;

    // Input values
    private float forwardInput = 0.0f;
    private float turnInput = 0.0f;
    
    private void Awake()
    {
        // Get the rigidbody component
        tankRigidbody = GetComponent<Rigidbody>();
        
        // Set the center of mass lower to prevent rollovers
        if (tankRigidbody != null)
        {
            tankRigidbody.centerOfMass = new Vector3(0, -0.8f, 0);
        }
    }
    
    private void Update()
    {
        // Get input (will be replaced with AresGameInput – C++)
        HandleInput();
    }
    
    private void HandleInput()
    {
        // Basic input using Unity's input system
        forwardInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }
    
    private void FixedUpdate()
    {
        // Calculate the current speed
        currentSpeed = Vector3.Dot(tankRigidbody.linearVelocity, transform.forward);

        // Apply tread forces
        ApplyTreadForces();

        // Limit speed
        LimitSpeed();
    }
    
    private void ApplyTreadForces()
    {
        // Calculate tread speeds based on input
        CalculateTreadSpeeds();
        
        // Apply forces from each tread
        ApplyTreadForce(leftTreadSpeed, rightTreadSpeed);
    }
    
    private void CalculateTreadSpeeds()
    {
        // Target speeds based on input
        float targetLeftTread = forwardInput * maxTreadSpeed;
        float targetRightTread = forwardInput * maxTreadSpeed;
        
        // Apply turning by modifying tread speeds differentially
        if (turnInput != 0)
        {
            // For left turns (negative input), slow down left tread
            // For right turns (positive input), slow down right tread
            if (turnInput < 0)
            {
                targetLeftTread -= turnInput * maxTreadSpeed; // This will be negative for left turns
            }
            else
            {
                targetRightTread += turnInput * maxTreadSpeed; // Reduce right tread for right turns
            }
            
            // Allow counter-rotation of treads when stationary for pivot turning
            if (Mathf.Abs(forwardInput) < 0.1f)
            {
                targetLeftTread = -turnInput * maxTreadSpeed * 0.5f;
                targetRightTread = turnInput * maxTreadSpeed * 0.5f;
            }
        }
        
        // Smoothly interpolate current tread speeds towards target speeds
        leftTreadSpeed = Mathf.Lerp(leftTreadSpeed, targetLeftTread, Time.fixedDeltaTime * accelerationForce);
        rightTreadSpeed = Mathf.Lerp(rightTreadSpeed, targetRightTread, Time.fixedDeltaTime * accelerationForce);
    }
    
    private void ApplyTreadForce(float leftSpeed, float rightSpeed)
    {
        // Calculate forward force based on average of tread speeds
        float forwardForce = (leftSpeed + rightSpeed) * 0.5f * treadPower;
        
        // Calculate torque (rotational force) based on the difference between tread speeds
        float torque = (rightSpeed - leftSpeed) * treadPower;
        
        // Apply forces to the tank
        tankRigidbody.AddForce(transform.forward * forwardForce, ForceMode.Force);
        tankRigidbody.AddTorque(transform.up * torque, ForceMode.Force);
        
        // Natural deceleration when no input
        if (Mathf.Abs(forwardInput) < 0.1f && Mathf.Abs(turnInput) < 0.1f)
        {
            tankRigidbody.linearDamping = decelerationForce;
        }
        else
        {
            tankRigidbody.linearDamping = 0.1f; // Lower drag when player is controlling
        }
    }
    
    private void LimitSpeed()
    {
        // Get current velocity
        Vector3 forwardVelocity = Vector3.Project(tankRigidbody.linearVelocity, transform.forward);
        float currentForwardSpeed = forwardVelocity.magnitude * Mathf.Sign(Vector3.Dot(forwardVelocity, transform.forward));
        
        // Check if we're exceeding max speed
        if (Mathf.Abs(currentForwardSpeed) > maxSpeed)
        {
            // Calculate limited velocity
            float speedOverLimit = Mathf.Abs(currentForwardSpeed) - maxSpeed;
            Vector3 limitForce = -forwardVelocity.normalized * speedOverLimit * 50; // Multiplier for more responsive limiting
            
            // Apply limiting force
            tankRigidbody.AddForce(limitForce, ForceMode.Acceleration);
        }
        
        // Limit rotation speed
        float currentAngularSpeed = tankRigidbody.angularVelocity.y * Mathf.Rad2Deg; // Convert to degrees per second
        if (Mathf.Abs(currentAngularSpeed) > maxRotationSpeed)
        {
            // Calculate limited angular velocity
            float targetAngularVelocity = Mathf.Sign(currentAngularSpeed) * maxRotationSpeed * Mathf.Deg2Rad;
            Vector3 newAngularVelocity = new Vector3(
                tankRigidbody.angularVelocity.x,
                targetAngularVelocity,
                tankRigidbody.angularVelocity.z
            );
            
            // Apply limited angular velocity
            tankRigidbody.angularVelocity = newAngularVelocity;
        }
    }
    
    // Public methods for external control with AresGameInput – C++
    
    /// <summary>
    /// Sets the forward/backward input value between -1 and 1
    /// </summary>
    public void SetForwardInput(float input)
    {
        forwardInput = Mathf.Clamp(input, -1f, 1f);
    }
    
    /// <summary>
    /// Sets the turning input value between -1 (left) and 1 (right)
    /// </summary>
    public void SetTurnInput(float input)
    {
        turnInput = Mathf.Clamp(input, -1f, 1f);
    }
    
    /// <summary>
    /// Gets the current forward speed in units per second
    /// </summary>
    public float GetCurrentSpeed()
    {
        return currentSpeed;
    }
    
    /// <summary>
    /// Gets the current speed as a percentage of max speed
    /// </summary>
    public float GetSpeedPercentage()
    {
        return Mathf.Abs(currentSpeed) / maxSpeed;
    }
    
    /// <summary>
    /// Gets the current left tread speed
    /// </summary>
    public float GetLeftTreadSpeed()
    {
        return leftTreadSpeed;
    }
    
    /// <summary>
    /// Gets the current right tread speed
    /// </summary>
    public float GetRightTreadSpeed()
    {
        return rightTreadSpeed;
    }
}
