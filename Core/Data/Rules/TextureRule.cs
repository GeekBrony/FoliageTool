using System;
using UnityEngine;

[Serializable]
public struct TextureRule
{
    public bool bypass;
    public TerrainLayer layer;

    public enum Rule
    {
        Include, Exclude
    }
    public Rule rule;
    
    [Range(0, 1)]
    public float threshold;
}