using UnityEngine;

public sealed record Root(Transform Transform)
{
	[OnSpawn]
	void OnSpawn()
	{
		Transform.gameObject.SetActive(true);
	}
	[OnDespawn]
	void OnDespawn()
	{
		if (!Transform)
			return;
		Transform.gameObject.SetActive(false);
	}
	public static implicit operator Vector3(Root root) => root.Transform.position;
	public Vector3 Position
	{
		get => Transform.position;
		set => Transform.position = value;
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