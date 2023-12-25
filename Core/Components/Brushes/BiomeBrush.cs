using System.Collections.Generic;
using System.Linq;
using FoliageTool.Utils;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Splines;

namespace FoliageTool.Core
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(SplineContainer))]
    public class BiomeBrush : SplineBrush
    {
        [Header("Biome")]
        public BiomeAsset biome;
        
        [Header("Debug")]
        public bool drawBounds = false;
        public bool drawPolygon = false;
        
        private void OnEnable()
        {
            if (biome)
                biome.OnBiomeEdit += OnBiomeEdit;

            ScheduleRefresh();
        }

        private void OnDisable()
        {
            if (biome)
                biome.OnBiomeEdit -= OnBiomeEdit;
            
#if UNITY_EDITOR
            if (Application.isPlaying)
                return;

            ScheduleRefresh();
#endif
        }

        private void OnBiomeEdit()
        {
            Refresh();
        }

        public void ScheduleRefresh()
        {
            if(_scheduleRefresh)
                return;

#if UNITY_EDITOR
            bool isUpdating = EditorApplication.isUpdating;
            bool isCompiling = EditorApplication.isCompiling;
            if(isUpdating || isCompiling)
                return;
#endif
            
            if (enabled)
            {
                _scheduleRefresh = true;
                return;
            }
            
            Refresh();
        }

        private bool _scheduleRefresh = false;
        void Update()
        {
            if (_scheduleRefresh)
            {
                _scheduleRefresh = false;
                
                Refresh();
            }
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

        protected override bool WillDrawDebugPolygon()
        {
            return drawPolygon;
        }

        protected override bool WillDrawDebugBounds()
        {
            return drawBounds;
        }
    }
}