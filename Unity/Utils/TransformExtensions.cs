using System.Collections.Generic;
using UnityEngine;
namespace BB
{
	public static class TransformExtensions
	{
		public static void DestroyAllChildren(this Transform t)
		{
			for (var i = t.childCount - 1; i >= 0; i--)
			{
				var child = t.GetChild(i);
				if (!child)
					continue;
				child.gameObject.Destroy();
			}
		}
		public static bool IsChild(this Transform t, Transform child, out int index)
		{
			foreach (var i in t.childCount)
				if (t.GetChild(i) == child)
				{
					index = i;
					return true;
				}
			index = -1;
			return false;
		}
		public static int GetSelfChildId(this Transform t)
			=> t.parent && t.parent.IsChild(t, out var index) ? index : 0;
		public static void ResetData(this Transform t, bool resetScale = true)
		{
			t.localPosition = Vector3.zero;
			t.localRotation = Quaternion.identity;
			if (resetScale)
				t.localScale = Vector3.one;
		}
		public static void CopyData(this Transform t, Transform target)
		{
			if (!target)
				return;
			t.SetPositionAndRotation(target.position, target.rotation);
			t.localScale = target.localScale;
		}
		public static IEnumerable<Transform> GetChildren(this Transform t)
		{
			for (var i = t.childCount - 1; i >= 0; i--)
			{
				yield return t.GetChild(i);
			}
		}
		public static float Distance(this Transform t, in Vector3Adapter p)
			=> Vector3.Distance(t.position, p._vector);
		public static void SetY(this Transform t, float y)
			=> t.position = t.position.SetY(y);

		public static bool IsInHierarchyOf(this Transform parent, Transform t)
		{
			if (!parent || !t)
				return false;
			if (parent == t)
				return true;
			return parent.IsInHierarchyOf(t.parent);
		}
	}
}