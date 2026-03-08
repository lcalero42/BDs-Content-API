using System;
using System.Collections.Generic;
using HarmonyLib;
using UnityEngine;

namespace DbsContentApi;

/// <summary>
///     Main plugin class for DbsContentApi.
/// </summary>
[ContentWarningPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION, false)]
public class DbsContentApiPlugin
{
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
        Modules.Logger.Log($"Plugin {MyPluginInfo.PLUGIN_GUID} is loaded!");
    }

    private Harmony? Harmony { get; set; }
    internal static Logger? Logger { get; }

    /// <summary>
    ///     Singleton instance of the DbsContentApi plugin.
    /// </summary>
    public static DbsContentApiPlugin Instance { get; }

    /// <summary>
    /// Global list of registered custom monsters.
    /// </summary>
    public static List<GameObject> customMonsters = new List<GameObject>();
    public static List<Action> customItemsRegistrationCallbacks = new List<Action>();
    public static List<ContentEvent> customContentEvents = new List<ContentEvent>();

    /// <summary>
    /// If true, only modded monsters will spawn in the round.
    /// </summary>
    public static bool moddedMobsOnly = false;
    public static bool allItemsFree = false;

    private void PatchAll()
    {
        if (_isPatched)
        {
            Modules.Logger.LogWarning("Already patched!");
            return;
        }

        Modules.Logger.Log("Patching...");

        Harmony ??= new Harmony(MyPluginInfo.PLUGIN_GUID);

        try
        {
            Harmony.PatchAll();
            _isPatched = true;
            Modules.Logger.Log("Patched!");
        }
        catch (Exception e)
        {
            Modules.Logger.LogError($"Failed to patch: {e}");
        }
    }

    /// <summary>
    ///     Unpatches all patches applied by the plugin.
    /// </summary>
    public void UnpatchAll()
    {
        if (!_isPatched)
        {
            Modules.Logger.LogWarning("Already unpatched!");
            return;
        }

        Modules.Logger.Log("Unpatching...");

        try
        {
            Harmony?.UnpatchSelf();
            _isPatched = false;
            Modules.Logger.Log("Unpatched!");
        }
        catch (Exception e)
        {
            Modules.Logger.LogError($"Failed to unpatch: {e}");
        }
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
