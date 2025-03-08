using Sirenix.OdinInspector;
using UnityEngine;
[HideMonoScript]
public abstract class BaseBehaviour : MonoBehaviour
{
	public void SetDirty()
	{
#if UNITY_EDITOR
		UnityEditor.EditorUtility.SetDirty(this);
#endif
	}
#if UNITY_EDITOR
	public void Undo(string message)
	{
		UnityEditor.Undo.RecordObject(this, message);
		SetDirty();
	}
#endif
}
