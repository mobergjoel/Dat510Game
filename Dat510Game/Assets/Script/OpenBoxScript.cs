using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class OpenBoxScript : MonoBehaviour
{
    public Animator boxOB;
    public GameObject keyOBNeeded1;
    public GameObject keyOBNeeded2;
    public GameObject keyOBNeeded3;
    public GameObject openText;
    public GameObject keyMissingText;
    public Text missingKeyText;
    public AudioSource openSound;

    public bool inReach;
    public bool isOpen;

    public int howManyKeys;
    public int keysNeeded = 3;
    private string f;

    void Start()
    {
        inReach = false;
        openText.SetActive(false);
        keyMissingText.SetActive(false);
    }


    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Reach" && !isOpen)
        {
            inReach = true;
            openText.SetActive(true);

        }
    }

    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.tag == "Reach" && !isOpen)
        {
            inReach = false;
            openText.SetActive(false);
            keyMissingText.SetActive(false);
        }
    }


    void Update()
    {
        if (howManyKeys == keysNeeded && inReach && Input.GetButtonDown("Interact") && !isOpen)
        {
            keyOBNeeded1.SetActive(false);
            keyOBNeeded2.SetActive(false);
            keyOBNeeded3.SetActive(false);
            openSound.Play();
            boxOB.SetBool("open", true);
            openText.SetActive(false);
            keyMissingText.SetActive(false);
            isOpen = true;
        }

        else if (inReach && Input.GetButtonDown("Interact") && !isOpen)
        {
            openText.SetActive(false);
            int key = keysNeeded - howManyKeys;
            if (key == 1)
            {
                missingKeyText.text = $"You need to find {key} more key";
            }
            else
            {
                missingKeyText.text = $"You need to find {key} more keys";
            }
            
            keyMissingText.SetActive(true);
        }

        
    }
}
