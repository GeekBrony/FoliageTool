using UnityEngine;

namespace FoliageTool.Core
{
    /// <summary>
    /// Helper struct to determine the terrain position given an x and y coordinate within a region, and translate it to positions on alphamap, heightmap, terrain, and world.
    /// </summary>
    public struct TerrainPosition
    {
        private Terrain Terrain { get; }
        private TerrainRegion Region { get; }
        
        /// <summary>
        /// 2D normalized REGION position
        /// </summary>
        public Vector2 RegionPosition2D { get; private set; }
        
        /// <summary>
        /// 2D normalized TERRAIN position
        /// </summary>
        public Vector2 TerrainPosition2D { get; private set; }
        
        /// <summary>
        /// world position 2D: ZX
        /// </summary>
        public Vector2 WorldPosition2D { get; private set; }
        
        public Vector3 WorldPosition3D { get; private set; }
        
        public Vector2Int DetailPosition { get; private set; }

        public Vector2Int AlphaPosition { get; private set; }
        
        public Vector2Int HeightPosition { get; private set; }
        
        public TerrainPosition(Terrain terrain, TerrainRegion region)
        {
            Terrain = terrain;
            Region = region;
            
            RegionPosition2D = default;
            TerrainPosition2D = default;
            WorldPosition2D = default;
            WorldPosition3D = default;
            DetailPosition = default;
            AlphaPosition = default;
            HeightPosition = default;
        }

        public void SetPosition(int x, int y)
        {
            TerrainData data = Terrain.terrainData;
            var worldRegion = Region.WorldRegion;
            var detailRegion = Region.DetailRegion;
            
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