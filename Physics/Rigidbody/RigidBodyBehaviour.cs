using BB.Di;
using UnityEngine;
namespace BB
{
	public interface IVelocityController
	{
		public Vector3 Velocity { get; set; }
		public Vector3 AngularVelocity { get; set; }
	}
	public sealed record RigidBodyVelocityController(Rigidbody Rb) : IVelocityController
	{
		public Vector3 Velocity
		{
			get => Rb.linearVelocity;
			set
			{
				var diff = value - Rb.linearVelocity;
				Rb.AddForce(diff, ForceMode.VelocityChange);
			}
		}

		public Vector3 AngularVelocity
		{
			get => Rb.angularVelocity;
			set => Rb.angularVelocity = value;
		}
	}
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
			container.BindCollision(GetComponent<Rigidbody>());
			container.System<IVelocityController, RigidBodyVelocityController>();
		}
		private void OnCollisionEnter(Collision collision)
		{
			_enter.Publish(new(collision));
		}
		private void OnCollisionExit(Collision collision)
		{
			_exit.Publish(new(collision));
		}
	}
}