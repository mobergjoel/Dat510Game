using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUpGun : MonoBehaviour
{
    public GameObject gunOB;
    public GameObject pickUpText;
    public GameObject invOB;
    public AudioSource pickUpGunSound;
    public bool canShowPickUpText = false; 

    public bool inReach;


    void Start()
    {
        inReach = false;
        pickUpText.SetActive(false);
        invOB.SetActive(false);
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
        }

        
    }

}
