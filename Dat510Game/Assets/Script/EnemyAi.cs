using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.SceneManagement;

public class EnemyAi : MonoBehaviour
{
    public NavMeshAgent agent;

    public Transform player;

    public PlayerHealth Health;

    public LayerMask whatIsGround, whatIsPlayer;

    //Patroling
    public Vector3 walkPoint;
    bool walkPointSet;
    public float walkPointRange;

    //Attacking
    public float timeBetweenAttacks;
    bool alreadyAttacked;

    //States
    public float stationaryCrouchSightRange, stationarySightRange, crouchSightRange, walkSightRange, sprintSightRange, attackRange, flashLightRange, searchRange;
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

    private float lastGroundAttack = 0f;
    private float GroundAttackCoolDown = 20f;

    private float lastThrowAttack = 0f;
    private float throwAttackCoolDown = 8f;

    public GameObject MonsterJumpscare;
    public GroundAttack groundAttack;

    public float distanceForProjectileThrow = 10;
    public float timeBetweenProjectileThrows = 5;
    public float damageForProjectile = 25;
    public float projectileThrowAnimationTimeOffset = 0f;
    public GameObject projectile;
    Rigidbody projectileRB;

    Vector3 projectileOrigin;

    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        projectile = transform.Find("Projectile").gameObject;
        projectileRB = projectile.GetComponent<Rigidbody>();
        projectileOrigin = new Vector3(projectile.transform.localPosition.x, projectile.transform.localPosition.y, projectile.transform.localPosition.z);
    }

    private void Update()
    {
        
        if (animator.GetBool("BossBattle"))
        {
            sightRange = walkSightRange;
        }
        else if (flashLightScript.getFlashLightOn()) 
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

        if (!agent.isOnNavMesh)
        {
            Debug.Log("Monster är inte på NavMesh!");
        }

        else if (!playerInSightRange && !playerInAttackRange)
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
        if (animator.GetBool("BossBattle"))
        {
           
            agent.SetDestination(transform.position);
            transform.LookAt(player);
            if (Time.time - lastGroundAttack >= GroundAttackCoolDown)
            {
                
                //GroundAttack();
                lastGroundAttack = Time.time; // Update the last shoot time
                
            }
            else if(Time.time - lastThrowAttack >= throwAttackCoolDown)
            {
                ThrowAttack();
                lastThrowAttack = Time.time;
            }
            else
            {
                animator.SetBool("GroundAttack", false);
                
            }
            
        }
        else
        {
            if (Time.time - lastScreamTime >= screamCooldown / 4)
            {
                monsterSound1.Play();
                lastScreamTime = Time.time; // Update the last shoot time
            }
            
            agent.SetDestination(player.position);
        }
        animator.SetBool("isInRange", true);
        ChasePlayerBool = true;

    }

    private void ThrowAttack()
    {
        ResetProjectile();
        StartCoroutine(ThrowProjectile());
    }

    private void GroundAttack()
    {
        animator.SetBool("GroundAttack", true); // Börja markattackanimationen

        // Vänta i 5 sekunder
        

        // Anropa metoden för att trigga markattack-vågen
        groundAttack.TriggerWave();
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
        animator.SetBool("isInRange", false);
    }

    IEnumerator ThrowProjectile() 
    {
        animator.SetTrigger("Throw");
        yield return new WaitForSeconds(projectileThrowAnimationTimeOffset);
        projectileRB.isKinematic = false;
        projectile.transform.parent = null;
        projectile.transform.LookAt(player);
        projectileRB.AddForce(projectileRB.transform.forward * 1000);
        
        

        
        
    }
    void ResetProjectile()
    {
        projectile.transform.parent = transform;
        projectileRB.isKinematic = true;
        projectile.transform.localPosition = new Vector3(projectileOrigin.x, projectileOrigin.y, projectileOrigin.z);

    }
    public void ProjectileCollision(Collision other)
    {
        if (other.transform.tag == "Player")
        {
            Health.Hit(damageForProjectile);
        }
    }
}
