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
            __instance.StartCoroutine(setModdedKeybinds());
        }

        private static IEnumerator setModdedKeybinds()
        {
            foreach(var keybind in Plugin._inputs)
            {
                yield return null;
                keybind.inputKey.SetKeybind(GetSetting(keybind));
            }
        }

        private static KeyCodeSetting GetSetting(BaseCWInput settingType)
        {
            foreach(Setting setting in GameHandler.Instance.SettingsHandler.settings)
            {
                if(setting is KeyCodeSetting keyCodeSetting && keyCodeSetting == settingType)
                {
                    return keyCodeSetting;
                }
            }
            return settingType;
        }
    }
}
