using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;

namespace VMCLight.HarmonyPatches;

[HarmonyPatch(typeof(ColorManager), nameof(ColorManager.ColorForSaberType))]
public static class ColorManagerPatch
{
    public static Color SaberColorA;
    public static Color SaberColorB;

    public static void Postfix(ColorManager __instance, ColorScheme ____colorScheme)
    {
        SaberColorA = ____colorScheme.saberAColor;
        SaberColorB = ____colorScheme.saberBColor;
    }
}