using IPA;
using IPA.Config;
using IPA.Config.Stores;
using IPA.Loader;
using IpaLogger = IPA.Logging.Logger;
using HarmonyLib;
using System.Reflection;
using UnityEngine;
using VMCLight.Configuration;
using VMCLight.UI;

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
    public Plugin(Config config,IpaLogger ipaLogger)
    {
        Instance = this;
        PluginConfig.Instance = config.Generated<Configuration.PluginConfig>();
        Log = ipaLogger;
    }

    [OnStart]
    public void OnApplicationStart()
    {
        existChroma = PluginManager.GetPlugin("Chroma") == null ? false : true;
        
        _harmony = new Harmony("com.snow1226.beatsaber.vmclight");
        _harmony.PatchAll(Assembly.GetExecutingAssembly());
        LightController = new GameObject("VMCLightController").AddComponent<VMCLightController>();
        BeatSaberMarkupLanguage.Util.MainMenuAwaiter.MainMenuInitializing += MenuInit;
    }

    public void MenuInit()
    {
        UIManager.Init();
    }

    [OnExit]
    public void OnApplicationQuit()
    {
        _harmony?.UnpatchSelf();
    }
}