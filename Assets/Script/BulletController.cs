using System.Collections;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    [SerializeField] private float instantVelocity;
    [SerializeField] private float countdownTime = 10f;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        this.GetComponent<Rigidbody>().linearVelocity = instantVelocity * this.transform.up;
        StartCoroutine(CountdownRoutine());
    }

    private IEnumerator CountdownRoutine()
    {
        float remainingTime = countdownTime;

        while (remainingTime > 0)
        {
            // Wait for a frame
            yield return null;

            // Decrease the remaining time
            remainingTime -= Time.deltaTime;
        }

        Destroy(this.gameObject);
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.transform.CompareTag("Target"))
        {
            Destroy(collision.transform.gameObject);
            Destroy(this.gameObject);
        }
    }
}
