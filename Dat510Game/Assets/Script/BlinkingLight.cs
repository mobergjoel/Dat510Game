using UnityEngine;
using System.Collections;

public class BlinkingLight : MonoBehaviour
{
    public float blinkDuration = 0.5f; // Time between blinks

    private Light lightSource;

    private void Awake()
    {
        lightSource = GetComponent<Light>();
    }

    private void OnEnable()
    {
        StartCoroutine(Blink());
    }

    private IEnumerator Blink()
    {
        while (true) // Loop indefinitely
        {
            lightSource.enabled = !lightSource.enabled;
            yield return new WaitForSeconds(blinkDuration);
        }
    }
}
