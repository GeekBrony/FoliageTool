using System;
using UnityEngine;

namespace FoliageTool.Core
{
    [Serializable]
    public class FoliageRefreshOptions
    {
        [Header("Change Events")]
        public bool onAlphamapChanged = true;
        public bool onHeightmapChanged = true;
        public bool onSplineChanged = true;
    }

}