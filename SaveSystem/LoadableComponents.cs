using System.Collections.Generic;

namespace BB
{
    public sealed class LoadableComponents : ILoadableComponents
    {
        readonly Dictionary<string, LoadableComponent> _components = new();
        public void Add(LoadableComponent component)
        {
            var key = component.Key;
            if (string.IsNullOrWhiteSpace(key))
            {
                Log.Error($"Attempted to add loadable component with empty key: {component}");
                return;
            }

            if (_components.TryGetValue(key, out var existingComponent))
            {
                Log.Error($"Attempted to add component with duplicate key {key}. " +
                    $"Existing: {existingComponent}. New: {component}");
                return;
            }

            _components.Add(key, component);
        }

        public void Remove(LoadableComponent component)
        {
            var key = component.Key;
            if (string.IsNullOrWhiteSpace(key))
                return;

            if (!_components.TryGetValue(key, out var existingComponent))
            {
                Log.Error($"Attempted to remove not registered component {component}.");
                return;
            }
            if (existingComponent != component)
            {
                Log.Error($"Attempted to remove wrong component for key {key}. " +
                    $"Existing: {existingComponent}. New: {component}");
                return;
            }
            _components.Remove(key);
        }

        public bool TryGet(string key, out LoadableComponent result)
        {
            var found = _components.TryGetValue(key, out result);
            if (!found)
                Log.Error($"LoadableBehaviour with key {key} not found.");
            return found;
        }
	}
}