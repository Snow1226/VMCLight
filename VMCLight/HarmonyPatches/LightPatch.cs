using HarmonyLib;
using UnityEngine;
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
                Plugin.Instance.LightController.ActiveLightData.Color = Color.Lerp(color, Plugin.Instance.LightController.MixColor, Plugin.Instance.LightController.BlendLevel);
                Plugin.Instance.LightController.ActiveLightData.Enabled = color.a == 0f ? false : true;
            }
            else
            {
                Plugin.Instance.LightController.ActiveLightData.Color = new Color(1f, 1f, 1f, 1f);
                Plugin.Instance.LightController.ActiveLightData.Enabled = true;
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
                Plugin.Instance.LightController.ActiveLightData.Color = Color.Lerp(color, Plugin.Instance.LightController.MixColor, Plugin.Instance.LightController.BlendLevel);
                Plugin.Instance.LightController.ActiveLightData.Enabled = color.a <= 0.1f ? false : true;
            }
            else
            {
                Plugin.Instance.LightController.ActiveLightData.Color = new Color(1f, 1f, 1f, 1f);
                Plugin.Instance.LightController.ActiveLightData.Enabled = true;
            }
        }
    }
}