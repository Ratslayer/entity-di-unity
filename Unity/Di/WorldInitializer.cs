// using UnityEngine;
using BB.Di;
// using UnityEditor;

namespace BB
{
    public interface IWorldConfigProvider
    {
        WorldSetupConfig GetConfig();
    }
}
//     public sealed class WorldInitializer : IWorldInitializer
//     {
//         const string CoreInstaller = "Core";
//         const string CoreEditorInstaller = "0Game/Configs/Editor/EditorCore";
//
//         public WorldSetupConfig Init()
//         {
//             var logger = new UnityLogger();
//             Log.BindLogger(logger);
//
//             var coreInstaller = Resources.Load<BaseCoreInstallerAsset>(CoreInstaller);
//             if (!coreInstaller)
//                 throw new DiException(
//                     $"No {CoreInstaller} resource " +
//                     $"of type {typeof(BaseCoreInstallerAsset).FullName} found");
//
// #if UNITY_EDITOR
//             if (!Application.isPlaying)
//             {
//                 coreInstaller = AssetDatabase.LoadAssetAtPath<EditorCoreInstaller>(CoreEditorInstaller);
//                 if (!coreInstaller)
//                     throw new DiException(
//                         $"No installer found at {CoreEditorInstaller} " +
//                         $"of type {typeof(BaseCoreInstallerAsset).FullName} ");
//             }
// #endif
//
//             return new WorldSetupConfig
//             {
//                 AdditionalInstaller = new UnityAdditionalInstaller(),
//                 CoreInstaller = coreInstaller,
//                 ForcedDynamicTypes = new[] { typeof(IBoard), typeof(IEvent<IBoard>) },
//                 Logger = logger
//             };
//         }
//     }
// }