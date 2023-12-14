using FoliageTool.Utils;
using UnityEngine;

namespace FoliageTool.Core
{
    /// <summary>
    /// Helper struct to define a terrain's region.
    /// </summary>
    public struct TerrainRegion
    {
        public Terrain Terrain;
        
        /// <summary>
        /// Normalized from [0 - 1]
        /// </summary>
        public Rect Region;
        
        /// <summary>
        /// Local space terrain region.
        /// This region is the size of the terrain, but does not have an offset.
        /// </summary>
        public Rect LocalRegion;
        
        /// <summary>
        /// World space terrain region.
        /// This region is the size of the terrain, and has the world offset of the terrain.
        /// </summary>
        public Rect WorldRegion;
        
        /// <summary>
        /// The region, translated to alpha map coordinates.
        /// </summary>
        public RectInt AlphaRegion;
        
        /// <summary>
        /// The region, translated to detail map coordinates.
        /// </summary>
        public RectInt DetailRegion;
        
        /// <summary>
        /// The region, translated to height map coordinates.
        /// </summary>
        public RectInt HeightRegion;

        public TerrainRegion(Terrain terrain, Rect rect)
        {
            Terrain = terrain;
            
            var data = terrain.terrainData;
            Vector3 terrainPos = terrain.GetPosition();
            
            Vector2 terrainPosition = new(terrainPos.x, terrainPos.z);
            Vector2 terrainSize = new(data.size.x, data.size.z);

            Region = rect.Saturate();
            LocalRegion = Region.Scale(terrainSize);
            WorldRegion = Region.ToWorldSpace(terrainPosition, terrainSize);
            AlphaRegion = Region.ScaleToInt(data.alphamapResolution);
            DetailRegion = Region.ScaleToInt(data.detailResolution);
            HeightRegion = Region.ScaleToInt(data.heightmapResolution);
        }
        
        public void FlipXY()
        {
            DetailRegion = DetailRegion.FlipXY();
        }

        /// <summary>
        /// Convert bounds to a Rect region on a terrain, and create a TerrainRegion from the result.
        /// </summary>
        public static TerrainRegion FromBounds(Terrain terrain, Bounds bounds, float padding = 10)
        {
            TerrainData data = terrain.terrainData;
            Vector3 terrainPos = terrain.GetPosition();
            var terrainBounds = data.bounds;

            terrainBounds.min += terrainPos;
            terrainBounds.max += terrainPos;

            // Add padding to the bounds
            bounds.Expand(padding);
            
            // Calculate the intersection between the spline bounds and terrain bounds
            bounds.min = Vector3.Max(bounds.min, terrainBounds.min);
            bounds.max = Vector3.Min(bounds.max, terrainBounds.max);
            
            Rect normalizedRect = new Rect();
            
            // Check if the intersection is still valid (not empty)
            if (bounds.min.x < bounds.max.x && bounds.min.z < bounds.max.z)
            {
                // Calculate the normalized coordinates within this terrain
                normalizedRect = new Rect(
                    (bounds.min.x - terrainBounds.min.x) / terrainBounds.size.x,
                    (bounds.min.z - terrainBounds.min.z) / terrainBounds.size.z,
                    bounds.size.x / terrainBounds.size.x,
                    bounds.size.z / terrainBounds.size.z
                );
            }

            return new TerrainRegion(terrain, normalizedRect);
        }

        public override string ToString()
        {
            return $"{Region:F3}\n"+
                   $"Local:\t{LocalRegion:F3}\n" +
                   $"World:\t{WorldRegion:F3}\n" +
                   $"Alpha:\t{AlphaRegion}\n" +
                   $"Detail:\t{DetailRegion}\n" +
                   $"Height:\t{HeightRegion}";
        }
    }
}