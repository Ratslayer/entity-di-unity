using BB.Di;

namespace BB
{
    public sealed class GameInstallerBehaviour : BaseComponent
    {
        public BaseGameInstallerAsset _installer;

        private void Awake()
        {
            if (WorldBootstrap.World.Game?.Entity is not null)
                return;
            World
                .Require<IGameScenariosService>()
                .StartGame(_installer, null);
        }
    }

    public sealed class GameLoadedAtRuntime : Variable<GameLoadedAtRuntime, bool>
    {
    }
}