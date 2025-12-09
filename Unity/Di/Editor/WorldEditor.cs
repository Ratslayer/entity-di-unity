#if UNITY_EDITOR
using BB.Di;
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
			WorldBootstrap.ClearWorldEntities();
		}
	}
}
#endif