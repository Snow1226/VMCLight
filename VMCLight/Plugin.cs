using IPA;
using IPA.Loader;
using IpaLogger = IPA.Logging.Logger;
using HarmonyLib;
using System.Reflection;
using UnityEngine;

namespace VMCLight;

[Plugin(RuntimeOptions.DynamicInit)]
internal class Plugin
{
    public static Plugin Instance  { get; private set; }
    public VMCLightController LightController { get; private set; }

    internal static IpaLogger Log { get; private set; } = null!;
    private Harmony _harmony;
    internal bool existChroma;
    [Init]
    public Plugin(IpaLogger ipaLogger, PluginMetadata pluginMetadata)
    {
        Instance = this;
        Log = ipaLogger;
        Log.Info($"{pluginMetadata.Name} {pluginMetadata.HVersion} initialized.");
    }

    [OnStart]
    public void OnApplicationStart()
    {
        existChroma = PluginManager.GetPlugin("Chroma") == null ? false : true;
        
        _harmony = new Harmony("com.snow1226.beatsaber.vmclight");
        _harmony.PatchAll(Assembly.GetExecutingAssembly());
        LightController = new GameObject("VMCLightController").AddComponent<VMCLightController>();
    }

    [OnExit]
    public void OnApplicationQuit()
    {
        _harmony?.UnpatchSelf();
    }
}