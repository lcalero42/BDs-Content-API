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
public static class ItemCategoriesPatch
{
    // [HarmonyPatch("DrawCategories"), HarmonyPrefix]
    // static bool DrawCategoriesPatch(ShopViewScreen __instance)
    // {
    //     Modules.Logger.Log("ItemCategoriesPatch: Registering custom categories.");
    //     __instance.ResetScreenIndex();
    //     __instance.DestroyCategoryGrid();

    //     ShopItemCategory[] array = (ShopItemCategory[])Enum.GetValues(typeof(ShopItemCategory));
    //     // array = array.Concat(new[] { DbsContentApiPlugin.WeaponsCategory }).ToArray();
    //     array = array.Concat(Modules.Items.customCategories.Select(c => (ShopItemCategory)c.index)).ToArray();

    //     foreach (ShopItemCategory shopItemCategory in array)
    //     {
    //         if (shopItemCategory != ShopItemCategory.Invalid && !__instance.excludeCategories.Contains(shopItemCategory))
    //         {
    //             __instance.SpawnCategoryCell(shopItemCategory);
    //         }
    //     }
    //     return false;
    // }

    [HarmonyPatch("DrawCategories"), HarmonyPrefix]
    static bool DrawCategoriesPatch(ShopViewScreen __instance)
    {
        // Modules.Logger.Log("ItemCategoriesPatch: Registering custom categories.");
        __instance.ResetScreenIndex();
        __instance.DestroyCategoryGrid();

        ShopItemCategory[] array = (ShopItemCategory[])Enum.GetValues(typeof(ShopItemCategory));
        // array = array.Concat(new[] { DbsContentApiPlugin.WeaponsCategory }).ToArray();
        array = array.Concat(Modules.Items.customCategories.Select(c => (ShopItemCategory)c.index)).ToArray();

        foreach (ShopItemCategory shopItemCategory in array)
        {
            if (shopItemCategory != ShopItemCategory.Invalid && !__instance.excludeCategories.Contains(shopItemCategory))
            {
                __instance.SpawnCategoryCell(shopItemCategory);
            }
        }
        return false;
    }

    [HarmonyPatch("UpdateViewScreen"), HarmonyPostfix]
    static void UpdateViewScreenPostfix(ShopViewScreen __instance)
    {
        // if (__instance.CurrentCategoryIndex == DbsContentApiPlugin.WeaponsCategory)
        // {
        //     __instance.m_CurrentCategoryNameText.text = "Weapons";
        // }
        foreach (var customCategory in Modules.Items.customCategories)
        {
            if ((byte)__instance.CurrentCategoryIndex == customCategory.index)
            {
                __instance.m_CurrentCategoryNameText.text = customCategory.name;
            }
        }
    }
}


[HarmonyPatch(typeof(ShopInteractibleCategory))]
public static class ShopInteractibleCategoryPatch
{

    [HarmonyPatch("Setup"), HarmonyPrefix]
    static bool SetupPrefix(ShopInteractibleCategory __instance, ShopHandler handler, ShopItemCategory category)
    {
        // if (category == DbsContentApiPlugin.WeaponsCategory)
        // {
        //     ShopInteractibleCategory.m_ShopHandler = handler;
        //     __instance.m_Category = category;
        //     __instance.hoverText = "Weapons";
        //     __instance.m_CategoryText.text = "Weapons";
        //     return false;
        // }
        foreach (var customCategory in Modules.Items.customCategories)
        {
            if ((byte)category == customCategory.index)
            {
                ShopInteractibleCategory.m_ShopHandler = handler;
                __instance.m_Category = category;
                __instance.hoverText = customCategory.name;
                __instance.m_CategoryText.text = customCategory.name;
                return false;
            }
        }
        return true;
    }
}


