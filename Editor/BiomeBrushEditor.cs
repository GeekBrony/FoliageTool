using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace FoliageTool.Core
{
    [CustomEditor(typeof(BiomeBrush)), CanEditMultipleObjects]
    public class BiomeBrushEditor : Editor
    {
        private BiomeBrush _biome;
        private BiomeBrush[] _dragBrushes;
        private void OnEnable()
        {
            _biome = target as BiomeBrush;
            if (!_biome)
                return;
            
            _lastBounds = _biome.GetBounds();
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
                foreach (var brush in _brushes)
                    brush.Refresh(brush.GetBounds());
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
                        foreach (var brush in _brushes)
                        {
                            brush.Refresh(_lastBounds);
                        }
                    }
                    
                    foreach (var brush in _brushes)
                    {
                        brush.Refresh(bounds);
                    }
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