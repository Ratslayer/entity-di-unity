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
}
public static class EditorComponentExtensions
{
    public static void SetDirty(this UnityEngine.Object obj)
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.SetDirty(obj);
#endif
    }
#if UNITY_EDITOR
    public static void Undo(this UnityEngine.Object obj, string message)
    {
        UnityEditor.Undo.RecordObject(obj, message);
        obj.SetDirty();
    }
#endif
}