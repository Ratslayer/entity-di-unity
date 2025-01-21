using UnityEngine;

public static class CameraUtils
{
	static Camera _main;
	public static Camera Main
	{
		get
		{
			if (!_main)
			{
				_main = Camera.main;
				Log.Logger.Assert(
					_main,
					$"There is no main camera (camera with MainCamera tag)");
			}
			return _main;
		}
	}
	public static Ray GetMouseRay() => Main.ScreenPointToRay(Input.mousePosition);
	public static Vector3 GetMouseWorldPos(float z)
	{
		var ray = GetMouseRay();
		if (!new Plane(Vector3.back, z).Raycast(ray, out var enter))
			return Vector3.zero;
		return ray.GetPoint(enter);
	}
	public static Vector3 PointToScreen(Vector3 point) => Main.WorldToScreenPoint(point);
}
