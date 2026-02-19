using UnityEngine;
namespace BB
{
    public enum TransformOperationSpace
    {
        Global = 0,
        Local = 1,
        Additive = 2
    }
    public readonly struct TransformOperation
    {
        public readonly Transform _parent;
        public readonly Vector3 _position;
        public readonly Vector3 _scale;
        public readonly Quaternion _rotation;
        public readonly TransformOperationUsage _usage;

        #region Properties
        public TransformOperationSpace Space { get; init; }
        public Transform Parent
        {
            init
            {
                _parent = value;
                _usage |= TransformOperationUsage.Parent;
            }
        }
        public Vector3Adapter Position
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
        public Vector3 Forward
        {
            init
            {
                Rotation = Quaternion.LookRotation(value);
            }
        }
        public bool DoNotDestroyOnLoad { get; init; }
        public TransformOperation CopySource
        {
            init
            {
                _parent = value._parent;
                _position = value._position;
                _scale = value._scale;
                _rotation = value._rotation;
                _usage = value._usage;
            }
        }
        public TransformAdapter CopyTransform
        {
            init
            {
                Position = value._transform.position;
                Rotation = value._transform.rotation;
                Scale = value._transform.localScale;
            }
        }
        #endregion
        #region Apply
        public void Apply(in TransformAdapter t)
        {
            if (!t)
                return;
            var transform = t._transform;
            if (DoNotDestroyOnLoad)
            {
                transform.SetParent(null);
                Object.DontDestroyOnLoad(transform.gameObject);
            }
            else if (HasFlag(TransformOperationUsage.Parent))
                transform.SetParent(_parent, true);
            switch (Space)
            {
                case TransformOperationSpace.Additive:
                    if (HasFlag(TransformOperationUsage.Position))
                        transform.localPosition += _position;
                    if (HasFlag(TransformOperationUsage.Rotation))
                        transform.localRotation *= _rotation;
                    break;
                case TransformOperationSpace.Local:
                    if (HasFlag(TransformOperationUsage.Position))
                        transform.localPosition = _position;
                    if (HasFlag(TransformOperationUsage.Rotation))
                        transform.localRotation = _rotation;
                    break;
                default:
                    if (HasFlag(TransformOperationUsage.Position))
                        transform.position = _position;
                    if (HasFlag(TransformOperationUsage.Rotation))
                        transform.rotation = _rotation;
                    break;
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
    }
}