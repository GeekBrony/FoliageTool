using UnityEngine;

namespace Flora.Utils
{
    public static class TerrainExtensions
    {
        /// <summary>
        /// Get the terrain's world position of a normalized point x,y,z.
        /// <br/>
        /// The coordinates are in the range of [0, 1] if they are within the bounds of the terrain.
        /// </summary>
        public static Vector3 GetWorldPosition(this Terrain terrain, Vector3 pos)
        {
            Vector3 size = terrain.terrainData.size;
            Vector3 offset = new Vector3(pos.x * size.x, pos.y * size.y, pos.z * size.z);
            return terrain.GetPosition() + offset;
        }
        
        /// <summary>
        /// Get the terrain's world position of a normalized point x,y,z.
        /// <br/>
        /// The coordinates are in the range of [0, 1] if they are within the bounds of the terrain.
        /// </summary>
        public static Vector3 GetWorldPosition(this Terrain terrain, Vector2 pos)
        {
            Vector3 size = terrain.terrainData.size;
            Vector3 offset = new Vector3(pos.x * size.x, terrain.terrainData.GetInterpolatedHeight(pos.y, pos.x), pos.y * size.z);
            return terrain.GetPosition() + offset;
        }
    }
}