using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class ServiceLocator
{
    private static readonly Dictionary<Type, object> services = new Dictionary<Type, object>();
    public static void Register<T>(T service) where T : class
    {
        Type type = typeof(T);
        if (services.ContainsKey(type))
        {
            //services.Add(type, service);
            Debug.LogWarning($"[ServiceLocator] {type.Name} already registered. Overwriting...");
        }
        services[type] = service;
    }
    public static T Get<T>() where T : class
    {
        Type type = typeof(T);
        if (services.TryGetValue(type, out object service))
        {
            return service as T;
        }
        throw new System.Exception($"[ServiceLocator] {type.Name} not found.");
    }
    public static bool TryGet<T>( out T service) where T : class
    {
        Type type = typeof(T);
        if (services.TryGetValue(type, out object obj))
        {
            service = obj as T;
            return service != null;
        }
        service = null;
        return false;
    }
    public static bool IsRegistered<T>() where T : class
    {
        return services.ContainsKey(typeof(T));
    }
    public static void Clear()
    {
        services.Clear();
    }



    public static void Unregister<T>() where T : class
    {
        Type type = typeof(T);
        if (services.ContainsKey(type))
        {
            services.Remove(type);
        }
        else
        {
            Debug.LogWarning($"[ServiceLocator] {type.Name} not registered. Cannot unregister.");
        }
    }
}
