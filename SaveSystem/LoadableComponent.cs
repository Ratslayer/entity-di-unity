namespace BB
{
    public abstract class LoadableComponent : BaseComponent
    {
        protected virtual string LoadName => name;
        protected virtual string LoadNamePrefix => null;
        public string Key
            => string.IsNullOrWhiteSpace(LoadNamePrefix)
            ? LoadName
            : $"{LoadNamePrefix}_{LoadName}";

		private void OnEnable()
		{
            World.Require<ILoadableComponents>().Add(this);
		}
		private void OnDisable()
		{
            World.Require<ILoadableComponents>().Remove(this);

        }
    }
}