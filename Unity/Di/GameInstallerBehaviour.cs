using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
namespace BB
{
    public sealed class GameInstallerBehaviour : BaseComponent
    {
        [SerializeField, Required]
        GameLevel _level;
        private void Awake()
        {
            World
                .Require<IGameManager>()
                .StartGame(new()
                {
                    Level = _level,
                    LoadedFromScene = true
                })
                .Forget();
        }
    }
    public sealed class GameLoadedAtRuntime : Variable<GameLoadedAtRuntime, bool> { }

}
