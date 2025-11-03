using UnityEngine;
namespace BB
{
	public readonly struct InstallerAdapter2D
    {
        public InstallerAsset2D Installer { get; init; }
        public RectTransform Prefab { get; init; }
        public static implicit operator InstallerAdapter2D(InstallerAsset2D asset)
            => new() { Installer = asset };
        public static implicit operator InstallerAdapter2D(GameObject prefab)
            => new() { Prefab = prefab.GetComponent<RectTransform>() };
        public static implicit operator InstallerAdapter2D(Component prefab)
            => new() { Prefab = prefab.GetComponent<RectTransform>() };
    }
}