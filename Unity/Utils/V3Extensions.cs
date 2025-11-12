using UnityEngine;
public static class V3Extensions
{
    public static Vector3 SetX(this Vector3 v, float value) => new(value, v.y, v.z);
    public static Vector3 SetY(this Vector3 v, float value) => new(v.x, value, v.z);
    public static Vector3 SetZ(this Vector3 v, float value) => new(v.x, v.y, value);
    public static Vector3 Flat(this Vector3 v) => v.SetY(0);
    public static bool Approximately(this Vector3 v1, Vector3 v2)
        => MathUtils.Approximately(v1.x, v2.x)
        && MathUtils.Approximately(v1.y, v2.y)
        && MathUtils.Approximately(v1.z, v2.z);
    public static bool IsZero(this Vector3 v) => Approximately(v, Vector3.zero);
    public static Vector3 Mid(this Vector3 v1, Vector3 v2)
        => 0.5f * (v1 + v2);
    public static Vector3 GetClosestPointOnLine(this Vector3 point, Vector3 origin, Vector3 direction)
    {
        direction.Normalize();
        var result = Vector3.Dot(point - origin, direction) * direction + origin;
        return result;
    }
    public static Quaternion ToForwardRotation(this Vector3 dir)
        => Quaternion.LookRotation(dir);
    public static void Break(this Vector3 v, out Vector3 dir, out float magnitude)
    {
        magnitude = v.magnitude;
        dir = v.normalized;
    }
    public static Vector3 Unproject(this Vector3 v, Vector3 normal)
    {
        var n = normal.normalized;
        var result = v - Vector3.Dot(v, n) * n;
        return result;
    }
    public static Vector3 Mul(this Vector3 v1, Vector3 v2)
        => new(v1.x * v2.x, v1.y * v2.y, v1.z * v2.z);
    public static float FlatAngle(this Vector3 v1, Vector3 v2)
        => Mathf.Abs(Vector3.SignedAngle(v1, v2, Vector3.up));
}
