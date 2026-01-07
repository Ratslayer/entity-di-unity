
using BB;
using BB.Di;
using UnityEngine;
public sealed class WorldUpdates : BaseComponent
{
    IEvent<UpdateEvent> _update;
    IEvent<LateUpdateEvent> _lateUpdate;
    IEvent<FixedUpdateEvent> _fixedUpdateEvent;
    public void SetEntity(IEntity entity)
    {
        _update = entity.Require<IEvent<UpdateEvent>>();
        _lateUpdate = entity.Require<IEvent<LateUpdateEvent>>();
        _fixedUpdateEvent = entity.Require<IEvent<FixedUpdateEvent>>();
    }
    private void Update()
    {
        _update.Publish(new()
        {
            Delta = Time.deltaTime,
            UnscaledDelta = Time.unscaledDeltaTime,
        });
    }
    private void FixedUpdate()
    {
        _fixedUpdateEvent.Publish(new()
        {
            Delta = Time.fixedDeltaTime,
            UnscaledDelta = Time.fixedUnscaledDeltaTime,
        });
    }
    private void LateUpdate()
    {
        _lateUpdate.Publish(new()
        {
            Delta = Time.deltaTime,
            UnscaledDelta = Time.unscaledDeltaTime,
        });
    }
}
