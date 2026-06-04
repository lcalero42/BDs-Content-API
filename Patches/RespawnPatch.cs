using HarmonyLib;
using UnityEngine;
using Photon.Pun;
using Zorro.Core;

[HarmonyPatch(typeof(Level))]
static class Patch_RandomSpawn
{
    [HarmonyPatch("SetupFinished")]
    [HarmonyPostfix]
    static void SpawnAndClear()
    {
        Transform form = Level.currentLevel.patrolGroups.Values.RandomElement().RandomElement().transform;

        RespawnPlayerAtPos player = PhotonGameLobbyHandler.Instance.gameObject.AddComponent<RespawnPlayerAtPos>();
        player.position = form.transform.position + Vector3.up;
        player.rotation = form.transform.rotation;

        if (SurfaceNetworkHandler.RoomStats.LevelToPlay == 2)
        {
            foreach (GameObject item in GameObject.FindObjectsOfType<GameObject>())
            {
                if (item.name.ToLower().Contains("plank")) GameObject.Destroy(item.gameObject);
            }
        }
    }
}

internal class RespawnPlayerAtPos : MonoBehaviour
{
    internal Vector3 position = Vector3.zero;
    internal Quaternion rotation = Quaternion.identity;

    void Awake()
    {
        PlayerHandler.instance.RemovePlayer(Player.localPlayer);
        PhotonNetwork.Destroy(Player.localPlayer.gameObject);
    }

    void Start()
    {
        GameObject player = PhotonNetwork.Instantiate("Player", position, rotation, 0, null);
        Destroy(this);
    }
}
