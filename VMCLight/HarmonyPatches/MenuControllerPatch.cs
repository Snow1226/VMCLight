using HarmonyLib;
using UnityEngine;
using UnityEngine.XR;
using UnityEngine.SceneManagement;
using VMCLight.Configuration;

namespace VMCLight.HarmonyPatches;

[HarmonyPatch(typeof(VRController))]
public static class MenuControllerPatch
{
    public static bool EnableVRController = false;
    public static GameObject LeftSaber;
    public static GameObject RightSaber;
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(VRController), "OnEnable", MethodType.Normal)]
    public static void EnablePostfix(VRController __instance, XRNode ____node)
    {
        EnableVRController = true;
        if(____node == XRNode.LeftHand)
            LeftSaber = __instance.gameObject;
        else
            RightSaber = __instance.gameObject;
    }
    
    [HarmonyPostfix]
    [HarmonyPatch(typeof(VRController), "OnDisable", MethodType.Normal)]
    public static void DisablePostfix(VRController __instance, XRNode ____node)
    {
        EnableVRController = false;
        if(____node == XRNode.LeftHand)
            LeftSaber = null;
        else
            RightSaber = null;
    }

}
