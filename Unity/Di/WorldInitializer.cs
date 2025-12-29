using UnityEngine;
using BB.Di;
namespace BB
{
	public sealed class WorldInitializer : IWorldInitializer
    {
        const string WorldInstaller = "World";

        public WorldSetupConfig Init()
        {
            Log.BindLogger(new UnityLogger());

            var worldInstaller = Resources.Load<BaseCoreInstallerAsset>(WorldInstaller);
            if (!worldInstaller)
                throw new DiException(
                    $"No {WorldInstaller} resource " +
                    $"of type {typeof(BaseCoreInstallerAsset).FullName} found");

            return new WorldSetupConfig
            {
                AdditionalInstaller = new UnityAdditionalInstaller(),
                WorldInstaller = worldInstaller,
                ForcedDinamicTypes = new[] { typeof(IBoard), typeof(IEvent<IBoard>) }
            };
        }
    }
}