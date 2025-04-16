using UnityEngine;
public sealed record Root(Transform Transform)
{
	[OnEnable]
	void OnEnable()
	{
		Transform.gameObject.SetActive(true);
	}
	[OnDisable]
	void OnDisable()
	{
		Transform.gameObject.SetActive(false);
	}
	public static implicit operator Vector3(Root root) 
		=> root.Transform.position;
	public float Scale
	{
		get => Transform.localScale.x;
		set => Transform.localScale = Vector3.one * value;
	}
	public Vector3 Position
	{
		get => Transform.position;
		set => Transform.position = value;
	}
	public Quaternion Rotation
	{
		get => Transform.rotation;
		set => Transform.rotation = value;
	}
	public float Z
	{
		get => Position.z;
		set => Position = Position.SetZ(value);
	}
	public float Y
	{
		get => Position.y;
		set => Position = Position.SetY(value);
	}
	public static implicit operator Transform(Root root) => root?.Transform;
}
public static class RootExtensions
{
	public static float Distance(this Root root, Vector3 point)
		=> Vector3.Distance(root.Position, point);
}