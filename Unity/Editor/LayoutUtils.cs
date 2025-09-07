using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BB
{
	public static class LayoutUtils
	{
		readonly static Dictionary<string, bool> _foldouts = new();
		public static HorizontalLayout Horizontal
		{
			get
			{
				EditorGUILayout.BeginHorizontal();
				return new();
			}
		}
		public static VerticalLayout Vertical
		{
			get
			{
				EditorGUILayout.BeginVertical();
				return new();
			}
		}
		public static GuiHorizontalLayout HorizontalBox(string name)
		{
			GUILayout.BeginHorizontal(name, "window");
			return new();
		}
		public static GuiVerticalLayout VerticalBox(string name)
		{
			GUILayout.BeginVertical(name, "window");
			return new();
		}
		public static RestoreEditorWidth LabelWidth(float label)
		{
			var result = new RestoreEditorWidth(EditorGUIUtility.labelWidth);
			EditorGUIUtility.labelWidth = label;
			return result;
		}
		public static FoldoutLayout Foldout(string name, ref bool foldout)
		{
			foldout = EditorGUILayout.Foldout(foldout, name);
			if (foldout)
				EditorGUI.indentLevel++;
			return new(foldout);
		}
		public static FoldoutLayout FoldoutWithHorizontalAdd(string name, ref bool foldout, Action add)
		{
			using (Horizontal)
			{
				foldout = EditorGUILayout.Foldout(foldout, name);
				add();
			}
			if (foldout)
				EditorGUI.indentLevel++;
			return new(foldout);
		}
		public static FoldoutLayout FoldoutWithHorizontalAdd(string name, string foldoutKey, Action add)
		{
			bool foldout;
			using (Horizontal)
			{
				foldout = Foldout(name, foldoutKey);
				add();
			}
			if (foldout)
				EditorGUI.indentLevel++;
			return new(foldout);
		}
		static bool Foldout(string name, string key)
		{
			var foldout = EditorGUILayout.Foldout(_foldouts.TryGetValue(key, out var f) ? f : false, name);
			_foldouts[key] = foldout;
			return foldout;
		}
		public static IndentLayout Indent
		{
			get
			{
				EditorGUI.indentLevel++;
				return new IndentLayout();
			}
		}
	}
	public readonly struct IndentLayout : IDisposable
	{
		public void Dispose()
		{
			EditorGUI.indentLevel--;
		}
	}
	public readonly struct FoldoutLayout : IDisposable
	{
		readonly bool _foldout;
		public FoldoutLayout(bool foldout)
		{
			_foldout = foldout;
		}
		public void Dispose()
		{
			if (_foldout)
				EditorGUI.indentLevel--;
		}
		public static implicit operator bool(FoldoutLayout fl) => fl._foldout;
	}
	public readonly struct RestoreEditorWidth : IDisposable
	{
		readonly float _labelWidth;
		public RestoreEditorWidth(float labelWidth)
		{
			_labelWidth = labelWidth;
		}
		public void Dispose()
		{
			EditorGUIUtility.labelWidth = _labelWidth;
		}
	}
	public readonly struct VerticalLayout : IDisposable
	{
		public void Dispose()
		{
			EditorGUILayout.EndVertical();
		}
	}
	public readonly struct HorizontalLayout : IDisposable
	{
		public void Dispose()
		{
			EditorGUILayout.EndHorizontal();
		}
	}
	public readonly struct GuiVerticalLayout : IDisposable
	{
		public void Dispose()
		{
			GUILayout.EndVertical();
		}
	}
	public readonly struct GuiHorizontalLayout : IDisposable
	{
		public void Dispose()
		{
			GUILayout.EndHorizontal();
		}
	}
}