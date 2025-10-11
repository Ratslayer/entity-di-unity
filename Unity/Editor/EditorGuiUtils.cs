using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BB
{
    public static class EditorGuiUtils
    {
        public static void Title(string name)
            => EditorGUILayout.LabelField(name);
        public static string Dropdown(string element, string[] options, params GUILayoutOption[] lops)
        {
            var index = Math.Max(0, Array.IndexOf(options, element));
            var newIndex = EditorGUILayout.Popup(index, options, lops);
            return options[newIndex];
        }
        public static string Dropdown<T>(string element, List<T> list, Func<T, string> getName, params GUILayoutOption[] lops)
        {
            var options = list.Select(getName).ToArray();
            return Dropdown(element, options, lops);
        }
        public static T ObjectField<T>(string name, T obj, bool allowSceneObjects, params GUILayoutOption[] options)
            where T : UnityEngine.Object
            => EditorGUILayout.ObjectField(name, obj, typeof(T), allowSceneObjects, options) as T;
        public static T ObjectField<T>(T obj, bool allowSceneObjects, params GUILayoutOption[] options)
            where T : UnityEngine.Object
            => EditorGUILayout.ObjectField(obj, typeof(T), allowSceneObjects, options) as T;

        public static void Button(string name, Action action, params GUILayoutOption[] options)
        {
            if (GUILayout.Button(name, options))
                action();
        }

        public static void TextField(string name, ref string value, Action onChange, params GUILayoutOption[] options)
        {
            if (onChange is null)
            {
                value = EditorGUILayout.TextField(name, value, options);
                return;
            }

            EditorGUI.BeginChangeCheck();
            value = EditorGUILayout.TextField(name, value, options);
            if (EditorGUI.EndChangeCheck())
                onChange();
        }

        static readonly Dictionary<object, bool> _foldouts = new();
        public static bool Foldout(string name, object obj)
        {
            var foldout = _foldouts.GetOrCreate(obj);
            var newFoldout = EditorGUILayout.Foldout(foldout, name);
            _foldouts[obj] = newFoldout;
            return newFoldout;
        }
    }
}