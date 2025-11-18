using System.Collections;
using Il2CppInterop.Runtime;
using Il2CppInterop.Runtime.InteropTypes.Arrays;
using Il2CppSystem.IO;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.Rendering;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace LimesFashionPods.Utilities;

public static class AddressableShaderCache
{
    private static Dictionary<string, Shader> Cache = new();
    private static bool _cacheRoutineCompleted = false;
    
    private static void ReapplyProperties(Material material)
    {
        Shader shader = material.shader;
        int shaderPropCount = shader.GetPropertyCount();

        for (int i = 0; i < shaderPropCount; i++)
        {
            string propName = shader.GetPropertyName(i);
            ShaderPropertyType propType = shader.GetPropertyType(i);
            switch (propType)
            {
                case ShaderPropertyType.Color:
                    Color color = material.GetColor(propName);
                    material.SetColor(propName, color);
                    break;
                case ShaderPropertyType.Float:
                case ShaderPropertyType.Range:
                    float floatVal = material.GetFloat(propName);
                    material.SetFloat(propName, floatVal);
                    break;
                case ShaderPropertyType.Texture:
                    material.SetTexture(propName, material.GetTexture(propName));
                    break;
                case ShaderPropertyType.Vector:
                    Vector4 vector = material.GetVector(propName);
                    material.SetVector(propName, vector);
                    break;

            }
        }
    }
    
    internal static IEnumerator CacheAddressableShaders()
    {
        _cacheRoutineCompleted = false;
        
        foreach (var locator in Addressables.ResourceLocators.ToArray())
        {
            foreach (var key in locator.Keys.ToArray())
            {
                if (locator.Locate(key, Il2CppType.Of<Shader>(), out var list))
                {
                    var castedList = list.Cast<Il2CppSystem.Collections.Generic.IEnumerable<IResourceLocation>>().ToArray();
                    foreach (var castedShaderLoc in castedList)
                    {
                        var shaderHandle = Addressables.LoadAsset<Shader>(castedShaderLoc);
                        yield return shaderHandle.HandleAsynchronousAddressableOperation();
                        if (!AssetUtilities.IsHandleSuccess(shaderHandle) || shaderHandle.Result == null) continue;
                        Cache.TryAdd(shaderHandle.Result.name, shaderHandle.Result);
                    }
                }
            }
        }

        _cacheRoutineCompleted = true;
    }
    
    public static IEnumerator ReloadAddressableShaders(GameObject? parent = null)
    {
        while (!_cacheRoutineCompleted)
        {
            yield return null;
        }
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
            if (meshRenderer.sharedMaterial == null) continue;
            Entrypoint.Logger.Msg("BEFORE: " + meshRenderer.material.shader.name);
            var dummyShader = meshRenderer.sharedMaterial.shader;
            var foundShader = Shader.Find(meshRenderer.sharedMaterial.shader.name);
            meshRenderer.sharedMaterial.shader = foundShader ?? Resources.FindObjectsOfTypeAll<Shader>().FirstOrDefault(sha => sha.name == dummyShader.name && meshRenderer.material.shader != sha) ?? Cache.GetValueOrDefault(dummyShader.name);
            ReapplyProperties(meshRenderer.sharedMaterial);
            Entrypoint.Logger.Msg("AFTER: " + meshRenderer.sharedMaterial.shader?.name ?? "null");
        }
    }
}