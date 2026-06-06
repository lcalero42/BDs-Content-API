using UnityEngine;
using Photon.Pun;

namespace DbsContentApi.Patches;

internal class RespawnPlayerAtPos : MonoBehaviour
{
    internal Vector3 position = Vector3.zero;
    internal Quaternion rotation = Quaternion.identity;

    internal static void RebootLocalPlayerAt(Vector3 position, Quaternion rotation)
    {
        if (PhotonGameLobbyHandler.Instance == null)
        {
            ApiLog.LogError("[Respawn] PhotonGameLobbyHandler is not available; cannot reboot player.");
            return;
        }

        ApiLog.Log("[Respawn] Rebooting local player at " + position + " with rotation " + rotation);

        RespawnPlayerAtPos player = PhotonGameLobbyHandler.Instance.gameObject.AddComponent<RespawnPlayerAtPos>();
        player.position = position;
        player.rotation = rotation;
    }

    void Awake()
    {
        if (Player.localPlayer != null)
        {
            PlayerHandler.instance.RemovePlayer(Player.localPlayer);
            PhotonNetwork.Destroy(Player.localPlayer.gameObject);
        }
    }

    void Start()
    {
        PhotonNetwork.Instantiate("Player", position, rotation, 0, null);
        Destroy(this);
    }
}
