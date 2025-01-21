#if UNITY_EDITOR
using UnityEditor;

namespace BB.Di
{
	public static class EditorWorld
	{
		const string InstallerPath = "Assets/0Game/EditorWorld.asset";
		static IEntity _entity;
		public static IEntity Entity
		{
			get
			{
				if (_entity == null)
				{
					var installer
						= AssetDatabase.LoadAssetAtPath
						<AbstractInstallerAsset>(InstallerPath);
					var impl = EntityImpl.CreateEntity(
						"Editor",
						null,
						installer.Install,
						null);
					_entity = impl;
				}
				return _entity;
			}
		}
		public static T Get<T>()
		{
			if (!Entity.Has(out T result))
				throw new System.Exception($"EditorInstaller has no {typeof(T).FullName} bound.");
			return result;
		}
		[InitializeOnLoadMethod]
		static void CreateEntity()
		{
			Log.BindLogger(new UnityLogger());
		}
	}
}
#endif
