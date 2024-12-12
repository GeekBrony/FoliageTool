using UnityEngine;

namespace FoliageTool.Utils
{
    public static class RectExtensions
    {
        /// <summary>
        /// Normalize a RectInt from 0 to 1 using a max value.
        /// </summary>
        public static Rect Normalize(this RectInt rect, int max)
        {
            return new Rect((Vector2)rect.position / max, (Vector2)rect.size / max);
        }
    
        /// <summary>
        /// Normalize a Rect from 0 to 1 using a max value.
        /// </summary>
        public static Rect Normalize(this Rect rect, int max)
        {
            return new Rect(rect.position / max, rect.size / max);
        }
        
        /// <summary>
        /// Scale a Rect
        /// </summary>
        public static Rect Scale(this Rect rect, Vector2 scale)
        {
            return new Rect(rect.position * scale, rect.size * scale);
        }
        
        public static Rect Grow(this Rect rect, float units)
        {
            float x = Mathf.Clamp(0, rect.x - (units/2), rect.x);
            float y = Mathf.Clamp(0, rect.y - (units/2), rect.y);
            rect.position = new Vector2(x, y);
            rect.size += new Vector2(units/2, units/2);
            return rect;
        }

        public static Rect ToWorldSpace(this Rect rect, Vector2 posBase, Vector2 sizeBase)
        {
            Vector2 pos = posBase + (rect.position * sizeBase);
            Vector2 size = rect.size * sizeBase;
            return new Rect(pos, size);
        }
    
        /// <summary>
        /// Scale a Rect and convert to a RectInt
        /// </summary>
        public static RectInt ScaleToInt(this Rect rect, Vector2 scale)
        {
            Vector2Int position = Vector2Int.RoundToInt(rect.position * scale);
            Vector2Int size = Vector2Int.RoundToInt(rect.size * scale);
            return new RectInt(position, size);
        }
        
        public static RectInt ScaleToInt(this Rect rect, int scale)
        {
            return rect.ScaleToInt(new Vector2(scale, scale));
        }

        public static RectInt Grow(this RectInt rect, int units)
        {
            int x = Mathf.Clamp(0, rect.x - units, rect.x);
            int y = Mathf.Clamp(0, rect.y - units, rect.y);
            rect.position = new Vector2Int(x, y);
            rect.size += new Vector2Int(units, units);
            return rect;
        }
        
        public static RectInt FlipXY(this RectInt rect)
        {
            return new RectInt(
                // x,y = y,x
                rect.yMin, rect.xMin,
                // w,h = h,w
                rect.height, rect.width
            );
        }

        public static Rect Saturate(this Rect rect)
        {
            Vector2 pos = rect.position;
            pos.x = pos.x.Saturate();
            pos.y = pos.y.Saturate();
            
            Vector2 size = rect.size;
            size.x = size.x.Saturate();
            size.y = size.y.Saturate();
            return new Rect(pos, size);
        }

    }
}