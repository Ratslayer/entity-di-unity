using BB.Di;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
namespace BB
{
    public interface IUnityEntityComponent
    {
        IEntity Entity { get; set; }
    }
    public abstract class BaseUnityComponent : BaseComponent, IUnityEntityComponent
    {
       public IEntity Entity { get; set; }
    }
    public sealed class TriggerVolumeBehaviour : BaseUnityComponent
    {
        //[Inject] IEvent<TriggerVolumeEnterEvent> _entered;
        //[Inject] IEvent<TriggerVolumeExitEvent> _exited;
        [ShowInInspector]
        readonly Dictionary<Entity, int> _enteredEntities = new();
        //public override void Install(IDiContainer container)
        //{
        //    base.Install(container);
        //    container.Event<TriggerVolumeEnterEvent>();
        //    container.Event<TriggerVolumeExitEvent>();
        //}
        private void OnTriggerEnter(Collider other)
            => Enter(other);
        private void OnTriggerExit(Collider other)
            => Exit(other);
		private void OnEnable()
		{
            foreach (var entity in _enteredEntities)
                PublishEnter(entity.Key);
        }
		private void OnDisable()
        {
            foreach (var entity in _enteredEntities)
                PublishExit(entity.Key);
        }
        public void Enter(Collider collider)
        {
            var entity = collider.GetEntity();
            if (!entity)
                return;

            var numEntrances = _enteredEntities.GetValueOrDefault(entity);
            _enteredEntities[entity] = numEntrances + 1;
            if (numEntrances == 0 && enabled)
                PublishEnter(entity);
        }
        public void Exit(Collider collider)
        {
            var entity = collider.GetEntity();
            if (!entity)
                return;
            var numEntrances = _enteredEntities.GetValueOrDefault(entity);
            _enteredEntities[entity] = numEntrances - 1;
            if (numEntrances == 1 && enabled)
                PublishExit(entity);
        }
        private void PublishEnter(Entity entity)
        {
            Entity.Publish(new TriggerVolumeEnterEvent(entity));
        }
        private void PublishExit(Entity entity)
        {
            Entity.Publish(new TriggerVolumeExitEvent(entity));
        }
    }
}