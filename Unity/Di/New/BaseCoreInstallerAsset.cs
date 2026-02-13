using BB.Di;
using UnityEngine;
namespace BB
{
    public abstract class EntityChildComponent : BaseComponent
    {
        public BaseEntityGameObject EntityGameObject => GetComponentInParent<BaseEntityGameObject>();
        public Entity Entity => EntityGameObject.Entity;
    }
    public abstract class BaseGameInstallerAsset : InstallerAsset, ILoadableAsset
    {
        public string _assetLoadKey;
        public abstract PlayerInstaller PlayerInstaller { get; }

        public string AssetLoadKey => _assetLoadKey;

        public override void Install(IDiContainer container)
        {
            base.Install(container);
            container.Instance(PlayerInstaller);
            container.Service<IEntitySpawnManager, EntitySpawnManager>();
            container.Service<IPrefabSpawnManager, PrefabSpawnManager>();
            container.Service<IUnityFromInstallerSpawner, UnityFromInstallerSpawner>();
            container.Service<IUnityFromPrefabSpawner, UnityFromPrefabSpawner>();
        }
    }
    public abstract class BaseCoreInstallerAsset : InstallerAsset
    {
        public override void Install(IDiContainer container)
        {
            base.Install(container);
            container.System<BindUpdates>();
            container.Service<IEntitySpawnManager, EntitySpawnManager>();
            container.Service<IPrefabSpawnManager, PrefabSpawnManager>();
            container.Service<IUnityFromInstallerSpawner, UnityFromInstallerSpawner>();
            container.Service<IUnityFromPrefabSpawner, UnityFromPrefabSpawner>();
        }
        sealed class BindUpdates : EntitySystem
        {
            [OnEvent]
            void OnCreate(EntityCreatedEvent _)
            {
                var updates = new GameObject("World Updates").AddComponent<WorldUpdates>();
                updates.SetEntity(Entity._ref);

                UnityEngine.Object.DontDestroyOnLoad(updates.gameObject);
            }
        }
    }
}