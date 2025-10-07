public enum TransformOperationUsage
{
    None = 0,
    Position = 1,
    Rotation = 1 << 1,
    Scale = 1 << 2,
    Parent = 1 << 3,
    Local = 1 << 4,
    Additive = 1 << 5,
}
public enum TransformOperation2DUsage
{
    None = 0,
    Position = 1,
    Rotation = 1 << 1,
    SizeDelta = 1 << 2,
    Parent = 1 << 3,
    Anchors = 1 << 4,
    AnchoredPosition = 1 << 5
}
