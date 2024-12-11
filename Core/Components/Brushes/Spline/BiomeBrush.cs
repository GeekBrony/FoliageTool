using System.Collections.Generic;
using System.Linq;
using FoliageTool.Utils;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace FoliageTool.Core
{
    [ExecuteAlways]
    [RequireComponent(typeof(SplineContainer))]
    public class BiomeBrush : SplineBrush
    {
        [Header("Biome")]
        public BiomeAsset biome;
        
        [Header("Debug")]
        public bool drawBounds = false;
        public bool drawPolygon = false;
        
        protected override void OnEnable()
        {
            base.OnEnable();
            
            if (biome)
                biome.OnBiomeEdit += OnBiomeEdit;
        }

        protected override void OnDisable()
        {
            base.OnDisable();
            
            if (biome)
                biome.OnBiomeEdit -= OnBiomeEdit;
        }

        private void OnBiomeEdit()
        {
            Refresh();
        }
        
        public static IEnumerable<BiomeAsset> GetBiomes(IEnumerable<BiomeBrush> splines)
        {
            List<BiomeAsset> biomes = new List<BiomeAsset>();
            foreach (var b in splines)
            {
                if (!biomes.Contains(b.biome))
                    biomes.Add(b.biome);
            }

            return biomes;
        }

        public override bool WillDrawDebugPolygon()
        {
            return drawPolygon;
        }

        public override bool WillDrawDebugBounds()
        {
            return drawBounds;
        }
    }
}