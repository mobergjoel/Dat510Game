using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

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
    public float stationaryCrouchSightRange, stationarySightRange, crouchSightRange, walkSightRange, sprintSightRange, attackRange, flashLightRange;
    private float sightRange;
    public bool playerInSightRange, playerInAttackRange;

    Animator animator;
    public FirstPersonController playerScript;
    public FlashLight flashLightScript;
    public AudioSource monsterSound1;
    public AudioSource monsterSound2;
    public AudioSource monsterSound3;
    public AudioSource monsterSound4;
    public AudioSource monsterSound5;
    public AudioSource monsterSound6;
    public AudioSource monsterSound7;
    public AudioSource monsterSound8;
    public AudioSource monsterSound9;
    public AudioSource monsterSound10;
    public AudioSource monsterSound11;

    private bool ChasePlayerBool = false;
    

    public float screamCooldown = 20f; // Time in seconds between screams
    private float lastScreamTime = 0f; // Keeps track of the last time the monster scream

    public GameObject MonsterJumpscare;

    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        Debug.Log(ChasePlayerBool);
        if (flashLightScript.getFlashLightOn()) 
        {
            sightRange = flashLightRange;
        }
        else if(playerScript.isWalking)
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

        if (!playerInSightRange && !playerInAttackRange)
        {
            if (ChasePlayerBool)
            {
                walkToPlayer();
            }
            else 
            {
                Patroling();
            }
        }
           
        if (playerInSightRange && !playerInAttackRange) ChasePlayer();
        if (playerInSightRange && playerInAttackRange) AttackPlayer();
    }

    private void Patroling()
    {
           
        if (Time.time - lastScreamTime >= screamCooldown)
        {
            monsterSound4.Play();
            lastScreamTime = Time.time; // Update the last shoot time
        }
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
        if (NavMesh.SamplePosition(randomPoint, out hit, 50f, NavMesh.AllAreas))
        {
            walkPoint = hit.position; // Set walkPoint to the closest valid NavMesh position
            walkPointSet = true;
        }
    }


    private void ChasePlayer()
    {
        ChasePlayerBool = true;
        
        if (Time.time - lastScreamTime >= screamCooldown/4)
        {
            monsterSound1.Play();
            lastScreamTime = Time.time; // Update the last shoot time
        }
        animator.SetBool("isInRange", true);
        agent.SetDestination(player.position);
    }

    private void AttackPlayer()
    {
        //Make sure enemy does't move
        agent.SetDestination(transform.position);

        transform.LookAt(player);

        gameObject.SetActive(false);

        MonsterJumpscare.SetActive(true);

        Invoke("loadGameOverScene", 3f);






        if (!alreadyAttacked)
        {
            //Attack code here


            //


            alreadyAttacked = true;
            Invoke(nameof(ResetAttacked), timeBetweenAttacks);
        }
    }

    

    private void loadGameOverScene()
    { 
        
        SceneManager.LoadScene("GameOverScene");
    }

    private void ResetAttacked()
    {
        alreadyAttacked = false;
    }

    public void walkToPlayer()
    {
        if (Time.time - lastScreamTime >= screamCooldown/5)
        {
            monsterSound11.Play();
            lastScreamTime = Time.time; // Update the last shoot time
        }
        walkPoint = player.transform.position;
        walkPointSet = true;
        ChasePlayerBool = false;
    }

}
