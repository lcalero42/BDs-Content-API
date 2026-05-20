using HarmonyLib;
using UnityEngine;
using System.Linq;
using Photon.Pun;
using System.Collections.Generic;
using DefaultNamespace;
namespace DbsContentApi.Patches;

/// <summary>
/// Harmony patches for the RoundSpawner class.
/// </summary>
[HarmonyPatch(typeof(RoundSpawner))]
internal static class RoundSpawnerPatch
{
    /// <summary>
    /// Intercepts the Start method of the RoundSpawner to register modded monsters and adjust spawn rates.
    /// </summary>
    [HarmonyPatch("Start")]
    [HarmonyPrefix]
    public static void StartPrefix(RoundSpawner __instance)
    {

        ApiLog.Log("RoundSpawnerPatch: Registering custom monsters in Photon pool.");
        ApiLog.Log("RoundSpawnerPatch: Modded mobs only: " + DbsContentApiPlugin.moddedMobsOnly);
        ApiLog.Log("RoundSpawnerPatch: Custom monsters: " + DbsContentApiPlugin.customMonsters.Count);

        foreach (var monster in DbsContentApiPlugin.customMonsters)
        {
            ApiLog.Log("RoundSpawnerPatch: Registering custom monster: " + monster.name);
            ContentLoader.RegisterPrefabInPhotonPool(monster);
        }

        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        var traverse = Traverse.Create(__instance);
        var possibleSpawnsField = traverse.Field<GameObject[]>("possibleSpawns");

        if (DbsContentApiPlugin.moddedMobsOnly)
        {
            // lets repeat the array 2 times since the updated game code now removes 1 monster from the array
            var customMonsters = DbsContentApiPlugin.customMonsters;
            for (int i = 0; i < 2; i++)
            {
                customMonsters.AddRange(customMonsters);
            }
            possibleSpawnsField.Value = DbsContentApiPlugin.customMonsters.ToArray();
            ApiLog.Log("RoundSpawnerPatch: Modded mobs only: " + possibleSpawnsField.Value.Length);
        }
        else
        {
            possibleSpawnsField.Value = possibleSpawnsField.Value.Concat(DbsContentApiPlugin.customMonsters).ToArray();
        }
    }
}
