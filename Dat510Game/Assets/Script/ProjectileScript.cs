using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileScript : MonoBehaviour
{

    public EnemyAi monsterScript;
    GameObject monster;
    // Start is called before the first frame update
    void Start()
    {
        monster = GameObject.Find("Monster");
        monsterScript = monster.GetComponent<EnemyAi>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnCollisionEnter(Collision collision)
    {
        StartCoroutine(monsterScript.ProjectileCollision(collision));
    }
}
