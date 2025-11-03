using UnityEngine;
namespace BB
{
	public readonly struct InstallerAdapter3D
    {
        public InstallerAsset3D Installer { get; init; }
        public GameObject Prefab { get; init; }
        public static implicit operator InstallerAdapter3D(InstallerAsset3D asset)
            => new() { Installer = asset };
        public static implicit operator InstallerAdapter3D(GameObject prefab)
            => new() { Prefab = prefab };
        public static implicit operator InstallerAdapter3D(Component prefab)
            => new() { Prefab = prefab.gameObject };
    }
}