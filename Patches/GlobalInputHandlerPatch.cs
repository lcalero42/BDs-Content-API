using DbsContentApi.Modules;
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
            Logger.Log("[GlobalInputHandlerPatch] OnCreated triggered.");
            Logger.Log($"[GlobalInputHandlerPatch] Found {DbsContentApiPlugin._inputs.Count} registered inputs.");
            __instance.StartCoroutine(setModdedKeybinds());
        }
        private static IEnumerator setModdedKeybinds()
        {
            Logger.Log("[GlobalInputHandlerPatch] setModdedKeybinds started.");
            foreach (var keybind in DbsContentApiPlugin._inputs)
            {
                yield return null;
                Logger.Log($"[GlobalInputHandlerPatch] Setting keybind for: {keybind.GetType().Name}");
                var setting = GetSetting(keybind);
                Logger.Log($"[GlobalInputHandlerPatch] Got setting: {setting?.GetType().Name ?? "null"}");
                keybind.inputKey.SetKeybind(setting);
            }
            Logger.Log("[GlobalInputHandlerPatch] setModdedKeybinds finished.");
        }
        private static KeyCodeSetting GetSetting(BaseCWInput settingType)
        {
            Logger.Log($"[GlobalInputHandlerPatch] Looking for setting of type: {settingType.GetType().Name}");
            Logger.Log($"[GlobalInputHandlerPatch] Total settings: {GameHandler.Instance.SettingsHandler.settings.Count}");
            foreach (Setting setting in GameHandler.Instance.SettingsHandler.settings)
            {
                if (setting is KeyCodeSetting keyCodeSetting && keyCodeSetting == settingType)
                {
                    Logger.Log($"[GlobalInputHandlerPatch] Found matching setting.");
                    return keyCodeSetting;
                }
            }
            Logger.LogWarning($"[GlobalInputHandlerPatch] No matching setting found, returning settingType itself.");
            return settingType;
        }
    }
}