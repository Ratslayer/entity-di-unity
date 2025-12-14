using UnityEngine;
namespace BB.Di
{
    public static class UnityWorldBootstrap
    {
        //const string PROJECT_CONTEXT = "World";
        //static void CreateWorldManager()
        //{
        //    //create world manager
        //    _worldManager = new GameObject("World Manager")
        //        .AddComponent<WorldUpdates>();
        //    UnityEngine.Object.DontDestroyOnLoad(_worldManager.gameObject);
        //    //load project context, aka base world container
        //    var context = Resources.Load<BaseWorldInstallerAsset>(PROJECT_CONTEXT);
        //    if (!context)
        //    {
        //        Debug.LogError($"No {PROJECT_CONTEXT} resource of type {typeof(BaseWorldInstallerAsset).FullName} found");
        //        UnityEngine.Object.Destroy(_worldManager.gameObject);
        //        return;
        //    }
        //    try
        //    {
        //        World.Init(context);
        //        _worldManager.SetEntity(World.EntityRef);
        //    }
        //    catch (Exception e)
        //    {
        //        Log.Logger.LogException(e);
        //    }
        //}
        ////run init on first scene load
        //static WorldUpdates _worldManager;
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void Clear()
        {
            WorldBootstrap.ClearWorldEntities();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            WorldBootstrap.Init();
        }
    }
}