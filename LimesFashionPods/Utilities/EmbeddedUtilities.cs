using System.Reflection;
using UnityEngine;

namespace LimesFashionPods.Utilities;

public static class EmbeddedUtilities
{
    public static AssetBundle LoadEmbeddedAssetBundle(Assembly assembly, string name)
    {
        if (assembly.GetManifestResourceNames().Contains(name))
        {
            Entrypoint.Logger.Msg($"Loading stream for resource '{name}' embedded from assembly...");
            using var str = assembly.GetManifestResourceStream(name) ?? throw new Exception(
                "Resource stream returned null. This could mean an inaccessible resource caller-side or an invalid argument was passed.");
            using MemoryStream memoryStream = new();
            str.CopyTo(memoryStream);
            Entrypoint.Logger.Msg(ConsoleColor.Green, "Done!");
            var resource = memoryStream.ToArray();

            Entrypoint.Logger.Msg($"Loading assetBundle from data '{name}', please be patient...");
            var bundle = AssetBundle.LoadFromMemory(resource);
            Entrypoint.Logger.Msg(ConsoleColor.Green, "Done!");
            return bundle;
        }

        throw new Exception(
            $"No resources matching the name '{name}' were found in the assembly '{assembly.FullName}'. Please ensure you passed the correct name.");
    }
}