using BB.Di;
using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace BB
{
    public sealed class DebugWindow : EditorWindow
    {
        [MenuItem("Tools/BB/Debug Window")]
        [Shortcut("Open Debug Window", KeyCode.Alpha1, ShortcutModifiers.Alt | ShortcutModifiers.Control)]
        static void ShowWindow()
        {
            GetWindow<DebugWindow>("Debug Window");
        }
        private void OnGUI()
        {
            if (Application.isPlaying)
                DrawGameGui();
            else DrawEditorGui();
        }
        void DrawEditorGui()
        {
        }
        void DrawGameGui()
        {
            using (LayoutUtils.Horizontal)
            {
                EditorGuiUtils.Button("Kill Player", KillPlayer);
            }
            using (LayoutUtils.Horizontal)
            {
                EditorGuiUtils.Button("Save Game", SaveGame);
                EditorGuiUtils.Button("Load Game", LoadGame);

            }
            void KillPlayer()
            {
                new AddBoardContext
                {
                    Entity = World.Require<Player>(),
                    Key = World.Require<BoardConfig>()._health,
                    Value = -1e10
                }.Add();
            }
            void SaveGame()
            {
                World.Require<IGameSaveSystem>().SaveGame(new SaveGameContext
                {
                    FilePath = "test_save_game.txt"
                });
            }
            void LoadGame()
            {
                World.Require<IGameSaveSystem>().LoadGame(new LoadGameContext
                {
                    FilePath = "test_save_game.txt"
                });
            }
        }
    }
    public sealed class EntityBrowserWindow : EditorWindow
    {
        const int MaxNumEntries = 10;
        [MenuItem("Tools/BB/Entity Browser")]
        [Shortcut("Open Entity Browser Window", KeyCode.Alpha2, ShortcutModifiers.Alt | ShortcutModifiers.Control)]
        public static void ShowWindow()
        {
            GetWindow<EntityBrowserWindow>("Entity Browser");
        }

        readonly List<Entity> _entities = new();
        readonly List<EntityEntry> _entries = new();
        string _searchValue;

        private void OnGUI()
        {
            using (var _ = LayoutUtils.Horizontal)
            {
                EditorGuiUtils.Button("Update", UpdateEntities);
                EditorGuiUtils.Button("Clear", ClearEntities);
            }

            EditorGuiUtils.TextField("Search", ref _searchValue, UpdatedSearchedEntries);

            DrawEntries();
        }
        void UpdateEntities()
        {
            _entities.Clear();
            var entitiesQueue = new List<IEntity>()
            {
                Application.isPlaying ? World.RootEntity : EditorWorld.Entity
            };

            while (entitiesQueue.Count > 0)
            {
                var entityRef = entitiesQueue.RemoveLast();
                if (entityRef.State is EntityState.Despawned or EntityState.Disabled)
                    continue;

                _entities.Add(entityRef.GetToken());
                if (entityRef is IEntityDetails details)
                    entitiesQueue.AddRange(details.GetChildren());
            }

            UpdatedSearchedEntries();
        }
        void ClearEntities()
        {
            _entities.Clear();
            UpdatedSearchedEntries();
        }
        void UpdatedSearchedEntries()
        {
            _entries.Clear();

            var searchData = new SearchData();
            if (!string.IsNullOrWhiteSpace(_searchValue))
            {
                var strs = _searchValue.Split();
                foreach (var str in strs)
                {
                    var pair = str.Split(':');
                    switch (pair.Length)
                    {
                        case 1:
                            if (!string.IsNullOrWhiteSpace(pair[0]))
                                searchData._entityNames.Add(pair[0]);
                            break;
                        case 2:
                            switch (pair[0])
                            {
                                case "c":
                                    searchData._componentName = pair[1];
                                    break;
                            }
                            break;
                    }
                }
            }

            foreach (var entity in _entities)
            {
                if (!entity)
                    continue;

                var er = entity._ref;
                var nameWords = er.Name.Split();
                if (searchData._entityNames.Count > nameWords.Length)
                    continue;

                var allMatches = true;
                for (var i = 0; i < searchData._entityNames.Count; i++)
                    if (!Matches(nameWords[i], searchData._entityNames[i]))
                    {
                        allMatches = false;
                        break;
                    }

                if (!allMatches)
                    continue;

                if (er is not IEntityDetails details)
                    continue;
                var entry = new EntityEntry
                {
                    Entity = entity
                };

                foreach (var (type, elem) in details.GetElements())
                {
                    if (!Matches(type.Name, searchData._componentName))
                        continue;
                    if (elem is IEvent)
                        entry._events.Add(elem);
                    else entry._components.Add(elem);
                }

                if (entry._components.Count > 0)
                    _entries.Add(entry);
            }

            static bool Matches(string str, SearchValue search)
            {
                if (string.IsNullOrWhiteSpace(search?._name))
                    return true;

                var capitalizedWords = StringExtensions.SplitByCapitalWords(str);
                if (capitalizedWords.Length < search._capitalizedWords.Length)
                    return false;

                for (int i = 0; i < search._capitalizedWords.Length; i++)
                    if (!capitalizedWords[i].StartsWith(
                        search._capitalizedWords[i],
                        StringComparison.InvariantCultureIgnoreCase))
                        return false;

                return true;
            }
        }
        void DrawEntries()
        {
            var numEntries = Mathf.Min(_entries.Count, MaxNumEntries);
            foreach (var i in numEntries)
            {
                var entry = _entries[i];
                if (entry._components.IsNullOrEmpty())
                {
                    EditorGUILayout.LabelField(entry.Entity);
                    continue;
                }

                if (!EditorGuiUtils.Foldout($"{entry.Entity} [{entry._components.Count}]", entry.Entity))
                    continue;
                using var _ = LayoutUtils.Indent;
                foreach (var comp in entry._components)
                {
                    switch (comp)
                    {
                        case IStackValue stack:
                            DrawFoldout(stack.CustomToString(), stack, () =>
                            {
                                foreach (var value in stack.GetTypelessSourceValues())
                                    EditorGUILayout.LabelField(value.ToString());
                            });
                            break;
                        case IBoard board:
                            DrawFoldout(board.GetType().Name, board, () =>
                            {
                                EditorBoardUtils.DrawBoard(board);
                            });
                            break;
                        default:
                            EditorGUILayout.LabelField(comp.GetType().Name);
                            break;
                    }
                }

                if (entry._events.Count == 0)
                    continue;
                EditorGUILayout.LabelField("Events:");
                foreach (var e in entry._events)
                {
                    var name = e.GetType().GenericTypeArguments[0].Name;
                    EditorGUILayout.LabelField(name);
                }
            }
            void DrawFoldout(string name, object key, Action draw)
            {
                if (!EditorGuiUtils.Foldout(name, key))
                    return;

                using (LayoutUtils.Indent)
                {
                    draw();
                }
            }
        }

        sealed class SearchData
        {
            public List<SearchValue> _entityNames = new();
            public SearchValue _componentName;
        }
        sealed class SearchValue
        {
            public string _name;
            public string[] _capitalizedWords;
            public static implicit operator SearchValue(string str)
                => new()
                {
                    _name = str,
                    _capitalizedWords = StringExtensions.SplitByCapitalWords(str)
                };
        }
        sealed class EntityEntry
        {
            public Entity Entity { get; init; }
            public readonly List<object> _components = new();
            public readonly List<object> _events = new();
        }
    }
}