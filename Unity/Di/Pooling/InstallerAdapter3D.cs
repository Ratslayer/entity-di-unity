using BB.Di;
using UnityEngine;
namespace BB
{
    public readonly struct InstallerAdapter3D
    {
        public InstallerAsset3D Installer { get; init; }
        public GameObject Prefab { get; init; }
        public EntityGameObject3D PrefabInstaller { get; init; }
        public static implicit operator InstallerAdapter3D((InstallerAsset3D, GameObject) installer)
            => new()
            {
                Installer = installer.Item1,
                Prefab = installer.Item2
            };
        public static implicit operator InstallerAdapter3D(EntityGameObject3D prefab)
            => new() { PrefabInstaller = prefab };

    }
}