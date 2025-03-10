using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class MonsterSpawn : MonoBehaviour
{

    public List<Transform> spawnPoints;
    public NavMeshAgent agent;

    void Start()
    {
        TeleportToRandomPosition();
    }

    public void TeleportToRandomPosition()
    {

        Vector3 targetPosition = spawnPoints[Random.Range(0, spawnPoints.Count)].position;

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
