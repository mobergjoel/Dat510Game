
using UnityEngine;
using UnityEngine.AI;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;
    public float fireRate = 10f;

    public Camera fpsCam;
    public AudioSource gunShoot;
    public EnemyAi monster;
    public Animator animator;
    Transform player;

    public float nextTimeToFire = 0f;
    


    public ParticleSystem muzzleFlash;
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.Find("PlayerObj").transform;
    }

    // Update is called once per frame

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time >= nextTimeToFire)
        {
            nextTimeToFire = Time.time + (5f / fireRate);
            Shoot();
            
        }
    }

    void Shoot()
    {
        float distanceToPlayer = Vector3.Distance(player.position, monster.transform.position);
        muzzleFlash.Play();
        gunShoot.Play();

        RaycastHit hit;
        int layerMask = ~LayerMask.GetMask("ReachTool");

        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hit, range, layerMask))
        {
            // Debug.Log(hit.transform.name);
            Enemy enemy = hit.transform.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage);
            }
        }
        if (animator.GetBool("BossBattle") && distanceToPlayer > 50)
        {
            monster.walkToPlayer();
        }

    }

}
