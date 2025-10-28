namespace BB
{
	public interface ILoadableBehaviours
    {
        void Add(LoadableBehaviour behaviour);
        void Remove(LoadableBehaviour behaviour);
        bool TryGet(string key, out LoadableBehaviour result);
    }
}