using Cysharp.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
namespace BB
{
	public sealed class GameInstallerBehaviour : BaseBehaviour
	{
		[SerializeField, Required]
		InstallerAsset _installer;
		private void Awake()
		{
			if (!_installer
				|| !World.Has(out GameLoadedFromScene loaded)
				|| loaded.Value)
				return;
			loaded.Value = true;
			World.SetGame(_installer);
			StartGame().Forget();
			
			static async UniTaskVoid StartGame()
			{
				await UniTask.NextFrame();
                World.Publish<BeforeLevelSpawnEvent>();
                await UniTask.NextFrame();
                World.Publish<AfterLevelSpawnEvent>();
                await UniTask.NextFrame();
                World.Publish<TryStartGameEvent>();
            }
		}
	}
	public sealed record GameLoadedFromScene : Variable<GameLoadedFromScene, bool>;

}
