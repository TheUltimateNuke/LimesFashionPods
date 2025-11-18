using Il2Cpp;
using Il2CppInterop.Runtime;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LimesFashionPods;

public class SRLookup
{
    private static readonly Dictionary<Type, Object[]> Cache = new();

    public static T? Get<T>(string name) where T : Object
    {
        if (!Cache.ContainsKey(typeof(T)))
            Cache.Add(typeof(T), Resources.FindObjectsOfTypeAll<T>());

        var found = Cache[typeof(T)].FirstOrDefault(x => x?.name == name)?.Cast<T>();
        if (found == null)
        {
            Cache[typeof(T)] = Resources.FindObjectsOfTypeAll<T>();
            found = Cache[typeof(T)].FirstOrDefault(x => x?.name == name)?.Cast<T>();
        }

        return found;
    }
}