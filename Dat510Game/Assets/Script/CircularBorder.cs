using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularBorder : MonoBehaviour
{
    public GameObject arenaPrefab;
    public int numberOfPrefabs = 45; 
    public float radius = 1f;

    void Start()
    {
        Vector3 centerPosition = transform.position;

        for (int i = 0; i < numberOfPrefabs; i++)
        {
            float angle = i * Mathf.PI * 2f / numberOfPrefabs;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Vector3 spawnPosition = centerPosition + offset;
            
            Vector3 directionToCenter = centerPosition - spawnPosition;
            Quaternion rotation = Quaternion.LookRotation(directionToCenter);
            
            rotation *= Quaternion.Euler(90, 0, 0);
            
            GameObject spawnedObject = Instantiate(arenaPrefab, spawnPosition, rotation);
            spawnedObject.transform.SetParent(transform);

        }
    }
    
    
}
