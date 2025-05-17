using UnityEngine;
using System.Collections.Generic;

public class TargetSpawner : MonoBehaviour
{
    [SerializeField] private GameObject targetPrefab;
    [SerializeField] private int numberOfTargets = 3;
    [SerializeField] private Vector3 spawnAreaMin = new Vector3(-10f, -5f, 0f);
    [SerializeField] private Vector3 spawnAreaMax = new Vector3(10f, 5f, 0f);

    private List<GameObject> activeTargets = new List<GameObject>();

    private void Start()
    {
        SpawnTargets();
    }

    private void SpawnTargets()
    {
        for (int i = 0; i < numberOfTargets; i++)
        {
            SpawnRandomTarget();
        }
    }

    private void SpawnRandomTarget()
    {
        // Generate random position within spawn area
        Vector3 spawnPosition = new Vector3(
            Random.Range(spawnAreaMin.x, spawnAreaMax.x),
            Random.Range(spawnAreaMin.y, spawnAreaMax.y),
            Random.Range(spawnAreaMin.z, spawnAreaMax.z)
        );

        // Instantiate the target
        GameObject target = Instantiate(targetPrefab, spawnPosition, Quaternion.identity);

        // Add to active targets list
        activeTargets.Add(target);
    }
}