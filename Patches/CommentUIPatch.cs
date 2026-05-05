using System.Collections.Generic;
using DbsContentApi.Modules;
using HarmonyLib;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;
using Logger = DbsContentApi.Modules.Logger;

namespace DbsContentApi.Patches
{
    [HarmonyPatch(typeof(CommentUI))]
    internal static class CommentUIPatch
    {
        private static bool _applied;

        [HarmonyPatch(nameof(CommentUI.Awake))]
        [HarmonyPostfix]
        private static void AwakePostfix()
        {
            if (_applied) return;

            var s_tables = AccessTools.StaticFieldRefAccess<CommentUI, Dictionary<LocaleIdentifier, StringTable>>("s_tables");

            if (s_tables == null)
            {
                Logger.LogError("[CommentUIPatch] s_tables is null in CommentUI.Awake postfix. Localization might not be loaded yet.");
                return;
            }

            CustomCommentRegistry.ApplyToStringTables(s_tables);
            _applied = true;
        }
    }
}
