using FoliageTool.Utils;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Splines;

namespace FoliageTool.Core
{
    public abstract class SplineBrush : Brush
    {
        [Header("Spline")]
        public SplineContainer spline;
        [Range(3,128)]
        public int resolution = 16;
        [Range(0, 1)]
        public float falloff = 0;
        [Range(0, 1)]
        public float alpha = 1;
        
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
        
        protected Bounds ComputeInnerBounds(Bounds b)
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
        
        private void OnValidate()
        {
            if (!spline)
            {
                spline = GetComponent<SplineContainer>();
            }
        }

        protected abstract bool WillDrawDebugPolygon();

        protected override void DrawGizmosSelected()
        {
            Gizmos.color = new Color(1,1,1, alpha);
            
            if (WillDrawDebugPolygon())
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
        }
        
        bool CanRefresh(FoliageTerrain terrain)
        {
            return terrain.refreshOptions.onSplineChanged
                   && Intersects(terrain.terrain);
        }
        
        public void Refresh()
        {
            Bounds b = GetBounds();
            Refresh(b);
        }
        
        public void Refresh(Bounds bounds)
        {
            foreach (FoliageTerrain terrain in FindObjectsOfType<FoliageTerrain>())
            {
                if(!CanRefresh(terrain))
                    continue;
                
                terrain.Sync(out DetailPrototype[] detailPrototypes);
                
                var region = TerrainRegion.FromBounds(terrain.terrain, bounds, 10);
                // sync all detail prototypes with the terrain
                terrain.Refresh(region, detailPrototypes);
            }
        }

        /*public void MovePivotToCenter()
        {
            foreach (var knot in spline.Spline.Knots)
            {
                knot.Position = 
            }
        }*/
    }
}