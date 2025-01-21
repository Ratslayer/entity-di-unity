using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace BB
{
	public static class CustomEditorGUILayout
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
	}
}