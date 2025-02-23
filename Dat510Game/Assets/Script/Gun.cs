
using UnityEngine;
using UnityEngine.AI;

public class Gun : MonoBehaviour
{
    public float damage = 10f;
    public float range = 100f;

    public Camera fpsCam;
    public AudioSource gunShoot;
    public EnemyAi monster;

    public float shootCooldown = 1f; // Time in seconds between shots
    private float lastShootTime = 0f; // Keeps track of the last time the player shot

    public ParticleSystem muzzleFlash;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame

    void Update()
    {
        if (Input.GetButtonDown("Fire1") && Time.time - lastShootTime >= shootCooldown)
        {
            Shoot();
            lastShootTime = Time.time; // Update the last shoot time
        }
    }

    void Shoot()
    {
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

        monster.walkToPlayer();
    }

}
