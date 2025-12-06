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

            var worldInstaller = Resources.Load<BaseWorldInstallerAsset>(WorldInstaller);
            if (!worldInstaller)
                throw new DiException(
                    $"No {WorldInstaller} resource " +
                    $"of type {typeof(BaseWorldInstallerAsset).FullName} found");

            return new WorldSetupConfig
            {
                AdditionalInstaller = new UnityAdditionalInstaller(),
                WorldInstaller = worldInstaller,
                ForcedDinamicTypes = new[] { typeof(IBoard), typeof(IEvent<IBoard>) }
            };
        }
    }
}