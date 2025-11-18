using System.Collections;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using UnityEngine;

namespace LimesFashionPods.Utilities;

public static class AddressableShaderCache
{
    public static IEnumerator ReloadAddressableShaders(GameObject? parent = null)
    {
        yield return new WaitForEndOfFrame();
        Il2CppArrayBase<Renderer> renderers;
        if (!parent)
        {
            renderers = UnityEngine.Object.FindObjectsOfType<Renderer>();
        }
        else
        {
            var rendList = new List<Renderer>();
            rendList.AddRange(parent.GetComponentsInChildren<Renderer>());

            renderers = new Il2CppReferenceArray<Renderer>([.. rendList]);
        }
        foreach (var meshRenderer in renderers) // for all components of type Renderer under this gameObject + children
        {
            if (meshRenderer.material == null) continue;
            Entrypoint.Logger.Msg("BEFORE: " + meshRenderer.material.shader.name);
            meshRenderer.material.shader = Shader.Find(meshRenderer.material.shader.name);
            Entrypoint.Logger.Msg("AFTER: " + meshRenderer.material.shader.name);
        }
    }
}