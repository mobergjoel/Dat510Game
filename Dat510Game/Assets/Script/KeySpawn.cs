using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawn : MonoBehaviour
{
    public List<Transform> spawnPoints;
    private Vector3 randomSpawnPoint1;
    private Vector3 randomSpawnPoint2;
    private Vector3 randomSpawnPoint3;


    public GameObject key1;
    public GameObject key2;
    public GameObject key3;
    void Start()
    {
        SpawnKey();
    }

    // Update is called once per frame
    void SpawnKey()
    {
        randomSpawnPoint1 = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        randomSpawnPoint2 = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        while(randomSpawnPoint2 == randomSpawnPoint1)
        {
            randomSpawnPoint2 = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        }
        randomSpawnPoint3 = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        while(randomSpawnPoint3 == randomSpawnPoint1 || randomSpawnPoint3 == randomSpawnPoint2)
        {
            randomSpawnPoint3 = spawnPoints[Random.Range(0, spawnPoints.Count)].position;
        }
        Instantiate(key1, randomSpawnPoint1, Quaternion.identity);
        Instantiate(key2, randomSpawnPoint2, Quaternion.identity);
        Instantiate(key3, randomSpawnPoint3, Quaternion.identity);

    }
}
