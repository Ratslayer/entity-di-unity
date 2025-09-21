using UnityEngine;
namespace BB
{
	public sealed class TriggerVolumeReceiver : MonoBehaviour
	{
		TriggerVolumeBehaviour _root;
		private void Awake()
		{
			if (this.TryGetComponentInParent(out _root))
				return;
			enabled = false;
			Log.Error($"{name} does not have {nameof(TriggerVolumeBehaviour)}");
		}
		private void OnTriggerEnter(Collider other)
			=> _root.Enter(other);
		private void OnTriggerExit(Collider other)
			=> _root.Exit(other);
	}
}