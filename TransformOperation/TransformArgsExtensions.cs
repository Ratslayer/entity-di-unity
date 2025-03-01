using UnityEngine;
using static TransformArgsUsage;
namespace BB
{
	public static class TransformArgsExtensions
	{
		public static TransformOperation ToTransformOperation(this Transform t)
			=> t ? new(t.position, t.rotation, t.localScale, t.parent) : default;
		public static ApplyTransformOperationDisposable ResetTransformOnDispose(
			this Transform t,
			bool record = true)
			=> record && t ? new(t.ToTransformOperation(), t) : default;
		public static TransformOperation ToArgsParentNoMove(this Transform t)
			=> t ? new(default, default, default, t, Parent | Reset2D) : default;
	}
}