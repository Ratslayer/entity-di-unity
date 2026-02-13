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
                .Require<IGameManager>()
                .StartGame(new()
                {
                    GameInstaller = _installer
                });
        }
    }
    public sealed class GameLoadedAtRuntime : Variable<GameLoadedAtRuntime, bool> { }
}