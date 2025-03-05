namespace BB
{
	public readonly struct TriggerVolumeExitEvent
	{
		public readonly Entity _entity;
		public TriggerVolumeExitEvent(Entity entity)
		{
			_entity = entity;
		}
	}
}