using System.Collections.Generic;
using UnityEngine;

public static class HashUtils
{
    public static float HashToFloat(this object obj)
    {
        if (obj == null) return 0f;

        int hash = obj.GetHashCode();

        var result = Mathf.Abs(hash % 10000) / 10000f;
        return result;
    }
}

public static class RandomUtils
{
    public static float Range(float min, float max) => Random.Range(min, max);
    public static int Range(int min, int max) => Random.Range(min, max);
    public static double Range(double min, double max) => Random.Range((float)min, (float)max);
    public static float RandomAngle() => Range(0f, 360f);
    public static T RandomElement<T>(this IReadOnlyList<T> list)
        => list.Count switch
        {
            0 => default,
            1 => list[0],
            _ => list[Range(0, list.Count)]
        };
    public static float Value => Range(0f, 1f);
    public static Color Color => Random.ColorHSV(0, 1, 1, 1, 1, 1, 1, 1);
    public static bool Roll(double chance)
        => Value <= chance;
    public static float Angle => Range(0, 360);
    public static Vector2 Direction2D
    {
        get
        {
            var angle = Angle;
            return new(Mathf.Cos(angle), Mathf.Sin(angle));
        }
    }
}
