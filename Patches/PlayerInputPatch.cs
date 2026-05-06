using HarmonyLib;

namespace DbsContentApi.Patches
{
    [HarmonyPatch(typeof(Player.PlayerInput))]
    internal class PlayerInputPatch
    {
        [HarmonyPatch(nameof(Player.PlayerInput.SampeInput))]
        [HarmonyPostfix]
        private static void SampeInput(Player.PlayerData data, Player player)
        {
            if (player != Player.localPlayer) return; // Only handle for local player

            foreach (var moddedInputs in DbsContentApiPlugin._inputs)
            {
                moddedInputs.HandleKeys(player);
            }
        }
    }
}
