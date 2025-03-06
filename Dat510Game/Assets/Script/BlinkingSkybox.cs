using UnityEngine;
using System.Collections;

public class BlinkingSkybox : MonoBehaviour
{
    public Color blinkColor = Color.red; // The red color for blinking
    public float blinkDuration = 0.5f;   // Time between blinks

    private Color originalColor;
    private Material skyboxMaterial;

    private void Start()
    {
        if (RenderSettings.skybox != null)
        {
            skyboxMaterial = RenderSettings.skybox;
            if (skyboxMaterial.HasProperty("_Tint")) // Procedural Skybox
            {
                originalColor = skyboxMaterial.GetColor("_Tint");
            }
            else if (skyboxMaterial.HasProperty("_Color")) // Regular Skybox
            {
                originalColor = skyboxMaterial.GetColor("_Color");
            }
        }
    }

    public void StartBlinking()
    {
        if (skyboxMaterial != null)
        {
            StartCoroutine(BlinkSkybox());
        }
    }

    private IEnumerator BlinkSkybox()
    {
        while (true)
        {
            SetSkyboxColor(blinkColor);
            yield return new WaitForSeconds(blinkDuration);
            SetSkyboxColor(originalColor);
            yield return new WaitForSeconds(blinkDuration);
        }
    }

    private void SetSkyboxColor(Color color)
    {
        if (skyboxMaterial.HasProperty("_Tint"))
        {
            skyboxMaterial.SetColor("_Tint", color);
        }
        else if (skyboxMaterial.HasProperty("_Color"))
        {
            skyboxMaterial.SetColor("_Color", color);
        }
    }
}
