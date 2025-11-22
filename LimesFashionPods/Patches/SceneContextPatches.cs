using HarmonyLib;
using Il2Cpp;
using UnityEngine;

namespace LimesFashionPods.Patches;

[HarmonyPatch]
internal static class SceneContextPatches
{
    [HarmonyPatch(typeof(LookupDirector), nameof(LookupDirector.Awake))]
    [HarmonyPrefix]
    private static void LookupDirectorAwakePatch(LookupDirector __instance)
    { 
        // Essentially the mod's main entry point.
        PodManager.Register();
    }

    [HarmonyPatch(typeof(GordoEat), nameof(GordoEat.Awake))]
    [HarmonyPostfix]
    private static void GordoAttachFashionsPatch(GordoEat __instance)
    {
        var attachFashionsComp = __instance.gameObject.GetComponent<AttachFashions>() ??
                                 __instance.gameObject.AddComponent<AttachFashions>();
        attachFashionsComp._gordoModel = __instance.GordoModel;
        attachFashionsComp.GordoMode = true;
    }

    [HarmonyPatch(typeof(AttachFashions), nameof(AttachFashions.GetParentForSlot))]
    [HarmonyPostfix]
    private static void FashionExtendSlotsPatch(AttachFashions __instance, FashionSlot slot, ref Transform __result)
    {
        switch (slot)
        {
            case PodManager.SecondaryFaceSlot:
                __result = __instance.transform.Find("model_slime_v4/bone_root/bone_slime/bone_core/bone_jiggle_bac");
                break;
        }
    }

    [HarmonyPatch(typeof(AttachFashions), nameof(AttachFashions.GetParentForSlot))]
    [HarmonyPrefix]
    [HarmonyPriority(Priority.Last)]
    private static bool FashionGordoFixPatch(AttachFashions __instance, FashionSlot slot, ref Transform __result)
    {
        if (__instance._gordoModel == null || !__instance.GordoMode)
        {
            return true;
        }
        // Fixes gordo fashion slot parenting.
        
        switch (slot)
        {
            case FashionSlot.FRONT:
                __result = __instance.transform.Find("model_slime_v4/bone_root/bone_slime/bone_core/bone_jiggle_bac");
                break;
            case FashionSlot.TOP:
                __result = __instance.transform.Find("model_slime_v4/bone_root/bone_slime/bone_core/bone_jiggle_top");
                break;
        }

        return false;
    }
}