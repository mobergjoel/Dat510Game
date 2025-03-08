using System.Collections;
using System.Collections.Generic;
using System.Threading;
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
    public Enemy enemy;
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

    public bool ChasePlayerBool = false;
    

    public float screamCooldown = 20f; // Time in seconds between screams
    private float lastScreamTime = 0f; // Keeps track of the last time the monster scream

    private float lastStompAttack = 0f;
    private float StompAttackCoolDown = 6f;

    private float lastRushAttack = 0f;
    private float rushAttackCoolDown = 10f;

    private float lastThrowAttack = 0f;
    private float throwAttackCoolDown = 1f;

    private float lastRageAttack = 0f;
    private float rageAttackCoolDown = 15f;

    public GameObject MonsterJumpscare;
    public GroundAttack groundAttack;
    public Transform fireAttackPoint;
    public GameObject fireballPrefab;

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
    public float monsterSpeed;
    float monsteracceleration;
    float lastThrowAttackCoolDown = 2f;
    float lastStompAttackCoolDown = 2f;
    float lastAttack = 0f;
    float lastRushAttackCoolDown = 2f;
    bool searchingFarAway = true;
    public bool canAttack = true;
    bool RushAttackBool = false;
    bool throwAttackBool = false;
    bool fromWalkToPlayerToBossBattle = true;
    bool hasAttacked = false;
    bool inWalkBetweenAttacksMode = false;
    MonsterShooter RageAttackScript;
    bool RageAttackBool = false;
    bool hasJustDoneRushAttack = false;


    private void Awake()
    {
        player = GameObject.Find("PlayerObj").transform;
        animator = GetComponent<Animator>();
        enemy = GetComponent<Enemy>();
        RageAttackScript = GetComponent<MonsterShooter>();
        projectile = transform.Find("Projectile").gameObject;
        projectileRB = projectile.GetComponent<Rigidbody>();
        projectileOrigin = new Vector3(projectile.transform.localPosition.x, projectile.transform.localPosition.y, projectile.transform.localPosition.z);
        shockWavePS = transform.Find("ShockWave").GetChild(0).GetComponent<ParticleSystem>();
        monsteracceleration = agent.acceleration;
        projectile.SetActive(false);
    }

    private void Update()
    {
        if (enemy.isDead)
        {
            // Disable NavMeshAgent and stop all actions
            agent.enabled = false;
            return;
        }
        else
        {


            distanceToPlayer = Vector3.Distance(player.position, transform.position);

            if (animator.GetBool("BossBattle"))
            {
                sightRange = 40;
            }
            else if (flashLightScript.getFlashLightOn())
            {
                sightRange = flashLightRange;
            }
            else if (playerScript.isWalking)
            {
                if (playerScript.isCrouched)
                {
                    sightRange = crouchSightRange;
                }
                else
                {
                    sightRange = walkSightRange;
                }
            }
            else if (playerScript.isSprinting)
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
                Debug.Log("Monster �r inte p� NavMesh!");
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



            if (distanceToPlayer > 100 && searchingFarAway)
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
        if (distanceToWalkPoint.magnitude < 4f) walkPointSet = false;
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
            if(!RushAttackBool && !hasAttacked && !(animator.GetBool("StrafeRight") || animator.GetBool("StrafeLeft")) && !inWalkBetweenAttacksMode)
            {
                agent.SetDestination(agent.transform.position);
            }

            if (hasAttacked && !inWalkBetweenAttacksMode && !animator.GetBool("StrafeRight") && !animator.GetBool("StrafeLeft"))
            {
                StartCoroutine(walkBetweenAttacks());
            }
            else if (inWalkBetweenAttacksMode)
            {

            }

            else
            { 
                if (Time.time - lastStompAttack >= StompAttackCoolDown && Time.time - lastAttack >= lastThrowAttackCoolDown && distanceToPlayer <= distanceForStomp && Time.time - lastRushAttack >= lastRushAttackCoolDown && !RushAttackBool && !throwAttackBool && !hasAttacked)
                {
                    monsterSound2.Play();
                    StartCoroutine(StompShockWave());
                    lastStompAttack = Time.time;
                    lastAttack = Time.time;
                    
                }
                else if (Time.time - lastRageAttack >= rageAttackCoolDown && Time.time - lastAttack >= lastRushAttackCoolDown && !throwAttackBool && !hasAttacked && distanceToPlayer <= 27 && !RushAttackBool)
                {
                    monsterSound3.Play();
                    StartCoroutine(RageAttack());
                    lastAttack = Time.time + 1.5f;
                    lastRageAttack = Time.time + 1.5f;

                }

                else if (Time.time - lastRushAttack >= rushAttackCoolDown && Time.time - lastAttack >= lastStompAttackCoolDown && Time.time - lastAttack >= lastThrowAttackCoolDown && !throwAttackBool && !hasAttacked && !RushAttackBool)
                {
                    monsterSound4.Play();
                    RushAttackBool = true;
                    RushAttack();
                    lastRushAttack = Time.time;
                    
                }
                
                else if (Time.time - lastThrowAttack >= throwAttackCoolDown && Time.time - lastAttack >= lastStompAttackCoolDown && Time.time - lastRushAttack >= lastRushAttackCoolDown && !RushAttackBool && !throwAttackBool && !hasAttacked && distanceToPlayer <= 35)
                {
                    float random = Random.Range(0, 4);
                    if (random == 0)
                    {
                        monsterSound1.Play();
                        Throw1Attack();
                    }
                    else if (random == 1)
                    {
                        monsterSound2.Play();
                        Throw2Attack();
                    }
                    else
                    {
                        monsterSound3.Play();
                        Throw3Attack();
                    }
                    throwAttackBool = true;
                    

                }
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
                    hasAttacked = true;
                    lastAttack = Time.time-2f;
                    hasJustDoneRushAttack = true;
                }
            }
            else if (RageAttackBool)
            {

            }
            else
            {
                transform.LookAt(player);
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

    private IEnumerator RageAttack()
    {
        RageAttackBool = true;
        hasAttacked = true;
        animator.SetTrigger("RageAttack");
        yield return new WaitForSeconds(0.1f);
        agent.transform.rotation *= Quaternion.Euler(0, -45, 0);
        yield return new WaitForSeconds(0.5f);
        StartCoroutine(RageAttackScript.ShootFireballs(player.transform));
        yield return new WaitForSeconds(1.755f);
        RageAttackBool = false;
        agent.transform.LookAt(player);
    }

    private IEnumerator walkBetweenAttacks()
    {
        monsterSound3.Play();
        int random = Random.Range(0, 2);
        inWalkBetweenAttacksMode = true;
        if (hasJustDoneRushAttack)
        {
            hasJustDoneRushAttack = false;
            yield return new WaitForSeconds(0.5f);
        }
        else
        {
            yield return new WaitForSeconds(2);
        }
        

        // Slå av tidigare animationer för att undvika konflikt
        animator.SetBool("StrafeRight", false);
        animator.SetBool("StrafeLeft", false);

        Vector3 targetPosition;
        if (random == 0)
        {
            animator.SetBool("StrafeRight", true);
            targetPosition = agent.transform.position + agent.transform.right * 6f;
        }
        else
        {
            animator.SetBool("StrafeLeft", true);
            targetPosition = agent.transform.position - agent.transform.right * 6f;
        }

        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 50f, NavMesh.AllAreas))
        {
            walkPoint = hit.position;
            walkPointSet = true;
            if (!enemy.isDead)
            {
                agent.SetDestination(walkPoint);
            }
        }
        else
        {
            // Om en giltig position inte hittas, avbryt attackmönstret
            inWalkBetweenAttacksMode = false;
            hasAttacked = false;
            yield break;
        }

        // Vänta tills vi når destinationen
        while (Vector3.Distance(agent.transform.position, walkPoint) > 1f)
        {
            yield return null;
        }

        // Återställ status för nästa attack
        animator.SetBool("StrafeRight", false);
        animator.SetBool("StrafeLeft", false);
        inWalkBetweenAttacksMode = false;
        hasAttacked = false;
        lastAttack = Time.time-4;
    }


    private void Throw1Attack()
    {
        StartCoroutine(Throw1Projectile());
    }

    private void Throw2Attack()
    {
        StartCoroutine(Throw2Projectile());
    }
    private void Throw3Attack()
    {
        StartCoroutine(Throw3Projectile());
    }

    private void GroundAttack()
    {
        animator.SetBool("GroundAttack", true); // B�rja markattackanimationen

        // V�nta i 5 sekunder
        

        // Anropa metoden f�r att trigga markattack-v�gen
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
        hasAttacked = true;
    }

    

    
    IEnumerator Throw1Projectile()
    {
        lastThrowAttack = Time.time;
        lastAttack = Time.time;
        animator.SetTrigger("Throw");

        yield return new WaitForSeconds(projectileThrowAnimationTimeOffset);

        // Skapa tre projektiler
        GameObject centerProjectile = Instantiate(fireballPrefab, fireAttackPoint.position, Quaternion.identity);
        GameObject leftProjectile = Instantiate(fireballPrefab, fireAttackPoint.position + transform.right * -0.2f, Quaternion.identity);
        GameObject rightProjectile = Instantiate(fireballPrefab, fireAttackPoint.position + transform.right * 0.2f, Quaternion.identity);


        Rigidbody rbCenter = centerProjectile.GetComponent<Rigidbody>();
        Rigidbody rbLeft = leftProjectile.GetComponent<Rigidbody>();
        Rigidbody rbRight = rightProjectile.GetComponent<Rigidbody>();

        Collider colCenter = centerProjectile.GetComponent<Collider>();
        Collider colLeft = leftProjectile.GetComponent<Collider>();
        Collider colRight = rightProjectile.GetComponent<Collider>();

        // Se till att projektilerna inte kolliderar med varandra
        Physics.IgnoreCollision(colCenter, colLeft);
        Physics.IgnoreCollision(colCenter, colRight);
        Physics.IgnoreCollision(colLeft, colRight);

        rbCenter.interpolation = RigidbodyInterpolation.None;
        rbLeft.interpolation = RigidbodyInterpolation.None;
        rbRight.interpolation = RigidbodyInterpolation.None;

        rbCenter.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rbLeft.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rbRight.collisionDetectionMode = CollisionDetectionMode.Discrete;

        // Gör dem fysiska och ta bort deras parent
        rbCenter.isKinematic = false;
        rbLeft.isKinematic = false;
        rbRight.isKinematic = false;

        centerProjectile.transform.parent = null;
        leftProjectile.transform.parent = null;
        rightProjectile.transform.parent = null;

        // Rikta centerprojektilen rakt mot spelaren
        centerProjectile.transform.LookAt(player);

        // Beräkna rotationsvinklar för sidoprojektiler
        Quaternion leftRotation = Quaternion.Euler(0, -10, 0) * centerProjectile.transform.rotation;
        Quaternion centerRotation = Quaternion.Euler(0, 0, 0) * centerProjectile.transform.rotation;
        Quaternion rightRotation = Quaternion.Euler(0, 0, 0) * centerProjectile.transform.rotation;

        // Rotera sidoprojektilerna
        leftProjectile.transform.rotation = leftRotation;
        centerProjectile.transform.rotation = centerRotation;
        rightProjectile.transform.rotation = rightRotation;

        // Skjut projektilerna framåt
        rbCenter.AddForce(agent.transform.forward * 1000);
        rbLeft.AddForce(agent.transform.forward * 1000);
        rbRight.AddForce(agent.transform.forward * 1000);

        throwAttackBool = false;
        hasAttacked = true;
        Destroy(centerProjectile, 4f);
        Destroy(leftProjectile, 4f);
        Destroy(rightProjectile, 4f);
    }

    IEnumerator Throw2Projectile() 
    {
        animator.SetTrigger("Throw");

        yield return new WaitForSeconds(projectileThrowAnimationTimeOffset);

        // Skapa tre projektiler
        GameObject centerProjectile = Instantiate(fireballPrefab, fireAttackPoint.position, Quaternion.identity);
        GameObject leftProjectile = Instantiate(fireballPrefab, fireAttackPoint.position + transform.right * -0.2f, Quaternion.identity);
        GameObject rightProjectile = Instantiate(fireballPrefab, fireAttackPoint.position + transform.right * 0.2f, Quaternion.identity);


        Rigidbody rbCenter = centerProjectile.GetComponent<Rigidbody>();
        Rigidbody rbLeft = leftProjectile.GetComponent<Rigidbody>();
        Rigidbody rbRight = rightProjectile.GetComponent<Rigidbody>();

        Collider colCenter = centerProjectile.GetComponent<Collider>();
        Collider colLeft = leftProjectile.GetComponent<Collider>();
        Collider colRight = rightProjectile.GetComponent<Collider>();

        // Se till att projektilerna inte kolliderar med varandra
        Physics.IgnoreCollision(colCenter, colLeft);
        Physics.IgnoreCollision(colCenter, colRight);
        Physics.IgnoreCollision(colLeft, colRight);

        rbCenter.interpolation = RigidbodyInterpolation.None;
        rbLeft.interpolation = RigidbodyInterpolation.None;
        rbRight.interpolation = RigidbodyInterpolation.None;

        rbCenter.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rbLeft.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rbRight.collisionDetectionMode = CollisionDetectionMode.Discrete;

        // Gör dem fysiska och ta bort deras parent
        rbCenter.isKinematic = false;
        rbLeft.isKinematic = false;
        rbRight.isKinematic = false;

        centerProjectile.transform.parent = null;
        leftProjectile.transform.parent = null;
        rightProjectile.transform.parent = null;

        // Rikta centerprojektilen rakt mot spelaren
        centerProjectile.transform.LookAt(player);

        // Beräkna rotationsvinklar för sidoprojektiler
        Quaternion leftRotation = Quaternion.Euler(0, -10, 0) * centerProjectile.transform.rotation;
        Quaternion centerRotation = Quaternion.Euler(0, 0, 0) * centerProjectile.transform.rotation;
        Quaternion rightRotation = Quaternion.Euler(0, 0, 0) * centerProjectile.transform.rotation;

        // Rotera sidoprojektilerna
        leftProjectile.transform.rotation = leftRotation;
        centerProjectile.transform.rotation = centerRotation;
        rightProjectile.transform.rotation = rightRotation;

        // Skjut projektilerna framåt
        rbCenter.AddForce(agent.transform.forward * 1000);
        rbLeft.AddForce(agent.transform.forward * 1000);
        rbRight.AddForce(agent.transform.forward * 1000);

        
        Destroy(centerProjectile, 4f);
        Destroy(leftProjectile, 4f);
        Destroy(rightProjectile, 4f);

        yield return new WaitForSeconds(1.85f);

        lastThrowAttack = Time.time;
        lastAttack = Time.time;

        animator.SetTrigger("Throw");

        yield return new WaitForSeconds(projectileThrowAnimationTimeOffset);

        // Skapa tre projektiler
        GameObject centerProjectile2 = Instantiate(fireballPrefab, fireAttackPoint.position, Quaternion.identity);
        GameObject leftProjectile2 = Instantiate(fireballPrefab, fireAttackPoint.position + transform.right * -0.2f, Quaternion.identity);
        GameObject rightProjectile2 = Instantiate(fireballPrefab, fireAttackPoint.position + transform.right * 0.2f, Quaternion.identity);


        Rigidbody rbCenter2 = centerProjectile2.GetComponent<Rigidbody>();
        Rigidbody rbLeft2 = leftProjectile2.GetComponent<Rigidbody>();
        Rigidbody rbRight2 = rightProjectile2.GetComponent<Rigidbody>();

        Collider colCenter2 = centerProjectile2.GetComponent<Collider>();
        Collider colLeft2 = leftProjectile2.GetComponent<Collider>();
        Collider colRight2 = rightProjectile2.GetComponent<Collider>();

        // Se till att projektilerna inte kolliderar med varandra
        Physics.IgnoreCollision(colCenter2, colLeft2);
        Physics.IgnoreCollision(colCenter2, colRight2);
        Physics.IgnoreCollision(colLeft2, colRight2);

        rbCenter2.interpolation = RigidbodyInterpolation.None;
        rbLeft2.interpolation = RigidbodyInterpolation.None;
        rbRight2.interpolation = RigidbodyInterpolation.None;

        rbCenter2.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rbLeft2.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rbRight2.collisionDetectionMode = CollisionDetectionMode.Discrete;

        // Gör dem fysiska och ta bort deras parent
        rbCenter2.isKinematic = false;
        rbLeft2.isKinematic = false;
        rbRight2.isKinematic = false;

        centerProjectile2.transform.parent = null;
        leftProjectile2.transform.parent = null;
        rightProjectile2.transform.parent = null;

        // Rikta centerprojektilen rakt mot spelaren
        centerProjectile2.transform.LookAt(player);

        // Beräkna rotationsvinklar för sidoprojektiler
        Quaternion leftRotation2 = Quaternion.Euler(0, -10, 0) * centerProjectile2.transform.rotation;
        Quaternion centerRotation2 = Quaternion.Euler(0, 0, 0) * centerProjectile2.transform.rotation;
        Quaternion rightRotation2 = Quaternion.Euler(0, 0, 0) * centerProjectile2.transform.rotation;

        // Rotera sidoprojektilerna
        leftProjectile2.transform.rotation = leftRotation2;
        centerProjectile2.transform.rotation = centerRotation2;
        rightProjectile2.transform.rotation = rightRotation2;

        // Skjut projektilerna framåt
        rbCenter2.AddForce(agent.transform.forward * 1000);
        rbLeft2.AddForce(agent.transform.forward * 1000);
        rbRight2.AddForce(agent.transform.forward * 1000);

        
        Destroy(centerProjectile2, 4f);
        Destroy(leftProjectile2, 4f);
        Destroy(rightProjectile2, 4f);

        throwAttackBool = false;
        hasAttacked = true;
    }
    IEnumerator Throw3Projectile()
    {
        animator.SetTrigger("Throw");

        yield return new WaitForSeconds(projectileThrowAnimationTimeOffset);

        // Skapa tre projektiler
        GameObject centerProjectile = Instantiate(fireballPrefab, fireAttackPoint.position, Quaternion.identity);
        GameObject leftProjectile = Instantiate(fireballPrefab, fireAttackPoint.position + transform.right * -0.2f, Quaternion.identity);
        GameObject rightProjectile = Instantiate(fireballPrefab, fireAttackPoint.position + transform.right * 0.2f, Quaternion.identity);


        Rigidbody rbCenter = centerProjectile.GetComponent<Rigidbody>();
        Rigidbody rbLeft = leftProjectile.GetComponent<Rigidbody>();
        Rigidbody rbRight = rightProjectile.GetComponent<Rigidbody>();

        Collider colCenter = centerProjectile.GetComponent<Collider>();
        Collider colLeft = leftProjectile.GetComponent<Collider>();
        Collider colRight = rightProjectile.GetComponent<Collider>();

        // Se till att projektilerna inte kolliderar med varandra
        Physics.IgnoreCollision(colCenter, colLeft);
        Physics.IgnoreCollision(colCenter, colRight);
        Physics.IgnoreCollision(colLeft, colRight);

        rbCenter.interpolation = RigidbodyInterpolation.None;
        rbLeft.interpolation = RigidbodyInterpolation.None;
        rbRight.interpolation = RigidbodyInterpolation.None;

        rbCenter.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rbLeft.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rbRight.collisionDetectionMode = CollisionDetectionMode.Discrete;

        // Gör dem fysiska och ta bort deras parent
        rbCenter.isKinematic = false;
        rbLeft.isKinematic = false;
        rbRight.isKinematic = false;

        centerProjectile.transform.parent = null;
        leftProjectile.transform.parent = null;
        rightProjectile.transform.parent = null;

        // Rikta centerprojektilen rakt mot spelaren
        centerProjectile.transform.LookAt(player);

        // Beräkna rotationsvinklar för sidoprojektiler
        Quaternion leftRotation = Quaternion.Euler(0, -10, 0) * centerProjectile.transform.rotation;
        Quaternion centerRotation = Quaternion.Euler(0, 0, 0) * centerProjectile.transform.rotation;
        Quaternion rightRotation = Quaternion.Euler(0, 0, 0) * centerProjectile.transform.rotation;

        // Rotera sidoprojektilerna
        leftProjectile.transform.rotation = leftRotation;
        centerProjectile.transform.rotation = centerRotation;
        rightProjectile.transform.rotation = rightRotation;

        // Skjut projektilerna framåt
        rbCenter.AddForce(agent.transform.forward * 1000);
        rbLeft.AddForce(agent.transform.forward * 1000);
        rbRight.AddForce(agent.transform.forward * 1000);


        Destroy(centerProjectile, 4f);
        Destroy(leftProjectile, 4f);
        Destroy(rightProjectile, 4f);

        yield return new WaitForSeconds(1.85f);


        animator.SetTrigger("Throw");

        yield return new WaitForSeconds(projectileThrowAnimationTimeOffset);

        // Skapa tre projektiler
        GameObject centerProjectile2 = Instantiate(fireballPrefab, fireAttackPoint.position, Quaternion.identity);
        GameObject leftProjectile2 = Instantiate(fireballPrefab, fireAttackPoint.position + transform.right * -0.2f, Quaternion.identity);
        GameObject rightProjectile2 = Instantiate(fireballPrefab, fireAttackPoint.position + transform.right * 0.2f, Quaternion.identity);


        Rigidbody rbCenter2 = centerProjectile2.GetComponent<Rigidbody>();
        Rigidbody rbLeft2 = leftProjectile2.GetComponent<Rigidbody>();
        Rigidbody rbRight2 = rightProjectile2.GetComponent<Rigidbody>();

        Collider colCenter2 = centerProjectile2.GetComponent<Collider>();
        Collider colLeft2 = leftProjectile2.GetComponent<Collider>();
        Collider colRight2 = rightProjectile2.GetComponent<Collider>();

        // Se till att projektilerna inte kolliderar med varandra
        Physics.IgnoreCollision(colCenter2, colLeft2);
        Physics.IgnoreCollision(colCenter2, colRight2);
        Physics.IgnoreCollision(colLeft2, colRight2);

        rbCenter2.interpolation = RigidbodyInterpolation.None;
        rbLeft2.interpolation = RigidbodyInterpolation.None;
        rbRight2.interpolation = RigidbodyInterpolation.None;

        rbCenter2.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rbLeft2.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rbRight2.collisionDetectionMode = CollisionDetectionMode.Discrete;

        // Gör dem fysiska och ta bort deras parent
        rbCenter2.isKinematic = false;
        rbLeft2.isKinematic = false;
        rbRight2.isKinematic = false;

        centerProjectile2.transform.parent = null;
        leftProjectile2.transform.parent = null;
        rightProjectile2.transform.parent = null;

        // Rikta centerprojektilen rakt mot spelaren
        centerProjectile2.transform.LookAt(player);

        // Beräkna rotationsvinklar för sidoprojektiler
        Quaternion leftRotation2 = Quaternion.Euler(0, -10, 0) * centerProjectile2.transform.rotation;
        Quaternion centerRotation2 = Quaternion.Euler(0, 0, 0) * centerProjectile2.transform.rotation;
        Quaternion rightRotation2 = Quaternion.Euler(0, 0, 0) * centerProjectile2.transform.rotation;

        // Rotera sidoprojektilerna
        leftProjectile2.transform.rotation = leftRotation2;
        centerProjectile2.transform.rotation = centerRotation2;
        rightProjectile2.transform.rotation = rightRotation2;

        // Skjut projektilerna framåt
        rbCenter2.AddForce(agent.transform.forward * 1000);
        rbLeft2.AddForce(agent.transform.forward * 1000);
        rbRight2.AddForce(agent.transform.forward * 1000);


        Destroy(centerProjectile2, 4f);
        Destroy(leftProjectile2, 4f);
        Destroy(rightProjectile2, 4f);

        yield return new WaitForSeconds(1.85f);

        lastThrowAttack = Time.time;
        lastAttack = Time.time;

        animator.SetTrigger("Throw");
        yield return new WaitForSeconds(projectileThrowAnimationTimeOffset);
        // Skapa tre projektiler
        GameObject centerProjectile3 = Instantiate(fireballPrefab, fireAttackPoint.position, Quaternion.identity);
        GameObject leftProjectile3 = Instantiate(fireballPrefab, fireAttackPoint.position + transform.right * -0.2f, Quaternion.identity);
        GameObject rightProjectile3 = Instantiate(fireballPrefab, fireAttackPoint.position + transform.right * 0.2f, Quaternion.identity);


        Rigidbody rbCenter3 = centerProjectile3.GetComponent<Rigidbody>();
        Rigidbody rbLeft3 = leftProjectile3.GetComponent<Rigidbody>();
        Rigidbody rbRight3 = rightProjectile3.GetComponent<Rigidbody>();

        Collider colCenter3 = centerProjectile3.GetComponent<Collider>();
        Collider colLeft3 = leftProjectile3.GetComponent<Collider>();
        Collider colRight3 = rightProjectile3.GetComponent<Collider>();

        // Se till att projektilerna inte kolliderar med varandra
        Physics.IgnoreCollision(colCenter3, colLeft3);
        Physics.IgnoreCollision(colCenter3, colRight3);
        Physics.IgnoreCollision(colLeft3, colRight3);

        rbCenter3.interpolation = RigidbodyInterpolation.None;
        rbLeft3.interpolation = RigidbodyInterpolation.None;
        rbRight3.interpolation = RigidbodyInterpolation.None;

        rbCenter3.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rbLeft3.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rbRight3.collisionDetectionMode = CollisionDetectionMode.Discrete;

        // Gör dem fysiska och ta bort deras parent
        rbCenter3.isKinematic = false;
        rbLeft3.isKinematic = false;
        rbRight3.isKinematic = false;

        centerProjectile3.transform.parent = null;
        leftProjectile3.transform.parent = null;
        rightProjectile3.transform.parent = null;

        // Rikta centerprojektilen rakt mot spelaren
        centerProjectile3.transform.LookAt(player);

        // Beräkna rotationsvinklar för sidoprojektiler
        Quaternion leftRotation3 = Quaternion.Euler(0, -10, 0) * centerProjectile3.transform.rotation;
        Quaternion centerRotation3 = Quaternion.Euler(0, 0, 0) * centerProjectile3.transform.rotation;
        Quaternion rightRotation3 = Quaternion.Euler(0, 0, 0) * centerProjectile3.transform.rotation;

        // Rotera sidoprojektilerna
        leftProjectile3.transform.rotation = leftRotation3;
        centerProjectile3.transform.rotation = centerRotation3;
        rightProjectile3.transform.rotation = rightRotation3;

        // Skjut projektilerna framåt
        rbCenter3.AddForce(agent.transform.forward * 1000);
        rbLeft3.AddForce(agent.transform.forward * 1000);
        rbRight3.AddForce(agent.transform.forward * 1000);


        Destroy(centerProjectile3, 4f);
        Destroy(leftProjectile3, 4f);
        Destroy(rightProjectile3, 4f);

        throwAttackBool = false;
        hasAttacked = true;
    }
    private void RushAttack()
    {
        animator.SetBool("RushAttack", true);
        monsterSpeed = agent.speed;
        agent.speed = 13;
        agent.acceleration = agent.acceleration * 10;
        agent.transform.LookAt(player);

        Vector3 targetPosition = agent.transform.position + agent.transform.forward * 35f;
        NavMeshHit hit;
        if (NavMesh.SamplePosition(targetPosition, out hit, 35f, NavMesh.AllAreas))
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
    public IEnumerator ProjectileCollision(Collision other)
    {
        if (other.transform.tag == "Player")
        {
            if (Shield)
            {

            }
            else
            {
                Health.Hit(damageForProjectile);
                Shield = true;
                yield return new WaitForSeconds(1f);
                Shield = false;
            }
        }
    }
    bool Shield = false;
    public IEnumerator ShockWaveCollision()
    {
        if(Shield) 
        {
          
        }
        else
        {
            Health.Hit(damageForStomp);
            Shield = true;
            yield return new WaitForSeconds(1f);
            Shield = false;
        }
        
    }
    private IEnumerator RushCollision(Collision other)
    {
        if (other.transform.tag == "Player")
        {
            if (Shield)
            {

            }
            else
            {
                Health.Hit(damageForRush);
                Shield = true;
                yield return new WaitForSeconds(1f);
                Shield= false;
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(RushCollision(collision));
    }


}
