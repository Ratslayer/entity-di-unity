using Sirenix.OdinInspector.Editor;
using UnityEditor;
using UnityEngine;

namespace BB
{
	public abstract class BaseEditor<UnityObjectType> : BaseEditor
		where UnityObjectType : UnityEngine.Object
	{
		public UnityObjectType T => (UnityObjectType)target;
		public void Save()
		{
			EditorUtility.SetDirty(T);
			AssetDatabase.SaveAssets();
		}
	}
	public abstract class BaseEditor : OdinEditor
	{
		protected override void OnEnable()
		{
			base.OnEnable();
			SceneView.duringSceneGui += SceneGUI;
		}
		protected override void OnDisable()
		{
			base.OnDisable();
			SceneView.duringSceneGui -= SceneGUI;
		}
		protected virtual void SceneGUI(SceneView view) { }
		public bool Button(string name, string undoMessage, params GUILayoutOption[] options)
		{
			if (Button(name, options))
			{
				Undo.RecordObject(this, undoMessage);
				return true;
			}
			return false;
		}
		public static bool Button(string name, params GUILayoutOption[] options)
			=> GUILayout.Button(name, options);
		public static bool ToggleButton(string name, bool toggled)
			=> GUILayout.Toggle(toggled, name, "Button");
		public override void OnInspectorGUI()
		{
			if (DrawBaseInspector)
				base.OnInspectorGUI();
			EditorGUI.BeginChangeCheck();
			InspectorGUI();
			if (EditorGUI.EndChangeCheck())
				EditorUtility.SetDirty(target);
		}
		protected virtual void InspectorGUI() { }
		protected virtual bool DrawBaseInspector => true;
	}
}
