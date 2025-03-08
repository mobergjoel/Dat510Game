using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;

public class PickUpGun : MonoBehaviour
{
    public GameObject gunOB;
    public GameObject pickUpText;
    public GameObject invOB;
    public AudioSource pickUpGunSound;
    public GameObject MonsterNoZone;
    public bool canShowPickUpText = false; 

    public bool inReach;

    public GameObject bossHealthBar;
    public GameObject playerHealthBar;

    public AudioSource bossBattleMusic;
    public GameObject bossBattleLight;
    public BlinkingSkybox blinkingSkybox;
    public EnemyAi monster;
    public GameObject arena;
    public Transform BossSpawnPoint;
    NavMeshAgent agent;


    public Animator animator;
    public NavMeshSurface stage1NavMesh;
    public NavMeshSurface BossBattleNavMesh;

    public void SwitchToBossBattle()
    {
        stage1NavMesh.gameObject.SetActive(false);
        BossBattleNavMesh.gameObject.SetActive(true);
    }

    void Start()
    {
        inReach = false;
        pickUpText.SetActive(false);
        invOB.SetActive(false);
        arena.SetActive(false);
        agent = monster.GetComponent<NavMeshAgent>();

    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach" && canShowPickUpText)
        {
            inReach = true;
            pickUpText.SetActive(true);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach")
        {
            inReach = false;
            pickUpText.SetActive(false);

        }
    }


    void Update()
    {
        if (inReach && Input.GetButtonDown("Interact"))
        {
            gunOB.SetActive(false);
            pickUpGunSound.Play();
            invOB.SetActive(true);
            pickUpText.SetActive(false);
            animator.SetBool("BossBattle", true);
            bossHealthBar.SetActive(true);
            playerHealthBar.SetActive(true);
            bossBattleMusic.Play();
            bossBattleLight.SetActive(true);
            blinkingSkybox.StartBlinking();
            monster.canAttack = false;
            arena.SetActive(true);
            MonsterNoZone.SetActive(true);
            agent.Warp(BossSpawnPoint.position);
            SwitchToBossBattle();

        }
        

    }

}
