using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace BB
{
    public static class LayoutUtils
    {
        readonly static Dictionary<object, bool> _foldouts = new();
        readonly static Dictionary<object, Vector2> _scrollPositions = new();
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
        public static ChangeCheckDisposable OnChange(Action action)
        {
            EditorGUI.BeginChangeCheck();
            return new()
            {
                OnChange = action
            };
        }
        public static ScrollLayout Scroll(object obj,
            bool alwaysShowVertical = false,
            bool alwaysShowHorizontal = false)
        {
            var scrollPosition = _scrollPositions.GetValueOrDefault(obj);
            _scrollPositions[obj] = EditorGUILayout.BeginScrollView(scrollPosition);
            return new();
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
        public static FoldoutLayout Foldout(string name, object key)
        {
            var foldout = FoldoutByKey(name, key);
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
                foldout = FoldoutByKey(name, foldoutKey);
                add();
            }
            if (foldout)
                EditorGUI.indentLevel++;
            return new(foldout);
        }
        static bool FoldoutByKey(string name, object key)
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
    public readonly struct ScrollLayout : IDisposable
    {
        public void Dispose()
        {
            EditorGUILayout.EndScrollView();
        }
    }
    public readonly struct ChangeCheckDisposable : IDisposable
    {
        public Action OnChange { get; init; }
        public void Dispose()
        {
            if (EditorGUI.EndChangeCheck())
                OnChange?.Invoke();
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