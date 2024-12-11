using UnityEditor;
using UnityEditor.Overlays;
using UnityEngine;

namespace FoliageTool.Core
{
    [Overlay(typeof(SceneView), OverlayID, ToolbarDisplayName, "", true)]
    internal class EditContextOverlay : IMGUIOverlay
    {
        private const string OverlayID = "FoliageToolEditContextOverlay";
        private const string ToolbarDisplayName = "Foliage Editor";

        private GUIContent _contentEnable;
        private GUIContent _contentDisable;
        
        public override void OnCreated()
        {
            _contentEnable = new GUIContent("Enable", "Enables the Foliage Editor.");
            _contentDisable = new GUIContent("Disable", "Disables the Foliage Editor.");
        }

        public override void OnGUI()
        {
            if (BrushEditing.Enabled)
            {
                if (GUILayout.Button(_contentDisable, GUILayout.Width(64)))
                    BrushEditing.Disable();
            }
            else
            {
                if (GUILayout.Button(_contentEnable, GUILayout.Width(64)))
                    BrushEditing.Enable();
            }
        }
    }
}