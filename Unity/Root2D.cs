using UnityEngine;
namespace BB
{
    public sealed class Root2D : BaseRoot
    {
        public RectTransform Transform { get; private set; }
        public CanvasGroup CanvasGroup { get; private set; }
        public Transform Parent
        {
            get => Transform.parent;
            set => Transform.SetParent(value);
        }
        public override void Init()
        {
            Transform = GameObject.GetComponent<RectTransform>();
            CanvasGroup = GameObject.GetOrAdd<CanvasGroup>();
        }
        public override void Clear()
        {
            Transform = null;
        }
        public Vector2 ScreenToLocalPos(Vector2 pos)
            => pos - new Vector2(Transform.rect.width, Transform.rect.height) * 0.5f;
    }
}