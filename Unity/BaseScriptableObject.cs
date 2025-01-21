using Sirenix.OdinInspector;
using UnityEngine;
[HideMonoScript]
public abstract class BaseScriptableObject : ScriptableObject 
{
	public void Dirty()
	{
#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(this);
#endif
	}
}
