using UnityEngine;
namespace BB
{
	public static class UnityEngineObjectExtensions
	{
		public static T NullIfDestroyedUnityEngineObject<T>(this T t)
			where T : class
			=> t is Object obj && !obj ? null : t;
		public static bool NotNullOrDestroyed<T>(this T t)
			where T : class
			=> t.NullIfDestroyedUnityEngineObject() is not null;
	}
}