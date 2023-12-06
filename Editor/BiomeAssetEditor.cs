using System;
using System.Linq;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;

namespace FoliageTool.Core
{
    [CustomEditor(typeof(BiomeAsset))]
    public class BiomeAssetEditor : Editor
    {
        private BiomeAsset _biome;

        private void OnEnable()
        {
            _biome = target as BiomeAsset;
            Refresh();
        }

        private Foliage[] _details;
        private DetailPrototype[] _prototypes;
        private GUIContent[] _detailIcons;

        void Refresh()
        {
            if (!_biome)
            {
                return;
            }

            _details = _biome.foliage.ToArray();

            _prototypes = new DetailPrototype[_details.Length];
            for (int i = 0; i < _prototypes.Length; ++i)
            {
                var detail = _details[i];
                if (detail.asset == null)
                {
                    _prototypes[i] = new DetailPrototype();
                    continue;
                }

                _prototypes[i] = detail.asset.Prototype;
            }

            _detailIcons = PaintDetailsToolUtility.LoadDetailIcons(_prototypes);
        }
        
        

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            for (int i = 0; i < _prototypes.Length; i++)
            {
                int moveDir = DrawDetailPreview(_prototypes[i], i);
                
                if (moveDir != 0)
                {
                    var detail = _biome.foliage[i];
                    if (moveDir > 0)
                    {
                        if (i - 1 >= 0 && i - 1 < _biome.foliage.Count)
                        {
                            _biome.foliage[i] = _biome.foliage[i - 1];
                            _biome.foliage[i - 1] = detail;
                        }
                    }
                    else
                    {
                        if (i + 1 >= 0 && i + 1 < _biome.foliage.Count)
                        {
                            _biome.foliage[i] = _biome.foliage[i + 1];
                            _biome.foliage[i + 1] = detail;
                        }
                    }
                
                    _selectedIndex = i - moveDir;
                    Refresh();
                    return;
                }
            }
            
            EditorGUILayout.BeginHorizontal();
            bool isSelected = _selectedIndex >= 0 && _biome.foliage != null && _biome.foliage.Count > _selectedIndex;
            GUI.enabled = isSelected;
            if (GUILayout.Button("-", GUILayout.Width(20)))
            {
                bool confirm = EditorUtility.DisplayDialog("Confirm delete detail?",
                    "Are you sure you want to delete this detail? This action cannot be undone.", "Yes", "No");
                if (confirm)
                {
                    if (_biome.foliage != null)
                    {
                        _biome.foliage.Remove(_biome.foliage[_selectedIndex]);
                        if(_selectedIndex > _biome.foliage.Count)
                            _selectedIndex = -1;
                    }

                    Refresh();
                }
            }
            GUI.enabled = true;
            if (GUILayout.Button("+", GUILayout.Width(20)))
            {
                if (_biome.foliage != null)
                    _biome.foliage.Add(new Foliage());
            
                Refresh();
            }
            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            
            serializedObject.Update();
            
            isSelected = _selectedIndex >= 0 && _biome.foliage != null && _biome.foliage.Count > _selectedIndex;
            if (isSelected)
            {
                DrawDetailProperties();
            }
            
            //base.OnInspectorGUI();
        }
        
        void DrawDetailProperties()
        {
            SerializedProperty detailsProp = serializedObject.FindProperty("foliage");
            SerializedProperty detailProp = detailsProp.GetArrayElementAtIndex(_selectedIndex);

            if(detailProp == null)
                return;

            SerializedProperty prototypeProp = detailProp.FindPropertyRelative("asset");
            GameObject prototype = prototypeProp.objectReferenceValue as GameObject;
            if (prototype)
            {
                EditorGUILayout.PropertyField(detailProp, new GUIContent(prototype.name), true);
            }
            else
            {
                EditorGUILayout.PropertyField(detailProp, true);
            }

            if (serializedObject.ApplyModifiedProperties())
            {
                Refresh();
                
                if (!EditorApplication.isPlaying)
                {
                    _biome.OnEdit();
                }
            }
        }

        public readonly Color prototypeColor = new Color(0.82745f, 1.07450f, 1.23333f);
        readonly int m_DistributionPrototypeCardId = "TerrainDetailCardIDHash".GetHashCode();
        const int k_CardSize = 48;
        const int k_CardPreviewOffset = 5;

        int DrawDetailPreview(DetailPrototype detail, int index)
        {
            Color tempColor = GUI.backgroundColor;
            bool isSelected = _selectedIndex == index && _biome.foliage != null &&
                              _biome.foliage.Count > _selectedIndex;
            if (!isSelected)
            {
                GUI.backgroundColor = Color.black;
            }

            Rect r = EditorGUILayout.BeginHorizontal(EditorStyles.selectionRect);
            if (isSelected) r.xMax -= 55;

            GUI.backgroundColor = tempColor;

            //bool prevSelectedValue = m_DetailDataList[index].isSelected;
            Rect cardRect = GUILayoutUtility.GetRect(k_CardSize, k_CardSize);
            Rect previewRect = new Rect(
                cardRect.x + k_CardPreviewOffset,
                cardRect.y + k_CardPreviewOffset,
                k_CardSize - (k_CardPreviewOffset * 2),
                k_CardSize - (k_CardPreviewOffset * 2));
            
            /*Rect prototypeCheckBoxRect = new Rect(previewRect.x + 2, previewRect.y + 9, 0, 0);*/
            
            GUI.backgroundColor = IsSelected(index) ? prototypeColor : tempColor;
            //new Color(0.479f, 0.708f, 0.983f, 1.0f)
            
            GUI.Box(cardRect, "", GUI.skin.button);
            GUI.backgroundColor = tempColor;
            if (_detailIcons[index]?.image != null)
            {
                EditorGUI.DrawPreviewTexture(previewRect, _detailIcons[index].image);
            }
            else
            {
                EditorGUI.DrawTextureAlpha(previewRect,
                    EditorGUIUtility.IconContent("SceneAsset Icon").image);
                Refresh();
            }

            Event currentEvent = Event.current;
            MouseClickOperations(currentEvent, r, r, _prototypes, _detailIcons, index);
            //GUILayout.FlexibleSpace();

            EditorGUILayout.BeginVertical();

            var asset = _details[index].asset;
            if (asset)
            {
                //GameObject prototype = asset.prefab;
                EditorGUILayout.LabelField(asset.name);
            }
            else
            {
                EditorGUILayout.LabelField("Foliage Asset is null.", EditorStyles.boldLabel);
            }

            EditorGUILayout.EndVertical();
            GUILayout.FlexibleSpace();
            
            int moveDir = 0;
            if (isSelected && _biome.foliage.Count > 1)
            {
                EditorGUILayout.BeginVertical();
                
                if (GUILayout.Button("Up", GUILayout.Width(50), GUILayout.Height(20)))
                {
                    moveDir++;
                }
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("Down", GUILayout.Width(50), GUILayout.Height(20)))
                {
                    moveDir--;
                }
    
                EditorGUILayout.EndVertical();
            }

            EditorGUILayout.EndHorizontal();

            return moveDir;
        }
        
        private int _selectedIndex = -1;
        bool IsSelected(int index)
        {
            return _selectedIndex == index;
        }
        
        private static Foliage _clipboard;
        private void CopyToClipboard(object userData)
        {
            int index = (int) userData;
        
            _clipboard = _biome.foliage[index].Clone();
        }
    
        private void Duplicate(object userData)
        {
            int index = (int) userData;
        
            var detail = _biome.foliage[index].Clone();
            _biome.foliage.Add(detail);
        
            Refresh();
        }
    
        private void PasteFromClipboard(object userData)
        {
            int index = (int) userData;
            _biome.foliage[index] = _clipboard.Clone();
        
            Refresh();
        }
    
        void MouseClickOperations(Event currentEvent, Rect leftClickAreaRect, Rect rightClickAreaRect, DetailPrototype[] prototypes, GUIContent[] detailIcons, int index)
        {
            EventType eventType = currentEvent.GetTypeForControl(GUIUtility.GetControlID(m_DistributionPrototypeCardId, FocusType.Passive));
            if (eventType == EventType.MouseDown)
            {
                if (currentEvent.button == 0 && leftClickAreaRect.Contains(currentEvent.mousePosition)) //Left click operation
                {
                    _selectedIndex = !IsSelected(index) ? index : -1;
                    currentEvent.Use();
                }
                
                if (currentEvent.button == 1 && rightClickAreaRect.Contains(currentEvent.mousePosition)) //Right click operation
                {
                    GenericMenu menu = new GenericMenu();
                
                    menu.AddItem(new GUIContent("Copy"), false, CopyToClipboard, index);
                
                    if(_clipboard == null)
                        menu.AddDisabledItem(new GUIContent("Paste"));
                    else
                        menu.AddItem(new GUIContent("Paste"), false, PasteFromClipboard, index);
                
                    menu.AddSeparator("");
                
                    menu.AddItem(new GUIContent("Duplicate"), false, Duplicate, index);
                
                    menu.ShowAsContext();
                    //DisplayPrototypeEditMenu(terrainData, prototypes, index);
                }
            }
        }
    }
}