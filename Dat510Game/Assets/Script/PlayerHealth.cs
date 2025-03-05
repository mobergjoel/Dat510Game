using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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
        }
    }

    public void Hit(float damage)
    {
        health -= damage/2;
        healthBar.value = health;
    }
}
