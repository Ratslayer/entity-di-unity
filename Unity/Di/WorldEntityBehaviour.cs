using BB.Di;
using Sirenix.OdinInspector;
using UnityEngine;
namespace BB
{
	public sealed class WorldEntityBehaviour : BaseBehaviour
	{
		[SerializeField, Required]
		BaseInstallerAsset _installer;
		private void Awake()
		{
			if (!_installer
				|| !World.Has(out GameLoadedFromScene loaded)
				|| loaded.Value)
				return;
			loaded.Value = true;
			World.PushWorld(name, Install);
		}
		void Install(IDiContainer container)
		{
			container.Event<GameLoadedFromScene>();
			//container.System<GameObjectPoolParents>(gameObject);
			_installer.Install(container);
		}
	}
	public sealed record GameLoadedFromScene : Variable<GameLoadedFromScene, bool>;

}
