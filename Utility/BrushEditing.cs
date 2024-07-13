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
            EditorApplication.update += OnFirstFrame;
            _firstFrameUpdated = false;
        }
        
        private static void OnFirstFrame()
        {
            _firstFrameUpdated = true;
            EditorApplication.update -= OnFirstFrame;
        }

        static PlayModeStateChange _playModeState;
        private static void PlayStateChanged(PlayModeStateChange state)
        {
            _playModeState = state;
        }
        
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
