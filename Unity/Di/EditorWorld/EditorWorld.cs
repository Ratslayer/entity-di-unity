#if UNITY_EDITOR
using UnityEditor;

namespace BB.Di
{
    public static class EditorWorld
    {
        const string InstallerPath = "Assets/0Game/World/EditorWorld.asset";
        static IEntity _entity;
        public static IEntity Entity
        {
            get
            {
                if (_entity == null)
                    InitWorld();
                return _entity;
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
            return;
            var editorWorld
                        = AssetDatabase.LoadAssetAtPath
                        <InstallerAsset>(InstallerPath);
            Log.BindLogger(new UnityLogger());
            //var impl = EntityImpl.CreateEntity(
            //    "Editor",
            //    null,
            //    editorWorld,
            //    null,
            //    true);
            //_entity = impl;
            IEntity impl = null;
            impl.SetState(EntityState.Enabled);
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
            _entity.SetState(EntityState.Destroyed);
            InitWorld();
        }

    }
}
#endif
