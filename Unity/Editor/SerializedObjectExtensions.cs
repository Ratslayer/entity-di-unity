using BB.Map;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BB
{
    public static class SerializedObjectExtensions
    {
        static readonly Dictionary<UnityEngine.Object, SerializedObject> _objects = new();
        public static SerializedObject GetSerializedObject(this UnityEngine.Object obj)
        {
            if (!_objects.TryGetValue(obj, out var serializedObject))
            {
                serializedObject = new SerializedObject(obj);
                _objects.Add(obj, serializedObject);
            }

            return serializedObject;
        }
        public static SerializedProperty GetSerializedProperty(this UnityEngine.Object obj, string path)
        {
            var so = obj.GetSerializedObject();
            return so.FindProperty(path);
        }
        public static void Draw(this SerializedProperty prop, string childProperty = null)
        {
            if (!string.IsNullOrWhiteSpace(childProperty))
                prop = prop.FindPropertyRelative(childProperty);
            EditorGUILayout.PropertyField(prop);
        }
        public static void DrawProperty(this UnityEngine.Object obj, params string[] elements)
        {
            var path = string.Join('/', elements);
            var property = obj.GetSerializedProperty(path);
            if (property is null)
                EditorGUILayout.LabelField($"Property '{path}' for object '{obj.name}' was not found.");
            else EditorGUILayout.PropertyField(property);
        }
        public static void EditName(this Object obj)
            => obj.name = EditorGUILayout.TextField("Name", obj.name);
    }
}