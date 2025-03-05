
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public float health = 50f;

    public Slider healthBar;
    public void TakeDamage(float amount)
    {
        health -= amount;
        healthBar.value = health;
        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
        healthBar.gameObject.SetActive(false);
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
        healthBar.maxValue = health;
        healthBar.value = health;
        healthBar.gameObject.SetActive(false);

    }

    // Update is called once per frame
    void Update()
    {
        
    }

  
}
