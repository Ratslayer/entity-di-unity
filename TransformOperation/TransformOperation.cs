using UnityEngine;
namespace BB
{
    public readonly struct TransformOperation
    {
        public readonly Transform _parent;
        public readonly Vector3 _position;
        public readonly Vector3 _scale;
        public readonly Quaternion _rotation;
        public readonly TransformOperationUsage _usage;
        #region Properties
        public Transform Parent
        {
            init
            {
                _parent = value;
                _usage |= TransformOperationUsage.Parent;
            }
        }
        public Vector3 Position
        {
            init
            {
                _position = value;
                _usage |= TransformOperationUsage.Position;
            }
        }
        public RotationAdapter Rotation
        {
            init
            {
                _rotation = value._rotation;
                _usage |= TransformOperationUsage.Rotation;
            }
        }
        public ScaleAdapter Scale
        {
            init
            {
                _scale = value._scale;
                _usage |= TransformOperationUsage.Scale;
            }
        }
        public bool Local
        {
            init
            {
                if (value)
                    _usage |= TransformOperationUsage.Local;
            }
        }
        #endregion
        #region Constructors
        public TransformOperation(
            Vector3 pos,
            in RotationAdapter rot,
            in ScaleAdapter scale,
            Transform parent,
            TransformOperationUsage usage)
        {
            _position = pos;
            _rotation = rot;
            _scale = scale;
            _parent = parent;
            _usage = usage;
        }
        #endregion
        #region Apply
        public void Apply(in TransformAdapter t)
        {
            if (!t)
                return;
            var transform = t._transform;
            if (HasFlag(TransformOperationUsage.Parent))
                transform.SetParent(_parent, true);
            if (HasFlag(TransformOperationUsage.Additive))
            {
                if (HasFlag(TransformOperationUsage.Position))
                    transform.position += _position;
                if (HasFlag(TransformOperationUsage.Rotation))
                    transform.rotation *= _rotation;
            }
            else if (HasFlag(TransformOperationUsage.Local))
            {
                if (HasFlag(TransformOperationUsage.Position))
                    transform.localPosition = _position;
                if (HasFlag(TransformOperationUsage.Rotation))
                    transform.localRotation = _rotation;
            }
            else
            {
                if (HasFlag(TransformOperationUsage.Position))
                    transform.position = _position;
                if (HasFlag(TransformOperationUsage.Rotation))
                    transform.rotation = _rotation;
            }
            if (HasFlag(TransformOperationUsage.Scale))
                transform.localScale = _scale;
        }
        public ApplyTransformOperationDisposable ApplyTemp(in TransformAdapter transform)
        {
            if (!transform)
                return default;
            var undoOperation = transform._transform.ToTransformOperation();
            Apply(transform);
            return new(undoOperation, transform._transform);
        }
        bool HasFlag(TransformOperationUsage usage)
            => _usage.HasFlag(usage);
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
        #endregion
    }
}