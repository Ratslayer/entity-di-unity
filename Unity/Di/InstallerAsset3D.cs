using BB.Di;
using UnityEngine;

namespace BB
{
    public abstract class InstallerAsset3D : InstallerAsset, IEntityInstaller3D
    {
        public Transform _prefab;
        public Transform Prefab => _prefab;
        public override void Install(IDiContainer container)
        {
            base.Install(container);
            container.System<GameObjectWrapper>();
            container.System<Root>();
        }
    }
}