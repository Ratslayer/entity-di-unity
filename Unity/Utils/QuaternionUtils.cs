using UnityEngine;

public static class QuaternionUtils
{
	public static Quaternion QuaternionFromMatrix(Matrix4x4 m)
	{
		// Adapted from: http://www.euclideanspace.com/maths/geometry/rotations/conversions/matrixToQuaternion/index.htm
		Quaternion q = new Quaternion();
		q.w = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] + m[1, 1] + m[2, 2])) / 2;
		q.x = Mathf.Sqrt(Mathf.Max(0, 1 + m[0, 0] - m[1, 1] - m[2, 2])) / 2;
		q.y = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] + m[1, 1] - m[2, 2])) / 2;
		q.z = Mathf.Sqrt(Mathf.Max(0, 1 - m[0, 0] - m[1, 1] + m[2, 2])) / 2;
		q.x *= Mathf.Sign(q.x * (m[2, 1] - m[1, 2]));
		q.y *= Mathf.Sign(q.y * (m[0, 2] - m[2, 0]));
		q.z *= Mathf.Sign(q.z * (m[1, 0] - m[0, 1]));
		return q;
	}
	public static Quaternion SpaceRotation(Vector3 right, Vector3 up)
	{
		var forward = Vector3.Cross(right, up);
		up = Vector3.Cross(forward, right);
		Matrix4x4 M = new();
		M.SetColumn(0, right);
		M.SetColumn(1, up);
		M.SetColumn(2, forward);

		return QuaternionFromMatrix(M);
	}
	public static void RightAt(this Transform t, Vector3 right, Vector3 up)
	{
		var forward = Vector3.Cross(right,up);
		var newUp = Vector3.Cross(forward, right);
		t.LookAt(t.position + forward, newUp);
	}

	public static Quaternion Inverse(this Quaternion q)
		=> Quaternion.Inverse(q);
}