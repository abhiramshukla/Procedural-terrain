using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Noise
{
    public static float[,] GenerateNoiseMap(int mapWidth, int mapHeight, float scale, int seed, int octaves, float persistence, float lacunarity, Vector2 offset)
    {
        float[,] noiseMap = new float[mapWidth, mapHeight];

        if(scale <= 0) scale = 0.0001f;

        System.Random rng = new System.Random(seed);
        Vector2[] octaveOffset = new Vector2[octaves];

        for(int i = 0; i < octaves; i++)
        {
            float offsetX = rng.Next(-100000,100000) + offset.x;
            float offsetY = rng.Next(-100000,100000) + offset.y;
            octaveOffset[i] = new Vector2(offsetX, offsetY);
        }

        float minNoiseHeight = float.MaxValue;
        float maxNoiseHeight = float.MinValue;

        float halfWidth = mapWidth / 2f;
        float halfHeight = mapHeight / 2f;

        for(int y = 0; y < mapHeight; y++) 
        {
            for(int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseheight = 0;

                for(int i = 0; i < octaves; i++)
                {
                    float sampleX = (x - halfWidth)/scale * frequency + octaveOffset[i].x;
                    float sampleY = (y - halfHeight)/scale * frequency + octaveOffset[i].y;

                    float perlinValue = Mathf.PerlinNoise(sampleX, sampleY)*2 - 1;
                    noiseheight += perlinValue * amplitude;

                    amplitude *= persistence;
                    frequency *= lacunarity;
                }

                if(noiseheight > maxNoiseHeight) maxNoiseHeight = noiseheight;
                else if (noiseheight < minNoiseHeight) minNoiseHeight = noiseheight;

                noiseMap[x,y] = noiseheight;
            }
        }

        for(int y = 0; y < mapHeight; y++) 
        {
            for(int x = 0; x < mapWidth; x++)
            {
                noiseMap[x,y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x,y]);
            }
        }

        return noiseMap;
    }
}
