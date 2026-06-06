using System.Linq;
using HarmonyLib;

namespace DbsContentApi.Patches;

/// <summary>
/// Adds API-registered custom artifacts to the game's artifact spawn table.
/// </summary>
[HarmonyPatch(typeof(RoundArtifactSpawner))]
internal static class RoundArtifactSpawnerPatch
{
    [HarmonyPatch("Start")]
    [HarmonyPrefix]
    private static void StartPrefix(RoundArtifactSpawner __instance)
    {
        if (DbsContentApiPlugin.customArtifacts.Count == 0)
        {
            return;
        }

        Item[] currentSpawns = __instance.possibleSpawns ?? new Item[0];
        Item[] customArtifacts = DbsContentApiPlugin.customArtifacts
            .Where(item => item != null && !currentSpawns.Any(existing => IsSameItem(existing, item)))
            .ToArray();

        if (customArtifacts.Length == 0)
        {
            return;
        }

        __instance.possibleSpawns = currentSpawns.Concat(customArtifacts).ToArray();
        ApiLog.Log($"RoundArtifactSpawnerPatch: Added {customArtifacts.Length} custom artifacts.");
    }

    private static bool IsSameItem(Item? first, Item second)
    {
        return first != null &&
               (first == second ||
                first.persistentID == second.persistentID ||
                first.displayName == second.displayName);
    }
}
