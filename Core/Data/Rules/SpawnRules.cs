using System;
using System.Collections.Generic;
using System.Linq;
using Flora.Utils;
using UnityEngine;

namespace Flora.Core
{
    [Serializable]
    public class SpawnRules
    {
        [Space(5)]
        [MinMax(0.0f, 90f)]
        public Vector2 steepnessLimit = new(0, 45);
        
        [Space(5)]
        [MinMax(-10000, 10000)]
        public Vector2 heightLimit = new(-10000, 10000);
          
        /*[Space(5)]
        [Range(0,10)]
        public int treePadding = 1;*/
        
        /// <summary>
        /// Rules to define where details should spawn, according to the terrain texture.
        /// </summary>
        public List<TextureRule> textureRules  = new List<TextureRule>();

        public TextureRule[] GetTextureRules()
        {
            return textureRules.Where(t => !t.bypass).ToArray();
        }
        
        public PerlinNoise perlinNoise = new PerlinNoise()
        {
            offset = new Vector2(0, 0),
            scaleX = 1, scaleY = 1,
            alpha = 1, remap = new Vector2(0,1),
            lacunarity = 5, octaves = 1, persistence = 0.25f
        };

        public SpawnRules Clone()
        {
            var rules = MemberwiseClone() as SpawnRules;
            if (rules == null)
                return null;

            return rules;
        }
    }
}