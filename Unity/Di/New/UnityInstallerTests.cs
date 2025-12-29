using Sirenix.OdinInspector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace BB.Di
{
    public sealed class UnityInstallerTests : BaseScriptableObject
    {
        [SerializeField] TestWorld[] _worlds = { };
        [Serializable]
        sealed class TestData
        {
            public TestInstaller[] _installers = { };
            public override string ToString()
                => string.Join('/', (object[])_installers);
        }
        [Serializable]
        sealed class TestInstaller
        {
            [ShowIf(nameof(ShowInstaller))]
            public InstallerAsset _installer;
            [ShowIf(nameof(ShowInstallerPrefab))]
            public BaseEntityInstallerGameObject _installerPrefab;
            [ShowIf(nameof(ShowPrefab))]
            public GameObject _prefab;

            bool ShowInstaller => !_installerPrefab;
            bool ShowInstallerPrefab => !_installer;
            bool ShowPrefab => _installer && _installer is IEntityInstaller3D or IEntityInstaller2D;
            public override string ToString()
            {
                if (ShowPrefab && _prefab)
                    return $"{_installer.name}:{_prefab.name}";
                if (_installerPrefab)
                    return _installerPrefab.name;
                if (_installer)
                    return _installer.name;
                return "NULL";
            }
            public IEntity CreateEntity(IEntity parent)
            {
                throw new NotImplementedException();
            }
        }
        [Serializable]
        sealed class TestWorld
        {
            public BaseCoreInstallerAsset _coreInstaller;
            public BaseGameInstallerAsset _gameInstaller;
            public TestData[] _tests = { };
        }
        [Button]
        void RunTests()
        {
            var entities = new List<IEntity>();
            foreach (var world in _worlds)
            {
                try
                {
                    WorldBootstrap.CreateWorld();
                    var setup = WorldBootstrap.World;
                    setup.CreateCore(world._coreInstaller);
                    setup.CreateGame(world._gameInstaller);
                    foreach (var test in world._tests)
                    {
                        foreach (var installerData in test._installers)
                        {
                            try
                            {
                                var parent = entities.LastOrDefault()
                                    ?? setup.Game.Entity
                                    ?? setup.Core.Entity;
                                var entity = installerData.CreateEntity(parent);
                                entities.Add(entity);
                            }
                            catch (Exception e)
                            {
                                LogError(e.Message);
                                break;
                            }
                        }
                        foreach (var i in -entities.Count)
                        {
                            try
                            {
                                entities[i].SetState(EntityState.Destroyed);
                            }
                            catch (Exception e)
                            {
                                LogError(e.Message);
                            }
                        }
                        entities.Clear();
                    }
                    WorldBootstrap.DestroyWorld();
                }
                catch (Exception e)
                {
                    LogError(e.Message);
                }
            }
            void LogError(string message)
            {
                var installerName = string.Join('/', entities.Select(e => e?.Name ?? "NULL"));
                Log.Error($"Error during test in installer {installerName}: {message}");
            }
            bool AssertInstallerExists(UnityEngine.Object installer)
            {
                if (installer)
                    return true;
                LogError("Null/destroyed installer encountered in a test");
                return false;
            }
        }
    }
}