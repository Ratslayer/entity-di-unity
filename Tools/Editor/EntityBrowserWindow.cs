using BB.Di;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEditor.ShortcutManagement;
using UnityEngine;

namespace BB
{
    public sealed class EntityBrowserWindow : EditorWindow
    {
        enum BrowserTag
        {
            Event,
            Child,
            Variable,
            System,
        }
        const int MaxNumEntries = 30;
        [MenuItem("Tools/BB/Entity Browser")]
        [Shortcut("Open Entity Browser Window", KeyCode.F2, ShortcutModifiers.Shift)]
        public static void ShowWindow()
        {
            GetWindow<EntityBrowserWindow>("Entity Browser");
        }

        readonly List<Entity> _entities = new();
        readonly List<EntityEntry> _entries = new();
        readonly List<BrowserTag> _tags = new();
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
            var rootEntity = Application.isPlaying ? WorldBootstrap.World.Core.Entity : EditorWorld.Entity;
            if (rootEntity is null)
                return;
            var entitiesQueue = new List<IEntity>() { rootEntity };

            while (entitiesQueue.Count > 0)
            {
                var entityRef = entitiesQueue.RemoveLast();
                if (entityRef.State is EntityState.Despawned or EntityState.Disabled)
                    continue;

                _entities.Add(entityRef.GetToken());
                entitiesQueue.AddRange(entityRef.Children);
            }

            _entities.SortByToString();

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

                var elements = details.GetElements();

                foreach (var element in elements)
                {
                    if (!Matches(element.ContractType.Name, searchData._componentName))
                        continue;
                    var component = new EntityBrowserComponent
                    {
                        _component = element
                    };

                    if (HasTag<IEvent>(BrowserTag.Event)
                        || HasTag<ChildEntityVariable>(BrowserTag.Child)
                        || HasTag<IVariable>(BrowserTag.Variable)
                        || HasTag<EntitySystem>(BrowserTag.System))
                        entry._components.Add(component);

                    bool HasTag<T>(BrowserTag tag)
                    {
                        if (_tags.Count == 0)
                            return true;
                        if (!typeof(T).IsAssignableFrom(element.ContractType))
                            return false;
                        return _tags.Contains(tag);
                    }
                }
                entry._components.SortByToString();
                if (entry._components.Count > 0)
                    _entries.Add(entry);
            }

            static bool Matches(string str, SearchValue search)
            {
                if (string.IsNullOrWhiteSpace(search?._name))
                    return true;

                if (search._name.ToLower().Contains(str.ToLower()))
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
            using (LayoutUtils.Horizontal)
            {
                using var change = LayoutUtils.OnChange(UpdatedSearchedEntries);
                var tags = new BrowserTag[]
                {
                    BrowserTag.Event,
                    BrowserTag.Variable,
                    BrowserTag.Child,
                    BrowserTag.System
                };
                foreach (var tag in tags)
                {
                    var tagIsSelected = _tags.Contains(tag);
                    EditorGuiUtils.Toggle(tag.ToString(), ref tagIsSelected);
                    if (tagIsSelected)
                        _tags.AddUnique(tag);
                    else _tags.Remove(tag);
                }
            }
            using var scroll = LayoutUtils.Scroll(this);
            foreach (var i in _entries.Count)
            {
                var entry = _entries[i];
                if (!entry.Entity)
                    continue;

                if (entry._components.IsNullOrEmpty())
                {
                    EditorGUILayout.LabelField(entry.Entity);
                    continue;
                }

                using (LayoutUtils.Horizontal)
                {
                    var foldout = EditorGuiUtils.Foldout($"{entry.Entity} [{entry._components.Count}]", entry.Entity);
                    if (entry.Entity._ref is IEntityDetails details)
                        EditorGuiUtils.Button(
                            "Installer",
                            () => EditorUtils.Select(details.Installer),
                            GUILayout.Width(100));
                    if (!foldout)
                        continue;
                }
                using var indent = LayoutUtils.Indent;
                EditorGUILayout.LabelField($"Parent: {entry.Entity._ref.Parent?.Name ?? "None"}");
                foreach (var ec in entry._components)
                {
                    var comp = ec._component;
                    switch (comp.Instance)
                    {
                        case IStackValue stack:
                            var topValue = stack.GetTypelessSourceValues().FirstOrDefault().Value;
                            EditorGuiUtils.Foldout($"{stack.GetType().Name}:{GetLabel(topValue)}", stack,
                                () =>
                                {
                                    foreach (var value in stack.GetTypelessSourceValues())
                                        using (LayoutUtils.Horizontal)
                                        {
                                            DrawLabel(value.Value);
                                            SelectObjButton(value.Value);
                                        }
                                },
                                () => SelectObjButton(topValue));
                            break;
                        case IBoard board:
                            DrawFoldout(board.GetType().Name, board, () =>
                            {
                                EditorBoardUtils.DrawBoard(board);
                            });
                            break;
                        case IEvent e:
                            if (e is not IEventHandlers h || h.Handlers.Count == 0)
                            {
                                DrawLabel(comp);
                                continue;
                            }
                            DrawFoldout($"{comp} [{h.Handlers.Count}]", e, () =>
                            {
                                foreach (var h in h.Handlers)
                                    EditorGUILayout.LabelField(h.ToString());
                            });
                            break;
                        case ChildEntityVariable cevar:
                            {
                                if (cevar.Entity)
                                {
                                    using var l = LayoutUtils.Horizontal;
                                    DrawLabel($"{comp}:{cevar.Installer.Name}");
                                    EditorGuiUtils.Button("Select", () =>
                                    {
                                        _searchValue = cevar.Entity;
                                        UpdatedSearchedEntries();
                                    });
                                }
                                else DrawLabel(comp);
                            }
                            break;
                        case IVariable variable:
                            DrawLabel(variable);
                            break;
                        default:
                            DrawLabel(comp);
                            break;
                    }
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
            void DrawLabel(object obj)
                => EditorGUILayout.LabelField(GetLabel(obj));
            string GetLabel(object obj)
                => obj switch
                {
                    Component comp => comp.gameObject.name,
                    null => "NULL",
                    _ => obj.ToString()
                };
            void SelectObjButton(object obj)
            {
                if (obj is UnityEngine.Object uObj)
                    EditorGuiUtils.Button(
                        "Select",
                        () => Selection.activeObject = uObj,
                        GUILayout.Width(100));
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
            public readonly List<EntityBrowserComponent> _components = new();
        }
        sealed class EntityBrowserComponent
        {
            public EntityComponentData _component;
        }
    }
}