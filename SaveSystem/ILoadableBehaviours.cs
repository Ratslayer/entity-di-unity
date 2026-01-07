namespace BB
{
	public interface ILoadableBehaviours
    {
        void Add(LoadableComponent behaviour);
        void Remove(LoadableComponent behaviour);
        bool TryGet(string key, out LoadableComponent result);
    }
}