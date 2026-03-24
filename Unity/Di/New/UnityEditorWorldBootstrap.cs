using UnityEditor;
using UnityEngine;

namespace BB.Di
{
    [InitializeOnLoad]
    public static class UnityEditorWorldBootstrap
    {
        static UnityEditorWorldBootstrap()
        {
            SubscribeToEvents();
            BuildWorld();
        }

        static void SubscribeToEvents()
        {
            EditorApplication.playModeStateChanged += OnPlayModeChanged;
        }

        static void OnPlayModeChanged(PlayModeStateChange state)
        {
            if (state is not PlayModeStateChange.ExitingPlayMode)
                return;
            
            Debug.Log("Creating editor world");
            WorldBootstrap.SpawnWorld(UnityWorldBootstrap.Settings._editorConfig);
        }

        public static void BuildWorld()
        {
            Debug.Log("Creating editor world");
            
            var settings = UnityWorldBootstrap.Settings;
            if (!settings)
            {
                settings = new();
                AssetDatabase.CreateAsset(settings, "Assets/Resources/UnityWorldBootstrapSettings.asset");
            }

            WorldBootstrap.SpawnWorld(settings._editorConfig);
        }
    }
}