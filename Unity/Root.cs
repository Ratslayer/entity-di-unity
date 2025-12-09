using BB;
using BB.Di;
using UnityEngine;
public abstract class BaseRoot
{
    public abstract void SetGameObject(GameObject go);
    protected GameObject GameObject { get; set; }
    public T GetComponent<T>() => GameObject.GetComponent<T>();

    #region Events
    [OnEvent]
    void OnEnable(EntityEnabledEvent _)
    {
        if (GameObject)
            GameObject.SetActive(true);
    }
    [OnEvent]
    void OnDisable(EndDragEvent _)
    {
        if (GameObject)
            GameObject.SetActive(false);
    }
    [OnEvent]
    void OnDespawn(EntityDespawnedEvent _)
    {
        if (!GameObject.TryGetComponent(out PooledGameObject pgo))
            return;
        SetGameObject(null);
        pgo.Despawn();
    }

    #endregion
}
public sealed class Root2D : BaseRoot
{
    public RectTransform Transform { get; private set; }
    public Transform Parent
    {
        get => Transform.parent;
        set => Transform.SetParent(value);
    }
    public override void SetGameObject(GameObject go)
    {
        Transform = go.GetComponent<RectTransform>();
    }
}
public sealed class Root : BaseRoot, ISerializableComponent
{
    public Transform Transform { get; private set; }
    public override void SetGameObject(GameObject go)
    {
        Transform = go.transform;
    }
    public IEntityComponentSerializer GetSerializer()
        => RootSerializerV1.Default;
    public float Scale
    {
        get => Transform.localScale.x;
        set => Transform.localScale = Vector3.one * value;
    }
    public Vector3 NonUniformScale
    {
        get => Transform.localScale;
        set => Transform.localScale = value;
    }
    public Vector3 Position
    {
        get => Transform.position;
        set => Transform.position = value;
    }
    public Quaternion Rotation
    {
        get => Transform.rotation;
        set => Transform.rotation = value;
    }
    public float Z
    {
        get => Position.z;
        set => Position = Position.SetZ(value);
    }
    public float Y
    {
        get => Position.y;
        set => Position = Position.SetY(value);
    }
    public Vector3 Forward
    {
        get => Transform.forward;
        set => Rotation = Quaternion.LookRotation(value);
    }
    public Transform Parent
    {
        get => Transform.parent;
        set => Transform.SetParent(value);
    }
    public Vector3 ForwardFlat => Forward.Flat();
    #region Operators
    public static implicit operator Vector3(Root root)
        => root.Transform.position;
    public static implicit operator Transform(Root root)
        => root?.Transform;
    public static implicit operator Quaternion(Root root)
        => root.Rotation;

    public static Vector3 operator -(Root l, Root r)
        => l.Position - r.Position;
    public static Vector3 operator -(Root root, in TransformAdapter adapter)
        => root.Position - adapter._transform.position;
    public static Vector3 operator -(in TransformAdapter adapter, Root root)
        => adapter._transform.position - root.Position;
    #endregion
}
public static class RootExtensions
{
    public static float DistanceTo(this Root root, Vector3 point)
        => Vector3.Distance(root.Position, point);
    public static Vector3 VectorTo(this Root root, Vector3 point)
        => point - root;
    public static Vector3 FlatVectorTo(this Root root, Vector3 point)
        => root.VectorTo(point).SetY(0);
    public static Vector3 FlatDirTo(this Root root, Vector3 point)
        => root.FlatVectorTo(point).normalized;

    public static float AngleToForward(this Root root, Vector3 dir)
        => Vector3.SignedAngle(root.Transform.forward, dir, Vector3.up);
}