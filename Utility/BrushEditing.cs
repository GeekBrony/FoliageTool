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
        
        public static bool Enabled { get; private set; } = false;
        public static void Enable()
        {
            Enabled = true;
        }
        public static void Disable()
        {
            Enabled = false;
        }

        static PlayModeStateChange _playModeState;
        private static void PlayStateChanged(PlayModeStateChange state)
        {
            _playModeState = state;
            
            if(Enabled && state == PlayModeStateChange.ExitingEditMode)
                Disable();
        }
        
        static bool IsChangingPlayMode()
        {
            return
                EditorApplication.isPlayingOrWillChangePlaymode ||
                _playModeState == PlayModeStateChange.ExitingEditMode ||
                _playModeState == PlayModeStateChange.EnteredPlayMode ||
                _playModeState == PlayModeStateChange.ExitingPlayMode;
        }

        public static bool CanRefresh()
        {
            return Enabled &&
                   !EditorApplication.isUpdating &&
                   !EditorApplication.isCompiling &&
                   !IsChangingPlayMode() &&
                   _firstFrameUpdated;
        }
    }

}
#endif
