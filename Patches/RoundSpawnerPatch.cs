using HarmonyLib;
using UnityEngine;
using System.Linq;
using DbsContentApi.Modules;
using Photon.Pun;
using System.Collections.Generic;
using DefaultNamespace;
using Logger = DbsContentApi.Modules.Logger;

namespace DbsContentApi.Patches;

/// <summary>
/// Harmony patches for the RoundSpawner class.
/// </summary>
[HarmonyPatch(typeof(RoundSpawner))]
public static class RoundSpawnerPatch
{
    /// <summary>
    /// Intercepts the Start method of the RoundSpawner to register modded monsters and adjust spawn rates.
    /// </summary>
    [HarmonyPatch("Start")]
    [HarmonyPrefix]
    public static void StartPrefix(RoundSpawner __instance)
    {

        Logger.Log("RoundSpawnerPatch: Registering custom monsters in Photon pool.");
        Logger.Log("RoundSpawnerPatch: Modded mobs only: " + DbsContentApiPlugin.moddedMobsOnly);
        Logger.Log("RoundSpawnerPatch: Custom monsters: " + DbsContentApiPlugin.customMonsters.Count);

        foreach (var monster in DbsContentApiPlugin.customMonsters)
        {
            Logger.Log("RoundSpawnerPatch: Registering custom monster: " + monster.name);
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
            possibleSpawnsField.Value = DbsContentApiPlugin.customMonsters.ToArray();
            Logger.Log("RoundSpawnerPatch: Modded mobs only: " + possibleSpawnsField.Value.Length);
        }
        else
        {
            possibleSpawnsField.Value = possibleSpawnsField.Value.Concat(DbsContentApiPlugin.customMonsters).ToArray();
        }
    }
}
