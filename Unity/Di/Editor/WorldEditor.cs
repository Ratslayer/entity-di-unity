#if UNITY_EDITOR
using UnityEditor;
namespace BB
{

	public static class WorldEditor
	{
		[InitializeOnLoadMethod]
		static void SubscribeToQuit()
		{
			EditorApplication.playModeStateChanged += Quit;
		}
		static void Quit(PlayModeStateChange c)
		{
			if (c != PlayModeStateChange.ExitingPlayMode)
				return;
			EditorApplication.playModeStateChanged -= Quit;
			World.DestroyAllWorldEntities();
		}
	}
}
#endif