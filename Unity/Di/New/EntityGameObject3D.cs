using BB.Di;

namespace BB
{
    public sealed class EntityGameObject3D : BaseEntityInstallerGameObject
    {
        public override void Install(IDiContainer container)
        {
            base.Install(container);
            container.System<Root>();
            foreach (var comp in GetComponents<EntityComponent3D>())
                comp.Install(container);
        }
        protected override IEntity SpawnEntity()
            => Entity.Spawn(new SpawnEntityFromPrefab3DContext
            {
                Prefab = this,
                DoNotInstantiate = true
            })._ref;
    }
}