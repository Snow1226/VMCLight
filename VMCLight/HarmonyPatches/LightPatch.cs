using HarmonyLib;
using UnityEngine;
using UnityEngine.SceneManagement;
using VMCLight.Configuration;

namespace VMCLight.HarmonyPatches;

[HarmonyPatch(typeof(LightWithIdManager), "SetColorForId", MethodType.Normal)]
public static class InGameLightPatch
{
    public static void Postfix(LightWithIdManager __instance,int lightId, Color color,Color[] ____colors)
    {
        if (____colors[lightId] != null)
        {
            if (SceneManager.GetActiveScene().name == Plugin.GameSceneName)
            {
                Plugin.Log.Notice("harmony GenuCore");
                Plugin.Instance.LightController.ActiveLightData.Color = 
                    Color.Lerp(color, PluginConfig.Instance.BlendColor, PluginConfig.Instance.BlendIntensity);
            }
            else
            {
                Plugin.Log.Notice("harmony MenuCore");
                Plugin.Instance.LightController.ActiveLightData.Color = PluginConfig.Instance.BlendColor;
            }
        }
    }
}

[HarmonyPatch(typeof(Chroma.Lighting.ChromaIDColorTween), "SetColor", MethodType.Normal)]
public static class ChromaLightPatch
{
    public static void Postfix(Chroma.Lighting.ChromaIDColorTween __instance, ILightWithId ____lightWithId, Color color)
    {
        if (Plugin.Instance.existChroma)
        {
            if (SceneManager.GetActiveScene().name == Plugin.GameSceneName)
            {
                Plugin.Log.Notice("harmony GenuCore");
                Plugin.Instance.LightController.ActiveLightData.Color = 
                    Color.Lerp(color, PluginConfig.Instance.BlendColor, PluginConfig.Instance.BlendIntensity);
                
            }
            else
            {
                Plugin.Log.Notice("harmony MenuCore");
                Plugin.Instance.LightController.ActiveLightData.Color = PluginConfig.Instance.BlendColor;
            }
        }
    }
}