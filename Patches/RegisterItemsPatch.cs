using HarmonyLib;

namespace DbsContentApi.Patches;

[HarmonyPatch(typeof(ShopViewScreen))]
internal static class ShopViewScreenPatch
{
	public static bool isRegistered = false;
	[HarmonyPatch("Awake"), HarmonyPostfix]
	static void AwakePatch(ShopViewScreen __instance)
	{
		if (isRegistered)
			return;
		isRegistered = true;

		GameMaterials.InitMaterials();

		ApiLog.Log("RegisterItemsPatch: Registering custom items.");
		DbsContentApiPlugin.customItemsRegistrationCallbacks.ForEach(callback => callback());

		if (DbsContentApiPlugin.allItemsFree)
		{
			ApiLog.Log("RegisterItemsPatch: Setting all items free.");
			Items.SetAllItemsFree();
		}
	}
}
