namespace BB
{
	public readonly struct TriggerVolumeEnterEvent
	{
		public readonly Entity _entity;
		public TriggerVolumeEnterEvent(Entity entity)
		{
			_entity = entity;
		}
	}
}