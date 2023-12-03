using UnityEngine;
using System.Collections;

namespace Flora.Utils
{
    public static class LayerExtensions
    {
        /// <summary>
        /// Does the layer mask contain the specified layer?
        /// </summary>
        public static bool Contains(this LayerMask mask, int layer)
        {
            return mask == (mask | (1 << layer));
        }
    }
}

