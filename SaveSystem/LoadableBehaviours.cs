using System.Collections.Generic;

namespace BB
{
    public sealed class LoadableBehaviours : ILoadableBehaviours
    {
        readonly Dictionary<string, LoadableBehaviour> _behaviours = new();
        public void Add(LoadableBehaviour behaviour)
        {
            var key = behaviour.Key;
            if (string.IsNullOrWhiteSpace(key))
            {
                Log.Error($"Attempted to add loadable behaviour with empty key: {behaviour}");
                return;
            }

            if (_behaviours.TryGetValue(key, out var existingBehaviour))
            {
                Log.Error($"Attempted to add behaviour with duplicate key {key}. " +
                    $"Existing: {existingBehaviour}. New: {behaviour}");
                return;
            }

            _behaviours.Add(behaviour.Key, behaviour);
        }

        public void Remove(LoadableBehaviour behaviour)
        {
            var key = behaviour.Key;
            if (string.IsNullOrWhiteSpace(key))
                return;

            if (!_behaviours.TryGetValue(key, out var existingBehaviour))
            {
                Log.Error($"Attempted to remove not registered behaviour {behaviour}.");
                return;
            }
            if (existingBehaviour != behaviour)
            {
                Log.Error($"Attempted to remove wrong behaviour for key {key}. " +
                    $"Existing: {existingBehaviour}. New: {behaviour}");
                return;
            }
            _behaviours.Remove(behaviour.Key);
        }

        public bool TryGet(string key, out LoadableBehaviour result)
        {
            var found = _behaviours.TryGetValue(key, out result);
            if (!found)
                Log.Error($"LoadableBehaviour with key {key} not found.");
            return found;
        }
    }
}