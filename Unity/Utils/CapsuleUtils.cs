using UnityEngine;
namespace BB
{
	public static class CapsuleUtils
	{
		public static CapsuleArgs GetVerticalCapsuleArgs(Vector3 bottom, float height, float radius, float skinWidth)
		{
			var r = Mathf.Max(0, radius - skinWidth);
			var b = bottom + Vector3.up * skinWidth;
			var h = height - 2 * skinWidth;
			var p1 = b + Vector3.up * r;
			var p2 = b + Vector3.up * Mathf.Max(r, h - r);
			return new(p1, p2, r);
		}
	}
}