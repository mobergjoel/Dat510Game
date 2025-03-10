
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;

public class Enemy : MonoBehaviour
{
    public float health = 50f;
    public Slider healthBar;
    public bool isDead = false;

    
    private Animator animator;
    private EnemyAi enemyAi;


    private void Awake() {
        animator = GetComponent<Animator>();
        enemyAi = GetComponent<EnemyAi>();
        animator.SetBool("IsDead", false);

    }

    private void Start()
    {
        healthBar.maxValue = health;
        healthBar.value = health;
        healthBar.gameObject.SetActive(false);

    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        health -= amount;
        healthBar.value = health;
        if (health <= 0f)
        {
            Die();
        }
    }

    private void Die()
    {
        isDead = true;
        StopAllCoroutines();
        StopAllAnimations();
        DisableEnemyAI();

        animator.SetBool("IsDead", true);
        StartCoroutine(HandleDeath());

    }

    private void StopAllAnimations()
    {
        animator.SetBool("RushAttack", false);
        animator.SetBool("StrafeLeft", false);
        animator.SetBool("StrafeRight", false);
        animator.SetBool("GroundAttack", false);
        animator.ResetTrigger("RageAttack");
        
    }

    private void DisableEnemyAI()
    {
        if (enemyAi != null)
        {
            enemyAi.enabled = false;
        }
    }

    private IEnumerator HandleDeath()
    {
        healthBar.gameObject.SetActive(false);
        yield return new WaitForSeconds(5f);
        gameObject.SetActive(false);
        Invoke("loadGameOver", 0f);
        Debug.Log("Game Over!");
    }

    private void loadGameOver()
    {
        Debug.Log("Game Over loaded");
        SceneManager.LoadScene("GameWinScene");
    }

  
}
