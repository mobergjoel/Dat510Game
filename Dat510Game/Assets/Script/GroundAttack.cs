using UnityEngine;
using System.Collections;

public class GroundAttack : MonoBehaviour
{
    public Terrain terrain;
    public Transform monster;
    public Transform player;
    public float waveSpeed = 2f;  // Slower wave movement
    public float waveWidth = 4f;
    public float waveHeight = 0.1f; // Lower wave height for realism
    public float waveLength = 10f;  // Total distance wave travels
    public int affectedSize = 50;   // Resolution of modified terrain section

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
        Vector3 waveDirection = (playerPos - monsterPos).normalized;

        float waveStartTime = Time.time;
        float waveDistance = 0f;

        // Get terrain heightmap coordinates of the monster's position
        int startX, startY;
        GetHeightmapCoords(monsterPos, out startX, out startY);

        // Ensure valid terrain modification area
        startX = Mathf.Clamp(startX, 0, heightmapWidth - affectedSize);
        startY = Mathf.Clamp(startY, 0, heightmapHeight - affectedSize);

        // Store the original terrain heights for resetting later
        originalHeights = terrainData.GetHeights(startX, startY, affectedSize, affectedSize);
        Debug.Log("Tjablaaaa");
        while (waveDistance < waveLength)
        {
            waveDistance = (Time.time - waveStartTime) * waveSpeed;
            ApplyWaveEffect(monsterPos, waveDirection, waveDistance, startX, startY);
            yield return new WaitForSeconds(0.01f); // Makes the wave move gradually
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
                // Convert heightmap indices to world space
                Vector3 worldPos = HeightmapToWorld(startX + x, startY + y);

                // Calculate wave center moving forward
                Vector3 waveCenter = origin + direction * distance;
                float distToWave = Mathf.Abs(Vector3.Dot(worldPos - waveCenter, Vector3.Cross(direction, Vector3.up)));

                if (distToWave < waveWidth / 2f)  // Only modify terrain within wave width
                {
                    // Check for buildings before modifying terrain
                    if (!Physics.Raycast(worldPos + Vector3.up * 10f, Vector3.down, 20f, LayerMask.GetMask("Building")))
                    {
                        float waveFactor = Mathf.Sin((distance / waveLength) * Mathf.PI); // Smooth sine wave
                        float heightChange = waveFactor * waveHeight;

                        heights[x, y] = Mathf.Clamp(originalHeights[x, y] + heightChange, 0, 1); // Prevents unnatural heights
                    }
                }
            }
        }

        // Apply modified heights to terrain
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
        float worldY = terrain.SampleHeight(new Vector3(worldX, 0, worldZ));  // Sample terrain height

        return new Vector3(worldX, worldY, worldZ);
    }

    private void ResetTerrain(int startX, int startY)
    {
        terrainData.SetHeights(startX, startY, originalHeights);
    }
}
