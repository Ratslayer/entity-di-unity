using Sirenix.OdinInspector;
using UnityEngine;
[HideMonoScript]
public abstract class BaseScriptableObject : SerializedScriptableObject 
{
	public void Dirty()
	{
#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(this);
#endif
	}
}
