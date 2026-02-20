using UnityEngine;
using Sirenix.OdinInspector;
using System;
using BB;
[HideMonoScript]
public abstract class BaseComponent : MonoBehaviour, IDisposable
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
    public static void RegisterUndo(this UnityEngine.Object obj, string message = null)
    {
        UnityEditor.Undo.RecordObject(obj, message);
        obj.SetDirty();
    }
    public static void UndoCreation(this UnityEngine.Object obj, string message = null)
    {
        UnityEditor.Undo.RegisterCreatedObjectUndo(obj, message);
    }
#endif
}