using System;
using FoliageTool.Utils;
using UnityEngine;

namespace FoliageTool.Core
{
    [CreateAssetMenu(menuName = "FoliageTool/Foliage", fileName = "Foliage", order = 10000)]
    public class FoliageAsset : ScriptableObject
    {
        [Header("Prototype")]
        [Tooltip("The GameObject/Prefab that will be used.")]
        public GameObject prefab;
    
        [Header("Placement")]
        [Range(0,1)]
        [Tooltip("Rotate detail axis parallel to the ground's normal direction, so that the detail is perpendicular to the ground.")]
        public float alignToGround = 0.0f;
        
        [Range(0,1)]
        [Tooltip("Apply positional jitter. 0 is ordered and are less likely to overlap, and 1 is random and more likely to overlap")]
        public float positionJitter = 0.0f;
    
        [Header("Scaling")]
        [MinMax(0.001f, 4f)]
        [Tooltip("Min and max width of this foliage type")]
        public Vector2 width = new(1, 2);
        
        [MinMax(0.001f, 4f)]
        [Tooltip("Min and max height of this foliage type")]
        public Vector2 height = new(1, 2);
        
        [Header("Noise")]
        [Tooltip("Specifies the random seed value for detail object placement")]
        public int noiseSeed = -1;
        
        [Tooltip("Controls the spatial frequency of the noise pattern used to vary the scale and color of the detail objects.")]
        public float noiseSpread = 0.1f;
        
        [Header("Density")]
        [Range(0, 5)]
        [Tooltip("Controls density for this foliage type, relative to it's size.")]
        public float foliageDensity = 1;
        
        [Tooltip("Enable if you require this prototype to change density in runtime (settings menu for example)")]
        public bool useTerrainDensity = true;
        
        [Header("Other")]
        [Range(0,1)]
        [Tooltip("When using Terrain Holes, this controls how far away detail objects are from the edge of the hole area.")]
        public float holeEdgePadding = 0f;

        #region Private Members

        /// <summary>
        /// Controls the amount of target coverage desired while scattering the detail.
        /// This is not needed as a configurable setting.
        /// </summary>
        private readonly float _targetCoverage = 1f;
        
        /// <summary>
        /// Specifies the render mode for detail prototypes.
        /// This is not needed as a configurable setting - VertexLit is the only supported mode.
        /// </summary>
        private readonly DetailRenderMode _renderMode = DetailRenderMode.VertexLit;
        
        /// <summary>
        /// Indicates whether this detail prototype uses GPU Instancing for rendering.
        /// This is not needed as a configurable setting.
        /// </summary>
        private readonly bool _useInstancing = true;
        
        /// <summary>
        /// Indicates whether this detail prototype uses the Mesh object from the GameObject specified by prototype.
        /// This is not needed as a setting because all foliage is already assumed to be meshes.
        /// </summary>
        private readonly bool _usePrototypeMesh = true;
        
        /// <summary>
        /// Texture used by the DetailPrototype.
        /// This is not needed as a setting because all foliage is already assumed to be meshes.
        /// </summary>
        private readonly Texture2D _prototypeTexture = null;

        #endregion

        private void Reset()
        {
            // Generate a seed at random.
            // This can help us track duplicates.
            System.Random r = new System.Random();
            noiseSeed = r.Next();
        }

        public DetailPrototype Prototype =>
            new() {
                prototype = prefab,
                prototypeTexture = _prototypeTexture,
                minWidth = width.x, maxWidth = width.y,
                minHeight = height.x, maxHeight = height.y,
                alignToGround = alignToGround,
                positionJitter = positionJitter,
                holeEdgePadding = holeEdgePadding,
                noiseSeed = noiseSeed,
                noiseSpread = noiseSpread,
                useDensityScaling = useTerrainDensity,
                targetCoverage = _targetCoverage,
                density = foliageDensity,
                usePrototypeMesh = _usePrototypeMesh,
                useInstancing = _useInstancing,
                renderMode = _renderMode,
            };
    }
}