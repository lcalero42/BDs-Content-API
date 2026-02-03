using System.Collections.Generic;
using BepInEx;
using HarmonyLib;
using UnityEngine;
using DbsContentApi.Modules;
using System;

namespace DbsContentApi;

[ContentWarningPlugin("db.contentapi", "1.0.0", false)]
[BepInPlugin("db.contentapi", "Db's content API", "1.0.0")]
public class DbsContentApiPlugin : BaseUnityPlugin
{
    internal static Harmony? Harmony { get; set; }
    public static DbsContentApiPlugin Instance { get; private set; } = null!;
    /// <summary>
    /// Global list of registered custom monsters.
    /// </summary>
    public static List<GameObject> customMonsters = new List<GameObject>();
    // public const ShopItemCategory WeaponsCategory = (ShopItemCategory)8;

    public static List<Action> customItemsRegistrationCallbacks = new List<Action>();
    public static List<ContentEvent> customContentEvents = new List<ContentEvent>();

    /// <summary>
    /// If true, only modded monsters will spawn in the round.
    /// </summary>
    public static bool moddedMobsOnly = false;
    public static bool allItemsFree = false;

    private void Awake()
    {
        Modules.Logger.Log("DbsContentApi API Initializing... [POST UPDATE]");

        Instance = this;

        Patch();

        GameMaterials.InitMaterials();

        Modules.Logger.Init(base.Logger);

        Modules.Logger.Log("DbsContentApi API Loaded successfully!");
    }

    internal static void Patch()
    {
        Harmony ??= new Harmony("db.contentapi");

        Modules.Logger.Log("Patching...");

        Harmony.PatchAll();

        Modules.Logger.Log("Finished patching!");
    }

    internal static void Unpatch()
    {
        Modules.Logger.Log("Unpatching...");

        Harmony?.UnpatchSelf();

        Modules.Logger.Log("Finished unpatching!");
    }

    public static void SetModdedMobsOnly(bool value)
    {
        moddedMobsOnly = value;
    }
    public static void SetAllItemsFree(bool value)
    {
        allItemsFree = value;
    }
}
