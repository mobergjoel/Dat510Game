using System.Collections;
using UnityEngine;

public class MonsterShooter : MonoBehaviour
{
    public GameObject fireballPrefab;  // Dra in din eldbolls-prefab här
    public Transform firePoint;        // Punkt där eldbollen skjuts från
    public int numberOfFireballs = 5;  // Antal skott
    public float fireballSpeed = 10f;  // Basfart på skotten
    public float spreadAngle = 15f;    // Hur mycket de sprider sig
    public float fireRate = 0.3f;      // Tid mellan skott

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
        Rigidbody rb = fireball.GetComponent<Rigidbody>();

        if (rb != null)
        {
            Vector3 targetPos = player.position;
            Vector3 direction = CalculateLaunchVelocity(targetPos);

            float randomAngle = Random.Range(-spreadAngle, spreadAngle);
            direction = Quaternion.Euler(0, randomAngle, 0) * direction;

            rb.velocity = direction;
        }

        Destroy(fireball, 4f); // Förstör eldbollen efter 5 sekunder
    }


    private Vector3 CalculateLaunchVelocity(Vector3 target)
    {
        Vector3 start = firePoint.position;
        Vector3 toTarget = target - start;

        float height = Mathf.Max(toTarget.y + 2f, 1f); // Minst 1 enhet höjd för att undvika NaN
        toTarget.y = 0; // Ignorera höjdskillnad i XZ-riktningen

        float distance = toTarget.magnitude;
        float gravity = Mathf.Abs(Physics.gravity.y); // Se till att gravitationen är positiv

        // Felskydd: Om avståndet är för litet, returnera en standardriktning
        if (distance < 0.1f) return Vector3.forward * 5f + Vector3.up * 2f;

        float velocityY = Mathf.Sqrt(2 * gravity * height);
        float timeToApex = velocityY / gravity;
        float totalTime = timeToApex + Mathf.Sqrt(2 * height / gravity);

        // Felskydd: Om totalTime är 0, returnera en standardriktning
        if (totalTime <= 0.01f) return Vector3.forward * 5f + Vector3.up * 2f;

        float velocityXZ = distance / totalTime;

        Vector3 result = toTarget.normalized * velocityXZ;
        result.y = velocityY;
        return result;
    }

}
