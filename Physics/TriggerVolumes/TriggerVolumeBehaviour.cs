using BB.Di;
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
		List<Entity> _enteredEntities = new();
		public override void Install(IDiContainer container)
		{
			base.Install(container);
			container.Event<TriggerVolumeEnterEvent>();
			container.Event<TriggerVolumeExitEvent>();
		}
		private void OnTriggerEnter(Collider other)
		{
			var entity = other.GetEntity();
			if (!entity || _enteredEntities.Contains(entity))
				return;

			_enteredEntities.Add(entity);
			if (enabled)
				_entered.Publish(new(entity));
		}
		private void OnTriggerExit(Collider other)
		{
			var entity = other.GetEntity();
			if (!entity|| !_enteredEntities.Contains(entity))
				return;

			_enteredEntities.Remove(entity);
			if(enabled)
				_exited.Publish(new(entity));
		}
		[OnEnable]
		void OnEnableEvent()
		{
			foreach (var entity in _enteredEntities)
				_entered.Publish(new(entity));
		}
		[OnDisable]
		void OnDisableEvent()
		{
			foreach (var entity in _enteredEntities)
				_exited.Publish(new(entity));
		}
	}
}