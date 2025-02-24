using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawn : MonoBehaviour
{
    public List<Transform> spawnPoints;
    private Transform randomSpawnPoint;

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
        randomSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
        Instantiate(key1, spawnPoints[0].position, Quaternion.identity);
        Instantiate(key2, spawnPoints[1].position, Quaternion.identity);
        Instantiate(key3, spawnPoints[2].position, Quaternion.identity);

    }
}
