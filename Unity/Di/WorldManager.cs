
using BB.Di;
using UnityEngine;
public sealed class WorldManager : BaseBehaviour
{
	IEntityLifecycle _entity;
	public void SetLifecycle(IEntityLifecycle lifecycle)
	{
		_entity = lifecycle;
	}
	private void Update()
	{
		_entity.Update(new(
			Time.deltaTime,
			Time.unscaledDeltaTime));
	}
	private void FixedUpdate()
	{
		_entity.FixedUpdate(new(
			Time.fixedDeltaTime,
			Time.fixedUnscaledTime));
	}
	private void LateUpdate()
	{
		_entity.LateUpdate(new(
			Time.deltaTime,
			Time.unscaledDeltaTime));
	}
}
