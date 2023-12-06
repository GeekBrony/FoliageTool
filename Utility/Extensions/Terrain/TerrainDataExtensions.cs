using System.Collections.Generic;
using FoliageTool.Core;
using Unity.Mathematics;
using UnityEngine;

namespace FoliageTool.Utils
{
    public static class TerrainDataExtensions
    {
        public static float[,,] GetAlphamaps(this TerrainData data, Vector2Int positionBase, Vector2Int size)
        {
            return data.GetAlphamaps(positionBase.x, positionBase.y, size.x, size.y);
        }
    }
}