using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeySpawn : MonoBehaviour
{
    public List<Transform> spawnPoints;
    private Vector3 randomSpawnPoint1;
    private Vector3 randomSpawnPoint2;
    private Vector3 randomSpawnPoint3;

    private Dictionary<Transform, string> spawnPointParents = new Dictionary<Transform, string>();

    public GameObject key1;
    public GameObject key2;
    public GameObject key3;
    void Start()
    {
        foreach (var spawnPoint in spawnPoints)
        {
            spawnPointParents[spawnPoint] = spawnPoint.parent.name;
        }
        SpawnKey();
    }

    // Update is called once per frame
    void SpawnKey()
    {
        List<Transform> selectedSpawnPoints = new List<Transform>();
        HashSet<string> usedParentLocations = new HashSet<string>();

        while (selectedSpawnPoints.Count < 3)
        {
            Transform potentialSpawnPoint = spawnPoints[Random.Range(0, spawnPoints.Count)];
            string parentLocation = spawnPointParents[potentialSpawnPoint];

            if(!usedParentLocations.Contains(parentLocation) && !selectedSpawnPoints.Contains(potentialSpawnPoint))
            {
                selectedSpawnPoints.Add(potentialSpawnPoint);
                usedParentLocations.Add(parentLocation);
            }
        }



        Instantiate(key1, selectedSpawnPoints[0].position, selectedSpawnPoints[0].rotation);
        Instantiate(key2, selectedSpawnPoints[1].position, selectedSpawnPoints[1].rotation);
        Instantiate(key3, selectedSpawnPoints[2].position, selectedSpawnPoints[2].rotation);

    }
}
