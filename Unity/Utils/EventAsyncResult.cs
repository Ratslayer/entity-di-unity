namespace BB
{
	public readonly struct EventAsyncResult<T>
	{
		public readonly bool _caughtEvent;
		public readonly T _event;
		public EventAsyncResult(bool caughtEvent, T e)
		{
			_caughtEvent = caughtEvent;
			_event = e;
		}
		public static implicit operator bool(EventAsyncResult<T> eventAsyncResult)
			=> eventAsyncResult._caughtEvent;
	}
}