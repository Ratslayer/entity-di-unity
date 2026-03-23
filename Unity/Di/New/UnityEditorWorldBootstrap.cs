using UnityEditor;
using UnityEngine;

namespace BB.Di
{
    [InitializeOnLoad]
    public static class UnityEditorWorldBootstrap
    {
        static UnityEditorWorldBootstrap()
        {
            Debug.Log("Creating editor world");

            SubscribeToEvents();

            var settings = UnityWorldBootstrap.Settings;
            if (!settings)
            {
                settings = new();
                AssetDatabase.CreateAsset(settings, "Assets/Resources/UnityWorldBootstrapSettings.asset");
            }

            WorldBootstrap.SpawnWorld(settings._editorConfig);
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
    }
}