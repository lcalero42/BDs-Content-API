using HarmonyLib;
using UnityEngine;

namespace DbsContentApi.Patches;

/// <summary>
/// Harmony patches to enable custom map loading and selection.
/// </summary>
[HarmonyPatch(typeof(Level))]
internal static class SpawnMapPatch
{
    [HarmonyPatch("SetupFinished")]
    [HarmonyPrefix]
    public static bool Prefix()
    {
        if (SurfaceNetworkHandler.RoomStats.LevelToPlay < 3)
        {
            DbsContentApiPlugin.selectedCustomMapId = null;
            ApiLog.Log($"[Maps] Level.SetupFinished: LevelToPlay is {SurfaceNetworkHandler.RoomStats.LevelToPlay}, skipping custom map loading.");
            return true;
        }

        if (!Level.currentLevel.levelIsReady)
        {
            CustomMap? map = Maps.ResolveMapForCurrentRound();
            ApiLog.Log($"[Maps] Level.SetupFinished: LevelToPlay is {SurfaceNetworkHandler.RoomStats.LevelToPlay}, adding CustomMapLoader.");
            var loader = PhotonGameLobbyHandler.Instance.gameObject.AddComponent<CustomMapLoader>();

            if (map != null)
            {
                loader.Map = map;
                ApiLog.Log($"[Maps] Assigned map: {map.DisplayName} (Id: {map.MapId})");
            }
            else
            {
                ApiLog.LogError($"[Maps] Could not resolve custom map for LevelToPlay {SurfaceNetworkHandler.RoomStats.LevelToPlay} (selected id: {DbsContentApiPlugin.selectedCustomMapId ?? "none"})");
            }

            DbsContentApiPlugin.selectedCustomMapId = null;
        }

        return Level.currentLevel.levelIsReady;
    }
}

[HarmonyPatch(typeof(RoomStatsHolder))]
internal static class ChooseMapPatch
{
    [HarmonyPatch("NewMapToPlay")]
    [HarmonyPrefix]
    public static bool Prefix(RoomStatsHolder __instance)
    {
        if (DbsContentApiPlugin.customMaps.Count == 0)
        {
            ApiLog.Log("[Maps] No custom maps registered, using vanilla selection.");
            return true;
        }

        ApiLog.Log($"[Maps] Choosing new map. Registered: {DbsContentApiPlugin.customMaps.Count}. Modded only: {DbsContentApiPlugin.moddedMapsOnly}");

        CustomMap? picked = Maps.PickMapForNextRound(__instance);
        if (picked == null)
        {
            int nextLevel = UnityEngine.Random.Range(0, 3);
            int attempts = 0;
            while (attempts < 100 && nextLevel == __instance.LevelToPlay)
            {
                nextLevel = UnityEngine.Random.Range(0, 3);
                attempts++;
            }
            __instance.LevelToPlay = nextLevel;
            DbsContentApiPlugin.selectedCustomMapId = null;
            ApiLog.Log($"[Maps] Selected vanilla map: {nextLevel}");
            return false;
        }

        DbsContentApiPlugin.selectedCustomMapId = picked.MapId;
        __instance.LevelToPlay = Maps.GetLevelIndexForMap(picked);
        ApiLog.Log($"[Maps] Selected custom map: {picked.DisplayName} (Id: {picked.MapId}, LevelToPlay: {__instance.LevelToPlay})");
        return false;
    }
}

[HarmonyPatch(typeof(DivingBell))]
internal static class FacilityTemplatePatch
{
    [HarmonyPatch("GetUndergroundSceneName")]
    [HarmonyPrefix]
    public static bool Prefix(ref string __result)
    {
        if (SurfaceNetworkHandler.RoomStats.LevelToPlay >= 3)
        {
            ApiLog.Log("[Maps] Forcing underground scene name to FactoryScene for custom level.");
            __result = "FactoryScene";
            return false;
        }
        return true;
    }
}

[HarmonyPatch(typeof(BigNumbers))]
internal static class MultiplierPatches
{
    [HarmonyPatch("GetMonsterBudgetForDayFirstWave")]
    [HarmonyPostfix]
    public static void FirstRoundMonster(ref int __result)
    {
        if (SurfaceNetworkHandler.RoomStats.LevelToPlay >= 3)
        {
            int old = __result;
            __result = (int)(__result * CustomMapLoader.multiplierMonster);
            ApiLog.Log($"[Maps] Applied monster multiplier (First Wave): {old} -> {__result} (x{CustomMapLoader.multiplierMonster})");
        }
    }

    [HarmonyPatch("GetMonsterBudgetForSecondWave")]
    [HarmonyPostfix]
    public static void SecondRoundMonster(ref int __result)
    {
        if (SurfaceNetworkHandler.RoomStats.LevelToPlay >= 3)
        {
            int old = __result;
            __result = (int)(__result * CustomMapLoader.multiplierMonster);
            ApiLog.Log($"[Maps] Applied monster multiplier (Second Wave): {old} -> {__result} (x{CustomMapLoader.multiplierMonster})");
        }
    }

    [HarmonyPatch("GetToolBudgetForDay")]
    [HarmonyPostfix]
    public static void RoundTool(ref int __result)
    {
        if (SurfaceNetworkHandler.RoomStats.LevelToPlay >= 3)
        {
            int old = __result;
            __result = (int)(__result * CustomMapLoader.multiplierTool);
            ApiLog.Log($"[Maps] Applied tool multiplier: {old} -> {__result} (x{CustomMapLoader.multiplierTool})");
        }
    }

    [HarmonyPatch("GetArtifactBudgetForDay")]
    [HarmonyPostfix]
    public static void RoundArtifact(ref int __result)
    {
        if (SurfaceNetworkHandler.RoomStats.LevelToPlay >= 3)
        {
            int old = __result;
            __result = (int)(__result * CustomMapLoader.multiplierArtifact);
            ApiLog.Log($"[Maps] Applied artifact multiplier: {old} -> {__result} (x{CustomMapLoader.multiplierArtifact})");
        }
    }
}

[HarmonyPatch(typeof(Player))]
internal static class PlayerHeightPatch
{
    [HarmonyPatch("Center")]
    [HarmonyPostfix]
    public static void Postfix(ref Vector3 __result)
    {
        if (SurfaceNetworkHandler.RoomStats.LevelToPlay >= 3)
        {
            if (__result.y > 100f)
            {
                ApiLog.Log($"[Maps] Capping player height: {__result.y} -> 100f");
                __result.y = 100f;
            }
        }
    }
}

[HarmonyPatch(typeof(VideoCamera))]
internal static class VideoOcclusionPatch
{
    [HarmonyPatch("ConfigItem")]
    [HarmonyPostfix]
    public static void Postfix(VideoCamera __instance)
    {
        if (SurfaceNetworkHandler.RoomStats.LevelToPlay >= 3)
        {
            ApiLog.Log("[Maps] Disabling occlusion culling for VideoCamera on custom level.");
            __instance.m_camera.useOcclusionCulling = false;
        }
    }
}
