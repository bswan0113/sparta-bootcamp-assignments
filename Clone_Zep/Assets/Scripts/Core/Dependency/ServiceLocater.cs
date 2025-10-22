using System;
using System.Collections.Generic;
using Core.Logging;

namespace Core.Dependency
{
    public static class ServiceLocator
    {
        private static readonly Dictionary<Type, object> _services = new Dictionary<Type, object>();

        public static void Register<T>(T service)
        {
            var type = typeof(T);
            if (_services.ContainsKey(type))
            {
                CLogger.LogWarning($"[ServiceLocator] Service of type '{type.Name}' is already registered. It will be overwritten.");
                _services[type] = service;
            }
            else
            {
                _services.Add(type, service);
            }
        }

        public static T Get<T>()
        {
            var type = typeof(T);
            if (!_services.TryGetValue(type, out var service))
            {
                CLogger.LogError($"[ServiceLocator] Service of type '{type.Name}' is not registered.");
                throw new InvalidOperationException($"Cannot find a service of type {type.Name}.");
            }
            return (T)service;
        }

        public static void Clear()
        {
            _services.Clear();
        }
    }
}