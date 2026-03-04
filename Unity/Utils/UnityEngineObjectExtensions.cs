using UnityEngine;
namespace BB
{
	public static class UnityEngineObjectExtensions
	{
		public static T NullIfDestroyedUnityEngineObject<T>(this T t)
			where T : class
			=> t is Object obj && !obj ? null : t;
		public static bool IsNotNullOrDestroyed<T>(this T t)
			where T : class
			=> t.NullIfDestroyedUnityEngineObject() is not null;
		public static bool IsNullOrDestroyed<T>(this T t)
            where T : class
            => t.NullIfDestroyedUnityEngineObject() is null;
    }
}