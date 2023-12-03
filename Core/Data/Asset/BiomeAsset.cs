using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Flora.Core
{
    [CreateAssetMenu(fileName = "Biome", menuName = "Flora/Biome", order = 0)]
    public class BiomeAsset : ScriptableObject
    {
        public List<Foliage> foliage = new List<Foliage>();

        public event Action OnBiomeEdit;
        public void OnEdit()
        {
            OnBiomeEdit?.Invoke();
        }
        
        /// <summary>
        /// Find the Foliage element that resembles this DetailPrototype.
        /// </summary>
        public Foliage GetFoliage(DetailPrototype prototype)
        {
            for (int i = 0; i < foliage.Count; ++i)
            {
                Foliage detail = foliage[i];
                if(detail == null)
                    continue;
            
                if (detail.asset.Prototype.Equals(prototype)) 
                    return detail;
            }

            return null;
        }
        
        /// <summary>
        /// Get all DetailPrototypes in this biome.
        /// </summary>
        public DetailPrototype[] GetPrototypes()
        {
            IEnumerable<DetailPrototype> prototypes = foliage.Select(f => f.asset.Prototype);
            return prototypes.Where(p => p != null).ToArray();
        }
    }
}