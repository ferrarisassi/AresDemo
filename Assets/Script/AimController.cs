using UnityEngine;
using TMPro;

public class AimController : MonoBehaviour
{

    [Header("Rotation Settings")]
    [Tooltip("MainCamera")]
    [SerializeField] private GameObject mainCamera;

    [Tooltip("Vertical (pitch) rotation limit - minimum angle in degrees")]
    [SerializeField] private float minVerticalAngle = -10f;
    
    [Tooltip("Vertical (pitch) rotation limit - maximum angle in degrees")]
    [SerializeField] private float maxVerticalAngle = 60f;

    [Tooltip("Horizontal rotation velocity")]
    [SerializeField] private float horizontalVelocity = 2.0f;
    
    [Tooltip("Vertical rotation velocity")]
    [SerializeField] private float verticalVelocity = 2.0f;

    [Tooltip("Horizontal UI Text")]
    [SerializeField] private TMP_Text horizontalUI;
    
    [Tooltip("Vertical UI Text")]
    [SerializeField] private TMP_Text verticalUI;

    [Tooltip("Object that follows the horizontal rotation")]
    [SerializeField] private GameObject horizontalObject;
    
    [Tooltip("Object that follows the vertical rotation")]
    [SerializeField] private GameObject verticalObject;

    // Variables not in inspector
    private float currentHorizontalAngle = 0f;  
    private float currentVerticalAngle = 0f; 

    // Input values
    private float upInput = 0.0f;
    private float turnInput = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = GetComponentInChildren<Camera>().gameObject;
        }
        currentHorizontalAngle = transform.eulerAngles.y;
        currentVerticalAngle = transform.eulerAngles.x;
    }

    // Update is called once per frame
    void Update()
    {
        // Get input (will be replaced with AresGameInput – C++)
        HandleInput();
    }

    private void HandleInput()
    {
        // Basic input using Unity's input system
        upInput = Input.GetAxis("VerticalAim");
        turnInput = Input.GetAxis("HorizontalAim");
    }

    void LateUpdate()
    {
        MoveCamera();
    }

    private void MoveCamera()
    {
        currentHorizontalAngle += turnInput * horizontalVelocity;
        currentVerticalAngle -= upInput * verticalVelocity;

        // Apply vertical angle limits
        currentVerticalAngle = Mathf.Clamp(currentVerticalAngle, minVerticalAngle, maxVerticalAngle);

        // Normalize horizontal angle to keep it in the 0-360 range
        if (currentHorizontalAngle >= 360f)
        {
            currentHorizontalAngle -= 360f;
        }
        else if (currentHorizontalAngle < 0f)
        {
            currentHorizontalAngle += 360f;
        }

        //Set the angle in the UI
        if (horizontalUI!=null) horizontalUI.text = currentHorizontalAngle.ToString("0.00");
        if (verticalUI!=null) verticalUI.text = (-currentVerticalAngle).ToString("0.00");

        //Rotate Objects
        if (horizontalObject != null) {
            Quaternion ObjRotation = horizontalObject.transform.rotation;
            horizontalObject.transform.rotation = Quaternion.Euler(ObjRotation.eulerAngles.x, currentHorizontalAngle, ObjRotation.eulerAngles.z);
        }
        if (verticalObject != null) {
            Quaternion ObjRotation = verticalObject.transform.rotation;
            verticalObject.transform.rotation = Quaternion.Euler(currentVerticalAngle, ObjRotation.eulerAngles.y, ObjRotation.eulerAngles.z);
        }

        // Create local rotation based on our input values
        Quaternion localRotation = Quaternion.Euler(currentVerticalAngle, currentHorizontalAngle, 0);
            
        // Apply the rotation to the camera (using parent's world rotation as reference)
        mainCamera.transform.rotation = mainCamera.transform.parent.rotation * localRotation;
    }
}
