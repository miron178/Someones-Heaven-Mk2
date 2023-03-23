using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField]
    GameObject spawnable;
    [SerializeField]
    int spawnableInitialCount;
    [SerializeField]
    int spawnableMore = 1;
    [SerializeField]
    float spawnInterval = 0f;

    [SerializeField]
    float maxDistance;

    [Tooltip("Number of spawn cycles, negative for infinate cycles")]
    [SerializeField]
    int cycleCount;
    int currentCycle = 0;

    float lastUpdate;

    public Color color = Color.red;

    void Start()
    {
        lastUpdate = Time.time;
        spawn(spawnableInitialCount);
    }

    void Update()
    {
        spawnMore();
    }

    void spawn(int count)
    {
        for (int j = 0; j < count; j++)
        {
            GameObject go = Instantiate(spawnable, SpawnPosition(), Quaternion.identity);
        }
    }

    void spawnMore()
    {
        if (spawnInterval > 0 && cycleCount != 0)
        {
            //cycleCount < 0 for inifinite spawn
            if (cycleCount < 0 || currentCycle < cycleCount)
            {
                float now = Time.time;
                float timePassed = now - lastUpdate;

                if (timePassed > spawnInterval)
                {
                    lastUpdate = now;
                    spawn(spawnableMore);
                    currentCycle++;
                }
            }
        }
    }

    Vector3 SpawnPosition()
    {
        float x = Random.Range(-maxDistance, maxDistance);
        float z = Random.Range(-maxDistance, maxDistance);
        return transform.position + new Vector3(x, 0f, z);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = color;
        Gizmos.DrawCube(transform.position, new Vector3(2 * maxDistance, 1, 2 * maxDistance));
    }
}
