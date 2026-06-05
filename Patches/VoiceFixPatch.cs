using System.Reflection;
using HarmonyLib;

namespace DbsContentApi.Patches;

[HarmonyPatch(typeof(PlayerVoiceHandler))]
internal static class Patch_VoiceFix
{
    private static readonly PropertyInfo? InstanceProperty =
        typeof(PlayerVoiceHandler).GetProperty("Instance", BindingFlags.Public | BindingFlags.Static);

    [HarmonyPatch("Update")]
    [HarmonyPrefix]
    private static void FixEmotes(PlayerVoiceHandler __instance)
    {
        if (PlayerVoiceHandler.Instance == null)
            InstanceProperty?.SetValue(null, __instance);
    }
}
