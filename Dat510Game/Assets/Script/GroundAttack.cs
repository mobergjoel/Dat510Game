using UnityEngine;
using System.Collections;

public class GroundAttack : MonoBehaviour
{
    public Terrain terrain;
    public Transform monster;
    public Transform player;
    public float waveSpeed = 2f;  
    public float waveWidth = 4f;
    public float waveHeight = 0.1f; 
    public float waveLength = 10f;  
    public int affectedSize = 50;   

    private TerrainData terrainData;
    private int heightmapWidth;
    private int heightmapHeight;
    private float[,] originalHeights;

    void Start()
    {
        terrainData = terrain.terrainData;
        heightmapWidth = terrainData.heightmapResolution;
        heightmapHeight = terrainData.heightmapResolution;
    }

    public void TriggerWave()
    {
        StartCoroutine(WaveEffect());
    }

    private IEnumerator WaveEffect()
    {
        Vector3 monsterPos = monster.position;
        Vector3 playerPos = player.position;
        Vector3 waveDirection = monster.forward;

        float waveStartTime = Time.time;
        float waveDistance = 0f;

        
        int startX, startY;
        GetHeightmapCoords(monsterPos, out startX, out startY);

        startX = Mathf.Clamp(startX, 0, heightmapWidth - affectedSize);
        startY = Mathf.Clamp(startY, 0, heightmapHeight - affectedSize);

        originalHeights = terrainData.GetHeights(startX, startY, affectedSize, affectedSize);
        while (waveDistance < waveLength)
        {
            waveDistance = (Time.time - waveStartTime) * waveSpeed;
            ApplyWaveEffect(monsterPos, waveDirection, waveDistance, startX, startY);
            yield return new WaitForSeconds(0.01f);
        }

        ResetTerrain(startX, startY);
    }

    private void ApplyWaveEffect(Vector3 origin, Vector3 direction, float distance, int startX, int startY)
    {
        float[,] heights = terrainData.GetHeights(startX, startY, affectedSize, affectedSize);

        for (int x = 0; x < affectedSize; x++)
        {
            for (int y = 0; y < affectedSize; y++)
            {
                Vector3 worldPos = HeightmapToWorld(startX + x, startY + y);

                Vector3 waveCenter = origin + direction * distance;
                Debug.DrawLine(origin, waveCenter, Color.red, 5f);

                float distToWave = Mathf.Abs(Vector3.Dot(worldPos - waveCenter, direction));


                if (distToWave < waveWidth / 2f)
                {
                    if (!Physics.Raycast(worldPos + Vector3.up * 10f, Vector3.down, 20f, LayerMask.GetMask("Building")))
                    {
                        float waveFactor = Mathf.Sin((distance / waveLength) * Mathf.PI);
                        float heightChange = waveFactor * waveHeight;

                        heights[x, y] = Mathf.Clamp(originalHeights[x, y] + heightChange, 0, 1);
                    }
                }
            }
        }

        terrainData.SetHeights(startX, startY, heights);
    }

    private void GetHeightmapCoords(Vector3 worldPos, out int x, out int y)
    {
        Vector3 terrainPos = terrain.transform.position;
        x = Mathf.FloorToInt((worldPos.x - terrainPos.x) / terrainData.size.x * heightmapWidth);
        y = Mathf.FloorToInt((worldPos.z - terrainPos.z) / terrainData.size.z * heightmapHeight);
    }

    private Vector3 HeightmapToWorld(int x, int y)
    {
        Vector3 terrainPos = terrain.transform.position;
        float worldX = terrainPos.x + (x / (float)heightmapWidth) * terrainData.size.x;
        float worldZ = terrainPos.z + (y / (float)heightmapHeight) * terrainData.size.z;
        float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));

        return new Vector3(worldX, worldY, worldZ);
    }

    private void ResetTerrain(int startX, int startY)
    {
        terrainData.SetHeights(startX, startY, originalHeights);
    }
}
