
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float health = 50f;
    Animator animator;

    public bool isDead = false;
    private void Awake() {
        animator = GetComponent<Animator>();
        animator.SetBool("IsDead", false);

    }

    public Slider healthBar;
    public void TakeDamage(float amount)
    {
        health -= amount;
        healthBar.value = health;
        if (health <= 0f)
        {
            //play animation
            Die();
        }
    }

    void Die()
    {
        isDead = true;
        StartCoroutine(HandleDeath());

    }

    private IEnumerator HandleDeath()
    {
        yield return new WaitForSeconds(4f);
        gameObject.SetActive(false);
        healthBar.gameObject.SetActive(false);
        Invoke("loadGameOver", 1f);
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
