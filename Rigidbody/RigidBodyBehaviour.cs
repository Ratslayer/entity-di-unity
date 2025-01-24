using BB.Di;
using UnityEngine;
namespace BB
{
	[RequireComponent(typeof(Rigidbody))]
	public sealed class RigidBodyBehaviour : EntityBehaviour
	{
		[Inject]
		IEvent<CollisionEnterEvent> _enter;
		[Inject]
		IEvent<CollisionExitEvent> _exit;
		public override void Install(IDiContainer container)
		{
			base.Install(container);
			container.Instance(GetComponent<Rigidbody>());
			container.Event<CollisionEnterEvent>();
			container.Event<CollisionExitEvent>();
		}
		private void OnCollisionEnter(Collision collision)
		{
			_enter.Raise(new(collision));
		}
		private void OnCollisionExit(Collision collision)
		{
			_enter.Raise(new(collision));
		}
	}
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
	public static class RigidBodyExtensions
	{
		public static void Push(this Entity entity, Vector3 force, ForceMode forceMode = ForceMode.Impulse)
		{
			if (entity.Has(out Rigidbody rb))
				rb.AddForce(force, forceMode);
		}
		public static void SetRbVelocity(this Entity entity, Vector3 velocity, Vector3 angularVelocity)
		{
			if (entity.Has(out Rigidbody rb))
			{
				rb.linearVelocity = velocity;
				rb.angularVelocity = angularVelocity;
			}
		}
	}
}