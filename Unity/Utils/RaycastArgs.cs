using BB;
using UnityEngine;

public struct Raycast
{
    private enum RaycastMode
    {
        Line,
        Sphere,
        Box,
        Capsule
    }

    private Vector3 _origin, _direction, _origin2, _halfExtents;
    private Quaternion _orientation;
    private float _distance, _radius;
    private int _layerMask;
    private QueryTriggerInteraction _triggerInteraction;
    private RaycastMode _mode;

    public bool Hits(out RaycastHit hit)
    {
        switch (_mode)
        {
            case RaycastMode.Line:
                return Physics.Raycast(
                    _origin,
                    _direction,
                    out hit,
                    _distance,
                    _layerMask,
                    _triggerInteraction);
            case RaycastMode.Sphere:
                return Physics.SphereCast(
                    _origin,
                    _radius,
                    _direction,
                    out hit,
                    _distance,
                    _layerMask,
                    _triggerInteraction);
            case RaycastMode.Capsule:
                return Physics.CapsuleCast(
                    _origin,
                    _origin2,
                    _radius,
                    _direction,
                    out hit,
                    _distance,
                    _layerMask,
                    _triggerInteraction);
            case RaycastMode.Box:
                return Physics.BoxCast(
                    _origin,
                    _halfExtents,
                    _direction,
                    out hit,
                    _orientation,
                    _distance,
                    _layerMask,
                    _triggerInteraction);
            default:
                hit = default;
                return false;
        }
    }

    public bool Hits() => Hits(out _);

    public static Raycast Line(Vector3Adapter from)
        => new()
        {
            _mode = RaycastMode.Line,
            _origin = from
        };

    public Raycast WithDir(Vector3Adapter direction, float maxDistance)
    {
        _direction = direction;
        _distance = maxDistance;
        return this;
    }

    public Raycast ToPoint(Vector3Adapter point)
    {
        var dir = point - _origin;
        _direction = dir.normalized;
        _distance = dir.magnitude;
        return this;
    }

    public Raycast WithLayers(int layerMask)
    {
        _layerMask = layerMask;
        return this;
    }
}

public readonly struct OverlapArgs
{
    public int LayerMask { get; init; }
    public QueryTriggerInteraction Trigger { get; init; }
}
public readonly struct RaycastArgs
{
    public readonly Vector3 _direction;
    public readonly float _distance;
    public readonly int _layerMask;
    public readonly QueryTriggerInteraction _triggerInteraction;

    public RaycastArgs(
        Vector3 direction,
        float distance,
        int layerMask = Physics.AllLayers,
        QueryTriggerInteraction trigger = QueryTriggerInteraction.Ignore)
    {
        _direction = direction;
        _distance = distance;
        _layerMask = layerMask;
        _triggerInteraction = trigger;
    }

    public RaycastArgs(
        Vector3 vector,
        int layerMask = Physics.AllLayers,
        QueryTriggerInteraction trigger = QueryTriggerInteraction.Ignore)
        : this(vector, vector.magnitude, layerMask, trigger)
    {
    }

    public static RaycastArgs FromTo(
        Vector3 from,
        Vector3 to,
        int layerMask = Physics.AllLayers,
        QueryTriggerInteraction trigger = QueryTriggerInteraction.Ignore)
    {
        var dir = to - from;
        return new RaycastArgs(dir.normalized, dir.magnitude, layerMask, trigger);
    }
}