using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashLight : MonoBehaviour
{

    public GameObject flashLight;
    private bool flashLightOn = true;
    // Start is called before the first frame update
    void Start()
    {
       
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetButtonDown("Flashlight") && flashLightOn)
        {
            flashLight.SetActive(false);
            flashLightOn = false;
        }
        else if (Input.GetButtonDown("Flashlight") && !flashLightOn)
        {
            flashLight.SetActive(true);
            flashLightOn = true;
        }
    }

    public bool getFlashLightOn()
    {
        return flashLightOn;
    }
}
