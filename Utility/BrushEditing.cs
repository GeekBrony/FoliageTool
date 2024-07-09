#if UNITY_EDITOR

using UnityEditor;
namespace FoliageTool.Core
{
    [InitializeOnLoad]
    public static class BrushEditing
    {
        private static bool _firstFrameUpdated;
        static BrushEditing()
        {
            EditorApplication.playModeStateChanged += PlayStateChanged;
            EditorApplication.update += OnUpdate;
            _firstFrameUpdated = false;
        }
        
        private static void OnUpdate()
        {
            if(!_firstFrameUpdated)
                return;
            
            _firstFrameUpdated = true;
        }

        private static void PlayStateChanged(PlayModeStateChange state)
        {
            _playModeState = state;
        }

        static PlayModeStateChange _playModeState;
        
        static bool IsChangingPlayMode()
        {
            return
                EditorApplication.isPlayingOrWillChangePlaymode ||
                _playModeState == PlayModeStateChange.ExitingEditMode ||
                _playModeState == PlayModeStateChange.EnteredPlayMode ||
                _playModeState == PlayModeStateChange.ExitingPlayMode;
        }

        public static bool CanRefreshBrushes()
        {
            return !EditorApplication.isUpdating &&
                   !EditorApplication.isCompiling &&
                   !IsChangingPlayMode() &&
                   _firstFrameUpdated;
        }
    }

}
#endif
