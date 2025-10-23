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
			World.Publish<BeforeLevelSpawnEvent>();
			World.Publish<AfterLevelSpawnEvent>();
			World.Publish<StartGameEvent>();
		}
	}
	public sealed record GameLoadedFromScene : Variable<GameLoadedFromScene, bool>;

}
