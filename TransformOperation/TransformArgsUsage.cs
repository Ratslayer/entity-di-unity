public enum TransformArgsUsage
{
	None = 0,
	Position = 1,
	Rotation = 1 << 1,
	Scale = 1 << 2,
	Parent = 1 << 3,
	Local = 1 << 4,
	Reset2D = 1 << 5,
	Additive = 1 << 6
}
