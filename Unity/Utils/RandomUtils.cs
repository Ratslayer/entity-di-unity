using System.Collections.Generic;
using UnityEngine;

public static class RandomUtils
{
	public static float Range(float min, float max) => Random.Range(min, max);
	public static int Range(int min, int max) => Random.Range(min, max);
	public static T RandomElement<T>(this IList<T> list)
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
}
