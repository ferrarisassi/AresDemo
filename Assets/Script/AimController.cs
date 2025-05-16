using UnityEngine;

public class AimController : MonoBehaviour
{

    [SerializeField] private GameObject mainCamera;

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
        
    }
}
