using UnityEngine;

public static class V2Utils
{
	public static Vector2 RandomDir
		=> Quaternion.AngleAxis(
			RandomUtils.RandomAngle(),
			Vector3.forward) * Vector3.right;
}
