using BB.Di;
using UnityEngine;
namespace BB
{
    public readonly struct Installer2DAdapter
    {
        public InstallerAsset2D Installer { get; init; }
        public RectTransform Prefab { get; init; }
        public EntityGameObject2D PrefabInstaller { get; init; }
        public static implicit operator Installer2DAdapter((InstallerAsset2D, RectTransform) installer)
            => new()
            {
                Installer = installer.Item1,
                Prefab = installer.Item2
            };
        public static implicit operator Installer2DAdapter(EntityGameObject2D prefab)
            => new() { PrefabInstaller = prefab };
        public static implicit operator Installer2DAdapter(EntityComponent2D prefab)
            => new() { PrefabInstaller = prefab.GetComponent<EntityGameObject2D>() };
        public static implicit operator Installer2DAdapter(InstallerAsset2D installer)
            => new()
            {
                Installer = installer,
                Prefab = installer ? installer.Prefab : null
            };
    }
}