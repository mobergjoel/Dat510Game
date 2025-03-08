using System.Collections;
using UnityEngine;

public class MonsterShooter : MonoBehaviour
{
    public GameObject fireballPrefab;  // Dra in din eldbolls-prefab h�r
    public Transform firePoint;        // Punkt d�r eldbollen skjuts fr�n
    public int numberOfFireballs = 5;  // Antal skott
    public float fireballSpeed = 10f;  // Basfart p� skotten
    public float spreadAngle = 15f;    // Hur mycket de sprider sig
    public float fireRate = 0.3f;      // Tid mellan skott
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

        Destroy(fireball, 4f); // F�rst�r eldbollen efter 5 sekunder
    }


    private Vector3 CalculateLaunchVelocity(Vector3 target)
    {
        Vector3 start = firePoint.position;
        Vector3 toTarget = target - start;

        float height = Mathf.Max(toTarget.y + 2f, 1f); // Minst 1 enhet h�jd f�r att undvika NaN
        toTarget.y = 0; // Ignorera h�jdskillnad i XZ-riktningen

        float distance = toTarget.magnitude;
        float gravity = Mathf.Abs(Physics.gravity.y); // Se till att gravitationen �r positiv

        // Felskydd: Om avst�ndet �r f�r litet, returnera en standardriktning
        if (distance < 0.1f) return Vector3.forward * 5f + Vector3.up * 2f;

        float velocityY = Mathf.Sqrt(2 * gravity * height);
        float timeToApex = velocityY / gravity;
        float totalTime = timeToApex + Mathf.Sqrt(2 * height / gravity);

        // Felskydd: Om totalTime �r 0, returnera en standardriktning
        if (totalTime <= 0.01f) return Vector3.forward * 5f + Vector3.up * 2f;

        float velocityXZ = distance / totalTime;

        Vector3 result = toTarget.normalized * velocityXZ;
        result.y = velocityY;
        return result;
    }

}
