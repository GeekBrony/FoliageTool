using System;
using UnityEngine;

namespace FoliageTool.Utils
{
    /// <summary>
    /// Attribute to select a range of floats in Vector2
    /// </summary>

    [AttributeUsage(AttributeTargets.Field, Inherited = true, AllowMultiple = false)]
    public class MinMaxAttribute : PropertyAttribute
    {
        public float Min { get; set; }
        public float Max { get; set; }
        public bool DataFields { get; set; } = true;
        public bool FlexibleFields { get; set; } = false;
        public bool Bound { get; set; } = true;
        public bool Round { get; set; } = true;

        public MinMaxAttribute() : this(0, 1)
        {
        }

        public MinMaxAttribute(float min, float max)
        {
            Min = min;
            Max = max;
        }
    }
}