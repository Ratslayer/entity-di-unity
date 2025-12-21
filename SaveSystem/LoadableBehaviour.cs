using BB.Di;

namespace BB
{
    public abstract class LoadableBehaviour : EntityBehaviour3D
    {
        [Inject]
        ILoadableBehaviours _behaviours;
        [OnEvent(typeof(EntitySpawnedEvent))]
        void OnSpawn() => _behaviours.Add(this);
        [OnEvent(typeof(EntityDespawnedEvent))]
        void OnDespawn() => _behaviours.Remove(this);
        protected virtual string LoadName => name;
        protected virtual string LoadNamePrefix => null;
        public string Key
            => string.IsNullOrWhiteSpace(LoadNamePrefix)
            ? LoadName
            : $"{LoadNamePrefix}_{LoadName}";
    }
}