using BB.Di;
using UnityEngine;
namespace BB
{
	public readonly struct InstallerAdapter2D
    {
        public InstallerAsset2D Installer { get; init; }
        public RectTransform Prefab { get; init; }
        public EntityGameObject2D PrefabInstaller { get; init; }
        public static implicit operator InstallerAdapter2D((InstallerAsset2D, RectTransform) installer)
            => new()
            {
                Installer = installer.Item1,
                Prefab = installer.Item2
            };
        public static implicit operator InstallerAdapter2D(EntityGameObject2D prefab)
            => new() { PrefabInstaller = prefab };
    }
}