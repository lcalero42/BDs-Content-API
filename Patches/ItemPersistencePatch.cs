using HarmonyLib;

namespace DbsContentApi.Patches;

[HarmonyPatch(typeof(SaveLoadHandler))]
internal static class ItemPersistencePatch
{
    [HarmonyPatch("SerializeInventoryItems")]
    [HarmonyPostfix]
    static void SerializeInventoryItemsPostfix(ref SavedInventoryItem[] __result)
    {
        __result = ItemPersistence.AppendProviderItems(__result);
    }
}
