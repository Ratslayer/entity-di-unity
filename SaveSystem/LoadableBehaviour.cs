using BB.Di;

namespace BB
{
    public abstract class LoadableBehaviour : EntityBehaviour
    {
        [Inject]
        ILoadableBehaviours _behaviours;
        [OnSpawn]
        void OnSpawn() => _behaviours.Add(this);
        [OnDespawn]
        void OnDespawn() => _behaviours.Remove(this);
        protected virtual string LoadName => name;
        protected virtual string LoadNamePrefix => null;
        public string Key
            => string.IsNullOrWhiteSpace(LoadNamePrefix)
            ? LoadName
            : $"{LoadNamePrefix}_{LoadName}";
    }
}