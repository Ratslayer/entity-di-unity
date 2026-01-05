namespace BB.Di
{
	public interface IUnityEntityPools3D
    {
        IUnityEntityPool3D GetPool(EntityGameObject3D prefab);
    }
}