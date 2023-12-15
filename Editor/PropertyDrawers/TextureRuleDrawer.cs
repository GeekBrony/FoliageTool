using FoliageTool.Core;
using UnityEditor;
using UnityEngine;

[CustomPropertyDrawer(typeof(TextureRule))]
public class TextureRuleDrawer : PropertyDrawer
{
    
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        //base.OnGUI(position, property, label);
        var layerProperty = property.FindPropertyRelative("layer");
        var disableProperty = property.FindPropertyRelative("disable");
        var ruleProperty = property.FindPropertyRelative("rule");
        var thresholdProperty = property.FindPropertyRelative("threshold");
        
        property.serializedObject.Update();
        
        TerrainLayer layer = layerProperty.objectReferenceValue as TerrainLayer;
        Rect cardRect = DrawRect(position, layer);

        Rect toggleRect = new Rect(new Vector2(cardRect.xMax - 18, cardRect.y + 2), new Vector2(18, 18));
        disableProperty.boolValue = !EditorGUI.Toggle(toggleRect, GUIContent.none, !disableProperty.boolValue);
        
        position.x += k_CardSize + 5;
        position.y += 3;
        
        Rect layerPropPos = new Rect(position.position, 
            new Vector2(position.width - k_CardSize - (k_CardPreviewOffset * 2), 24));
        EditorGUI.PropertyField(layerPropPos, layerProperty, GUIContent.none);
        
        Rect rulePropPos = new Rect(
            new Vector2(layerPropPos.x, layerPropPos.y + layerPropPos.height + 5),
            new Vector2(70, 18));
        EditorGUI.PropertyField(rulePropPos, ruleProperty, GUIContent.none);
        
        Rect thresholdPropPos = new Rect(
            new Vector2(rulePropPos.xMax + 10, layerPropPos.y + layerPropPos.height + 5),
            new Vector2(layerPropPos.width - rulePropPos.width - 10, 18));
        Rect thresholdLabelPos = new Rect(thresholdPropPos.position, new Vector2(65, thresholdPropPos.height));
        thresholdPropPos.width -= thresholdLabelPos.width;
        EditorGUI.HandlePrefixLabel(thresholdPropPos, thresholdLabelPos, new GUIContent("Threshold"));
        thresholdPropPos.x += thresholdLabelPos.width;
        EditorGUI.PropertyField(thresholdPropPos, thresholdProperty, GUIContent.none);

        property.serializedObject.ApplyModifiedProperties();
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        return k_CardSize + k_CardPreviewOffset;
    }

    const int k_CardSize = 60;
    const int k_CardPreviewOffset = 2;
    
    Rect DrawRect(Rect pos, TerrainLayer layer)
    {
        Vector2 cardSize = new Vector2(k_CardSize, k_CardSize);
        Rect cardRect = new Rect(pos.position, cardSize);
        cardRect.y += 2;
        
        Rect previewRect = new Rect(cardRect.x + k_CardPreviewOffset, cardRect.y + k_CardPreviewOffset,
            cardRect.width - (k_CardPreviewOffset * 2), cardRect.height - (k_CardPreviewOffset * 2));
                
        GUI.Box(cardRect, GUIContent.none, EditorStyles.helpBox);
        if (layer && layer.diffuseTexture)
        {
            EditorGUI.DrawPreviewTexture(previewRect, layer.diffuseTexture);
        }

        return cardRect;
    }
}
