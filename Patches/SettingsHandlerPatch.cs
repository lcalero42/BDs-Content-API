using System.Collections.Generic;
using DbsContentApi.Modules;
using HarmonyLib;
using Zorro.Settings;

namespace DbsContentApi.Patches
{
    [HarmonyPatch(typeof(SettingsHandler))]
    internal static class SettingsHandlerPatch
    {
        [HarmonyPatch(MethodType.Constructor)]
        [HarmonyPostfix]
        private static void ConstructorPostfix(SettingsHandler __instance)
        {
            Logger.Log("[SettingsHandlerPatch] Constructor postfix triggered.");
            Logger.Log($"[SettingsHandlerPatch] Registering {DbsContentApiPlugin._inputs.Count} custom inputs.");
            foreach (BaseCWInput input in DbsContentApiPlugin._inputs)
            {
                __instance.AddSetting(input);
                Logger.Log($"[SettingsHandlerPatch] Registered input: {input.GetType().Name}");
            }
        }
    }
}