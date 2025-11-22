using System.Collections;
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
    public const FashionSlot SecondaryFaceSlot = (FashionSlot)3;
    public const string EmbeddedAssetsPath = "LimesFashionPods.Assets";
    public const string TableName = "UI";

    private const string PodBasePrefabName = "gadgetFashionPodBase";

    public static AssetBundle BundleHandle
    {
        get
        {
            _bundleHandle ??= EmbeddedUtilities.LoadEmbeddedAssetBundle(Melon<Entrypoint>.Instance.MelonAssembly.Assembly, $"{EmbeddedAssetsPath}.limesfashionpods.bundle");
            return _bundleHandle;
        }
    }
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
        public FashionSlot? SlotOverride;
        public Sprite? Icon;
        public GameObject? AttachPrefab;
        public GameObject DisplayPrefab;
    }

    public static T[] LoadFashionAsset<T>(string assetName, bool appendAttach) where T : Object
    {
        var ret = new List<T>();
        var asset = LoadFashionAsset<T>(assetName);
        ret.Add(asset);
        if (appendAttach)
        {
            var attach = LoadFashionAsset<T>(assetName+"Attach");
            ret.Add(attach);
        }
        return ret.ToArray();
    }
    
    public static T LoadFashionAsset<T>(string assetName) where T : Object
    {
        var asset = BundleHandle.LoadAsset<T>(assetName);
        asset.MakePersistent(true);
        return asset;
    }

    public static void MakePersistent(this Object asset, bool dontDestroyOnLoad = false)
    {
        asset.hideFlags = HideFlags.DontUnloadUnusedAsset | HideFlags.HideAndDontSave;
        if (dontDestroyOnLoad) Object.DontDestroyOnLoad(asset);
    }
    
    internal static void Register()
    {
        // Load fashion assets
        var fashionGoogly = LoadFashionAsset<GameObject>("fashionGoogly", true);
        var fashionTiara = LoadFashionAsset<GameObject>("fashionTiara", true);
        var fashionDealWithIt = LoadFashionAsset<GameObject>("fashionDealWithIt", true);
        var fashionCursedLoaf = LoadFashionAsset<GameObject>("fashionCursedLoaf", true);
        var fashionLoaf = LoadFashionAsset<GameObject>("fashionLoaf", true);
        var fashionBlunt = LoadFashionAsset<GameObject>("fashionBlunt", true);
        var fashionRemover = LoadFashionAsset<GameObject>("fashionRemove");
        
        // Register fashions + their pod gadgets into the game
        GeneratePodGadget(new PodParams
        {
            DisplayName = "Googly Fashion Pod",
            InternalName = "GooglyFashionPod",
            Description = "wolololo",
            ItemId = GenerateFashion(new FashionParams()
            {
                DisplayName = "Googly Fashion",
                InternalName = "GooglyFashion",
                Icon = LoadFashionAsset<Sprite>("iconFashionGooglyEyes"),
                DisplayPrefab = fashionGoogly[0],
                AttachPrefab = fashionGoogly[1]
            })
        });
        GeneratePodGadget(new PodParams
        {
            DisplayName = "Tiara Fashion Pod",
            InternalName = "TiaraFashionPod",
            Description = "prety",
            ItemId = GenerateFashion(new FashionParams()
            {
                DisplayName = "Tiara Fashion",
                InternalName = "TiaraFashion",
                DisplayPrefab = fashionTiara[0],
                AttachPrefab = fashionTiara[1]
            })
        });
        GeneratePodGadget(new PodParams
        {
            DisplayName = "Deal With It Fashion Pod",
            InternalName = "DealWithItFashionPod",
            Description = "em el gee",
            ItemId = GenerateFashion(new FashionParams()
            {
                DisplayName = "Deal With It Fashion",
                InternalName = "DeathWithItFashion",
                DisplayPrefab = fashionDealWithIt[0],
                AttachPrefab = fashionDealWithIt[1]
            })
        });
        GeneratePodGadget(new PodParams
        {
            DisplayName = "Loaf Fashion Pod",
            InternalName = "CursedLoafFashionPod",
            Description = "suffering",
            ItemId = GenerateFashion(new FashionParams()
            {
                DisplayName = "Loaf Fashion",
                InternalName = "CursedLoafFashion",
                DisplayPrefab = fashionCursedLoaf[0],
                AttachPrefab = fashionCursedLoaf[1]
            })
        });
        GeneratePodGadget(new PodParams
        {
            DisplayName = "Loaf Fashion Pod",
            InternalName = "LoafFashionPod",
            Description = ":3",
            ItemId = GenerateFashion(new FashionParams()
            {
                DisplayName = "Loaf Fashion",
                InternalName = "LoafFashion",
                DisplayPrefab = fashionLoaf[0],
                AttachPrefab = fashionLoaf[1]
            })
        });
        GeneratePodGadget(new PodParams
        {
            DisplayName = "Blunt Fashion Pod",
            InternalName = "BluntFashionPod",
            Description = "smok gweed everyday (don't :pleading_face:)",
            ItemId = GenerateFashion(new FashionParams()
            {
                DisplayName = "Blunt Fashion",
                InternalName = "BluntFashion",
                SlotOverride = SecondaryFaceSlot,
                DisplayPrefab = fashionBlunt[0],
                AttachPrefab = fashionBlunt[1]
            })
        });
        GeneratePodGadget(new PodParams
        {
            DisplayName = "Fashion Pod Remover",
            InternalName = "RemoverFashionPod",
            Description = "loud incorrect buzzer",
            ItemId = GenerateFashion(new FashionParams()
            {
                DisplayName = "Remover Fashion",
                InternalName = "RemoverFashion",
                DisplayPrefab = fashionRemover,
            })
        });
    }

    private static GameObject CreatePodPrefab(GadgetDefinition gadgetDefinition, ref PodParams p)
    {
        var podBase = LoadFashionAsset<GameObject>(PodBasePrefabName);
        var newPod = Object.Instantiate(podBase);
        var gadgetBehaviour = newPod.GetComponent<Gadget>();
        gadgetBehaviour.enabled = true;
        gadgetBehaviour.identType = gadgetDefinition;
        
        newPod.GetComponent<FashionPod>().fashionId.Set(p.ItemId);
        return newPod;
    }

    private static IdentifiableType GenerateFashion(FashionParams p)
    {
        var newDisplay = Object.Instantiate(p.DisplayPrefab);
        GameObject? newAttach = null;
        if (p.AttachPrefab != null)
        {
            newAttach = Object.Instantiate(p.AttachPrefab);
        }
        var ident = ScriptableObject.CreateInstance<IdentifiableType>();
        ident.MakePersistent();
        var genericIdentGroup = SRLookup.Get<IdentifiableTypeGroup>("IdentifiableTypesGroup");
        var identGroup = SRLookup.Get<IdentifiableTypeGroup>("VaccableNonLiquids");
        ident.name = p.InternalName;
        ident.Categories = new Il2CppSystem.Collections.Generic.List<IdentifiableCategory>();
        ident.groupType = genericIdentGroup;
        ident.prefab = newDisplay;
        ident.icon = p.Icon ?? ident.icon;
        ident.color = new Color(0.13f, 0.84f, 0.46f);
        ident.LocalizedName = TranslationPatcher.AddTranslation(TableName, p.InternalName, p.DisplayName);
        
        var actor = newDisplay.GetComponent<IdentifiableActor>();
        actor.identType = ident;
        
        var fashion = newDisplay.GetComponent<Fashion>();
        if (fashion != null && newAttach != null)
        {
            fashion.Slot = p.SlotOverride ?? fashion.Slot;
            fashion.AttachPrefab = newAttach;
            fashion.AttachFX = SRLookup.Get<GameObject>("FX FashionApply");
        }
        
        var fashionRemover = newDisplay.GetComponent<FashionRemover>();
        if (fashionRemover != null)
        {
            fashionRemover.RemoveFX = SRLookup.Get<GameObject>("FX FashionApply");
        }
        
        var destroyOnTouching = newDisplay.GetComponent<DestroyOnTouching>();
        destroyOnTouching.DestroyFX = SRLookup.Get<GameObject>("FX FashionBurst");
        destroyOnTouching.enabled = true;
        
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
        newGadgetDef.icon = SRLookup.Get<Sprite>("iconFashionPod");
        
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