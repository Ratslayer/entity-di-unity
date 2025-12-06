using UnityEngine;
using BB.Di;
namespace BB
{
	public abstract class BaseWorldInstallerAsset : InstallerAsset
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
        sealed class BindUpdates
        {
            [Inject]
            IEntity _entity;
            [OnEvent]
            void OnCreate(CreatedEvent _)
            {
                var updates = new GameObject("World Updates").AddComponent<WorldUpdates>();
                updates.SetEntity(_entity);

                UnityEngine.Object.DontDestroyOnLoad(updates.gameObject);
            }
        }
    }
}