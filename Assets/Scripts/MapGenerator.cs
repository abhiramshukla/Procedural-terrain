using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapGenerator : MonoBehaviour
{
    [SerializeField] private DrawMode _drawMode;
    [SerializeField][Range(0,6)] private int _levelOfDetail;
    [SerializeField] private float _noiseScale;
    [SerializeField] private int _octaves;
    [SerializeField][Range(0,1)] private float _persistence;
    [SerializeField] private float _lacunarity;
    [SerializeField] private Vector2 _offset;
    [SerializeField] private TerrainType[] _regions;
    [SerializeField] private float _meshHeightMultiplier;
    [SerializeField] private AnimationCurve _meshHeightCurve;
    [SerializeField] private bool _useFallOff;

    public int Seed;
    public bool AutoUpdate;
    
    private const int _mapChunkSize = 241;
    private float[,] _fallOffMap;

    public void GenerateMap() 
    {
        float[,] noiseMap = Noise.GenerateNoiseMap(_mapChunkSize, _mapChunkSize, _noiseScale, Seed, _octaves, _persistence, _lacunarity, _offset);


        Color[] colourMap = new Color[_mapChunkSize * _mapChunkSize];
        for(int y = 0; y < _mapChunkSize; y++)
        {
            for(int x = 0; x< _mapChunkSize; x++)
            {
                if (_useFallOff)
                    noiseMap[x,y] = Mathf.Clamp01(noiseMap[x,y] - _fallOffMap[x,y]);
                float currentHeight = noiseMap[x,y];
                for(int i = 0; i < _regions.Length; i ++)
                {
                    if(currentHeight <= _regions[i].Height)
                    {
                        colourMap[y * _mapChunkSize + x] = _regions[i].Colour;
                        break;
                    }
                }
            }
        }

        MapDisplay display = FindObjectOfType<MapDisplay>();
        if(_drawMode == DrawMode.NoiseMap) 
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(noiseMap));
        else if(_drawMode == DrawMode.ColourMap) 
            display.DrawTexture(TextureGenerator.TextureFromColourMap(colourMap, _mapChunkSize, _mapChunkSize));
        else if(_drawMode == DrawMode.Mesh) 
            display.DrawMesh(MeshGenerator.GenerateTerrainMesh(noiseMap, _meshHeightMultiplier, _meshHeightCurve, _levelOfDetail),TextureGenerator.TextureFromColourMap(colourMap, _mapChunkSize, _mapChunkSize));
        else if(_drawMode == DrawMode.FallOffMap)
            display.DrawTexture(TextureGenerator.TextureFromHeightMap(FallOffGenerator.GenerateFallOffMap(_mapChunkSize)));
    }

    void OnValidate()
    {
        if(_lacunarity < 1) _lacunarity = 1;
        if(_octaves < 0) _octaves = 0;
        _fallOffMap = FallOffGenerator.GenerateFallOffMap(_mapChunkSize);
    }

    [System.Serializable]
    public struct TerrainType
    {
        public string Name;
        public float Height;
        public Color Colour;
    }

    public enum DrawMode {NoiseMap, ColourMap, Mesh, FallOffMap}
}
