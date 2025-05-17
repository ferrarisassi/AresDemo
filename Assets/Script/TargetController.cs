using UnityEngine;

public class TargetController : MonoBehaviour
{

    [SerializeReference] private int patternIndex;
    private bool isDead = false;
    private Vector3 initialPosition;
    private float elapsedTime = 0f;
    [Header("Horizontal Movement Settings")]
    [SerializeField] private float speedHorizontal = 3f;
    [SerializeField] private float movementRangeHorizontal = 5f;
    [Header("Circular Movement Settings")]
    [SerializeField] private float speedCircular = 3f;
    [SerializeField] private float movementRangeCircular = 5f;
    [Header("Sign Movement Settings")]
    [SerializeField] private float horizontalRange = 5f;
    [SerializeField] private float horizontalSpeed = 2f;
    [SerializeField] private float verticalFrequency = 3f;
    [SerializeField] private float verticalRange = 5f;
    
    private Vector3 currentPosition;
    private int horizontalDirection = 1; // 1 = right, -1 = left
    private Camera mainCamera;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        initialPosition = this.transform.position;
        patternIndex = Random.Range(0, 3);
        mainCamera = Camera.main;
    }

    // Update is called once per frame
    void Update()
    {
        if (!isDead)
        {
            FaceCamera();
            switch (patternIndex)
            {
                case 0:
                    UpdateSignMovement();
                    break;
                case 1:
                    UpdateHorizontalMovement();
                    break;
                case 2:
                    UpdateCircularMovement();
                    break;
            }
        }
    }
    
    private void FaceCamera()
    {
        // Calculate direction from target to camera
        Vector3 directionToCamera = mainCamera.transform.position - transform.position;
        
        // Remove any y component to only rotate around y-axis (if in 3D space)
        // Comment out this line if you want full billboard effect
        directionToCamera.y = 0;
        
        // Make the target look at the camera
        if (directionToCamera != Vector3.zero)
        {
            transform.rotation = Quaternion.LookRotation(directionToCamera);
        }
    }

    public void UpdateSignMovement()
    {
        elapsedTime += Time.deltaTime;
        
        // Calculate vertical sine wave movement
        float yOffset = Mathf.Sin(elapsedTime * verticalFrequency) * verticalRange;
        
        // Calculate horizontal linear movement with direction change
        float xMovement = horizontalSpeed * Time.deltaTime * horizontalDirection;
        currentPosition.x += xMovement;
        
        // Check if we need to change direction
        if (currentPosition.x > initialPosition.x + horizontalRange)
        {
            currentPosition.x = initialPosition.x + horizontalRange;
            horizontalDirection = -1; // Change direction to left
        }
        else if (currentPosition.x < initialPosition.x - horizontalRange)
        {
            currentPosition.x = initialPosition.x - horizontalRange;
            horizontalDirection = 1; // Change direction to right
        }
        
        // Apply both movements
        transform.position = new Vector3(currentPosition.x, initialPosition.y + yOffset, initialPosition.z);
    }

    public void UpdateHorizontalMovement()
    {
        elapsedTime += Time.deltaTime;
        float xOffset = Mathf.Sin(elapsedTime * speedHorizontal) * movementRangeHorizontal;
        transform.position = initialPosition + new Vector3(xOffset, 0, 0);
    }

    public void UpdateCircularMovement()
    {
        elapsedTime += Time.deltaTime;
        float xOffset = Mathf.Cos(elapsedTime * speedCircular) * movementRangeCircular;
        float yOffset = Mathf.Sin(elapsedTime * speedCircular) * movementRangeCircular;
        transform.position = initialPosition + new Vector3(xOffset, yOffset, 0);
    }
}
