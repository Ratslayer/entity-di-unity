namespace BB
{
    public readonly struct UpdateEvent
    {
        public float Delta { get; init; }
        public float UnscaledDelta { get; init; }
    }
    public readonly struct LateUpdateEvent
    {
        public float Delta { get; init; }
        public float UnscaledDelta { get; init; }
    }
    public readonly struct FixedUpdateEvent
    {
        public float Delta { get; init; }
        public float UnscaledDelta { get; init; }
    }
}