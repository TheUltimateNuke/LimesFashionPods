using Il2Cpp;
using LimesFashionPods;
using MelonLoader;
using UnityEngine;
using UnityEngine.SceneManagement;

[assembly: MelonInfo(typeof(Entrypoint), MyModInfo.Name, MyModInfo.Version, MyModInfo.Author)]
[assembly: MelonGame("MonomiPark", "SlimeRancher2")]

namespace LimesFashionPods;

public class Entrypoint : MelonMod
{
    internal static MelonLogger.Instance Logger => Melon<Entrypoint>.Logger;
    
    public override void OnInitializeMelon()
    {
        LoggerInstance.Msg(System.ConsoleColor.Green, $"Mod \"{MyModInfo.Name}\" initialized OK");
    }
}