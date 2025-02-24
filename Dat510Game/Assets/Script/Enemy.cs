
using UnityEngine;
using UnityEngine.SceneManagement;

public class Enemy : MonoBehaviour
{
    public float health = 50f;
    public void TakeDamage(float amount)
    {
        health -= amount;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
        Debug.Log("Enemy died!");
        Invoke("loadGameOver", 2f);
        Debug.Log("Game Over!");
    }

    private void loadGameOver()
    {
        Debug.Log("Game Over loaded");
        SceneManager.LoadScene("GameWinScene");
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

  
}
