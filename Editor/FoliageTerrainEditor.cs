using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace FoliageTool.Core
{
    [CustomEditor(typeof(FoliageTerrain)), CanEditMultipleObjects]
    public class FoliageTerrainEditor : Editor
    {
        private FoliageTerrain _tComponent;
        private void OnEnable()
        {
            _tComponent = target as FoliageTerrain;
            
            List<FoliageTerrain> terrains = GetSelectedFloraTerrains();
            RefreshStats(terrains.ToArray());
        }

        private static readonly string[] _dontIncludeMe = new string[]{"m_Script"};

        private readonly GUILayoutOption[] _buttonLayout = new[]
        {
            GUILayout.Height(22)
        };

        List<BiomeBrush> brushes = new List<BiomeBrush>();
        List<BiomeAsset> biomes = new List<BiomeAsset>();
        List<DetailPrototype> detailPrototypes = new List<DetailPrototype>();
        List<TreePrototype> treePrototypes = new List<TreePrototype>();
        int treeInstances = 0;

        void RefreshStats(params FoliageTerrain[] terrains)
        {
            brushes.Clear();
            biomes.Clear();
            detailPrototypes.Clear();
            treePrototypes.Clear();
            treeInstances = 0;
                
            foreach (FoliageTerrain f in terrains)
            {
                Terrain terrain = f.terrain;
                TerrainData data = terrain.terrainData;
                    
                foreach (var b in BiomeBrush.GetSplines(terrain, true).ToArray())
                {
                    if (brushes.Contains(b))
                        continue;

                    brushes.Add(b);
                }
                    
                foreach (var b in f.GetBiomes())
                {
                    if (biomes.Contains(b))
                        continue;

                    biomes.Add(b);
                }

                foreach (var prototype in data.detailPrototypes)
                {
                    if(detailPrototypes.Contains(prototype))
                        continue;
                        
                    detailPrototypes.Add(prototype);
                }
                    
                foreach (var prototype in data.treePrototypes)
                {
                    if(treePrototypes.Contains(prototype))
                        continue;
                        
                    treePrototypes.Add(prototype);
                }

                treeInstances += data.treeInstanceCount;
            }
        }
        
        private static bool _showStatistics = false;
        public override void OnInspectorGUI()
        {
            List<FoliageTerrain> terrains = GetSelectedFloraTerrains();
            
            EditorGUILayout.Space(2);
            EditorGUILayout.BeginHorizontal();
            
            if (GUILayout.Button("Clear", _buttonLayout))
            {
                bool action = EditorUtility.DisplayDialog("Destructive action",
                    "Are you sure you want to remove all foliage on this terrain?", "Yes", "No");
                if (action)
                {
                    foreach (FoliageTerrain t in terrains)
                    {
                        if(!t) continue;
                        
                        TerrainRegion region = new TerrainRegion(t.terrain, new Rect(0, 0, 1, 1));
                        t.Clear(region);
                    }
                    
                    RefreshStats(terrains.ToArray());
                }
            }
            GUILayout.FlexibleSpace();

            if (GUILayout.Button("Sync", _buttonLayout))
            {
                foreach (FoliageTerrain t in terrains)
                {
                    if(!t) continue;
                    
                    t.Sync(out DetailPrototype[] prototypes);
                }
                
                RefreshStats(terrains.ToArray());
            }

            if (GUILayout.Button("Refresh", _buttonLayout))
            {
                EditorCoroutineUtility.StartCoroutine(Refresh(terrains.ToArray()), this);
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space(5);
            
            DrawPropertiesExcluding(serializedObject, _dontIncludeMe);

            if (serializedObject.ApplyModifiedProperties())
            {
                RefreshStats(terrains.ToArray());
            }

            EditorGUILayout.Space(5);

            _showStatistics = EditorGUILayout.BeginFoldoutHeaderGroup(_showStatistics, "Statistics");
            if (_showStatistics)
            {
                string splineCountString = $"Splines: {brushes.Count}";
                string biomeCountString = $"Biomes: {biomes.Count}";
                string detailCountString = $"Detail Prototypes: {detailPrototypes.Count}";
                string treePrototypesString = $"Tree Prototypes: {treePrototypes.Count}";
                string treeCountString = $"Tree Instances: {treeInstances}";
                
                GUILayout.Label(splineCountString);
                GUILayout.Label(biomeCountString);
                GUILayout.Label(detailCountString);
                GUILayout.Label(treePrototypesString);
                GUILayout.Label(treeCountString);
            }
            EditorGUILayout.EndFoldoutHeaderGroup();
        }

        IEnumerator Refresh(params FoliageTerrain[] terrains)
        {
            foreach (FoliageTerrain t in terrains)
            {
                if(!t)
                    continue;

                Rect fullRect = new Rect(0, 0, 1, 1);
                TerrainRegion region = new TerrainRegion(t.terrain, fullRect);
                
                yield return EditorCoroutineUtility.StartCoroutine(
                    FoliageTerrain.Refresh(t, region), this);
            }
        }

        List<FoliageTerrain> GetSelectedFloraTerrains()
        {
            List<FoliageTerrain> list = new List<FoliageTerrain>();
            foreach (Object o in targets)
            {
                if(!o) continue;
                    
                FoliageTerrain t = o as FoliageTerrain;
                if (t)
                {
                    list.Add(t);
                } 
            }

            return list;
        }
    }
}