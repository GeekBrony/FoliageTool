using System.Collections;
using System.Collections.Generic;
using System.Linq;
using FoliageTool.Utils;
using Unity.Mathematics;
using UnityEngine;
using Random = UnityEngine.Random;

#if FOLIAGE_DEBUG
using System.Diagnostics;
using Debug = UnityEngine.Debug;
#endif

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace FoliageTool.Core
{
    [RequireComponent(typeof(Terrain))]
    [ExecuteAlways]
    public class FoliageTerrain : MonoBehaviour
    {
        [HideInInspector]
        public Terrain terrain;
        private TerrainData Data => terrain.terrainData;
        public BiomeAsset biome;

        TerrainLayer[] _layers;

        public TerrainLayer[] Layers
        {
            get
            {
                if (_layers == null || _layers.Length != Data.alphamapTextureCount)
                    _layers = Data.terrainLayers;

                return _layers;
            }
        }

        [Header("Brushes")]
        public bool evaluateBrushes = true;
        public bool evaluateBrushFalloff = true;

        [Header("Trees")]
        public bool evaluateTrees = true;
        [MinMax(0, 1)]
        public Vector2 treeBlendRange = new(0.25f, 0.5f);
        [Range(0, 16)]
        public int treePadding = 4;

        [Header("Advanced")]
        public RefreshOptions refreshOptions = new RefreshOptions();

        #region MonoBehaviour Lifecycle

        private void OnEnable()
        {
            OnValidate();
            
#if UNITY_EDITOR
            TerrainCallbacks.textureChanged += OnTextureChanged;
            TerrainCallbacks.heightmapChanged += OnHeightChanged;

            // pre-cache the bounds
            _bounds = GetBounds();
#endif
        }

        private void OnDisable()
        {
            OnValidate();
            
#if UNITY_EDITOR
            TerrainCallbacks.textureChanged -= OnTextureChanged;
            TerrainCallbacks.heightmapChanged -= OnHeightChanged;
#endif
        }

        private void OnValidate()
        {
            if (!terrain)
                terrain = GetComponent<Terrain>();
        }

        #endregion
        
        /// <summary>
        /// Cached bounds object.
        /// </summary>
        private Bounds _bounds;
        
        /// <summary>
        /// Get the world-mapped coordinates of the Terrain's bounds.
        /// </summary>
        public Bounds GetBounds()
        {
            Bounds b = Data.bounds;
            return new Bounds(b.center + terrain.transform.position, b.size);
        }

        /// <summary>
        /// Does a bounding box intersect with this terrain?
        /// </summary>
        public bool Intersects(Bounds bounds)
        {
            return _bounds.Intersects(bounds);
        }

        private void OnHeightChanged(Terrain t, RectInt rect, bool isSync)
        {
#if UNITY_EDITOR
            if(!BrushEditing.CanRefresh())
                return;
#endif
            
            if (!isSync) return;
            if (t != terrain) return;
            
            if (!refreshOptions.onHeightmapChanged)
                return;

            Rect normalized = rect.Normalize(Data.heightmapResolution);
            TerrainRegion region = new TerrainRegion(terrain, normalized);

            // if region is too big, don't refresh
            // reduces the calls to smaller chunks.
            float regionSize = rect.size.magnitude;
            if (regionSize >= refreshOptions.maxChunkResolution / 4f)
                return;

            Sync(out DetailPrototype[] detailPrototypes);
            Refresh(region, detailPrototypes);
        }

        private void OnTextureChanged(Terrain t, string textureName, RectInt rect, bool isSync)
        {
#if UNITY_EDITOR
            if(!BrushEditing.CanRefresh())
                return;
#endif
            
            if (!isSync) return;
            if (t != terrain) return;
            
            if (!refreshOptions.onAlphamapChanged)
                return;

            Rect normalized = rect.Normalize(Data.alphamapResolution);
            TerrainRegion region = new TerrainRegion(terrain, normalized);

            Sync(out DetailPrototype[] detailPrototypes);
            Refresh(region, detailPrototypes);
        }

        public BiomeAsset[] GetBiomes()
        {
            IEnumerable<BiomeBrush> splines = Brush.GetBrushes<BiomeBrush>(terrain);

            List<BiomeAsset> biomes = new List<BiomeAsset>();
            
            if (biome)
                biomes.Add(biome);    
            
            biomes.AddRange(BiomeBrush.GetBiomes(splines));

            return biomes.ToArray();
        }

        public void Sync(out DetailPrototype[] detailPrototypes)
        {
            List<DetailPrototype> prototypes = new List<DetailPrototype>();
            foreach (BiomeAsset biomeAsset in GetBiomes())
            {
                if (!biomeAsset) continue;
                foreach (var prototype in biomeAsset.GetPrototypes())
                {
                    // Don't add duplicates.
                    if (prototypes.Contains(prototype))
                        continue;

                    prototypes.Add(prototype);
                }
            }
            
            for (int i = Data.detailPrototypes.Length - 1; i >= 0; i--)
            {
                if(prototypes.Contains(Data.detailPrototypes[i])) 
                    continue;
                    
                Data.RemoveDetailPrototype(i);
            }

            Data.detailPrototypes = prototypes.ToArray();
            Data.RefreshPrototypes();

            detailPrototypes = prototypes.ToArray();
        }

        public float[][,] GetBiomeMasks(BiomeBrush[] brushes, TerrainRegion region)
        {
            if (!evaluateBrushes)
                return null;

            float[][,] brushMasks = new float[brushes.Length][,];
            for (int i = 0; i < brushMasks.Length; ++i)
            {
                brushMasks[i] = brushes[i].GetMask(region, evaluateBrushFalloff);
            }

            return brushMasks;
        }

        public float[,] BuildTreeMap(TerrainRegion region)
        {
            Rect normalizedRegion = region.Region;
            RectInt detailRegion = region.DetailRegion;

            float[,] treeMap = new float[detailRegion.width, detailRegion.height];
            if (!evaluateTrees)
                return treeMap;

            List<TreeInstance> trees = new List<TreeInstance>();

            int treeCount = Data.treeInstanceCount;
            for (int i = 0; i < treeCount; i++)
            {
                TreeInstance treeInstance = Data.GetTreeInstance(i);
                Vector3 pos = treeInstance.position;
                if (!normalizedRegion.Contains(new Vector2(pos.x, pos.z)))
                    continue;

                trees.Add(treeInstance);
            }

            if (trees.Count == 0)
                return treeMap;

            Bounds bounds = new Bounds();
            foreach (var tree in trees)
            {
                Vector3 nPos = new Vector3(tree.position.z, tree.position.y, tree.position.x);
                Vector3 wPos = terrain.GetWorldPosition(nPos);
                Vector3 scale = new Vector3(2, 2, 2) * treePadding;
                Bounds treeBounds = new Bounds(wPos, scale);

                if (bounds.size.magnitude <= 0)
                    bounds = treeBounds;

                bounds.Encapsulate(treeBounds);
            }

            // iterate through all the pixels in the bounds
            for (int y = 0; y < detailRegion.height; ++y)
            {
                for (int x = 0; x < detailRegion.width; ++x)
                {
                    TerrainPosition positions = new TerrainPosition(terrain, region, x, y);
                    Vector3 worldPos = terrain.GetWorldPosition(positions.TerrainPosition2D);

                    // OPTIMIZATION: only evaluate trees within the pre-calculated bounds
                    // Still needs further optimization for terrains with trees covering the whole terrain
                    if (!bounds.Contains(worldPos))
                        continue;

                    foreach (TreeInstance tree in trees)
                    {
                        Vector3 pos = tree.position;
                        Vector3Int treePos = Vector3Int.FloorToInt(terrain.GetWorldPosition(pos));

                        int treeSize = Mathf.CeilToInt(tree.widthScale * treePadding);

                        // distance function to fill a circle
                        float dist = treeSize -
                                     math.distance(new float2(treePos.z, treePos.x), positions.WorldPosition2D);
                        float step = math.smoothstep(treeBlendRange.x, treeBlendRange.y,
                            math.saturate(dist / treeSize));
                        treeMap[x, y] = math.lerp(treeMap[x, y], 1, math.saturate(step));
                    }
                }
            }

            return treeMap;
        }
        
        public static TerrainRegion[,] SplitToRegions(TerrainRegion region, int maxResolution)
        {
            RectInt totalRegion = region.DetailRegion;
            int widthDivision = math.clamp(totalRegion.width / maxResolution, 1, 16);
            int heightDivision = math.clamp(totalRegion.height / maxResolution, 1, 16);
            
            TerrainRegion[,] regions = new TerrainRegion[widthDivision, heightDivision];

            for (int x = 0; x < widthDivision; x++)
            {
                for (int y = 0; y < heightDivision; y++)
                {
                    Rect rect = new Rect(
                        (x+region.Region.x) / widthDivision,
                        (y+region.Region.y) / heightDivision,
                        region.Region.width / widthDivision,
                        region.Region.height / heightDivision);
                    
                    regions[x, y] = new TerrainRegion(region.Terrain, rect);
                }
            }

            return regions;
        }

        public static IEnumerator Refresh(FoliageTerrain component, TerrainRegion region)
        {
            component.Sync(out DetailPrototype[] detailPrototypes);
            int maxRes = component.refreshOptions.maxChunkResolution;
            
            TerrainRegion[,] regions = SplitToRegions(region, maxRes);
            
            int xLength = regions.GetLength(0);
            int yLength = regions.GetLength(1);
            
            int n = 0;
            int count = xLength * yLength;
            
            for (int x = 0; x < xLength; x++)
            {
                for (int y = 0; y < yLength; y++)
                {
                    n++;
                    TerrainRegion chunk = regions[x, y];
                    
#if UNITY_EDITOR
                    float progress = (n) / (float)count;
                    EditorUtility.DisplayProgressBar($"Refreshing foliage on \"{component.name}\": {n}/{count}",
                        $"Processing region: ({x}, {y}) {chunk.Region}", progress);
#endif
                    
                    component.Refresh(chunk, detailPrototypes);
                }
            }
#if UNITY_EDITOR
            EditorUtility.ClearProgressBar();
#endif
            yield return null;
        }
        
        public void Refresh(TerrainRegion region, DetailPrototype[] detailPrototypes)
        {
            // Flip the region coordinates XY -> YX
            region.FlipXY();

            RectInt alphaRegion = region.AlphaRegion;
            RectInt detailRegion = region.DetailRegion;

            // get all maps
            float[,,] alphaMaps = Data.GetAlphamaps(alphaRegion.position, alphaRegion.size);

            int[][,] detailMaps = new int[detailPrototypes.Length][,];
            for (int i = 0; i < detailMaps.Length; ++i)
                detailMaps[i] = new int[detailRegion.width, detailRegion.height];

#if FOLIAGE_DEBUG
        long baseRefreshTimeMs = 0;
        long treeEvalTime = 0;
        long treeMapTime = 0;
        long biomeMasksMs = 0;
        long biomeEvalsMs = 0;

        Stopwatch timer = new Stopwatch();
        timer.Reset();
        long treeMapTime = 0;
        timer.Start();
#endif

            float[,] treeMap = BuildTreeMap(region);

#if FOLIAGE_DEBUG
        timer.Stop();
        treeMapTime = timer.ElapsedMilliseconds;
        timer.Reset();
        
        timer.Start();

#endif
            // Loop through all the pixels
            for (int y = 0; y < detailRegion.height; y++)
            {
                for (int x = 0; x < detailRegion.width; x++)
                {
                    // For each pixel
                    // get position data
                    TerrainPosition position = new TerrainPosition(terrain, region, x, y);

                    // Loop through all the detail maps
                    for (int i = 0; i < detailMaps.Length; ++i)
                    {
                        DetailPrototype prototype = detailPrototypes[i];
                        Foliage foliage = biome.GetFoliage(prototype);
                        if (foliage == null)
                            continue;

                        if (foliage.disable)
                            continue;

                        // Set the density to the result of the evaluation
                        detailMaps[i][x, y] = EvaluateFoliage(foliage, i, position, alphaMaps);
                    }
                }
            }

#if FOLIAGE_DEBUG
        timer.Stop();
        baseRefreshTimeMs = timer.ElapsedMilliseconds;
        timer.Reset();
#endif

            if (evaluateBrushes)
            {
                BiomeBrush[] biomeBrushes = Brush.GetBrushes<BiomeBrush>(terrain).ToArray();

#if FOLIAGE_DEBUG
            timer.Start();
#endif

                float[][,] biomeMasks = GetBiomeMasks(biomeBrushes, region);

#if FOLIAGE_DEBUG
            timer.Stop();
            biomeMasksMs = timer.ElapsedMilliseconds;
            timer.Reset();
            timer.Start();
#endif

                // Loop through all the pixels
                for (int y = 0; y < detailRegion.height; y++)
                {
                    for (int x = 0; x < detailRegion.width; x++)
                    {
                        // For each pixel, get position data
                        TerrainPosition position = new TerrainPosition(terrain, region, x, y);
                        EvaluateBiomes(position, biomeBrushes, biomeMasks, detailPrototypes, detailMaps, alphaMaps);
                    }
                }

#if FOLIAGE_DEBUG
            timer.Stop();
            biomeEvalsMs = timer.ElapsedMilliseconds;
            timer.Reset();
#endif
            }

            if (evaluateTrees)
            {
#if FOLIAGE_DEBUG
            timer.Start();
#endif
                for (int y = 0; y < detailRegion.height; y++)
                {
                    for (int x = 0; x < detailRegion.width; x++)
                    {
                        // Remove details that intersect with trees
                        for (int i = 0; i < detailMaps.Length; ++i)
                        {
                            // lerps between the current detail density and 0, with the tree map influence
                            float density = math.lerp(detailMaps[i][x, y], 0, treeMap[x, y]);
                            detailMaps[i][x, y] = Mathf.FloorToInt(density);
                        }
                    }
                }
#if FOLIAGE_DEBUG
            timer.Stop();
            treeEvalTime = timer.ElapsedMilliseconds;
            timer.Reset();
#endif
            }

            // Flip the region coordinates YX -> XY
            region.FlipXY();

            // Finally, set the detail layers within this region
            for (int i = 0; i < detailMaps.Length; ++i)
                Data.SetDetailLayer(region.DetailRegion.position, i, detailMaps[i]);

#if FOLIAGE_DEBUG
        long totalMs = baseRefreshTimeMs + biomeMasksMs + biomeEvalsMs + treeMapTime + treeEvalTime;
        Debug.Log(
            $"Base refresh time: {baseRefreshTimeMs} ms\n\n"+
            "BIOMES...\n"+ 
            $"mask evaluation time: {biomeMasksMs} ms\n"+
            $"biome evaluation time: {biomeEvalsMs} ms\n\n"+ 
            "TREES...\n"+ 
            $"mask evaluation time: {treeMapTime} ms\n"+
            $"tree evaluation time: {treeEvalTime} ms\n\n"+
            $"Region total time: {totalMs} ms");
#endif
        }

        void EvaluateBiomes(TerrainPosition position, BiomeBrush[] brushes,
            float[][,] brushMasks, DetailPrototype[] prototypes, int[][,] detailMaps, float[,,] alphaMaps)
        {
            int x = position.DetailPosition.x;
            int y = position.DetailPosition.y;

            for (int j = 0; j < brushMasks.Length; ++j)
            {
                BiomeBrush brush = brushes[j];
                float[,] brushMask = brushMasks[j];

                // if brush influence is 0, continue onto the next spline
                if (brushMask[x, y] <= 0)
                    continue;

                // for each detail map
                for (int i = 0; i < detailMaps.Length; ++i)
                {
                    float density = 0;

                    // Evaluate the current density within this spline
                    DetailPrototype prototype = prototypes[i];
                    if (brush.biome)
                    {
                        Foliage foliage = brush.biome.GetFoliage(prototype);
                        if (foliage != null && !foliage.disable)
                        {
                            density = EvaluateFoliage(foliage, i, position, alphaMaps);
                        }
                    }

                    switch (brush.blendMode)
                    {
                        case Brush.BlendMode.Blend:
                            // lerp between the current detail density and the spline's density,
                            // with the spline mask influence
                            density = Mathf.Lerp(detailMaps[i][x, y], density, brushMask[x, y]);
                            break;
                        case Brush.BlendMode.Add:
                            density = detailMaps[i][x, y] + (density * brushMask[x, y]);
                            break;
                        case Brush.BlendMode.Subtract:
                            float d = detailMaps[i][x, y] - (density * brushMask[x, y]);
                            density = Mathf.Clamp(d, 0, detailMaps[i][x, y]);
                            break;
                    }

                    detailMaps[i][x, y] = Mathf.FloorToInt(density);
                }
            }
        }

        public void Clear(TerrainRegion region)
        {
            RectInt detailRegion = region.DetailRegion;

            Sync(out DetailPrototype[] foliage);

            for (int i = 0; i < foliage.Length; ++i)
                Data.SetDetailLayer(detailRegion.position, i, new int[detailRegion.width, detailRegion.height]);
        }

        public int EvaluateFoliage(Foliage foliage, int detailIndex, TerrainPosition positions, float[,,] alphaMaps)
        {
            Vector2 terrainPos = positions.TerrainPosition2D;
            Vector2Int alphaPosition = positions.AlphaPosition;

            float initialDensity = foliage.density * Data.detailResolutionPerPatch;
            if (Data.detailScatterMode == DetailScatterMode.CoverageMode)
                initialDensity *= Data.ComputeDetailCoverage(detailIndex);

            TerrainLayer[] layers = Layers;
            SpawnRules spawnRules = foliage.spawnRules;
            TextureRule[] textureRules = spawnRules.GetTextureRules();
            PerlinNoise perlin = spawnRules.perlinNoise;

            float density = initialDensity;

            // Evaluate foliage's texture rules at this terrain position
            density = EvaluateTextureRules(textureRules, alphaPosition, density, layers, alphaMaps);

            // Evaluate foliage's perlin noise at this terrain position
            float perlinValue = perlin.Evaluate(positions.WorldPosition2D);
            density = Mathf.Lerp(density, density * perlinValue, perlin.alpha);

            // Evaluate steepness / height rules
            float steepness = Data.GetSteepness(terrainPos.y, terrainPos.x);
            if (steepness < spawnRules.steepnessLimit.x || steepness > spawnRules.steepnessLimit.y)
                density = 0.0f;

            float height = Data.GetInterpolatedHeight(terrainPos.y, terrainPos.x);
            if (height < spawnRules.heightLimit.x || height > spawnRules.heightLimit.y)
                density = 0.0f;

            return Mathf.FloorToInt(density);
        }

        float EvaluateTextureRules(TextureRule[] rules, Vector2Int pos, float density, TerrainLayer[] layers,
            float[,,] alphas)
        {
            if (rules.Length == 0)
                return density;

            // Check if pos.x and pos.y are within the valid range
            Vector2Int len = new Vector2Int(alphas.GetLength(0), alphas.GetLength(1));
            if (pos.x < 0 || pos.x >= len.x || pos.y < 0 || pos.y >= len.y)
                pos = pos.Clamp(0, 0, len.x - 1, len.y - 1);

            float d = 0;
            foreach (var r in rules)
            {
                if (!r.layer)
                    continue;

                int splatIndex = layers.IndexOf(r.layer);
                if (splatIndex < 0)
                    continue;

                float alpha = alphas[pos.x, pos.y, splatIndex];
                if (alpha < r.threshold)
                    continue;

                switch (r.rule)
                {
                    case TextureRule.Rule.Include:
                        d += alpha * density;
                        break;
                    case TextureRule.Rule.Exclude:
                        d -= alpha * density;
                        break;
                }
            }

            // Finally, clamp the density and return it.
            return math.clamp(d, 0, density);
        }
    }
}