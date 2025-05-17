using UnityEngine;
using Sirenix.OdinInspector;
using System;
using BB;
[HideMonoScript]
public abstract class BaseBehaviour : MonoBehaviour, IDisposable
{
	public void Dispose()
	{
		if (this.HasEntity(out var entity))
			entity.Despawn();
		else Destroy(gameObject);
	}

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
