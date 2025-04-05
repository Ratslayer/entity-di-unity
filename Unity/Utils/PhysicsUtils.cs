using System;
using System.Collections.Generic;
using UnityEngine;
namespace BB
{
	public static class PhysicsUtils
	{
		readonly static RaycastHit[] _raycastHits = new RaycastHit[30];
		readonly static List<RaycastHit> _hitList = new();
		public static int ToLayerMask(int layerId)
			=> 1 << layerId;
		public static bool HasEntity(this RaycastHit hit, out Entity entity)
			=> hit.collider.HasEntity(out entity);
		public static Vector3 GetPosition(this SphereCollider collider)
			=> collider.transform.TransformPoint(collider.center);
		public static Vector3 GetPosition(this BoxCollider collider)
			=> collider.transform.TransformPoint(collider.center);
		public static int RaycastAll(Vector3 origin, in RaycastArgs args, out RaycastHit[] hits)
		{
			var numHits = Physics.RaycastNonAlloc(origin, args._direction, _raycastHits, args._distance, args._layerMask, args._triggerInteraction);
			hits = _raycastHits;
			return numHits;
		}
		public static bool RaycastInParent<T>(Vector3 origin, in RaycastArgs args, out T comp)
		{
			var numHits = RaycastAll(origin, args, out var hits);
			for (var i = 0; i < numHits; i++)
				if (hits[i].collider.TryGetComponentInParent(out comp))
					return true;
			comp = default;
			return false;
		}
		public static bool Raycast(Vector3 origin, in RaycastArgs args, out RaycastHit hit)
		{
			return Physics.Raycast(
				origin, args._direction, out hit,
				args._distance, args._layerMask, args._triggerInteraction);
		}
		public static bool CapsuleCast(CapsuleCollider collider, float skinWidth, in RaycastArgs args, out RaycastHit hit)
		{
			var r = collider.radius + skinWidth;
			var h = collider.height + 2 * skinWidth;
			var hh = Mathf.Max(0, h * 0.5f - r) * collider.transform.up;
			var c = collider.transform.TransformPoint(collider.center) - args._direction.normalized * skinWidth;
			return CapsuleCast(c + hh, c - hh, collider.radius, args, out hit);
		}
		public static bool CapsuleCast(Vector3 bottom, float height, float radius, float skinWidth, in RaycastArgs args, out RaycastHit hit)
		{
			var cargs = CapsuleUtils.GetVerticalCapsuleArgs(bottom, height, radius, skinWidth);
			return CapsuleCast(cargs.P1, cargs.P2, cargs.Radius, args, out hit);
		}
		public static bool CapsuleCast(Vector3 p1, Vector3 p2, float radius, in RaycastArgs args, out RaycastHit hit)
		{
			return Physics.CapsuleCast(
				p1, p2,
				radius, args._direction,
				out hit, args._distance, args._layerMask, args._triggerInteraction);
		}
		public static bool SphereCast(
			Vector3 point,
			float radius,
			in RaycastArgs args,
			out RaycastHit hit)
			=> Physics.SphereCast(point, radius, args._direction,
				out hit,
				args._distance, args._layerMask, args._triggerInteraction);
		public static List<RaycastHit> SphereCastAll(Vector3 point,
			float radius,
			in RaycastArgs args)
		{
			var numHits = Physics.SphereCastNonAlloc(
				point, radius,
				args._direction,
				_raycastHits,
				args._distance,
				args._layerMask,
				args._triggerInteraction);
			_hitList.Clear();
			for (var i = 0; i < numHits; i++)
				_hitList.Add(_raycastHits[i]);
			return _hitList;
		}
		public static List<(T component, RaycastHit hit)> RaycastAll<T>(
			Vector3 point,
			Vector3 dir,
			float distance,
			int layerMask,
			QueryTriggerInteraction triggerInteraction)
		{
			var numHits = Physics.RaycastNonAlloc(point, dir, _raycastHits, distance, layerMask, triggerInteraction);
			var result = new List<(T component, RaycastHit hit)>();
			for (var i = 0; i < numHits; i++)
				if (_raycastHits[i].collider.TryGetComponent(out T comp))
					result.Add((comp, _raycastHits[i]));
			return result;
		}
		public static bool BoxCast(
			BoxCollider collider,
			float skinWidth,
			in RaycastArgs args,
			out RaycastHit hit)
			=> BoxCast(
				GetPosition(collider),
				(collider.size - Vector3.one * skinWidth) * 0.5f,
				collider.transform.rotation,
				in args,
				out hit);
		public static bool BoxCast(
			Vector3 origin,
			Vector3 halfExtents,
			Quaternion rotation,
			in RaycastArgs args,
			out RaycastHit hit)
		=> Physics.BoxCast(
				origin,
				halfExtents,
				args._direction,
				out hit,
				rotation,
				args._distance,
				args._layerMask,
				args._triggerInteraction);

	}
}