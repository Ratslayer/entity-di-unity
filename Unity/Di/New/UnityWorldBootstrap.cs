using UnityEngine;

namespace BB.Di
{
    public static class UnityWorldBootstrap
    {
        public static EntityBootstrapSettings Settings
            => Resources.Load<EntityBootstrapSettings>("UnityWorldBootstrapSettings");

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSplashScreen)]
        static void Clear()
        {
            if (WorldBootstrap.World is null)
                return;

            Debug.Log("Destroying current world.");
            WorldBootstrap.DestroyWorld();
        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        static void Init()
        {
            if (WorldBootstrap.World is not null)
                return;

            Debug.Log("Creating runtime world");
            WorldBootstrap.SpawnWorld(Settings._runtimeConfig);
        }
    }
}