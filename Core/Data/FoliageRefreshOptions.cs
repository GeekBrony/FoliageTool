using System;
using UnityEngine;

namespace FoliageTool.Core
{
    [Serializable]
    public class FoliageRefreshOptions
    {
        [Tooltip("Using this max resolution, divide the terrain into chunks for refreshing.")]
        public int maxChunkResolution = 256;
        
        [Header("Refresh Events Toggle")]
        public bool onAlphamapChanged = true;
        public bool onHeightmapChanged = true;
        public bool onSplineChanged = true;
    }

}