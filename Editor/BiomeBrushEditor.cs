using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace FoliageTool.Core
{
    [CustomEditor(typeof(BiomeBrush)), CanEditMultipleObjects]
    public class BiomeBrushEditor : Editor
    {
        private BiomeBrush _biome;
        private BiomeBrush[] _dragBrushes;
        private FoliageTerrain[] _terrains;
        private void OnEnable()
        {
            _biome = target as BiomeBrush;
            if (!_biome)
                return;
            
            _lastBounds = _biome.GetBounds();
            _terrains = FindObjectsOfType<FoliageTerrain>();
        }
        
        private BiomeBrush[] _brushes;
        
        private static readonly string[] _dontIncludeMe = new string[]{"m_Script"};
        private bool IsPlaying => EditorApplication.isPlaying;
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            _brushes = GetTargets();
            
            if (GUILayout.Button("Refresh") && !IsPlaying)
            {
                EditorCoroutineUtility.StartCoroutine(Refresh(_brushes, _terrains), this);
            }
            
            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, _dontIncludeMe);
            serializedObject.ApplyModifiedProperties();
            
            if (EditorGUI.EndChangeCheck())
            {
                if (IsPlaying)
                    return;
                
                foreach (var brush in _brushes)
                    brush.ScheduleRefresh();
            }

        }
        
        IEnumerator Refresh(Bounds bounds, params FoliageTerrain[] terrains)
        {
            foreach (FoliageTerrain t in terrains)
            {
                if(!t || !t.Intersects(bounds))
                    continue;

                var region = TerrainRegion.FromBounds(t.terrain, bounds, 10);
                    
                if (region.DetailRegion.width <= t.refreshOptions.maxChunkResolution &&
                    region.DetailRegion.height <= t.refreshOptions.maxChunkResolution)
                {
                    t.Sync(out DetailPrototype[] detailPrototypes);
                    t.Refresh(region, detailPrototypes);
                    continue;
                }

                IEnumerator refreshAction = FoliageTerrain.Refresh(t, region);
                yield return EditorCoroutineUtility.StartCoroutine(refreshAction, this);
            }
            
            
        }
        
        IEnumerator Refresh(BiomeBrush[] brushes, params FoliageTerrain[] terrains)
        {
            foreach (FoliageTerrain t in terrains)
            {
                if(!t) continue;

                foreach (var brush in brushes)
                {
                    if(!brush.Intersects(t.terrain))
                        continue;
                    
                    var region = TerrainRegion.FromBounds(t.terrain, brush.GetBounds(), 10);
                    
                    if (region.DetailRegion.width <= t.refreshOptions.maxChunkResolution &&
                        region.DetailRegion.height <= t.refreshOptions.maxChunkResolution)
                    {
                        t.Sync(out DetailPrototype[] detailPrototypes);
                        t.Refresh(region, detailPrototypes);
                        continue;
                    }
                    
                    yield return EditorCoroutineUtility.StartCoroutine(
                        FoliageTerrain.Refresh(t, region), this);
                }
            }
        }

        private bool _mouseWasDown = false;
        
        private Bounds _lastBounds;
        private void OnSceneGUI()
        {
            if (EditorApplication.isPlaying)
                return;
            
            Event currentEvent = Event.current;
            bool isMouseDown = currentEvent.type == EventType.MouseDown && currentEvent.button == 0;
            bool isMouseUp = currentEvent.type == EventType.MouseUp && currentEvent.button == 0;
            bool isMouseLeft = currentEvent.type == EventType.MouseLeaveWindow;

            if (isMouseDown)
            {
                _mouseWasDown = true;
                
                _lastBounds = GetBounds();
            }

            if (_mouseWasDown && _brushes.Any(b => b.drawBounds))
            {
                Bounds bounds = GetBounds();
                if (_lastBounds.Intersects(bounds))
                {
                    bounds.Encapsulate(_lastBounds);
                }
                else
                {
                    Handles.DrawWireCube(_lastBounds.center, _lastBounds.size);
                }

                Handles.DrawWireCube(bounds.center, bounds.size);
            }
            
            if (isMouseUp || (_mouseWasDown && isMouseLeft))
            {
                _mouseWasDown = false;
                
                Bounds bounds = GetBounds();
                if (bounds != _lastBounds)
                {
                    if (_lastBounds.Intersects(bounds))
                    {
                        bounds.Encapsulate(_lastBounds);
                    }
                    else
                    {
                        EditorCoroutineUtility.StartCoroutine(Refresh(_lastBounds, _terrains), this);
                    }
                    
                    EditorCoroutineUtility.StartCoroutine(Refresh(bounds, _terrains), this);
                }

            }

        }


        BiomeBrush[] GetTargets()
        {
            List<BiomeBrush> allSplines = new List<BiomeBrush>();
            foreach (var o in targets)
            {
                BiomeBrush s = o as BiomeBrush;
                if(!s)
                    continue;
                    
                allSplines.Add(s);
            }

            return allSplines.OrderBy(s=> s.drawOrder).ToArray();
        }

        Bounds GetBounds()
        {
            Bounds b = _biome.GetBounds();
            
            foreach (BiomeBrush brush in _brushes)
            {
                if(!brush || !brush.enabled)
                    continue;

                Bounds brushBounds = brush.GetBounds();
                b.Encapsulate(brushBounds);
            }
            
            return b;
        }
    }
}