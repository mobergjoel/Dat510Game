using UnityEngine;
using System.Collections;

public class BlinkingSkybox : MonoBehaviour
{
    public Color blinkColor = Color.red; // The red color for blinking
    public float blinkDuration = 0.5f;   // Time between blinks

    public Color originalColor = new Color(94f / 255f, 94f / 255f, 94f / 255f);
    private Material skyboxMaterial;
    private Coroutine blinkCoroutine;

    private void Start()
    {
        if (RenderSettings.skybox != null)
        {
            skyboxMaterial = RenderSettings.skybox;
            SetSkyboxColor(originalColor);

            /*if (skyboxMaterial.HasProperty("_Tint")) // Procedural Skybox
            {
                originalColor = skyboxMaterial.GetColor("_Tint");
            }
            else if (skyboxMaterial.HasProperty("_Color")) // Regular Skybox
            {
                originalColor = skyboxMaterial.GetColor("_Color");
            }*/
        }
    }

    public void NoBlinking()
    {
        skyboxMaterial.SetColor("_Tint", originalColor);
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
