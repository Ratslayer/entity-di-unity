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
        public void Apply(in TransformAdapter t)
        {
            if (!t)
                return;
            var transform = t._transform;
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
        public ApplyTransformOperationDisposable ApplyTemp(in TransformAdapter transform)
        {
            if (!transform)
                return default;
            var undoOperation = transform._transform.ToTransformOperation();
            Apply(transform);
            return new(undoOperation, transform._transform);
        }
        bool HasFlag(TransformArgsUsage usage)
            => _usage.HasFlag(usage);
        #endregion

        #region Constructors
        public TransformOperation(Vector3 pos, RotationAdapter rot, ScaleAdapter scale, Transform parent, TransformArgsUsage usage)
        {
            _parent = parent;
            _position = pos;
            _rotation = rot._rotation;
            _scale = scale;
            _usage = usage;
        }
        public TransformOperation(Vector3 pos, RotationAdapter rot, Transform parent = null)
            : this(pos, rot, default, parent, Position | Rotation | Parent)
        {

        }
        public TransformOperation(Vector3 pos, float scale, Transform parent = null)
            : this(pos, default, Vector3.one * scale, parent, Position | Parent | Scale)
        {
        }
        public TransformOperation(Vector3 pos, RotationAdapter rot, ScaleAdapter scale, Transform parent = null)
            : this(pos, rot, scale, parent, Position | Rotation | Parent | Scale)
        {
        }
        public TransformOperation(Vector3 pos, Transform parent = null)
            : this(pos, Quaternion.identity, default, parent, Position | Rotation | Parent) { }
        public TransformOperation(Transform parent)
            : this(Vector3.zero, parent) { }
        #endregion

        #region Operators
        public static TransformOperation ParentReset(Transform parent)
            => new(parent.position, parent.rotation, parent);
        public static implicit operator TransformOperation(Transform parent) => new(parent);
        public static implicit operator TransformOperation(Vector3 position) => new(position);
        #endregion

        #region Methods
        public TransformOperation Add(Vector3 pos, RotationAdapter rot, ScaleAdapter scale)
        {
            return new(
                _position + pos,
                rot._rotation * _rotation,
                _scale.Mul(scale),
                _parent,
                _usage);
        }

        public TransformOperation WithPos(Vector3 pos)
            => new(pos, _rotation, _scale, _parent, _usage | Position);

        public TransformOperation WithParent(Transform t)
            => new(_position, _rotation, _scale, t, _usage | Parent);
        #endregion
    }
}