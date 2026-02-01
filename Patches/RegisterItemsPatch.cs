using HarmonyLib;
using UnityEngine;
using System;
using Zorro.Core;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using System.Reflection;
using DbsContentApi;

namespace DbsContentApi.Patches;

[HarmonyPatch(typeof(ShopViewScreen))]
public static class ShopViewScreenPatch
{

	[HarmonyPatch("Awake"), HarmonyPostfix]
	static void AwakePatch(ShopViewScreen __instance)
	{
		Modules.Logger.Log("RegisterItemsPatch: Registering custom items.");
		DbsContentApi.DbsContentApiPlugin.customItemsRegistrationCallbacks.ForEach(callback => callback());
	}
}
