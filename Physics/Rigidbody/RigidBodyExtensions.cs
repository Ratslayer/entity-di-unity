using UnityEngine;
namespace BB
{
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
		public static void ExcludeAllLayers(this Rigidbody rb)
			=> rb.excludeLayers = ~0;
		public static void RemoveExcludeAllLayers(this Rigidbody rb)
			=> rb.excludeLayers = 0;
	}
}