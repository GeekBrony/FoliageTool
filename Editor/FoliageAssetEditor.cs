using System;
using System.Linq;
using UnityEditor;
using UnityEditor.TerrainTools;
using UnityEngine;
using UnityEngine.UIElements;

namespace Flora.Core
{
    [CustomEditor(typeof(FoliageAsset))]
    public class FoliageAssetEditor : Editor
    {
        private FoliageAsset _foliage;

        private void OnEnable()
        {
            _foliage = target as FoliageAsset;
            Refresh();
        }
        
        private DetailPrototype[] _prototypes;
        private GUIContent[] _detailIcons;

        void Refresh()
        {
            if (!_foliage)
            {
                return;
            }

            _prototypes = new DetailPrototype[1];
            
            if (_foliage == null)
            {
                _prototypes[0] = new DetailPrototype();
            }
            else
            {
                _prototypes[0] = _foliage.Prototype;
            }

            _detailIcons = PaintDetailsToolUtility.LoadDetailIcons(_prototypes);
        }
        
        

        public override void OnInspectorGUI()
        {
            EditorGUILayout.BeginVertical(EditorStyles.helpBox);
            EditorGUILayout.BeginHorizontal();
            DrawDetailPreview(_prototypes[0]);

            GUILayout.FlexibleSpace();
            GUILayout.Space(10);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.EndVertical();
            
            serializedObject.Update();

            base.OnInspectorGUI();

            if (serializedObject.ApplyModifiedProperties())
            {
                Refresh();
                
                if (!EditorApplication.isPlaying)
                {
                    //_biome.OnEdit();
                }
            }
        }

        public readonly Color prototypeColor = new Color(0.82745f, 1.07450f, 1.23333f);
        readonly int m_DistributionPrototypeCardId = "TerrainDetailCardIDHash".GetHashCode();
        const int k_CardSize = 48;
        const int k_CardPreviewOffset = 5;

        void DrawDetailPreview(DetailPrototype detail)
        {
            Color tempColor = GUI.backgroundColor;
            GUI.backgroundColor = Color.black;
            EditorGUILayout.BeginHorizontal();
            GUI.backgroundColor = tempColor;

            //bool prevSelectedValue = m_DetailDataList[index].isSelected;
            Rect cardRect = GUILayoutUtility.GetRect(k_CardSize, k_CardSize);
            Rect previewRect = new Rect(
                cardRect.x + k_CardPreviewOffset,
                cardRect.y + k_CardPreviewOffset,
                k_CardSize - (k_CardPreviewOffset * 2),
                k_CardSize - (k_CardPreviewOffset * 2));
            Rect prototypeCheckBoxRect = new Rect(previewRect.x + 2, previewRect.y + 9, 0, 0);
            
            GUI.backgroundColor = tempColor; //new Color(0.479f, 0.708f, 0.983f, 1.0f)
            //GUI.Box(cardRect, "", GUI.skin.button);
            //GUI.backgroundColor = tempColor;
            if (_detailIcons[0]?.image != null)
            {
                EditorGUI.DrawPreviewTexture(previewRect, _detailIcons[0].image);
            }
            else
            {
                EditorGUI.DrawTextureAlpha(previewRect,
                    EditorGUIUtility.IconContent("SceneAsset Icon").image);
                Refresh();
            }
            
            //GUILayout.FlexibleSpace();
            

            EditorGUILayout.BeginVertical();
            
            if (_foliage)
            {
                //GameObject prototype = asset.prefab;
                EditorGUILayout.LabelField(_foliage.name);
            }
            else
            {
                EditorGUILayout.LabelField("Foliage Asset is null.", EditorStyles.boldLabel);
            }

            EditorGUILayout.EndVertical();
            EditorGUILayout.EndHorizontal();
        }
    }
}