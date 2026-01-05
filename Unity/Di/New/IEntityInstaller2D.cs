using UnityEngine;
namespace BB.Di
{
	public interface IEntityInstaller2D : IEntityInstaller
    {
        RectTransform Prefab { get; }
    }
}