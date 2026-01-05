namespace BB
{
	public interface IPrefabPool
    {
        PooledGameObject GetDisabledInstance();
        void ReturnInstance(PooledGameObject instance);
    }
}