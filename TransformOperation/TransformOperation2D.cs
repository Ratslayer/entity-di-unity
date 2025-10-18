using System;
using UnityEngine;
namespace BB
{
    [Flags]
    public enum AnchorSides
    {
        None = 0,
        Left = 1 << 0,
        Right = 1 << 1,
        Top = 1 << 2,
        Bottom = 1 << 3,
        All = Left | Right | Top | Bottom
    }
    public readonly struct TransformOperation2D
    {
        readonly TransformOperation2DUsage _usage;
        readonly Transform _parent;
        readonly Vector2 _sizeDelta, _anchoredPosition;
        readonly AnchorSides _sides;
        public TransformAdapter Parent
        {
            init
            {
                _parent = value._transform;
                _usage |= TransformOperation2DUsage.Parent;
            }
        }
        public Vector2 SizeDelta
        {
            init
            {
                _sizeDelta = value;
                _usage |= TransformOperation2DUsage.SizeDelta;
            }
        }
        public AnchorSides Anchors
        {
            init
            {
                _sides = value;
                _usage |= TransformOperation2DUsage.Anchors;
            }
        }
        public Vector2 AnchoredPosition
        {
            init
            {
                _anchoredPosition = value;
                _usage |= TransformOperation2DUsage.AnchoredPosition;
            }
        }
        public bool StretchToFull
        {
            init
            {
                Anchors = AnchorSides.All;
                SizeDelta = Vector2.zero;
                AnchoredPosition = Vector2.zero;
            }
        }
        public void Apply(in TransformAdapter ta)
        {
            if (!ta._transform.TryGetComponent(out RectTransform rt))
                return;
            if (_usage.HasFlag(TransformOperation2DUsage.Parent))
                rt.SetParent(_parent);
            if (_usage.HasFlag(TransformOperation2DUsage.SizeDelta))
                rt.sizeDelta = _sizeDelta;
            if (_usage.HasFlag(TransformOperation2DUsage.AnchoredPosition))
                rt.anchoredPosition = _anchoredPosition;
            if (_usage.HasFlag(TransformOperation2DUsage.Anchors))
            {
                float minX, minY, maxX, maxY, pivotX, pivotY;

                var l = _sides.HasFlag(AnchorSides.Left);
                var r = _sides.HasFlag(AnchorSides.Right);
                if (l && r)
                {
                    minX = 0;
                    maxX = 1;
                    pivotX = 0.5f;
                }
                else if (l)
                {
                    minX = 0;
                    maxX = 0;
                    pivotX = 0;
                }
                else if (r)
                {
                    minX = 1;
                    maxX = 1;
                    pivotX = 1;
                }
                else
                {
                    minX = 0.5f;
                    maxX = 0.5f;
                    pivotX = 0.5f;
                }

                var t = _sides.HasFlag(AnchorSides.Top);
                var b = _sides.HasFlag(AnchorSides.Bottom);
                if (t && b)
                {
                    minY = 0;
                    maxY = 1;
                    pivotY = 0.5f;
                }
                else if (t)
                {
                    minY = 1;
                    maxY = 1;
                    pivotY = 1;
                }
                else if (b)
                {
                    minY = 0;
                    maxY = 0;
                    pivotY = 0;
                }
                else
                {
                    minY = 0.5f;
                    maxY = 0.5f;
                    pivotY = 0.5f;
                }

                rt.anchorMin = new(minX, minY);
                rt.anchorMax = new(maxX, maxY);
                rt.pivot = new(pivotX, pivotY);
            }
        }
        #region Transformers
        public TransformOperation2D WithParent(in TransformAdapter parent)
            => new(parent._transform,
                _sizeDelta,
                _anchoredPosition,
                _sides,
                _usage | TransformOperation2DUsage.Parent);
        private TransformOperation2D(
            Transform parent,
            Vector2 sizeDelta,
            Vector2 anchoredPosition,
            AnchorSides sides,
            TransformOperation2DUsage usage)
        {
            _usage = usage;
            _parent = parent;
            _sizeDelta = sizeDelta;
            _anchoredPosition = anchoredPosition;
            _sides = sides;
        }
        #endregion
    }
}