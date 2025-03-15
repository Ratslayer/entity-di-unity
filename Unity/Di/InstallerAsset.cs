using BB.Di;
namespace BB
{
	public abstract class InstallerAsset : BaseInstallerAsset
	{
		[Inject]
		IEntity _entity;
		protected Entity Entity => _entity.GetToken();
		public override void Install(IDiContainer container)
		{
			container.InjectedInstance(GetType(), this);
		}
	}
}