using UnityEngine;
namespace BB
{
	public readonly struct Movement
	{
		readonly Vector3 _velocity, _angularVelocity;

		public Movement(Vector3 velocity, Vector3 angularVelocity = default)
		{
			_velocity = velocity;
			_angularVelocity = angularVelocity;
		}

		public void Apply(Rigidbody rb)
		{
			rb.linearVelocity = _velocity;
			rb.angularVelocity = _angularVelocity;
		}

		public static implicit operator Movement(Vector3 v)
			=> new(v);
	}
	public static class MovementExtensions
	{
		public static void Apply(this Movement movement, in Entity entity)
		{
			if (entity.Has(out Rigidbody rb))
				movement.Apply(rb);
		}
	}
}