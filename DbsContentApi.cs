using System;
using System.Collections.Generic;
using System.Reflection;
using HarmonyLib;
using UnityEngine;

namespace DbsContentApi;

/// <summary>
///     Main plugin class for DbsContentApi.
/// </summary>
[ContentWarningPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION, false)]
public class DbsContentApiPlugin
{
    private const string ApiAssetBundleFileName = "dbscontentapi";

    private bool _isPatched;

    static DbsContentApiPlugin()
    {
        // Create new instance
        Instance = new DbsContentApiPlugin();
    }

    /// <summary>
    ///     Constructor for the DbsContentApi plugin.
    /// </summary>
    public DbsContentApiPlugin()
    {
        LoadApiAssetBundle();
        ApiLog.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    /// <summary>
    /// Asset bundle shipped alongside DbsContentApi.dll (filename: dbscontentapi, no extension).
    /// </summary>
    internal static AssetBundle? ApiAssetBundle { get; private set; }

    /// <summary>
    /// Prefab used for temporary content-trigger volumes (loaded from the dbscontentapi bundle).
    /// </summary>
    public static GameObject? TemporaryContentTriggerPrefab { get; private set; }

    private static void LoadApiAssetBundle()
    {
        string location = Assembly.GetExecutingAssembly().Location;
        ApiAssetBundle = ContentLoader.LoadAssetBundle(location, ApiAssetBundleFileName);
        if (ApiAssetBundle == null)
        {
            ApiLog.LogError(
                $"DbsContentApi: failed to load asset bundle '{ApiAssetBundleFileName}' next to {location}");
            return;
        }

        try
        {
            TemporaryContentTriggerPrefab =
                ContentLoader.LoadPrefabFromBundle(ApiAssetBundle, "TemporaryContentProviderCube.prefab");
        }
        catch (Exception e)
        {
            ApiLog.LogError(
                $"DbsContentApi: failed to load prefab TemporaryContentProviderCube from bundle: {e.Message}");
        }
    }

    private Harmony? Harmony { get; set; }
    /// <summary>
    ///     Singleton instance of the DbsContentApi plugin.
    /// </summary>
    public static DbsContentApiPlugin Instance { get; }

    internal static List<GameObject> customMonsters = new();
    internal static List<CustomMap> customMaps = new();
    internal static List<Action> customItemsRegistrationCallbacks = new();
    internal static List<ContentEvent> customContentEvents = new();

    internal static bool moddedMobsOnly;
    internal static bool moddedMapsOnly;
    internal static bool allItemsFree;

    internal static List<BaseCWInput> _inputs = new();

    internal static void RegisterCustomMap(CustomMap map)
    {
        customMaps.Add(map);
        ApiLog.Log($"[Maps] Registered custom map: {map.DisplayName}");
    }

    private void PatchAll()
    {
        if (_isPatched)
        {
            ApiLog.LogError("Already patched!");
            return;
        }

        ApiLog.Log("Patching...");

        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        try
        {
            Harmony.PatchAll();
            _isPatched = true;
            ApiLog.Log("Patched!");
        }
        catch (Exception e)
        {
            ApiLog.LogError($"Failed to patch: {e}");
        }
    }

    /// <summary>
    ///     Unpatches all patches applied by the plugin.
    /// </summary>
    public void UnpatchAll()
    {
        if (!_isPatched)
        {
            ApiLog.LogError("Already unpatched!");
            return;
        }

        ApiLog.Log("Unpatching...");

        try
        {
            Harmony?.UnpatchSelf();
            _isPatched = false;
            ApiLog.Log("Unpatched!");
        }
        catch (Exception e)
        {
            ApiLog.LogError($"Failed to unpatch: {e}");
        }
    }

    /// <summary>
    /// When enabled, only custom monsters registered via <see cref="Mobs.RegisterMonster"/> are spawned during rounds.
    /// </summary>
    /// <param name="value"><c>true</c> to restrict spawning to modded monsters only.</param>
    public static void SetModdedMobsOnly(bool value)
    {
        moddedMobsOnly = value;
    }

    /// <summary>
    /// When enabled, only custom maps registered via <see cref="Maps.RegisterMap"/> are selected for rounds.
    /// </summary>
    /// <param name="value"><c>true</c> to restrict map selection to modded maps only.</param>
    public static void SetModdedMapsOnly(bool value)
    {
        moddedMapsOnly = value;
    }

    /// <summary>
    /// When enabled, all items in the shop are free (price set to 0) when the shop initializes.
    /// </summary>
    /// <param name="value"><c>true</c> to make all shop items free.</param>
    public static void SetAllItemsFree(bool value)
    {
        allItemsFree = value;
    }
}
