using UnityEngine;

public readonly struct RaycastArgs
{
	public readonly Vector3 _direction;
	public readonly float _distance;
	public readonly int _layerMask;
	public readonly QueryTriggerInteraction _triggerInteraction;
	public RaycastArgs(
		Vector3 direction,
		float distance,
		int layerMask = Physics.AllLayers,
		QueryTriggerInteraction trigger = QueryTriggerInteraction.Ignore)
	{
		_direction = direction;
		_distance = distance;
		_layerMask = layerMask;
		_triggerInteraction = trigger;
	}
	public RaycastArgs(
		Vector3 vector,
		int layerMask = Physics.AllLayers,
		QueryTriggerInteraction trigger = QueryTriggerInteraction.Ignore)
		: this(vector, vector.magnitude, layerMask, trigger)
	{

	}
}
