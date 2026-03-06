using HarmonyLib;
using UnityEngine;
using VMCLight.Configuration;

namespace VMCLight.HarmonyPatches;

[HarmonyPatch(typeof(LightWithIdManager), "SetColorForId", MethodType.Normal)]
public static class InGameLightPatch
{
    public static void Postfix(LightWithIdManager __instance,int lightId, Color color,Color[] ____colors)
    {
        if (____colors[lightId] != null)
        {
            if (Plugin.Instance.LightController.inGameCoreScene)
            {
                Plugin.Instance.LightController.ActiveLightData.Color = 
                    Color.Lerp(color, PluginConfig.Instance.BlendColor, PluginConfig.Instance.BlendIntensity);
            }
            else
            {
                Plugin.Instance.LightController.ActiveLightData.Color = new Color(1f, 1f, 1f, 1f);
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
            if(Plugin.Instance.LightController.inGameCoreScene)
            {
                Plugin.Instance.LightController.ActiveLightData.Color = 
                    Color.Lerp(color, PluginConfig.Instance.BlendColor, PluginConfig.Instance.BlendIntensity);
            }
            else
            {
                Plugin.Instance.LightController.ActiveLightData.Color = new Color(1f, 1f, 1f, 1f);
            }
        }
    }
}