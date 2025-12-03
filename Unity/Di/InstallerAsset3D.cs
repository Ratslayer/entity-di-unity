using BB.Di;
using UnityEngine;

namespace BB
{
	public abstract class InstallerAsset3D : InstallerAsset
	{
		public Transform _prefab;
		public override void Install(IDiContainer container)
		{
			base.Install(container);
			container.System<Root>();
		}
	}
}