using HarmonyLib;

namespace DbsContentApi.Patches;

[HarmonyPatch(typeof(ShopViewScreen))]
public static class ShopViewScreenPatch
{
	public static bool isRegistered = false;
	[HarmonyPatch("Awake"), HarmonyPostfix]
	static void AwakePatch(ShopViewScreen __instance)
	{
		if (isRegistered)
			return;
		isRegistered = true;
		Modules.Logger.Log("RegisterItemsPatch: Registering custom items.");
		DbsContentApiPlugin.customItemsRegistrationCallbacks.ForEach(callback => callback());

		if (DbsContentApiPlugin.allItemsFree)
		{
			Modules.Logger.Log("RegisterItemsPatch: Setting all items free.");
			Modules.Items.SetAllItemsFree();
		}
	}
}
