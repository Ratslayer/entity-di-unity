using UnityEngine;

public static class V2Utils
{
	public static Vector2 RandomDir
		=> Quaternion.AngleAxis(
			RandomUtils.RandomAngle(),
			Vector3.forward) * Vector3.right;

	public static Vector3 YToZ(this Vector2 v)
		=> new(v.x, 0, v.y);
	public static Vector3 ToV3(this Vector2 v, Vector3 right, Vector3 up)
		=> v.x * right + v.y * up;
}