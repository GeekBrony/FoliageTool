using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FoliageTool.Core
{
    /// <summary>
    /// An abstract class representing a brush that will be drawn on the terrain details.
    /// </summary>
    [ExecuteAlways]
    public abstract class Brush : MonoBehaviour
    {
        [Range(-32, 32)]
        [Header("Brush")]
        [Tooltip("The layer at which this brush will draw")]
        public int drawOrder = 0;
    
        public enum BlendMode { Blend, Add, Subtract }
        [Tooltip("The mode that will determine how the brush will blend.")]
        public BlendMode blendMode;

        public abstract Bounds GetBounds();

        public abstract Bounds GetInnerBounds();
        
        public abstract bool WillDrawDebugBounds();

        protected virtual void OnEnable()
        {
#if UNITY_EDITOR
            if(Application.isPlaying || !BrushEditing.CanRefresh())
                return;

            ScheduleRefresh();
#endif
        }
        
        protected virtual void OnDisable()
        {
#if UNITY_EDITOR
            if(Application.isPlaying || !BrushEditing.CanRefresh())
                return;

            Refresh();
#endif
        }

        private void OnValidate()
        {
            Validate();
        }

        public virtual void Validate()
        {
            
        }

        /// <summary>
        /// Check if this brush intersects with a given terrain
        /// </summary>
        /// <param name="terrain"></param>
        /// <returns></returns>
        public bool Intersects(Terrain terrain)
        {
            TerrainData terrainData = terrain.terrainData;
            Bounds b = terrainData.bounds;
            Bounds terrainBounds = new Bounds(b.center + terrain.transform.position, b.size);
            
            return GetBounds().Intersects(terrainBounds);
        }

        public static IEnumerable<Brush> GetBrushes(Terrain terrain, bool unordered = false, bool includeInactive = false)
        {
#if UNITY_6000_0_OR_NEWER
            IEnumerable<Brush> brushes = FindObjectsByType<Brush>(includeInactive ? 
                FindObjectsInactive.Include : FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
#else
            IEnumerable<Brush> brushes = FindObjectsOfType<Brush>(includeInactive);
#endif
            
            brushes = brushes.Where(b => b.Intersects(terrain));
            
            if (unordered) return brushes;
            
            return brushes.OrderBy(b=> b.drawOrder);
        }
        
        public static IEnumerable<T> GetBrushes<T>(Terrain terrain, bool unordered = false, bool includeInactive = false)
            where T : Brush
        {
            return GetBrushes(terrain, unordered, includeInactive).OfType<T>();
        }
        
        void OnDrawGizmosSelected()
        {
            if (WillDrawDebugBounds())
            {
                Bounds b = GetBounds();
                b.size = new Vector3(b.size.x, 0, b.size.z);
                Gizmos.DrawWireCube(b.center, b.size);
                
                Bounds ib = GetInnerBounds();
                ib.size = new Vector3(ib.size.x, 0, ib.size.z);
                Gizmos.DrawWireCube(ib.center, ib.size);
            }
            
            DrawGizmosSelected();
        }

        /// <summary>
        /// Virtual function - override to draw extra gizmos when this brush is selected.
        /// </summary>
        protected virtual void DrawGizmosSelected()
        {
            
        }
        
        private bool _scheduleRefresh = false;
        public void ScheduleRefresh()
        {
            if(_scheduleRefresh)
                return;
            
#if UNITY_EDITOR
            if(!BrushEditing.CanRefresh())
                return;
#endif
            
            if (enabled)
            {
                _scheduleRefresh = true;
                return;
            }
            
            Refresh();
        }
        
        void Update()
        {
            if (_scheduleRefresh)
            {
                _scheduleRefresh = false;
                
                Refresh();
            }
        }

        protected abstract bool CanRefresh(FoliageTerrain terrain);
        
        public void Refresh()
        {
            Bounds b = GetBounds();
            Refresh(b);
        }
        
        public void Refresh(Bounds bounds)
        {
#if UNITY_6000_0_OR_NEWER
            IEnumerable<FoliageTerrain> terrains = FindObjectsByType<FoliageTerrain>(FindObjectsInactive.Exclude, FindObjectsSortMode.InstanceID);
#else
            IEnumerable<FoliageTerrain> terrains = FindObjectsOfType<FoliageTerrain>();
#endif
            
            foreach (FoliageTerrain terrain in terrains)
            {
                if(!CanRefresh(terrain))
                    continue;
                
                terrain.Sync(out DetailPrototype[] detailPrototypes);
                
                var region = TerrainRegion.FromBounds(terrain.terrain, bounds, 10);
                // sync all detail prototypes with the terrain
                terrain.Refresh(region, detailPrototypes);
            }
        }
        
    }

}