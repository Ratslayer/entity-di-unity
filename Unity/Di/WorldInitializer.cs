using UnityEngine;
using BB.Di;
namespace BB
{
    public sealed class WorldInitializer : IWorldInitializer
    {
        const string CoreInstaller = "Core";

        public WorldSetupConfig Init()
        {
            var logger = new UnityLogger();
            Log.BindLogger(logger);

            var coreInstaller = Resources.Load<BaseCoreInstallerAsset>(CoreInstaller);
            if (!coreInstaller)
                throw new DiException(
                    $"No {CoreInstaller} resource " +
                    $"of type {typeof(BaseCoreInstallerAsset).FullName} found");

            return new WorldSetupConfig
            {
                AdditionalInstaller = new UnityAdditionalInstaller(),
                CoreInstaller = coreInstaller,
                ForcedDinamicTypes = new[] { typeof(IBoard), typeof(IEvent<IBoard>) },
                Logger = logger
            };
        }
    }
}