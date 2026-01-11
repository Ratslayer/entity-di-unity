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
			if (!_installer
				|| !World.Has(out GameLoadedFromScene loaded)
				|| loaded.Value)
				return;
			loaded.Value = true;
			World
				.Require<IGameManager>()
				.StartGame(new() { Installer = _installer })
				.Forget();
		}
	}
	public sealed class GameLoadedFromScene : Variable<GameLoadedFromScene, bool> { }

}
