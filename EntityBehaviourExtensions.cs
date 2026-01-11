using UnityEngine;
using BB.Di;
namespace BB
{
	public static class EntityBehaviourExtensions
	{
		public static bool TryGetDirection(this Entity e1, Entity e2, out Vector3 dir)
		{
			if (e1.Has(out Root r1) && e2.Has(out Root r2))
			{
				dir = r2.Position - r1.Position;
				return true;
			}
			dir = default;
			return false;
		}
		public static bool TryGetDistance(this Entity e1, Entity e2, out float distance)
		{
			var result = TryGetDirection(e1, e2, out var dir);
			distance = dir.magnitude;
			return result;
		}
		public static bool HasEntityRedirect(this GameObject obj, out IEntityBehaviour bh)
		{
			bh = obj.TryGetComponent(out EntityRedirectBehaviour rbh) && rbh._redirectTo
				? rbh._redirectTo : null;
			return bh is not null;
		}
		public static bool HasEntityBehaviour(this GameObject obj, out IEntityBehaviour bh)
		{
			bh = default;
			if (!obj)
				return false;
			bh = obj.GetComponentInParent<IEntityBehaviour>();
			return bh is not null;
		}
		public static void Warp(this Entity entity, Vector3 pos)
		{
			if (entity.Has(out Root root))
				root.Position = pos;
		}
		public static bool HasEntityInParent(this GameObject obj, out Entity entity)
		{
			entity = default;
			if (!obj.HasEntityBehaviour(out var bh))
				return false;
			entity = bh.Entity;
			return true;
		}
		public static bool HasEntityInParent(this Component comp, out Entity entity)
		{
			entity = default;
			if (!comp)
				return false;
			return HasEntityInParent(comp.gameObject, out entity);
		}

		public static Entity GetEntityInParent(this Component comp)
			=> comp && comp.HasEntityInParent(out var entity) ? entity : default;
		public static Entity GetEntityInParent(this GameObject go)
			=> go && go.HasEntityInParent(out var entity) ? entity : default;
	}
}