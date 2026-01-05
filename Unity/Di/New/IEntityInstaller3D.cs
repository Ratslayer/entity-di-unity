using UnityEngine;
namespace BB.Di
{
	public interface IEntityInstaller3D : IEntityInstaller
    {
        Transform Prefab { get; }
    }
}