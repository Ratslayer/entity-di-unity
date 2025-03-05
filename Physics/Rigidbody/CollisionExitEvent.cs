using UnityEngine;
namespace BB
{
	public readonly struct CollisionExitEvent
	{
		public readonly Collision _collision;
		public readonly Entity _collidedEntity;
		public CollisionExitEvent(Collision collision)
		{
			_collision = collision;
			_collidedEntity = _collision.collider.GetEntity();
		}
	}
}