using System;
using System.Collections.Generic;
using System.ComponentModel;
using Unity.Mathematics;
using UnityEngine;

namespace FoliageTool.Utils
{
    [Serializable]
    public struct PerlinNoise
    {
        private sealed class PerlinNoiseEqualityComparer : IEqualityComparer<PerlinNoise>
        {
            public bool Equals(PerlinNoise x, PerlinNoise y)
            {
                return x.offset.Equals(y.offset) && x.scaleX.Equals(y.scaleX) && x.scaleY.Equals(y.scaleY) && x.octaves == y.octaves && x.persistence.Equals(y.persistence) && x.lacunarity.Equals(y.lacunarity) && x.remap.Equals(y.remap) && x.invert == y.invert && x.alpha.Equals(y.alpha);
            }

            public int GetHashCode(PerlinNoise obj)
            {
                var hashCode = new HashCode();
                hashCode.Add(obj.offset);
                hashCode.Add(obj.scaleX);
                hashCode.Add(obj.scaleY);
                hashCode.Add(obj.octaves);
                hashCode.Add(obj.persistence);
                hashCode.Add(obj.lacunarity);
                hashCode.Add(obj.remap);
                hashCode.Add(obj.invert);
                hashCode.Add(obj.alpha);
                return hashCode.ToHashCode();
            }
        }

        public static IEqualityComparer<PerlinNoise> PerlinNoiseComparer { get; } = new PerlinNoiseEqualityComparer();

        public Vector2 offset;
        [Range(0.01f,20)]
        public float scaleX;
        [Range(0.01f,20)]
        public float scaleY;
        [Range(1, 4)]
        [DefaultValue(1)]
        public int octaves;
        
        [Range(0,1)]
        public float persistence;
        [Range(1,10)]
        public float lacunarity;
        
        [MinMax(0f,1f)]
        public Vector2 remap;
        public bool invert;
        [Range(0,1)]
        public float alpha;

        public float Evaluate(float x, float y)
        {
            Vector2 frequency = new Vector2(1/scaleX, 1/scaleY);
            Vector2 position = offset;
            
            float amplitude = 1;
            float perlin = 1;
            
            for (int i = 0; i < octaves; i++)
            {
                float value = EvaluateNoise(x, y, frequency, position);
                perlin = math.lerp(perlin, value, amplitude);
                amplitude *= persistence;
                frequency *= lacunarity;
            }
            
            perlin = perlin.Remap(remap, 0, 1);
            perlin = math.saturate(perlin);
            perlin = invert ? perlin.OneMinus() : perlin;
            perlin = math.lerp(1, perlin, alpha);
            return math.saturate(perlin);
        }

        float EvaluateNoise(float x, float y, Vector2 frequency, Vector2 position)
        {
            float perlin = Mathf.PerlinNoise((x + position.x) * frequency.x, (y + position.y) * frequency.y);
            
            return math.saturate(perlin);
        }

        public float Evaluate(Vector2 vector)
        {
            return Evaluate(vector.x, vector.y);
        }
    }
}
