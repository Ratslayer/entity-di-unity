#if UNITY_EDITOR
using UnityEditor;

namespace BB.Di
{
    public static class EditorWorld
    {
        const string InstallerPath = "Assets/0Game/World/EditorCore.asset";
        static WorldSetup _worldSetup;
        public static IEntity Entity
        {
            get
            {
                if (_worldSetup == null)
                    InitWorld();
                return _worldSetup.Core.Entity;
            }
        }
        public static T Get<T>()
        {
            if (!Entity.Has(out T result))
                throw new System.Exception($"EditorInstaller has no {typeof(T).FullName} bound.");
            return result;
        }
        static void InitWorld()
        {
            _worldSetup?.ClearCore();
            var logger = new UnityLogger();
            Log.BindLogger(logger);

            var editorWorld = AssetDatabase.LoadAssetAtPath<InstallerAsset>(InstallerPath);
            _worldSetup = WorldSetup.CreateFromConfig(new WorldSetupConfig
            {
                AdditionalInstaller = new UnityAdditionalInstaller(),
                Logger = logger
            });
            _worldSetup.CreateCore(editorWorld);
        }
        [InitializeOnLoadMethod]
        static void CreateEntity()
        {
            InitWorld();
            EditorApplication.playModeStateChanged -= OnExitPlayMode;
            EditorApplication.playModeStateChanged += OnExitPlayMode;
        }
        static void OnExitPlayMode(PlayModeStateChange change)
        {
            if (change is not PlayModeStateChange.EnteredEditMode)
                return;
            EditorApplication.playModeStateChanged -= OnExitPlayMode;

            WorldBootstrap.DestroyWorld();

            InitWorld();
        }

    }
}
#endif
