using System;
using System.Collections.Generic;
using UnityEngine;

namespace Flora.Core
{
    [Serializable]
    public class Foliage
    {
        public FoliageAsset asset;
        public bool bypass;
        
        [Range(0, 4)]
        public float density = 1;
        [Space(10)]
        public SpawnRules spawnRules;

        public Foliage()
        {
            spawnRules = new SpawnRules()
            {
                textureRules = new List<TextureRule>()
            };
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