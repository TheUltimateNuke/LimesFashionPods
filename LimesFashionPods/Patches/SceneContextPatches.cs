using HarmonyLib;
using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher.Regions;
using Il2CppMonomiPark.SlimeRancher.World;
using LimesFashionPods.Behaviours;
using LimesFashionPods.Utilities;
using MelonLoader;
using UnityEngine;
using Object = UnityEngine.Object;

namespace LimesFashionPods.Patches;

[HarmonyPatch]
internal static class SceneContextPatches
{
    [HarmonyPatch(typeof(LookupDirector), nameof(LookupDirector.Awake))]
    [HarmonyPrefix]
    private static void SceneContextAwakePatch(LookupDirector __instance)
    { 
        PodManager.Register();
    }

    [HarmonyPatch(typeof(RegisteredActorBehaviour), nameof(RegisteredActorBehaviour.OnEnable))]
    [HarmonyPostfix]
    private static void ParticleSystemPlayPatch(RegisteredActorBehaviour __instance)
    {
        var instance = __instance.TryCast<Gadget>();
        if (instance == null) return;
        
        MelonCoroutines.Start(AddressableShaderCache.ReloadAddressableShaders(__instance.gameObject));
    }

    [HarmonyPatch(typeof(CFX_AutoDestructShuriken), nameof(CFX_AutoDestructShuriken.OnEnable))]
    [HarmonyPostfix]
    private static void ParticleSystemPlayPatch(CFX_AutoDestructShuriken __instance)
    {
        MelonCoroutines.Start(AddressableShaderCache.ReloadAddressableShaders(__instance.gameObject));
    }
}