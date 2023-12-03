using UnityEngine;

namespace Flora.Core
{
    /// <summary>
    /// Helper struct to determine the terrain position given an x and y coordinate within a region, and translate it to positions on alphamap, heightmap, terrain, and world.
    /// </summary>
    public struct TerrainPosition
    {
        /// <summary>
        /// 2D normalized REGION position
        /// </summary>
        public Vector2 RegionPosition2D { get; }
        
        /// <summary>
        /// 2D normalized TERRAIN position
        /// </summary>
        public Vector2 TerrainPosition2D { get; }
        
        /// <summary>
        /// world position 2D: ZX
        /// </summary>
        public Vector2 WorldPosition2D { get; }
        
        public Vector3 WorldPosition3D { get; }
        
        public Vector2Int DetailPosition { get; }

        public Vector2Int AlphaPosition { get; }
        
        public Vector2Int HeightPosition { get; }
        

        public TerrainPosition(Terrain terrain, TerrainRegion region, int x, int y)
        {
            TerrainData data = terrain.terrainData;
            var worldRegion = region.WorldRegion;
            var detailRegion = region.DetailRegion;
            
            // position in detail space
            DetailPosition = new Vector2Int(x, y);
            
            // position in normalized REGION space
            RegionPosition2D = new Vector2(x, y) / data.detailResolution;
            
            // position in normalized TERRAIN space
            TerrainPosition2D = (detailRegion.position + new Vector2(x, y)) / data.detailResolution;
            
            Vector2 alphaPos = RegionPosition2D * data.alphamapResolution;
            AlphaPosition = Vector2Int.FloorToInt(alphaPos);
            
            HeightPosition = Vector2Int.FloorToInt(RegionPosition2D * data.heightmapResolution);

            // position in world space
            WorldPosition2D = new Vector2(worldRegion.position.y, worldRegion.position.x) +
                              (RegionPosition2D * new Vector2(data.size.x, data.size.z));
            
            WorldPosition3D = new Vector3(WorldPosition2D.x, 0, WorldPosition2D.y);
        }
    }
}