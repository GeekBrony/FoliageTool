// Copyright (c) 1970-2003, Wm. Randolph Franklin
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated
// documentation files (the "Software"), to deal in the Software without restriction, including without limitation
// the rights to use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of the Software,
// and to permit persons to whom the Software is furnished to do so, subject to the following conditions:
//
//  1. Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimers.
//  2. Redistributions in binary form must reproduce the above copyright notice in the documentation and/or other materials provided with the distribution.
//  3. The name of W. Randolph Franklin may not be used to endorse or promote products derived from this Software without specific prior written permission. 
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO
// THE WARRANTIES OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT,
// TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE. 

using System.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace FoliageTool.Utils
{
    public struct Polygon
    {
        public float3[] Points;

        public Polygon(float3[] points)
        {
            Points = points;
        }
        
        // Point inclusion in polygon test
        // Source: https://wrf.ecse.rpi.edu/Research/Short_Notes/pnpoly.html
        // See copyright notice at the top of the file for licensing.
        // Trace a ray horizontally from a point (p), and count the intersections (n).
        // If n is odd, the point is inside the polygon. If n is even, the point is outside.
        public bool Contains(float3 point)
        {
            float2 min = new float2(Points[0].x, Points[0].z);
            float2 max = new float2(Points[0].x, Points[0].z);
            for (int i = 1; i < Points.Length; i++)
            {
                float3 q = Points[i];
                min.x = math.min(q.x, min.x);
                max.x = math.max(q.x, max.x);
                min.y = math.min(q.z, min.y);
                max.y = math.max(q.z, max.y);
            }

            if (point.x < min.x || point.x > max.x || point.z < min.y || point.z > max.y)
                return false;

            bool inside = false;
            for (int i = 0, j = Points.Length - 1; i < Points.Length; j = i++)
            {
                if ((Points[i].z > point.z) != (Points[j].z > point.z) &&
                    point.x < (Points[j].x - Points[i].x) *(point.z - Points[i].z) / (Points[j].z - Points[i].z) + Points[i].x)
                {
                    inside = !inside;
                }
            }

            return inside;
        }

        public Bounds GetBounds()
        {
            if (Points.Length == 0)
                return new Bounds();
            
            Bounds b = new Bounds(Points[0], Vector3.zero);
            foreach (float3 f in Points)
                b.Encapsulate(f);

            return b;
        }
        
        public Vector3 GetClosestPoint(Vector3 pos)
        {
            float min = float.MaxValue;
            Vector3 p = pos;
            foreach (float3 v in Points)
            {
                float dist = math.distance(pos, v);
                if (dist < min)
                {
                    p = v;
                    min = dist;
                }
            }
            return p;
        }
    }
}