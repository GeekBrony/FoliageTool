using System;
using UnityEngine;
using Random = UnityEngine.Random;

namespace Flora.Utils
{
    public static class FloatExtensions
    {
        /// <summary>
        /// Get a float random range
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static float GetRandom(this Vector2 value)
        {
            return Random.Range(value.x, value.y);
        }

        /// <summary>
        /// Reverse (one-minus) a float from a to b
        /// </summary>
        /// <returns></returns>
        public static float OneMinus(this float f, float magnitude = 1)
        {
            return magnitude - f;
        }
        
        /// <summary>
        /// Remap a float value from one range to another
        /// </summary>
        /// <returns></returns>
        public static float Remap(this float f, float inMin, float inMax, float outMin, float outMax)
        {
            return outMin + (f - inMin) * (outMax - outMin) / (inMax - inMin);
        }
        
        /// <summary>
        /// Remap a float value from the range of a, to the range of b.
        /// This can be expressed in Vector2 to make it easy to read
        /// </summary>
        /// <returns></returns>
        public static float Remap(this float f, float inMin, float inMax, Vector2 b)
        {
            return Remap(f, inMin, inMax, b.x, b.y);
        }
        
        /// <summary>
        /// Remap a float value from the range of a, to the range of b.
        /// This can be expressed in Vector2 to make it easy to read
        /// </summary>
        /// <returns></returns>
        public static float Remap(this float f, Vector2 a, Vector2 b)
        {
            return Remap(f, a.x, a.y, b.x, b.y);
        }
        
        /// <summary>
        /// Remap a float value from the range of a, to the range of b.
        /// This can be expressed in Vector2 to make it easy to read
        /// </summary>
        /// <returns></returns>
        public static float Remap(this float f, Vector2 a, float bMin, float bMax)
        {
            return Remap(f, a.x, a.y, bMin, bMax);
        }
        
        /// <summary>
        /// Normalize a float
        /// </summary>
        public static float Normal(this float f, float min, float max)
        {
            return (f - min) / (max - min);
        }
        
        /// <summary>
        /// Normalize the input from min to max, absolute value, then saturate the value from [0 - 1]
        /// </summary>
        public static float NormalAbsClamp(this float f, float min, float max)
        {
            float normal = f.Normal(min, max);
            float abs = Mathf.Abs(normal);
            return abs.Saturate();
        }

        /// <summary>
        /// Normalize the input, 0-1, from min to max, then clamp the value from min to max.
        /// </summary>
        public static float NormalizeClamp(this float f, float min, float max)
        {
            float normal = Normal(f, min, max);
            return Mathf.Clamp01(normal);
        }

        /// <summary>
        /// Clamp a value from Min to Max
        /// </summary>
        public static float Clamp(this float f, float min, float max)
        {
            return Mathf.Clamp(f, min, max);
        }

        /// <summary>
        /// Saturate a value, clamping to a 0 to 1 value
        /// </summary>
        /// <returns></returns>
        public static float Saturate(this float f)
        {
            return Mathf.Clamp01(f);
        }
    }
}