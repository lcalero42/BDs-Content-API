using HarmonyLib;
using System.Collections;
using Zorro.Settings;
namespace DbsContentApi.Patches
{
    [HarmonyPatch(typeof(GlobalInputHandler))]
    internal class GlobalInputHandlerPatch
    {
        [HarmonyPatch(nameof(GlobalInputHandler.OnCreated))]
        [HarmonyPostfix]
        static void OnCreatedPatch(GlobalInputHandler __instance)
        {
            ApiLog.Log("[GlobalInputHandlerPatch] OnCreated triggered.");
            ApiLog.Log($"[GlobalInputHandlerPatch] Found {DbsContentApiPlugin._inputs.Count} registered inputs.");
            __instance.StartCoroutine(setModdedKeybinds());
        }
        private static IEnumerator setModdedKeybinds()
        {
            ApiLog.Log("[GlobalInputHandlerPatch] setModdedKeybinds started.");
            foreach (var keybind in DbsContentApiPlugin._inputs)
            {
                yield return null;
                ApiLog.Log($"[GlobalInputHandlerPatch] Setting keybind for: {keybind.GetType().Name}");
                var setting = GetSetting(keybind);
                ApiLog.Log($"[GlobalInputHandlerPatch] Got setting: {setting?.GetType().Name ?? "null"}");
                keybind.inputKey.SetKeybind(setting);
            }
            ApiLog.Log("[GlobalInputHandlerPatch] setModdedKeybinds finished.");
        }
        private static KeyCodeSetting GetSetting(BaseCWInput settingType)
        {
            ApiLog.Log($"[GlobalInputHandlerPatch] Looking for setting of type: {settingType.GetType().Name}");
            ApiLog.Log($"[GlobalInputHandlerPatch] Total settings: {GameHandler.Instance.SettingsHandler.settings.Count}");
            foreach (Setting setting in GameHandler.Instance.SettingsHandler.settings)
            {
                if (setting is KeyCodeSetting keyCodeSetting && keyCodeSetting == settingType)
                {
                    ApiLog.Log($"[GlobalInputHandlerPatch] Found matching setting.");
                    return keyCodeSetting;
                }
            }
            ApiLog.LogError($"[GlobalInputHandlerPatch] No matching setting found, returning settingType itself.");
            return settingType;
        }
    }
}