using System.Collections;
using UnityEngine;

public class MonsterShooter : MonoBehaviour
{
    public GameObject fireballPrefab;  
    public Transform firePoint;        
    public int numberOfFireballs = 5;  
    public float fireballSpeed = 10f;  
    public float spreadAngle = 15f;    
    public float fireRate = 0.3f;   
    public LayerMask treeLayer;

    public void StartShooting(Transform player)
    {
        StartCoroutine(ShootFireballs(player));
    }

    public IEnumerator ShootFireballs(Transform player)
    {
        for (int i = 0; i < numberOfFireballs; i++)
        {
            ShootAtPlayer(player);
            yield return new WaitForSeconds(fireRate);
        }
    }

    private void ShootAtPlayer(Transform player)
    {
        GameObject fireball = Instantiate(fireballPrefab, firePoint.position, Quaternion.identity);

        Fireball fireballScript = fireball.GetComponent<Fireball>();

        Rigidbody rb = fireball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 targetPos = player.position;
            Vector3 direction = CalculateLaunchVelocity(targetPos);

            float randomAngle = Random.Range(-spreadAngle, spreadAngle);
            direction = Quaternion.Euler(0, randomAngle, 0) * direction;

            rb.velocity = direction;
            fireballScript.treeLayer = treeLayer;
        }

        Destroy(fireball, 4f);
    }


    private Vector3 CalculateLaunchVelocity(Vector3 target)
    {
        Vector3 start = firePoint.position;
        Vector3 toTarget = target - start;

        float height = Mathf.Max(toTarget.y + 2f, 1f); 
        toTarget.y = 0; 

        float distance = toTarget.magnitude;
        float gravity = Mathf.Abs(Physics.gravity.y); 

        if (distance < 0.1f) return Vector3.forward * 5f + Vector3.up * 2f;

        float velocityY = Mathf.Sqrt(2 * gravity * height);
        float timeToApex = velocityY / gravity;
        float totalTime = timeToApex + Mathf.Sqrt(2 * height / gravity);

        if (totalTime <= 0.01f) return Vector3.forward * 5f + Vector3.up * 2f;

        float velocityXZ = distance / totalTime;

        Vector3 result = toTarget.normalized * velocityXZ;
        result.y = velocityY;
        return result;
    }

}
