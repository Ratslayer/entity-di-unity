using UnityEngine;

public static class RotUtils
{
	public static Quaternion Forward(Vector3 dir)
		=> Quaternion.FromToRotation(Vector3.forward, dir);
}