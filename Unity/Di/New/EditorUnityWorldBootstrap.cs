using UnityEditor;

namespace BB.Di
{
    [InitializeOnLoad]
    public static class EditorUnityWorldBootstrap
    {
        static EditorUnityWorldBootstrap()
        {
            EditorApplication.playModeStateChanged += OnPlayModeStateChanged;
        }

        private static void OnPlayModeStateChanged(PlayModeStateChange state)
        {
            if (state != PlayModeStateChange.ExitingPlayMode)
                return;
            WorldBootstrap.DestroyWorld();
        }
    }
}