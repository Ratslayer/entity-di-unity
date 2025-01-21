using System.Collections.Generic;
using UnityEngine;

public static class TransformUtils
{
	public static void DestroyAllChildren(this Transform t)
	{
		for (var i = t.childCount - 1; i >= 0; i--)
		{
			var child = t.GetChild(i);
			child.gameObject.Destroy();
		}
	}
	public static bool IsChild(this Transform t, Transform child, out int index)
	{
		foreach(var i in t.childCount)
			if(t.GetChild(i)==child)
			{
				index = i;
				return true;
			}
		index = -1;
		return false;
	}
	public static int GetSelfChildId(this Transform t)
		=> t.parent && t.parent.IsChild(t, out var index) ? index : 0;
	public static void ResetData(this Transform t)
	{
		t.localPosition = Vector3.zero;
		t.localRotation = Quaternion.identity;
		t.localScale = Vector3.one;
	}
	public static IEnumerable<Transform> GetChildren(this Transform t)
	{
		for (var i = t.childCount - 1; i >= 0; i--)
		{
			yield return t.GetChild(i);
		}
	}
	public static float Distance(this Transform t, Transform t2)
		=> Vector3.Distance(t.position, t2.position);
	public static float Distance(this Transform t, Vector3 p)
		=> Vector3.Distance(t.position, p);
}
