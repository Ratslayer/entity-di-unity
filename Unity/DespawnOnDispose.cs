using UnityEngine;
using BB;
using System;

public sealed class DespawnOnDispose : PooledObject<DespawnOnDispose>
{
	public Component _component;
	public Entity _entity;
	public override void Dispose()
	{
		if (_entity)
			_entity.Despawn();
		else if (_component.HasEntity(out var entity))
			entity.Despawn();
		base.Dispose();
	}
}
public static class DespawnOnDisposeExtensions
{
	public static IDisposable GetDespawnDisposable(this Entity entity)
	{
		var result = DespawnOnDispose.GetPooled();
		result._entity = entity;
		result._component = null;
		return result;
	}
	public static IDisposable GetDespawnDisposable(this Component component)
	{
		var result = DespawnOnDispose.GetPooled();
		result._entity = default;
		result._component = component;
		return result;
	}
}