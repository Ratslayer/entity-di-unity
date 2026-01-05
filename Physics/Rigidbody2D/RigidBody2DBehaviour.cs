using BB.Di;
using UnityEngine;
namespace BB
{
	[RequireComponent(typeof(Rigidbody2D))]
	public sealed class RigidBody2DBehaviour : EntityComponent3D
	{
		private void OnCollisionEnter2D(Collision2D collision)
		{
		}
	}
}