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

    private float lastStompAttack = 0f;
    private float StompAttackCoolDown = 7f;

    private float lastRushAttack = 0f;
    private float rushAttackCoolDown = 12f;

    private float lastThrowAttack = 0f;
    private float throwAttackCoolDown = 3f;

    public GameObject MonsterJumpscare;
    public GroundAttack groundAttack;

    public float distanceForProjectileThrow = 10;
    public float timeBetweenProjectileThrows = 5;
    public float damageForProjectile = 25;
    public float damageForStomp = 35;
    public float damageForRush = 60;
    public float projectileThrowAnimationTimeOffset = 0f;
    public float shockWaveAnimationTimeOffset = 0.7f;
    public GameObject projectile;
    Rigidbody projectileRB;

    Vector3 projectileOrigin;

    ParticleSystem shockWavePS;
    float distanceToPlayer;
    public float distanceForStomp = 10;
    float countdownTimer;
    float monsterSpeed;
    float monsteracceleration;
    float lastThrowAttackCoolDown = 3.5f;
    float lastStompAttackCoolDown = 4f;
    float lastAttack = 0f;
    float lastRushAttackCoolDown = 5f;
    bool searchingFarAway = true;
    public bool canAttack = true;
    bool RushAttackBool = false;
    bool throwAttackBool = false;
    bool fromWalkToPlayerToBossBattle = true;


    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        agent = GetComponent<NavMeshAgent>();
        animator = GetComponent<Animator>();
        projectile = transform.Find("Projectile").gameObject;
        projectileRB = projectile.GetComponent<Rigidbody>();
        projectileOrigin = new Vector3(projectile.transform.localPosition.x, projectile.transform.localPosition.y, projectile.transform.localPosition.z);
        shockWavePS = transform.Find("ShockWave").GetChild(0).GetComponent<ParticleSystem>();
        monsteracceleration = agent.acceleration;
        projectile.SetActive(false);
    }

    private void Update()
    {
        distanceToPlayer = Vector3.Distance(player.position, transform.position);

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
                if (animator.GetBool("BossBattle"))
                {
                    fromWalkToPlayerToBossBattle = true;
                    RushAttackBool = false;
                }
                
            }
            else 
            {
                Patroling();
            }
        }
           
        if (playerInSightRange && (!playerInAttackRange || !canAttack)) ChasePlayer();
        if (playerInSightRange && playerInAttackRange && canAttack) AttackPlayer();
        


        if(distanceToPlayer > 100 && searchingFarAway)
        {
            monsterSpeed = agent.speed;
            agent.speed = agent.speed * 2;
            agent.acceleration = agent.speed * 1000;
            searchingFarAway = false;
           
        }
        else if (RushAttackBool)
        {
            
            searchingFarAway = true;
        }
        else
        {
            agent.speed = monsterSpeed;
            agent.acceleration = monsteracceleration;
            searchingFarAway = true;
        }

        
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
            if (fromWalkToPlayerToBossBattle)
            {
                lastAttack = Time.time;
                fromWalkToPlayerToBossBattle = false;
            }
            if(!RushAttackBool)
            {
                agent.SetDestination(agent.transform.position);
                transform.LookAt(player);
            }
            

            if (Time.time - lastStompAttack >= StompAttackCoolDown && Time.time - lastAttack >= lastThrowAttackCoolDown && distanceToPlayer <= distanceForStomp && Time.time - lastRushAttack >= lastRushAttackCoolDown && !RushAttackBool && !throwAttackBool)
            {
                StartCoroutine(StompShockWave());
                lastStompAttack = Time.time;
                lastAttack = Time.time;
            }
            else if (Time.time - lastRushAttack >= rushAttackCoolDown && Time.time - lastAttack >= lastStompAttackCoolDown && Time.time - lastAttack >= lastThrowAttackCoolDown && !throwAttackBool)
            {
               
                RushAttackBool = true;
                RushAttack();
                lastRushAttack = Time.time;
                lastAttack = Time.time;
            }
            else if (Time.time - lastThrowAttack >= throwAttackCoolDown && Time.time - lastAttack >= lastStompAttackCoolDown && Time.time - lastRushAttack >= lastRushAttackCoolDown && !RushAttackBool && !throwAttackBool)
            {
                projectile.SetActive(true);
                float random = Random.Range(0, 3);
                if(random == 0) 
                {
                    Throw1Attack();
                }
                else if(random == 1)
                {
                    Throw2Attack();
                }
                else
                {
                    Throw3Attack();
                }
                throwAttackBool = true;
                
            }
            
            if(RushAttackBool)
            {
                Vector3 distanceToWalkPoint = transform.position - walkPoint;
                if (distanceToWalkPoint.magnitude < 2f)
                {
                    animator.SetBool("RushAttack", false);
                    RushAttackBool = false;
                    agent.speed = monsterSpeed;
                    agent.acceleration = monsteracceleration;
                    walkPointSet = false;
                }
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

    

    private void Throw1Attack()
    {
        ResetProjectile();
        StartCoroutine(Throw1Projectile());
    }

    private void Throw2Attack()
    {
        ResetProjectile();
        StartCoroutine(Throw2Projectile());
    }
    private void Throw3Attack()
    {
        ResetProjectile();
        StartCoroutine(Throw3Projectile());
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
        if (!animator.GetBool("BossBattle"))
        {
            //Make sure enemy does't move
            agent.SetDestination(transform.position);

            transform.LookAt(player);

            gameObject.SetActive(false);

            MonsterJumpscare.SetActive(true);

            Invoke("loadGameOverScene", 3f);
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
    IEnumerator StompShockWave()
    {
        animator.SetTrigger("Jump");

        yield return new WaitForSeconds(shockWaveAnimationTimeOffset);

        shockWavePS.Play();
    }

    

    IEnumerator Throw1Projectile() 
    {
        lastThrowAttack = Time.time;
        lastAttack = Time.time;
        animator.SetTrigger("Throw");
        yield return new WaitForSeconds(projectileThrowAnimationTimeOffset);
        projectileRB.isKinematic = false;
        projectile.transform.parent = null;
        projectile.transform.LookAt(player);
        projectileRB.AddForce(projectileRB.transform.forward * 1000);

        throwAttackBool = false;
 
    }
    IEnumerator Throw2Projectile() 
    {
        animator.SetTrigger("Throw");
        yield return new WaitForSeconds(projectileThrowAnimationTimeOffset);
        projectileRB.isKinematic = false;
        projectile.transform.parent = null;
        projectile.transform.LookAt(player);
        projectileRB.AddForce(projectileRB.transform.forward * 1000);

        yield return new WaitForSeconds(2);
        ResetProjectile();

        lastThrowAttack = Time.time;
        lastAttack = Time.time;

        animator.SetTrigger("Throw");
        yield return new WaitForSeconds(projectileThrowAnimationTimeOffset);
        projectileRB.isKinematic = false;
        projectile.transform.parent = null;
        projectile.transform.LookAt(player);
        projectileRB.AddForce(projectileRB.transform.forward * 1000);

        throwAttackBool = false;
    }
    IEnumerator Throw3Projectile()
    {
        animator.SetTrigger("Throw");
        yield return new WaitForSeconds(projectileThrowAnimationTimeOffset);
        projectileRB.isKinematic = false;
        projectile.transform.parent = null;
        projectile.transform.LookAt(player);
        projectileRB.AddForce(projectileRB.transform.forward * 1000);

        yield return new WaitForSeconds(2);
        ResetProjectile();

        animator.SetTrigger("Throw");
        yield return new WaitForSeconds(projectileThrowAnimationTimeOffset);
        projectileRB.isKinematic = false;
        projectile.transform.parent = null;
        projectile.transform.LookAt(player);
        projectileRB.AddForce(projectileRB.transform.forward * 1000);

        yield return new WaitForSeconds(2);
        ResetProjectile();

        lastThrowAttack = Time.time;
        lastAttack = Time.time;
        animator.SetTrigger("Throw");
        yield return new WaitForSeconds(projectileThrowAnimationTimeOffset);
        projectileRB.isKinematic = false;
        projectile.transform.parent = null;
        projectile.transform.LookAt(player);
        projectileRB.AddForce(projectileRB.transform.forward * 1000);

        throwAttackBool = false;
    }
    private void RushAttack()
    {
        animator.SetBool("RushAttack", true);
        monsterSpeed = agent.speed;
        agent.speed = 13;
        agent.acceleration = agent.acceleration * 10;
        agent.transform.LookAt(player);

        Vector3 targetPosition = agent.transform.position + agent.transform.forward * 25f;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 50f, NavMesh.AllAreas))
        {
            walkPoint = hit.position; // Set walkPoint to the closest valid NavMesh position
            walkPointSet = true;
            agent.SetDestination(walkPoint);
        }
        

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

    public void ShockWaveCollision(GameObject other)
    {
        if (other.CompareTag("Player"))
        {
            Health.Hit(damageForStomp);
        }
    }
    private void RushCollision(Collision other)
    {
        if (other.transform.tag == "Player")
        {
            Health.Hit(damageForRush);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        RushCollision(collision);
    }


}
