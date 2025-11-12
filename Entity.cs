using UnityEngine;
namespace BB
{
	public readonly partial struct Entity
	{
		public Root Root => this.Require<Root>();
		public Transform Transform => this.Has(out Root root) ? root.Transform : null;
		public Vector3 Position => this.Has(out Root root) ? root.Position : Vector3.zero;
	}
}