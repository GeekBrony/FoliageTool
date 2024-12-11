using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.EditorCoroutines.Editor;
using UnityEditor;
using UnityEngine;

namespace FoliageTool.Core
{
    [CustomEditor(typeof(Brush), true), CanEditMultipleObjects]
    public class BrushEditor : Editor
    {
        private Brush _brush;
        private Brush[] _dragBrushes;
        private FoliageTerrain[] _terrains;
        
        private void OnEnable()
        {
            _brush = target as Brush;
            if (!_brush)
                return;
            
            _lastBounds = _brush.GetBounds();
            _terrains = FindObjectsOfType<FoliageTerrain>();
        }
        
        private Brush[] _brushes;
        
        private static readonly string[] _dontIncludeMe = new string[]{"m_Script"};
        private bool IsPlaying => EditorApplication.isPlaying;
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            _brushes = GetTargets();

            if (!IsPlaying)
            {
                if (GUILayout.Button("Refresh"))
                {
                    isManualRefresh = true;
                    EditorCoroutineUtility.StartCoroutine(Refresh(_brushes, _terrains), this);
                }
            }
            
            EditorGUI.BeginChangeCheck();
            DrawPropertiesExcluding(serializedObject, _dontIncludeMe);
            serializedObject.ApplyModifiedProperties();
            
            if (EditorGUI.EndChangeCheck())
            {
                if (IsPlaying)
                    return;

                foreach (var brush in _brushes)
                {
                    brush.Validate();
                    brush.ScheduleRefresh();
                }
            }

        }
        
        IEnumerator Refresh(Bounds bounds, params FoliageTerrain[] terrains)
        {
            if (!BrushEditing.CanRefresh() && !isManualRefresh)
                yield break;
            
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
            
            if(isManualRefresh)
                isManualRefresh = false;
        }

        private bool isManualRefresh = false;
        
        IEnumerator Refresh(Brush[] brushes, params FoliageTerrain[] terrains)
        {
            if (!BrushEditing.CanRefresh() && !isManualRefresh)
                yield break;
            
            foreach (FoliageTerrain t in terrains)
            {
                if(!t) continue;

                foreach (var brush in brushes)
                {
                    brush.Validate();
                    
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
            
            if(isManualRefresh)
                isManualRefresh = false;
        }

        private bool _mouseWasDown = false;
        private Bounds _lastBounds;
        private void OnSceneGUI()
        {
            if (EditorApplication.isPlaying)
                return;
            
            if (!BrushEditing.CanRefresh())
                return;
            
            Event currentEvent = Event.current;
            bool isMouseDown = currentEvent.type == EventType.MouseDown && currentEvent.button == 0;
            bool isMouseUp = currentEvent.type == EventType.MouseUp && currentEvent.button == 0;
            bool isMouseLeft = currentEvent.type == EventType.MouseLeaveWindow;

            Bounds bounds = GetBounds();
            if (isMouseDown)
            {
                _mouseWasDown = true;
                _lastBounds = GetBounds();
            }

            if (_mouseWasDown &&
                _brushes.Any(b => b.WillDrawDebugBounds()) &&
                !bounds.Equals(_lastBounds))
            {
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
                
                if (bounds != _lastBounds)
                {
                    foreach (var brush in _brushes)
                    {
                        brush.Validate();
                    }
                    
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


        Brush[] GetTargets()
        {
            List<Brush> allBrushes = new List<Brush>();
            foreach (var o in targets)
            {
                Brush s = o as Brush;
                if(!s)
                    continue;
                    
                allBrushes.Add(s);
            }
            return allBrushes.OrderBy(s=> s.drawOrder).ToArray();
        }

        Bounds GetBounds()
        {
            Bounds b = _brush.GetBounds();
            
            foreach (Brush brush in _brushes)
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