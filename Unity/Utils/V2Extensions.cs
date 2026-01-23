using UnityEngine;
public static class V2Extensions
{
    public static Vector2 SetX(this Vector2 v, float value) => new(value, v.y);
    public static Vector2 SetY(this Vector2 v, float value) => new(v.x, value);
    public static bool Approximately(this Vector2 v1, Vector2 v2)
        => MathUtils.Approximately(v1.x, v2.x)
        && MathUtils.Approximately(v1.y, v2.y);
    public static bool IsZero(this Vector2 v) => Approximately(v, Vector2.zero);
    public static Vector2 Mid(this Vector2 p1, Vector2 p2)
        => 0.5f * (p1 + p2);
    public static Vector2 Mul(this Vector2 v1, Vector2 v2)
        => new(v1.x * v2.x, v1.y * v2.y);
    public static Vector2 FlipY(this Vector2 v)
        => new(v.x, -v.y);
}
