using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FoliageTool.Core
{
    /// <summary>
    /// An abstract class representing a brush that will be drawn on the terrain details.
    /// </summary>
    [ExecuteInEditMode]
    public abstract class Brush : MonoBehaviour
    {
        [Header("Brush")]
        [Range(-32, 32)]
        public int drawOrder = 0;
    
        public enum BlendMode { Blend, Add, Subtract }
        public BlendMode blendMode;

        public abstract Bounds GetBounds();

        public abstract Bounds GetInnerBounds();

        public static IEnumerable<Brush> GetBrushes(Terrain terrain, bool unordered = false, bool includeInactive = false)
        {
            IEnumerable<Brush> brushes = FindObjectsOfType<Brush>(includeInactive);
            brushes = brushes.Where(b => b.Intersects(terrain));
            
            if (unordered) return brushes;
            
            return brushes.OrderBy(b=> b.drawOrder);
        }

        public bool Intersects(Terrain terrain)
        {
            TerrainData terrainData = terrain.terrainData;
            Bounds b = terrainData.bounds;
            Bounds terrainBounds = new Bounds(b.center + terrain.transform.position, b.size);
            
            return GetBounds().Intersects(terrainBounds);
        }
    }

}