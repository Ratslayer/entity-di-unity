using BB.Di;

namespace BB
{
    public sealed class CoreConfigAsset : BaseCoreConfigAsset
    {
        public InstallerAsset _installer;
        public override WorldSetupConfig GetConfig()
        {
            var logger = new UnityLogger();
            Log.BindLogger(logger);
            
            return new WorldSetupConfig
            {
                AdditionalInstaller = new UnityAdditionalInstaller(),
                CoreInstaller = _installer,
                ForcedDynamicTypes = new[] { typeof(IBoard), typeof(IEvent<IBoard>) },
                Logger = logger
            };
        }
    }
}