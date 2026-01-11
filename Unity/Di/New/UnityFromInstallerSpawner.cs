using UnityEngine;

namespace BB.Di
{
    public sealed class UnityFromInstallerSpawner
          : BaseEntitySpawnManager<IUnityFromInstallerSpawner.CommonContext>, IUnityFromInstallerSpawner
    {
        [Inject] IPrefabSpawnManager _prefabSpawnManager;
        public Entity Spawn(in IUnityFromInstallerSpawner.Context3D context)
        {
            if (Log.Assert(context.Installer is not null, "Installer is null"))
                return default;

            var prefab = context.Prefab ? context.Prefab
                : context.Installer.Prefab ? context.Installer.Prefab.gameObject
                : null;
            if (Log.Assert(prefab, "Prefab is null or destroyed"))
                return default;

            var entity = GetUnspawnedEntity(new()
            {
                Installer = context.Installer,
                Parent = context.Parent,
                Prefab = prefab
            });

            var root = entity.Require<Root>();
            root.Init();

            context.Operation.Apply(root);
            root.Transform.gameObject.SetActive(true);

            entity.SetState(EntityState.Enabled);
            return entity.GetToken();
        }

        public Entity Spawn(in IUnityFromInstallerSpawner.Context2D context)
        {
            if (Log.Assert(context.Installer is not null, "Installer is null"))
                return default;

            var prefab = context.Prefab ? context.Prefab
                : context.Installer.Prefab ? context.Installer.Prefab.gameObject
                : null;
            if (Log.Assert(prefab, "Prefab is null or destroyed"))
                return default;

            var entity = GetUnspawnedEntity(new()
            {
                Installer = context.Installer,
                Parent = context.Parent,
                Prefab = prefab
            });

            var root = entity.Require<Root2D>();
            root.Init();

            context.Operation.Apply(root);
            root.Transform.gameObject.SetActive(true);

            entity.SetState(EntityState.Enabled);
            return entity.GetToken();
        }
        protected override void InitEntityBeforeInjection(
            IEntity entity,
            in IUnityFromInstallerSpawner.CommonContext context)
        {
            var instance = _prefabSpawnManager.GetDisabledInstance(context.Prefab);
            entity.AttachGameObject(instance);
        }
    }
    public static class UnityEntityExtensions
    {
        public static void AttachGameObject(this IEntity entity, GameObject instance)
        {
            var gameObjectWrapper = entity.Require<GameObjectWrapper>();
            gameObjectWrapper.GameObject = instance;

            if (instance.TryGetComponent(out BaseEntityGameObject entityGameObject))
                entityGameObject.Init(entity);
        }
        public static bool IsInstaller2D(this Entity entity, in Installer2DAdapter installer)
        {
            if (entity._ref is not IEntityDetails details)
                return false;

            if (details.Installer == installer.Installer)
                return true;
            if (details.Installer == installer.PrefabInstaller)
                return true;
            return false;
        }
    }
}