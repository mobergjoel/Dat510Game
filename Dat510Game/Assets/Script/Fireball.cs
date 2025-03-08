using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public LayerMask treeLayer;
    public GameObject fireEffectPrefab; 
    public int numberOfEffects = 7;


    private void OnCollisionEnter(Collision collision)
    {
        if (treeLayer == (treeLayer | (1 << collision.gameObject.layer)))
        {
            IgniteTree(collision.gameObject);
        }
    }

    private void IgniteTree(GameObject tree)
    {
        Collider treeCollider = tree.GetComponent<Collider>();
        Bounds bounds = treeCollider.bounds; 
        for (int i = 0; i < numberOfEffects; i++)
        {
            Vector3 spawnPoint = GetRandomPointOnBounds(bounds);
            GameObject fireEffect = Instantiate(fireEffectPrefab, spawnPoint, Quaternion.identity);
            fireEffect.transform.parent = tree.transform;
        }
        Destroy(tree, 10f);
    }

    // Function to get a random point on the collider's bounds
    private Vector3 GetRandomPointOnBounds(Bounds bounds)
    {
        float randomX = Random.Range(bounds.min.x, bounds.max.x);
        float randomY = Random.Range(bounds.min.y, bounds.max.y);
        float randomZ = Random.Range(bounds.min.z, bounds.max.z);

        return new Vector3(randomX, randomY, randomZ);
    }
    
 
}
