using UnityEngine;

public class MapController : MonoBehaviour
{
    public GameObject mapUI; // Assign your Raw Image GameObject

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            mapUI.SetActive(!mapUI.activeSelf);
        }
    }
}