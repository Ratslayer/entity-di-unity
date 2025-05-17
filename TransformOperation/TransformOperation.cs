using UnityEngine;
using static TransformArgsUsage;
namespace BB
{
	public readonly struct TransformOperation
	{
		public readonly Transform _parent;
		public readonly Vector3 _position, _scale;
		public readonly Quaternion _rotation;
		public readonly TransformArgsUsage _usage;

		#region Apply
		public void Apply(Transform transform)
		{
			if (transform)
				ApplyInternal(transform);
		}

		public void Apply(Component comp)
		{
			if (comp)
				ApplyInternal(comp.transform);
		}
		public void Apply(GameObject go)
		{
			if (go)
				ApplyInternal(go.transform);
		}
		public ApplyTransformOperationDisposable ApplyTemp(Transform transform)
		{
			if (!transform)
				return default;
			return ApplyTempInternal(transform);
		}
		public ApplyTransformOperationDisposable ApplyTemp(GameObject go)
		{
			if (!go)
				return default;
			return ApplyTempInternal(go.transform);
		}
		public ApplyTransformOperationDisposable ApplyTemp(Component comp)
		{
			if (!comp)
				return default;
			return ApplyTempInternal(comp.transform);
		}
		ApplyTransformOperationDisposable ApplyTempInternal(Transform transform)
		{
			var undoOperation = transform.ToTransformOperation();
			ApplyInternal(transform);
			return new(undoOperation, transform);
		}
		void ApplyInternal(Transform transform)
		{
			if (HasFlag(Parent))
				transform.SetParent(_parent, true);
			if (HasFlag(Additive))
			{
				if (HasFlag(Position))
					transform.position += _position;
				if (HasFlag(Rotation))
					transform.rotation *= _rotation;
			}
			else if (HasFlag(Local))
			{
				if (HasFlag(Position))
					transform.localPosition = _position;
				if (HasFlag(Rotation))
					transform.localRotation = _rotation;
			}
			else
			{
				if (HasFlag(Position))
					transform.position = _position;
				if (HasFlag(Rotation))
					transform.rotation = _rotation;
			}
			if (HasFlag(Scale))
				transform.localScale = _scale;
			if (HasFlag(Reset2D) && transform.TryGetComponent(out RectTransform rect))
			{
				rect.anchoredPosition = default;
				rect.sizeDelta = default;
			}
		}
		bool HasFlag(TransformArgsUsage usage)
			=> _usage.HasFlag(usage);
		#endregion

		#region Constructors
		public TransformOperation(Vector3 pos, Quaternion rot, Vector3 scale, Transform parent, TransformArgsUsage usage)
		{
			_parent = parent;
			_position = pos;
			_rotation = rot;
			_scale = scale;
			_usage = usage;
		}
		public TransformOperation(Vector3 pos, Quaternion rot, Transform parent = null)
			: this(pos, rot, default, parent, Position | Rotation | Parent)
		{

		}
		public TransformOperation(Vector3 pos, Quaternion rot, Vector3 scale, Transform parent = null)
			: this(pos, rot, scale, parent, Position | Rotation | Parent | Scale)
		{

		}
		public TransformOperation(Vector3 pos, Transform parent = null)
			: this(pos, Quaternion.identity, default, parent, Position | Rotation | Parent) { }
		public TransformOperation(Transform parent)
			: this(Vector3.zero, parent) { }
		#endregion

		public static TransformOperation ParentReset(Transform parent)
			=> new(parent.position, parent.rotation, parent);
		public static implicit operator TransformOperation(Transform parent) => new(parent);
		public static implicit operator TransformOperation(Vector3 position) => new(position);
	}
}