using UnityEngine;
using System.Collections;

public class BlinkingSkybox : MonoBehaviour
{
    public Color blinkColor = Color.red;
    public float blinkDuration = 0.5f;   

    public Color originalColor = new Color(94f / 255f, 94f / 255f, 94f / 255f);
    private Material skyboxMaterial;
    private Coroutine blinkCoroutine;

    private void Start()
    {
        if (RenderSettings.skybox != null)
        {
            skyboxMaterial = RenderSettings.skybox;
            SetSkyboxColor(originalColor);

            
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
