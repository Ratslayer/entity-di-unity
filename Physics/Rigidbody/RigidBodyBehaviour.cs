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
			_enter.Publish(new(collision));
		}
		private void OnCollisionExit(Collision collision)
		{
			_exit.Publish(new(collision));
		}
	}
}