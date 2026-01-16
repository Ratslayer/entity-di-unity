using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
namespace BB
{
    public sealed class GameInstallerBehaviour : BaseComponent
    {
        [SerializeField, Required]
        BaseGameInstallerAsset _installer;
        private void Awake()
        {
            World
                .Require<IGameManager>()
                .StartGame(new()
                {
                    Installer = _installer,
                    LoadedFromScene = true
                })
                .Forget();
        }
    }
    public sealed class GameLoadedAtRuntime : Variable<GameLoadedAtRuntime, bool> { }

}
