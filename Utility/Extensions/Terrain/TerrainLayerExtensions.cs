using UnityEngine;

namespace Flora.Utils
{
    public static class TerrainLayerExtensions
    {
        public static int IndexOf(this TerrainLayer[] layers, TerrainLayer layer)
        {
            for (int i = 0; i < layers.Length; i++)
            {
                if (layers[i].Equals(layer))
                    return i;
            }
            return -1;
        }
    }
}