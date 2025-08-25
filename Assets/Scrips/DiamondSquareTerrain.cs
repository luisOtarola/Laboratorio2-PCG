using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DiamondSquareTerrain : MonoBehaviour
{
    public int size = 129;
    public float heightScale = 20f;
    public float roughness = 0.5f;

    private float[,] heightMap;

    void Start()
    {
        heightMap = GenerateTerrain(size, roughness);
        ApplyToTerrain();
    }

    float[,] GenerateTerrain(int size, float roughness)
    {
        float[,] map = new float[size, size];
        int step = size - 1;
        float scale = heightScale;

        map[0, 0] = Random.Range(-scale, scale);
        map[0, step] = Random.Range(-scale, scale);
        map[step, 0] = Random.Range(-scale, scale);
        map[step, step] = Random.Range(-scale, scale);

        while (step > 1)
        {
            int halfStep = step / 2;

            for (int x = halfStep; x < size; x += step)
            {
                for (int y = halfStep; y < size; y += step)
                {
                    float avg = (map[x - halfStep, y - halfStep] +
                                 map[x - halfStep, y + halfStep] +
                                 map[x + halfStep, y - halfStep] +
                                 map[x + halfStep, y + halfStep]) / 4f;

                    map[x, y] = avg + Random.Range(-scale, scale);
                }
            }

            for (int x = 0; x < size; x += halfStep)
            {
                for (int y = (x + halfStep) % step; y < size; y += step)
                {
                    float sum = 0f;
                    int count = 0;

                    if (x - halfStep >= 0) { sum += map[x - halfStep, y]; count++; }
                    if (x + halfStep < size) { sum += map[x + halfStep, y]; count++; }
                    if (y - halfStep >= 0) { sum += map[x, y - halfStep]; count++; }
                    if (y + halfStep < size) { sum += map[x, y + halfStep]; count++; }

                    float avg = sum / count;
                    map[x, y] = avg + Random.Range(-scale, scale);
                }
            }

            step /= 2;
            scale *= roughness;
        }

        return map;
    }

    void ApplyToTerrain()
    {
        Terrain terrain = GetComponent<Terrain>();
        TerrainData data = terrain.terrainData;

        int width = data.heightmapResolution;
        int height = data.heightmapResolution;
        float[,] heights = new float[width, height];

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                heights[x, y] = Mathf.InverseLerp(-heightScale, heightScale, heightMap[x, y]);
            }
        }

        data.SetHeights(0, 0, heights);
    }
}

