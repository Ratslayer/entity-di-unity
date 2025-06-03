using BB.Di;
using UnityEngine;
namespace BB
{
	public static class CollisionExtensions
	{
		public static void BindCollision(this IDiContainer container, Rigidbody rb = null)
		{
			if (rb)
				container.Instance(rb);
			container.Event<CollisionEnterEvent>();
			container.Event<CollisionExitEvent>();
		}
	}
}