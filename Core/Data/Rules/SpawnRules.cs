using System;
using System.Collections.Generic;
using System.Linq;
using FoliageTool.Utils;
using UnityEngine;

namespace FoliageTool.Core
{
    [Serializable]
    public class SpawnRules
    {
        [Space(5)]
        [MinMax(0.0f, 90f)]
        [Tooltip("Rule to limit the foliage to a range of steepness (in degrees)")]
        public Vector2 steepnessLimit = new(0, 45);
        
        [Space(5)]
        [MinMax(-10000, 10000)]
        [Tooltip("Rule to limit the foliage to a range of heights (in units)")]
        public Vector2 heightLimit = new(-10000, 10000);
          
        /*[Space(5)]
        [Range(0,10)]
        public int treePadding = 1;*/
        
        /// <summary>
        /// Rules to define where details should spawn, according to the terrain texture.
        /// </summary>
        [Space(5)]
        [Tooltip("Rules to define where details should spawn, according to the terrain texture.")]
        public List<TextureRule> textureRules  = new List<TextureRule>();
        
        [Tooltip("Rule to filter foliage with perlin noise.")]
        public PerlinNoise perlinNoise = new PerlinNoise()
        {
            offset = new Vector2(0, 0),
            scaleX = 1, scaleY = 1,
            alpha = 0, remap = new Vector2(0,1),
            lacunarity = 5, octaves = 1, persistence = 0.25f
        };
        

        public TextureRule[] GetTextureRules()
        {
            return textureRules.Where(t => !t.disable).ToArray();
        }
        

        public SpawnRules Clone()
        {
            var rules = MemberwiseClone() as SpawnRules;
            if (rules == null)
                return null;

            return rules;
        }
    }
}