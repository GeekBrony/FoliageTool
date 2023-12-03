using System.Collections.Generic;
using Flora.Core;
using Unity.Mathematics;
using UnityEngine;

namespace Flora.Utils
{
    public static class TerrainDataExtensions
    {
        public static float[,,] GetAlphamaps(this TerrainData data, Vector2Int positionBase, Vector2Int size)
        {
            return data.GetAlphamaps(positionBase.x, positionBase.y, size.x, size.y);
        }
    }
}