using UnityEngine;

public class AimController : MonoBehaviour
{

    [SerializeField] private GameObject mainCamera;

    private float upInput = 0.0f;
    private float turnInput = 0.0f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = GetComponentInChildren<Camera>().gameObject;
        }
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
        upInput = Input.GetAxis("Vertical");
        turnInput = Input.GetAxis("Horizontal");
    }
}
