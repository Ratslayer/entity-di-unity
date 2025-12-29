using BB.Di;
using UnityEngine;
namespace BB
{
    public abstract class EntityChildComponent : BaseBehaviour
    {
        public BaseEntityGameObject EntityGameObject => GetComponentInParent<BaseEntityGameObject>();
        public Entity Entity => EntityGameObject.Entity;
    }
    public abstract class BaseGameInstallerAsset : InstallerAsset
    {
        public override void Install(IDiContainer container)
        {
            base.Install(container);
            container.System<IEntitySpawnManager, EntitySpawnManager>();
            container.System<IPrefabSpawnManager, PrefabSpawnManager>();
            container.System<IUnityFromInstallerSpawner, UnityFromInstallerSpawner>();
            container.System<IUnityFromPrefabSpawner, UnityFromPrefabSpawner>();
        }
    }
    public abstract class BaseCoreInstallerAsset : InstallerAsset
    {
        public override void Install(IDiContainer container)
        {
            base.Install(container);
            container.System<BindUpdates>();
            container.System<IEntitySpawnManager, EntitySpawnManager>();
            container.System<IPrefabSpawnManager, PrefabSpawnManager>();
            container.System<IUnityFromInstallerSpawner, UnityFromInstallerSpawner>();
            container.System<IUnityFromPrefabSpawner, UnityFromPrefabSpawner>();
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