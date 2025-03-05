using UnityEngine;
namespace BB
{
	public readonly struct CollisionEnterEvent
	{
		public readonly Collision _collision;
		public readonly Entity _collidedEntity;
		public CollisionEnterEvent(Collision collision)
		{
			_collision = collision;
			_collidedEntity = _collision.collider.GetEntity();
		}
	}
}