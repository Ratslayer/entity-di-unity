using UnityEngine;
using BB.Di;
namespace BB
{
	public readonly partial struct Entity
	{
		public Transform Root => this.Has(out Root root) ? root.Transform : null;
		public Vector3 Position => this.Has(out Root root) ? root.Position : Vector3.zero;
	}
}