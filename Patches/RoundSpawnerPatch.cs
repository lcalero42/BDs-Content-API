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

        ApiLog.Log("RoundSpawnerPatch: possibleSpawnsField: " + possibleSpawnsField.Value.Length);


        if (DbsContentApiPlugin.moddedMobsOnly)
        {
            // Repeat registered monsters 4x in a local list — updated game code removes one entry from the pool.
            GameObject[] registered = DbsContentApiPlugin.customMonsters.ToArray();
            var spawnPool = new List<GameObject>(registered.Length * 4);
            for (int i = 0; i < 4; i++)
            {
                spawnPool.AddRange(registered);
            }

            possibleSpawnsField.Value = spawnPool.ToArray();
            ApiLog.Log("RoundSpawnerPatch: Modded mobs only: " + possibleSpawnsField.Value.Length);
        }
        else
        {
            possibleSpawnsField.Value = possibleSpawnsField.Value.Concat(DbsContentApiPlugin.customMonsters).ToArray();
        }
    }
}
