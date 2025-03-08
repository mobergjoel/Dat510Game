using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public float health = 100;
    public Slider healthBar;
    // Start is called before the first frame update
    void Start()
    {
        healthBar.value = health;
        healthBar.gameObject.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        if (health < 0)
        {
            Debug.Log("player Dead");
            SceneManager.LoadScene("GameOverScene");
        }
    }

    public void Hit(float damage)
    {
        health -= damage;
        healthBar.value = health;
    }
}
