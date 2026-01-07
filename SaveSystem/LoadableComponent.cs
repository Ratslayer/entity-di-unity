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
    }
}