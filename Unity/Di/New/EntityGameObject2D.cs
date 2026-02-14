namespace BB.Di
{
    public sealed class EntityGameObject2D : BaseEntityInstallerGameObject
    {
        public override void Install(IDiContainer container)
        {
            base.Install(container);
            container.System<Root2D>();
            foreach (var comp in GetComponents<EntityComponent2D>())
                comp.Install(container);
        }
        protected override IEntity SpawnEntity()
           => Entity.Spawn(new SpawnEntityFromPrefab2DContext
           {
               Prefab = this,
               DoNotInstantiate = true,
               SerializationName = _serializationName
           })._ref;
    }
}