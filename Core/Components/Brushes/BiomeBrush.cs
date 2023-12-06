using System;
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
    public class BiomeBrush : Brush
    {
        [HideInInspector]
        public SplineContainer spline;

        [Header("Biome")]
        public BiomeAsset biome;

        [Header("Spline")]
        [Range(3,128)]
        public int resolution = 16;
        [Range(0, 1)]
        public float falloff = 0;
        [Range(0, 1)]
        public float alpha = 1;
        
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
            Bounds b = GetBounds();
            Refresh(b);
        }

        private const float FMaxHeight = 10000;

        public override Bounds GetBounds()
        {
            // this discards the polygon after it's done.
            // TODO: cache so calculation doesn't happen too often.
            Bounds b = GetPolygon().GetBounds();

            // adjust height to make sure bounds checks intersect with a terrain.
            float height = FMaxHeight / 2;
            b.max = new Vector3(b.max.x, b.max.y + height, b.max.z);
            b.min = new Vector3(b.min.x, b.min.y - height, b.min.z);
            
            return b;
        }
        
        public override Bounds GetInnerBounds()
        {
            return ComputeInnerBounds(GetBounds());
        }

        Bounds ComputeInnerBounds(Bounds b)
        {
            float f = falloff.OneMinus();
            b.size = new Vector3(b.size.x * f, b.size.y, b.size.z * f);
            return b;
        }
        
        /// <summary>
        /// Evaluate all points on this spline using the resolution.
        /// </summary>
        public Polygon GetPolygon()
        {
            float3[] p = new float3[resolution];
            
            if (!spline)
                return new Polygon(p);

            for (int i = 0; i < resolution; i++)
            {
                p[i] = spline.EvaluatePosition((float)i / resolution);
            }
            
            return new Polygon(p);
        }

        void OnDrawGizmosSelected()
        {
            if(!enabled)
                return;
            
            Gizmos.color = new Color(1,1,1, alpha);
            if (drawPolygon)
            {
                Polygon p = GetPolygon();
                var pos = p.Points;
                for (int i = 0; i < pos.Length; i++)
                {
                    if (i == pos.Length - 1)
                        Gizmos.DrawLine(pos[i], pos[0]);
                    else
                        Gizmos.DrawLine(pos[i], pos[i + 1]);
                }
            }

            if (drawBounds)
            {
                Bounds b = GetBounds();
                b.size = new Vector3(b.size.x, 0, b.size.z);
                Gizmos.DrawWireCube(b.center, b.size);
                
                Bounds ib = ComputeInnerBounds(b);
                ib.size = new Vector3(ib.size.x, 0, ib.size.z);
                Gizmos.DrawWireCube(ib.center, ib.size);
            }
        }
        
        private void OnValidate()
        {
            if (!spline)
            {
                spline = GetComponent<SplineContainer>();
            }
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
            
            Bounds b = GetBounds();
            Refresh(b);
        }

        private bool _scheduleRefresh = false;
        void Update()
        {
            if (_scheduleRefresh)
            {
                _scheduleRefresh = false;
                
                Bounds b = GetBounds();
                Refresh(b);
            }
        }

        public void Refresh(Bounds bounds)
        {
            foreach (FoliageTerrain terrain in FindObjectsOfType<FoliageTerrain>())
            {
                if(!Intersects(terrain.terrain))
                    continue;
                
                var region = TerrainRegion.FromBounds(terrain.terrain, bounds, 10);
                terrain.Refresh(region);
            }
        }
        
        public static IEnumerable<BiomeBrush> GetSplines(Terrain terrain, bool unordered = false)
        {
            IEnumerable<BiomeBrush> biomes = FindObjectsOfType<BiomeBrush>(false);
            biomes = biomes.Where(b => b.Intersects(terrain));
            
            if (unordered)
                return biomes;
            
            return biomes.OrderBy(b=> b.drawOrder);
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
        
        
        private const float FalloffDefault = 1;
        public float CalculateFalloff(Bounds outerBounds, Bounds innerBounds, float3 pos, Polygon polygon)
        {
            if (falloff < 0.001f)
                return FalloffDefault;

            // get the closest spline point
            float3 v = polygon.GetClosestPoint(pos);
            
            // size difference between the outer bounds and inner bounds
            float3 size = outerBounds.size - innerBounds.size;

            // scale the positions with the size difference.
            float3 a = new float3(v.x / size.x, 0, v.z / size.z);
            float3 b = new float3(pos.x / size.x, 0, pos.z / size.z);
            
            // measure the distance from the position to the closest point
            float dist = math.distance(a, b);
            return math.saturate(dist) * (1 - falloff);
        }
    }
}