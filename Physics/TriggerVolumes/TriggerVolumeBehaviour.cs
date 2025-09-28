using BB.Di;
using Sirenix.OdinInspector;
using System.Collections.Generic;
using UnityEngine;
namespace BB
{
    public sealed class TriggerVolumeBehaviour : EntityBehaviour
    {
        [Inject]
        IEvent<TriggerVolumeEnterEvent> _entered;
        [Inject]
        IEvent<TriggerVolumeExitEvent> _exited;
        [ShowInInspector]
        readonly Dictionary<Entity, int> _enteredEntities = new();
        public override void Install(IDiContainer container)
        {
            base.Install(container);
            container.Event<TriggerVolumeEnterEvent>();
            container.Event<TriggerVolumeExitEvent>();
        }
        private void OnTriggerEnter(Collider other)
            => Enter(other);
        private void OnTriggerExit(Collider other)
            => Exit(other);
        [OnEnable]
        void OnEnableEvent()
        {
            foreach (var entity in _enteredEntities)
                _entered.Publish(new(entity.Key));
        }
        [OnDisable]
        void OnDisableEvent()
        {
            foreach (var entity in _enteredEntities)
                _exited.Publish(new(entity.Key));
        }
        public void Enter(Collider collider)
        {
            var entity = collider.GetEntity();
            if (!entity)
                return;

            var numEntrances = _enteredEntities.GetValueOrDefault(entity);
            _enteredEntities[entity] = numEntrances + 1;
            if (numEntrances == 0 && enabled)
                _entered.Publish(new(entity));
        }
        public void Exit(Collider collider)
        {
            var entity = collider.GetEntity();
            if (!entity)
                return;
            var numEntrances = _enteredEntities.GetValueOrDefault(entity);
            _enteredEntities[entity] = numEntrances - 1;
            if (numEntrances == 1 && enabled)
                _exited.Publish(new(entity));
        }
    }
}