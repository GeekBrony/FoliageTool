using System;
using System.Collections.Generic;
using UnityEngine;

namespace FoliageTool.Core
{
    [Serializable]
    public class Foliage
    {
        public bool bypass;
        public FoliageAsset asset;
        
        [Range(0, 4)]
        [Tooltip("Density of the foliage on the terrain.")]
        public float density = 1;
        
        [Space(10)]
        [Tooltip("The procedural rules for this foliage.")]
        public SpawnRules spawnRules;

        public Foliage()
        {
            spawnRules = new SpawnRules() { textureRules = new List<TextureRule>() };
        }
        
        public Foliage Clone()
        {
            var foliage = MemberwiseClone() as Foliage;
            if (foliage == null)
            {
                return null;
            }
            
            foliage.spawnRules = foliage.spawnRules.Clone();
        
            return foliage;
        }
    }
}