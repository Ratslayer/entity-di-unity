using BB.Di;

namespace BB
{
    public sealed class EntityBootstrapSettings : BaseScriptableObject
    {
        public BaseCoreConfigAsset _runtimeConfig;
#if UNITY_EDITOR
        public BaseCoreConfigAsset _editorConfig;
#endif
    }

    public abstract class BaseCoreConfigAsset : BaseScriptableObject, IWorldConfigProvider
    {
        public abstract WorldSetupConfig GetConfig();
    }
}