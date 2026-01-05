using UnityEngine;

namespace BB
{
	public interface IPrefabSpawnManager
    {
        GameObject GetDisabledInstance(GameObject prefab);
    }
}