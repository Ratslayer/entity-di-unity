namespace BB.Di
{
	public sealed class UnityAdditionalInstaller : AdditionalInstaller
    {
		public override void Install(IDiContainer container)
		{
			base.Install(container);

            container.CascadingEvent<UpdateEvent>();
            container.CascadingEvent<LateUpdateEvent>();
            container.CascadingEvent<FixedUpdateEvent>();
		}
    }
}