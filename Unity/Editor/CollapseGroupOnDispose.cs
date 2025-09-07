using System;
using UnityEditor;

namespace BB
{
	public readonly struct CollapseGroupOnDispose : IDisposable
	{
		public int GroupId { get; init; }

		public void Dispose()
		{
			Undo.CollapseUndoOperations(GroupId);
		}
	}
}