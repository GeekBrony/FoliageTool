using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

namespace FoliageTool.Utils
{
    public static class VectorExtensions
    {
        /// <summary>
        /// Randomize a Vector3 on three axis.
        /// </summary>
        public static Vector3 Randomized(this Vector3 v, float xMagnitude, float yMagnitude, float zMagnitude)
        {
            v.x += Random.Range(-xMagnitude, xMagnitude);
            v.y += Random.Range(-yMagnitude, yMagnitude);
            v.z += Random.Range(-zMagnitude, zMagnitude);

            return v;
        }
        
        public static Vector3 Direction(this Vector3 from, Vector3 to)
        {
            Vector3 dir = to - from;
            return dir / dir.magnitude;
        }

        public static Vector2Int Clamp(this Vector2Int v, int minX, int minY, int maxX, int maxY)
        {
            return new Vector2Int(math.clamp(v.x, minX, maxX), math.clamp(v.y, minY, maxY));
        }
    }
    
}