using UnityEditor;

namespace BB
{
	public static class UndoUtils
	{
		public static CollapseGroupOnDispose GroupUndos()
		{
			Undo.IncrementCurrentGroup();
			var groupId = Undo.GetCurrentGroup();
			return new() { GroupId = groupId };
		}
        public static ActionOnDispose RegisterUndoInCaseOfGuiChange(this UnityEngine.Object obj, string name = null)
        {
            name ??= $"{obj.name} changed";
            obj.RegisterUndo(name);
            EditorGUI.BeginChangeCheck();
            return ActionOnDispose.GetPooled(() =>
            {
                if (EditorGUI.EndChangeCheck())
                    obj.SetDirty();
                else Undo.ClearUndo(obj);

            });
        }
    }
}