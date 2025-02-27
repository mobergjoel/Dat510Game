using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class MonsterSpawn : MonoBehaviour
{

    public List<Transform> spawnPoints; // Lägg in 4 positioner i Unity Inspector
    public NavMeshAgent agent;

    void Start()
    {
        TeleportToRandomPosition(); // Teleportera direkt vid start
    }

    public void TeleportToRandomPosition()
    {

        // Väljer en slumpmässig position
        Vector3 targetPosition = spawnPoints[Random.Range(0, spawnPoints.Count)].position;

        // Teleportera agenten dit
        if (agent.isOnNavMesh)
        {
            agent.Warp(targetPosition);
        }
        else
        {
            Debug.LogError("Agenten är inte på NavMesh!");
        }
    }
}
