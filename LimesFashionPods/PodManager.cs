using Il2Cpp;
using Il2CppMonomiPark.SlimeRancher;
using Il2CppMonomiPark.SlimeRancher.Pedia;
using Il2CppMonomiPark.SlimeRancher.World;
using LimesFashionPods.Behaviours;
using LimesFashionPods.Utilities;
using MelonLoader;
using UnityEngine;
using UnityEngine.AddressableAssets;
using Object = UnityEngine.Object;

namespace LimesFashionPods;

public static class PodManager
{
    public const string EmbeddedAssetsPath = "LimesFashionPods.Assets";
    public const string TableName = "UI";

    private const string PodBasePrefabName = "gadgetFashionPodBase";

    private static AssetBundle? _bundleHandle;
    
    public struct PodParams
    {
        public string DisplayName;
        public string InternalName;
        public string Description;
        public IdentifiableType ItemId;
    }

    public struct FashionParams
    {
        public string DisplayName;
        public string InternalName;
        public GameObject AttachPrefab;
        public GameObject DisplayPrefab;
    }
    
    internal static void Register()
    {
        _bundleHandle ??= EmbeddedUtilities.LoadEmbeddedAssetBundle(Melon<Entrypoint>.Instance.MelonAssembly.Assembly, $"{EmbeddedAssetsPath}.limesfashionpods.bundle");
        var test = GeneratePodGadget(new PodParams
        {
            DisplayName = "Test Fashion Pod",
            InternalName = "TestFashionPod",
            Description = "Testing Testing 1-2-3!",
            ItemId = GenerateFashion(new FashionParams()
            {
                DisplayName = "Test Fashion",
                InternalName = "TestFashion",
                DisplayPrefab = _bundleHandle.LoadAsset<GameObject>("fashionGoogly"),
                AttachPrefab = _bundleHandle.LoadAsset<GameObject>("fashionGooglyAttach")
            })
        });
    }

    private static GameObject CreatePodPrefab(GadgetDefinition gadgetDefinition, ref PodParams p)
    {
        _bundleHandle ??= EmbeddedUtilities.LoadEmbeddedAssetBundle(Melon<Entrypoint>.Instance.MelonAssembly.Assembly, $"{EmbeddedAssetsPath}.limesfashionpods.bundle");
        var podBase = _bundleHandle.LoadAsset<GameObject>(PodBasePrefabName);
        if (podBase == null) throw new Exception($"Pod base prefab, name \"{PodBasePrefabName}\", could not be loaded from embedded AssetBundle!");
        podBase.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        var newPod = Object.Instantiate(podBase);
        newPod.GetComponent<Gadget>().identType = gadgetDefinition;
        newPod.GetComponent<FashionPod>().fashionId.Set(p.ItemId);
        return newPod;
    }

    private static IdentifiableType GenerateFashion(FashionParams p)
    {
        var newDisplay = Object.Instantiate(p.DisplayPrefab);
        var newAttach =  Object.Instantiate(p.AttachPrefab);
        var ident = ScriptableObject.CreateInstance<IdentifiableType>();
        var genericIdentGroup = SRLookup.Get<IdentifiableTypeGroup>("IdentifiableTypesGroup");
        var identGroup = SRLookup.Get<IdentifiableTypeGroup>("VaccableNonLiquids");
        ident.Categories = new Il2CppSystem.Collections.Generic.List<IdentifiableCategory>();
        ident.groupType = genericIdentGroup;
        ident.prefab = newDisplay;
        ident.icon = _bundleHandle?.LoadAsset<Sprite>("iconFashionGooglyEyes") ?? ident.icon;
        ident.color = new Color(0.13f, 0.84f, 0.46f);
        ident.LocalizedName = TranslationPatcher.AddTranslation(TableName, p.InternalName, p.DisplayName);
        newDisplay.GetComponent<IdentifiableActor>().identType = ident;
        newDisplay.GetComponent<Fashion>().AttachPrefab = newAttach;

        GameContext.Instance.LookupDirector.AddIdentifiableTypeToGroup(ident, identGroup);
        GameContext.Instance.LookupDirector.AddIdentifiableTypeToGroup(ident, genericIdentGroup);
        return ident;
    }

    private static GadgetDefinition GeneratePodGadget(PodParams p)
    {
        var newGadgetDef = ScriptableObject.CreateInstance<GadgetDefinition>();
        newGadgetDef.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        newGadgetDef.name = p.InternalName;
        newGadgetDef.localizedName = TranslationPatcher.AddTranslation(TableName, p.InternalName, p.DisplayName);
        newGadgetDef._localizedDescription = TranslationPatcher.AddTranslation(TableName, p.InternalName+"_desc", p.Description);
        newGadgetDef.prefab = CreatePodPrefab(newGadgetDef, ref p);
        newGadgetDef.referenceId = p.InternalName;
        var gadgetGroup = SRLookup.Get<IdentifiableTypeGroup>("GadgetUtilitiesGroup");
        newGadgetDef.groupType = gadgetGroup;
        newGadgetDef.Categories = new();
        newGadgetDef.Categories.Add(SRLookup.Get<IdentifiableCategory>("Utilities Gadget Category")); 
        var pediaEntry = ScriptableObject.CreateInstance<IdentifiablePediaEntry>();
        pediaEntry.hideFlags = HideFlags.HideAndDontSave | HideFlags.DontUnloadUnusedAsset;
        pediaEntry._identifiableType = newGadgetDef;
        pediaEntry.name = p.InternalName+"_pedia";
        pediaEntry._description = newGadgetDef._localizedDescription;
        newGadgetDef._pediaLink = pediaEntry;

        GameContext.Instance.LookupDirector.AddIdentifiableTypeToGroup(newGadgetDef, gadgetGroup);
        GameContext.Instance.LookupDirector.AddGadget(newGadgetDef, pediaEntry);
        
        return newGadgetDef;
    }
}