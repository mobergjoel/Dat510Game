using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CircularBorder : MonoBehaviour
{
    public GameObject arenaPrefab; // The prefab of the box you want to arrange
    public int numberOfPrefabs = 12; // How many boxes in the circle
    public float radius = 5f; // Radius of the circle

    void Start()
    {
        Vector3 centerPosition = transform.position;

        for (int i = 0; i < numberOfPrefabs; i++)
        {
            float angle = i * Mathf.PI * 2f / numberOfPrefabs;
            Vector3 offset = new Vector3(Mathf.Cos(angle) * radius, 0, Mathf.Sin(angle) * radius);
            Vector3 spawnPosition = centerPosition + offset;
            
            // Calculate the rotation to face the center
            Vector3 directionToCenter = centerPosition - spawnPosition;
            Quaternion rotation = Quaternion.LookRotation(directionToCenter);
            
            // Apply an additional 90-degree rotation around the X-axis if needed
            rotation *= Quaternion.Euler(90, 0, 0);
            
            GameObject spawnedObject = Instantiate(arenaPrefab, spawnPosition, rotation);
            spawnedObject.transform.SetParent(transform);

        }
    }
    
    
}
