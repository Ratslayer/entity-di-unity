using BB.Di;
using UnityEngine;

namespace BB
{
    public abstract class InstallerAsset2D : InstallerAsset, IEntityInstaller2D
    {
        public RectTransform _prefab;
        public RectTransform Prefab => _prefab;
        public override void Install(IDiContainer container)
        {
            base.Install(container);
            container.System<GameObjectWrapper>();
            container.System<Root2D>();
        }
    }
}