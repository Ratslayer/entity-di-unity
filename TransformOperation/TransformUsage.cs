using System;

[Flags]
public enum TransformUsage
{
	None = 0,
	Position = 1,
	Rotation = 1 << 1,
	Scale = 1 << 2
}
