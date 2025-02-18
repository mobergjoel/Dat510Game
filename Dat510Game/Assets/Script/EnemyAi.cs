using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public LayerMask whatIsGround, whatIsPlayer;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float stationaryCrouchSightRange, stationarySightRange, crouchSightRange, walkSightRange, sprintSightRange, attackRange;
    private float sightRange;
    public bool playerInSightRange, playerInAttackRange;

    Animator animator;
    public FirstPersonController playerScript;

    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if(playerScript.isWalking)
        {
            if(playerScript.isCrouched)
            {
                sightRange = crouchSightRange;
            }
            else
            {
                sightRange = walkSightRange;
            }  
        }
        else if(playerScript.isSprinting) 
        {
            sightRange = sprintSightRange;
        }
        else if (playerScript.isCrouched && !playerScript.isWalking)
        {
            sightRange = stationaryCrouchSightRange;
        }
        else
        {
            sightRange = stationarySightRange;
        }
        //Check for sight and attack range
        playerInSightRange = Physics.CheckSphere(transform.position, sightRange, whatIsPlayer);
        playerInAttackRange = Physics.CheckSphere(transform.position, attackRange, whatIsPlayer);

        if (!playerInSightRange && !playerInAttackRange) Patroling();
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
        animator.SetBool("isInRange", false);
        if (!walkPointSet) SearchWalkPoint();

        if (walkPointSet) agent.SetDestination(walkPoint);

        Vector3 distanceToWalkPoint = transform.position - walkPoint;

        //Walkpoint reached
        if (distanceToWalkPoint.magnitude < 2f) walkPointSet = false;
    }

    private void SearchWalkPoint()
    {
        // Generate a random point in range
        float randomZ = Random.Range(-walkPointRange, walkPointRange);
        float randomX = Random.Range(-walkPointRange, walkPointRange);

        Vector3 randomPoint = new Vector3(transform.position.x + randomX, transform.position.y, transform.position.z + randomZ);

        // Check if the random point is on the NavMesh
        NavMeshHit hit;
        if (NavMesh.SamplePosition(randomPoint, out hit, 2f, NavMesh.AllAreas))
        {
            walkPoint = hit.position; // Set walkPoint to the closest valid NavMesh position
            walkPointSet = true;
        }
    }


    private void ChasePlayer()
    {
        animator.SetBool("isInRange", true);
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy does't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        if (!alreadyAttacked)
        {
            //Attack code here


            //


            alreadyAttacked = true;
            Invoke(nameof(ResetAttacked), timeBetweenAttacks);
        }
    }
    private void ResetAttacked()
    {
        alreadyAttacked = false;
    }

    public void walkToPlayer()
    {
        walkPoint = player.transform.position;
    }

}
