using System;
using System.Collections.Generic;
using UnityEngine;

namespace BB
{
    public static class UnityLoggerConstants
    {
        public const string UnityObject = "unity_object";
        public const string Target = "target";
    }
    public sealed class UnityLoggerScope : PooledObject<UnityLoggerScope>, ILoggerScope
    {
        private UnityEngine.Object _context;
        private readonly Dictionary<string, object> _values = new();

        public void Info(string msg)
        {
            Debug.Log(ProcessMessage(msg), _context);
            Dispose();
        }

        public void Warning(string msg)
        {
            Debug.LogWarning(ProcessMessage(msg), _context);
            Dispose();
        }

        public void Error(string msg)
        {
            Debug.LogError(ProcessMessage(msg), _context);
            Dispose();
        }

        public void Exception(Exception ex, string msg)
        {
            Debug.LogError(ProcessMessage(msg), _context);
            Debug.LogException(ex, _context);
            Dispose();
        }

        public void AddToScope(string key, object value)
        {
            if (key is UnityLoggerConstants.UnityObject && value is UnityEngine.Object unityObject)
                _context = unityObject;
            _values[key] = value;
        }

        public override void Dispose()
        {
            base.Dispose();
            _context = null;
            _values.Clear();
        }

        private string ProcessMessage(string msg)
        {
            using var builder = PooledStringBuilder.GetPooled();

            builder.Append(msg);
            builder.Append("\n===\n");
            
            foreach (var kvp in _values)
                builder.Append($"{kvp.Key}:{kvp.Value}\n");

            return builder.ToString();
        }
    }
}