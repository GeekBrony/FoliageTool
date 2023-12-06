using FoliageTool.Utils;
using UnityEditor;
using UnityEngine;

namespace FoliageTool.Core
{
    [CustomPropertyDrawer(typeof(PerlinNoise))]
    public class PerlinNoiseDrawer : PropertyDrawer
    {
        private Texture2D _perlinTexture;

        const int k_CardSize = 100;
        const int k_CardPreviewOffset = 5;

        private const int NoiseResolution = 100;
        private const int ScaleRes = NoiseResolution / 4;
        
        private bool _expanded = false;
        private bool _advancedExpanded = false;
        private PerlinNoise _lastNoise;

        public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
        {
            if (!_perlinTexture)
            {
                _perlinTexture = new Texture2D(NoiseResolution, NoiseResolution);
                
                PerlinNoise perlin = (PerlinNoise) property.boxedValue;
                RenderPerlin(_perlinTexture, perlin);
                _lastNoise = perlin;
            }

            _expanded = EditorGUILayout.PropertyField(property, label, false);

            if (_expanded)
            {
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(EditorGUI.indentLevel * 15f);
                EditorGUILayout.BeginVertical();
                
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(2);
                
                DrawPerlinRect();

                EditorGUILayout.BeginVertical(EditorStyles.helpBox);
                GUILayout.Space(2);
                
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;
                
                DrawProperty(property.FindPropertyRelative("offset"));
                DrawProperty(property.FindPropertyRelative("scaleX"));
                DrawProperty(property.FindPropertyRelative("scaleY"));
                DrawProperty(property.FindPropertyRelative("remap"));
                DrawProperty(property.FindPropertyRelative("invert"));
                DrawProperty(property.FindPropertyRelative("alpha"));
                
                GUILayout.Space(5);
                EditorGUILayout.BeginHorizontal();
                GUILayout.Space(_expanded ? 13 : 15);
                EditorGUILayout.BeginVertical();
                _advancedExpanded = EditorGUILayout.BeginFoldoutHeaderGroup(_advancedExpanded, "Advanced Settings");
                if (_advancedExpanded)
                {
                    DrawProperty(property.FindPropertyRelative("octaves"), 85);
                    DrawProperty(property.FindPropertyRelative("persistence"), 85);
                    DrawProperty(property.FindPropertyRelative("lacunarity"), 85);
                }
                
                EditorGUILayout.EndFoldoutHeaderGroup();
                EditorGUILayout.EndVertical();
                GUILayout.Space(6);
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;
                EditorGUI.indentLevel++;
                
                GUILayout.Space(2);
                EditorGUILayout.EndVertical();
                
                EditorGUILayout.EndHorizontal();
                
                EditorGUILayout.EndVertical();
                EditorGUILayout.EndHorizontal();
            }

            EditorGUILayout.EndFoldoutHeaderGroup();

            PerlinNoise n = (PerlinNoise) property.boxedValue;
            if (!_lastNoise.Equals(n))
            {
                RenderPerlin(_perlinTexture, n);
                _lastNoise = n;
            }
        }

        void RenderPerlin(Texture2D tex, PerlinNoise perlin)
        {
            for (int y = 0; y < tex.height; ++y)
            {
                for (int x = 0; x < tex.width; ++x)
                {
                    float perlinX = x / (float) ScaleRes;
                    float perlinY = y / (float) ScaleRes;
                    float value = perlin.Evaluate(perlinX, perlinY);
                    
                    Color color = Color.Lerp(Color.black, Color.white, value);
                    
                    tex.SetPixel(x, y, color);
                }
            }
            tex.Apply();
        }

        void DrawPerlinRect()
        {
            Vector2 cardSize = new Vector2(k_CardSize, k_CardSize);
            Rect cardRect = GUILayoutUtility.GetRect(GUIContent.none, GUIStyle.none, 
                GUILayout.Width(cardSize.x), GUILayout.Height(cardSize.y));
            cardRect.y += 2;
            Rect previewRect = new Rect(
                cardRect.x + k_CardPreviewOffset, cardRect.y + k_CardPreviewOffset,
                cardRect.width - (k_CardPreviewOffset * 2), cardRect.height - (k_CardPreviewOffset * 2));
                
            GUI.Box(cardRect, GUIContent.none, EditorStyles.helpBox);

            EditorGUI.DrawPreviewTexture(previewRect, _perlinTexture);
        }

        void DrawProperty(SerializedProperty prop, int width = 64)
        {
            EditorGUILayout.BeginHorizontal();
            GUILayout.Space(2);
            EditorGUILayout.LabelField(prop.displayName, GUILayout.Width(width));
            EditorGUILayout.PropertyField(prop, GUIContent.none);
            GUILayout.Space(2);
            EditorGUILayout.EndHorizontal();
        }
    }
}