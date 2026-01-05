namespace BB.Di
{
	public sealed class ComponentDiComponent : BaseDiComponent
    {
        public override bool AlwaysCreate => true;
        public ComponentDiComponent(in DiComponentContext context) : base(context)
        {
        }

        public override object Create(IEntity entity)
        {
            var goWrapper = entity.Require<GameObjectWrapper>();
            var component = goWrapper.GameObject.GetComponent(ContractType);
            return component;
        }

        public override bool Validate(IEntityInstaller installer) => true;
    }
}