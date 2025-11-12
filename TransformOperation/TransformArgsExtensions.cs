using UnityEngine;
namespace BB
{
    public static class TransformArgsExtensions
    {
        public static TransformOperation ToTransformOperation(this Transform t, bool useSameParent = true)
            => t ? new()
            {
                Position = t.position,
                Rotation = t.rotation,
                Scale = t.localScale,
                Parent = useSameParent ? t.parent : null
            } : default;
        public static TransformOperation ToTransformOperation(this Root root)
            => new()
            {
                Position = root.Position,
                Rotation = root.Rotation,
                Scale = root.Scale
            };
        public static ApplyTransformOperationDisposable ResetTransformOnDispose(
            this Transform t,
            bool record = true)
            => record && t ? new(t.ToTransformOperation(), t) : default;
    }
}