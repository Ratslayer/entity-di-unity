using UnityEditor;

namespace BB
{
	public static class UndoUtils
	{
		public static CollapseGroupOnDispose BeginUndo()
		{
			Undo.IncrementCurrentGroup();
			var groupId = Undo.GetCurrentGroup();
			return new() { GroupId = groupId };
		}
	}
}