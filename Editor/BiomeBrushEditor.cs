using System;
using System.Collections.Generic;
using System.Linq;
using Flora.Core.Brushes;
using UnityEditor;
using UnityEngine;

namespace Flora.Core
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
        
        /*void OnGUI () {
            DragAndDrop.visualMode = DragAndDropVisualMode.Generic;
            if (Event.current.type == EventType.DragExited) {
                _dragBrushes = new BiomeBrush[DragAndDrop.objectReferences.Length];
                for (int i = 0; i < DragAndDrop.objectReferences.Length; i++) {
                    _dragBrushes[i] = DragAndDrop.objectReferences[i] as BiomeBrush;
                }
                foreach (var b in _dragBrushes)
                {
                    if(!_isSpawned) b.Refresh(b.GetBounds());
                }
                
            }
        }*/
        private BiomeBrush[] _brushes;
        public override void OnInspectorGUI()
        {
            bool modified = DrawDefaultInspector();

            if (EditorApplication.isPlaying)
                return;

            _brushes = GetTargets();
            
            if (modified)
            {
                foreach (var brush in _brushes)
                    brush.ScheduleRefresh();
            }

            if (GUILayout.Button("Refresh"))
            {
                foreach (var brush in _brushes)
                {
                    brush.Refresh(brush.GetBounds());
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
                _lastBounds = GetBounds();
                
                _mouseWasDown = true;
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

                _mouseWasDown = false;
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