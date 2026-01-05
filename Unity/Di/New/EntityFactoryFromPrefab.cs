using UnityEngine;
namespace BB.Di
{
	public sealed record EntityFactoryFromPrefab(
        IEntityPool Pool,
        IEntityInjector Injector,
        IEntityInstaller Installer,
        GameObject Prefab,
        bool DoNotInstantiate)
        : EntityFactory(Pool, Injector, Installer)
    {
        public override IEntity Create(in CreateEntityContext context)
        {
            var entity = base.Create(context);
            GameObject instance;
            if (DoNotInstantiate)
                instance = Prefab;
            else
            {
                Prefab.SetActive(false);
                instance = UnityEngine.Object.Instantiate(Prefab);
                Prefab.SetActive(true);
            }

            var gameObjectWrapper = ((IFullEntity)entity).GetComponentData(new()
            {
                ContractType = typeof(GameObjectWrapper),
                RequestingType = GetType(),
                Init = true
            });
            ((GameObjectWrapper)gameObjectWrapper.Instance).GameObject = instance;

            if (instance.TryGetComponent(out BaseEntityGameObject gameObject))
                gameObject.Init(entity);

            return entity;
        }
    }
}