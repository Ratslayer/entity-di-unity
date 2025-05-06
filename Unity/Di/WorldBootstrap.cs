using UnityEngine;
using BB.Di;
using System;
namespace BB
{
	public static class WorldBootstrap
	{
		const string PROJECT_CONTEXT = "World";
		static void CreateWorldManager()
		{
			//create world manager
			_worldManager = new GameObject("World Manager")
				.AddComponent<WorldManager>();
			UnityEngine.Object.DontDestroyOnLoad(_worldManager.gameObject);
			//load project context, aka base world container
			var context = Resources.Load<InstallerAsset>(PROJECT_CONTEXT);
			if (!context)
			{
				Debug.LogError($"No {PROJECT_CONTEXT} resource of type {typeof(InstallerAsset).FullName} found");
				UnityEngine.Object.Destroy(_worldManager.gameObject);
				return;
			}
			try
			{
				World.Init(context.Install);
				_worldManager.SetLifecycle(World.EntityRef as IEntityLifecycle);
			}
			catch (Exception e)
			{
				Log.Logger.LogException(e);
			}
		}
		//run init on first scene load
		static WorldManager _worldManager;
		[RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
		static void CreateWorldManagerBootstrap()
		{
			if (_worldManager)
				return;
			Log.BindLogger(new UnityLogger());
			CreateWorldManager();
		}
	}
}