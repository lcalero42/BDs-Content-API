using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DbsContentApi;

/*
 * Map authoring contract:
 * - Register with a stable mapId (e.g. "mymod.cw_mines"). Multiplayer sync uses LevelToPlay (3 + registration index);
 *   all clients must register maps in the same order.
 * - Markers: DiveBellSpawn*, PatrolPoint* or PatrolPoint_Dog (group suffix), optional _w2 spawn weight.
 * - Use MapLifecycleHooks for custom setup; MapPipelineFlags to skip built-in phases.
 */

/// <summary>Skip flags for built-in <see cref="CustomMapLoader"/> pipeline phases.</summary>
public class MapPipelineFlags
{
    public bool skipVanillaCleanup;
    public bool skipRetexture;
    public bool skipLightSpheres;
    public bool skipPatrolSetup;
    public bool skipMonsterRemoval;
    public bool skipMultipliers;
    public bool skipAmbience;
    public bool skipDiveBellTeleport;
    public bool skipFinalize;
}

/// <summary>
/// Configuration for how a custom map scene is loaded and integrated into a round.
/// </summary>
public class MapConfig
{
    public MapPipelineFlags pipeline = new();

    /// <summary>Maximum distance between patrol points to auto-connect them.</summary>
    public float patrolConnectionRadius = 64f;

    /// <summary>Layer mask used when raycasting between patrol points for connectivity.</summary>
    public int patrolConnectionLayerMask = 1 << 10;

    /// <summary>Whether to retexture scene objects to match the diving bell material.</summary>
    public bool retextureScene = true;

    /// <summary>Color tint applied during scene retexturing.</summary>
    public Color retextureColor = Color.gray;

    /// <summary>Whether to configure LightSphere renderers with a custom shader and color.</summary>
    public bool setupLightSpheres = true;

    /// <summary>Color applied to LightSphere materials.</summary>
    public Color lightSphereColor = Color.white;

    /// <summary>Shader name used for LightSphere materials (e.g. <c>NiceShader</c>).</summary>
    public string lightSphereShader = "NiceShader";

    /// <summary>Whether to replace the default ambience audio with the map's custom ambience clip.</summary>
    public bool setupAmbience = true;

    /// <summary>Volume multiplier applied to the custom ambience audio source.</summary>
    public float ambienceVolumeMultiplier = 6f;

    /// <summary>Whether to scan for patrol point markers and wire up patrol groups.</summary>
    public bool setupPatrolPoints = true;

    /// <summary>Parse patrol group from marker name suffix (e.g. <c>PatrolPoint_Dog</c>).</summary>
    public bool patrolGroupFromName = true;

    /// <summary>Separator between marker prefix and group suffix.</summary>
    public string patrolNameSeparator = "_";

    /// <summary>Group used when suffix is missing and <see cref="assignUnspecifiedToAllGroups"/> is false.</summary>
    public PatrolPoint.PatrolGroup? defaultPatrolGroup;

    /// <summary>When true, markers without a group suffix are added to every patrol group (legacy behavior).</summary>
    public bool assignUnspecifiedToAllGroups = true;

    /// <summary>Include existing <see cref="PatrolPoint"/> components from the loaded scene.</summary>
    public bool respectScenePatrolPoints = true;

    public float defaultPatrolSpawnWeight = 1f;

    /// <summary>Only connect patrol points that share a patrol group.</summary>
    public bool patrolGroupScopedConnections = true;

    /// <summary>Whether to filter monster spawns using the RoundRemoveHandler object in the map.</summary>
    public bool applyMonsterRemoval = true;

    /// <summary>Whether to read spawn multipliers from the RoundMultiplierHandler object in the map.</summary>
    public bool setupMultipliers = true;

    /// <summary>Substring matched against scene object names to find diving bell spawn points.</summary>
    public string diveBellSpawnMarker = "DiveBellSpawn";

    /// <summary>Substring matched against scene object names to find patrol point markers.</summary>
    public string patrolPointMarker = "PatrolPoint";

    /// <summary>Name of the root GameObject listing monsters to exclude from spawning.</summary>
    public string roundRemoveHandlerName = "RoundRemoveHandler";

    /// <summary>Name of the root GameObject containing spawn budget multiplier child objects.</summary>
    public string roundMultiplierHandlerName = "RoundMultiplierHandler";

    /// <summary>Name of the GameObject holding the custom ambience AudioSource.</summary>
    public string ambienceHolderName = "AmbienceHolder";

    /// <summary>Additional root object names preserved when cleaning up the vanilla level.</summary>
    public List<string> extraKeepObjects = new();
}

/// <summary>Callbacks invoked during custom map loading. All are optional.</summary>
public class MapLifecycleHooks
{
    public Action<MapLoadContext>? onBeforeCleanup;
    public Action<MapLoadContext>? onAfterSceneLoaded;
    public Action<MapLoadContext>? onAfterSetup;
}

/// <summary>Runtime context passed to map lifecycle hooks.</summary>
public class MapLoadContext
{
    public CustomMap Map { get; }
    public MapConfig Config => Map.Config;
    public CustomMapLoader Loader { get; }
    public Scene ActiveScene { get; internal set; }
    public Scene? LoadedScene { get; internal set; }
    public List<GameObject> DiveBellSpawns { get; } = new();
    public List<PatrolPoint> PatrolPoints { get; } = new();

    internal MapLoadContext(CustomMap map, CustomMapLoader loader)
    {
        Map = map;
        Loader = loader;
    }
}

/// <summary>
/// Data descriptor for a custom map.
/// </summary>
public class CustomMap
{
    public AssetBundle Bundle { get; }
    public string SceneName { get; }
    public string DisplayName { get; }
    public string MapId { get; }
    public MapConfig Config { get; }
    public MapLifecycleHooks Hooks { get; }

    public CustomMap(
        AssetBundle bundle,
        string sceneName,
        string displayName,
        string mapId,
        MapConfig? config = null,
        MapLifecycleHooks? hooks = null)
    {
        Bundle = bundle;
        SceneName = sceneName;
        DisplayName = displayName;
        MapId = mapId;
        Config = config ?? new MapConfig();
        Hooks = hooks ?? new MapLifecycleHooks();
    }
}

/// <summary>
/// API for registering and querying custom map scenes loaded from AssetBundles.
/// </summary>
public static class Maps
{
    /// <summary>
    /// Registers a custom map with the game.
    /// </summary>
    public static CustomMap RegisterMap(
        AssetBundle bundle,
        string sceneName,
        string displayName,
        MapConfig? config = null,
        string? mapId = null,
        MapLifecycleHooks? hooks = null)
    {
        CustomMap? map = RegisterMapCore(bundle, sceneName, displayName, config, mapId, hooks);
        if (map == null)
            throw new InvalidOperationException($"[Maps] Failed to register map '{displayName}'.");
        return map;
    }

    /// <summary>
    /// Registers a custom map; returns <c>null</c> if <paramref name="mapId"/> duplicates an existing registration.
    /// </summary>
    public static CustomMap? TryRegisterMap(
        AssetBundle bundle,
        string sceneName,
        string displayName,
        MapConfig? config = null,
        string? mapId = null,
        MapLifecycleHooks? hooks = null)
        => RegisterMapCore(bundle, sceneName, displayName, config, mapId, hooks);

    static CustomMap? RegisterMapCore(
        AssetBundle bundle,
        string sceneName,
        string displayName,
        MapConfig? config,
        string? mapId,
        MapLifecycleHooks? hooks)
    {
        string id = string.IsNullOrWhiteSpace(mapId) ? SanitizeMapId(displayName) : mapId.Trim();
        var map = new CustomMap(bundle, sceneName, displayName, id, config, hooks);
        if (!DbsContentApiPlugin.RegisterCustomMap(map))
            return null;
        return map;
    }

    public static bool TryGetMap(string mapId, out CustomMap? map)
        => DbsContentApiPlugin.TryGetCustomMap(mapId, out map);

    public static bool TryGetMapByIndex(int index, out CustomMap? map)
        => DbsContentApiPlugin.TryGetCustomMapByIndex(index, out map);

    public static IReadOnlyList<CustomMap> GetRegisteredMaps()
        => DbsContentApiPlugin.customMaps;

    public static void ForceNextMap(string mapId)
    {
        DbsContentApiPlugin.forcedNextMapId = mapId;
        ApiLog.Log($"[Maps] Next map selection forced to: {mapId}");
    }

    public static void ClearForcedMap()
        => DbsContentApiPlugin.forcedNextMapId = null;

    /// <summary>
    /// Sets the map id resolved when <see cref="CustomMapLoader"/> runs.
    /// Call from external map-selection patches when bypassing <see cref="ChooseMapPatch"/>.
    /// </summary>
    public static void SetSelectedMapId(string? mapId)
        => DbsContentApiPlugin.selectedCustomMapId = mapId;

    public static int GetLevelIndexForMap(CustomMap map)
    {
        int index = DbsContentApiPlugin.customMaps.IndexOf(map);
        return index < 0 ? -1 : 3 + index;
    }

    public static string? FindScenePath(AssetBundle bundle, string hint)
    {
        return bundle.GetAllScenePaths().FirstOrDefault(p =>
            p.Contains(hint, StringComparison.OrdinalIgnoreCase));
    }

    public static string SanitizeMapId(string displayName)
    {
        if (string.IsNullOrWhiteSpace(displayName))
            return "map.unnamed";
        var chars = displayName.Trim().ToLowerInvariant()
            .Select(c => char.IsLetterOrDigit(c) ? c : '_')
            .ToArray();
        string slug = new string(chars).Trim('_');
        while (slug.Contains("__", StringComparison.Ordinal))
            slug = slug.Replace("__", "_", StringComparison.Ordinal);
        return string.IsNullOrEmpty(slug) ? "map.unnamed" : slug;
    }

    internal static CustomMap? ResolveMapForCurrentRound()
    {
        if (!string.IsNullOrEmpty(DbsContentApiPlugin.selectedCustomMapId)
            && TryGetMap(DbsContentApiPlugin.selectedCustomMapId, out CustomMap? byId)
            && byId != null)
            return byId;

        int index = SurfaceNetworkHandler.RoomStats.LevelToPlay - 3;
        return TryGetMapByIndex(index, out CustomMap? byIndex) ? byIndex : null;
    }

    internal static CustomMap? PickMapForNextRound(RoomStatsHolder roomStats)
    {
        if (DbsContentApiPlugin.customMaps.Count == 0)
            return null;

        string? forcedId = DbsContentApiPlugin.forcedNextMapId;
        DbsContentApiPlugin.forcedNextMapId = null;

        if (!string.IsNullOrEmpty(forcedId))
        {
            if (TryGetMap(forcedId, out CustomMap? forced) && forced != null)
                return forced;
            ApiLog.LogError($"[Maps] Forced map id '{forcedId}' not found.");
        }

        int poolSize = 3 + DbsContentApiPlugin.customMaps.Count;
        int minPick = DbsContentApiPlugin.moddedMapsOnly ? 3 : 0;

        int pick = UnityEngine.Random.Range(minPick, poolSize);
        int attempts = 0;
        while (attempts < 100)
        {
            if (!DbsContentApiPlugin.moddedMapsOnly && pick < 3)
                return null;

            int mapIndex = pick - 3;
            if (mapIndex >= 0 && mapIndex < DbsContentApiPlugin.customMaps.Count)
            {
                CustomMap candidate = DbsContentApiPlugin.customMaps[mapIndex];
                if (GetLevelIndexForMap(candidate) != roomStats.LevelToPlay
                    || DbsContentApiPlugin.customMaps.Count == 1)
                    return candidate;
            }

            pick = UnityEngine.Random.Range(minPick, poolSize);
            attempts++;
        }

        return DbsContentApiPlugin.customMaps[0];
    }
}

public class CustomMapLoader : MonoBehaviour
{
    public CustomMap? Map { get; set; }

    private readonly List<PatrolPointEntry> _patrolEntries = new();

    public static float multiplierMonster = 1f;
    public static float multiplierTool = 1f;
    public static float multiplierArtifact = 1f;

    private struct PatrolPointEntry
    {
        public PatrolPoint Point;
        public HashSet<PatrolPoint.PatrolGroup> Groups;
    }

    private IEnumerator Start()
    {
        if (Map == null)
        {
            ApiLog.LogError("CustomMapLoader: Map is null!");
            yield break;
        }

        var config = Map.Config;
        var pipeline = config.pipeline;
        var ctx = new MapLoadContext(Map, this);

        multiplierMonster = 1f;
        multiplierTool = 1f;
        multiplierArtifact = 1f;

        if (SurfaceNetworkHandler.RoomStats.LevelToPlay < 3)
        {
            ApiLog.Log($"[Maps] LevelToPlay ({SurfaceNetworkHandler.RoomStats.LevelToPlay}) is vanilla. Enabling occlusion culling and exiting loader.");
            Camera.main.useOcclusionCulling = true;
            yield break;
        }

        ApiLog.Log($"[Maps] Loading custom map: {Map.DisplayName} (Id: {Map.MapId}, Scene: {Map.SceneName})");

        Camera.main.useOcclusionCulling = false;
        _patrolEntries.Clear();
        ctx.ActiveScene = SceneManager.GetActiveScene();

        DivingBell? tmp = GameObject.FindObjectOfType<DivingBell>();
        if (tmp == null)
        {
            ApiLog.LogError("[Maps] DivingBell not found in scene!");
            yield break;
        }

        Material? bellMaterial = GetDivingBellMaterial(tmp);
        if (bellMaterial == null)
            yield break;

        tmp.locked = true;

        Map.Hooks.onBeforeCleanup?.Invoke(ctx);

        if (!pipeline.skipVanillaCleanup)
            RunVanillaCleanup(config);

        ApiLog.Log($"[Maps] Asynchronously loading scene: {Map.SceneName}");
        AsyncOperation? asyncLoad = SceneManager.LoadSceneAsync(Map.SceneName, LoadSceneMode.Additive);
        if (asyncLoad == null)
        {
            ApiLog.LogError($"[Maps] Failed to start loading scene: {Map.SceneName}");
            yield break;
        }
        yield return asyncLoad;

        for (int i = 0; i < SceneManager.sceneCount; i++)
        {
            Scene scene = SceneManager.GetSceneAt(i);
            if (scene.path == Map.SceneName
                || scene.name == System.IO.Path.GetFileNameWithoutExtension(Map.SceneName))
            {
                ctx.LoadedScene = scene;
                break;
            }
        }
        ApiLog.Log($"[Maps] Scene loaded: {Map.SceneName}");

        Map.Hooks.onAfterSceneLoaded?.Invoke(ctx);

        if (!pipeline.skipLightSpheres && config.setupLightSpheres)
            SetupLightSpheres(config);

        if (!pipeline.skipRetexture && config.retextureScene)
            RetextureScene(config, bellMaterial);

        CollectMarkers(config, ctx);

        if (!pipeline.skipPatrolSetup && config.setupPatrolPoints)
            SetupPatrolPoints(config, ctx);

        if (!pipeline.skipMonsterRemoval && config.applyMonsterRemoval)
            ApplyMonsterRemoval(config);

        if (!pipeline.skipMultipliers && config.setupMultipliers)
            LoadMultipliers(config);

        if (!pipeline.skipAmbience && config.setupAmbience)
            SetupAmbience(config);

        Map.Hooks.onAfterSetup?.Invoke(ctx);

        if (pipeline.skipFinalize)
            yield break;

        if (!pipeline.skipDiveBellTeleport)
            TeleportToSpawn(tmp, ctx);

        ApiLog.Log("[Maps] Waiting for all players to join...");
        while (PhotonNetwork.PlayerList.Length != PlayerHandler.instance.players.Count)
            yield return null;

        yield return null;

        ApiLog.Log("[Maps] Map setup complete. Level is ready.");
        Level.currentLevel.levelIsReady = true;
        tmp.locked = false;

        MethodInfo? setupFinishedMethod = typeof(Level).GetMethod("SetupFinished", BindingFlags.Instance | BindingFlags.NonPublic);
        if (setupFinishedMethod == null)
            ApiLog.LogError("[Maps] Level.SetupFinished method not found!");
        else
            setupFinishedMethod.Invoke(Level.currentLevel, null);
    }

    private static Material? GetDivingBellMaterial(DivingBell tmp)
    {
        Renderer? bellRenderer = tmp.GetComponentInChildren<Renderer>();
        if (bellRenderer == null)
        {
            ApiLog.LogError("[Maps] DivingBell has no Renderer!");
            return null;
        }
        Material render = bellRenderer.material;
        foreach (Renderer item in tmp.gameObject.GetComponentsInChildren<Renderer>())
        {
            if (item.name == "Frame")
            {
                render = new Material(item.material);
                break;
            }
        }
        return render;
    }

    private static void RunVanillaCleanup(MapConfig config)
    {
        ApiLog.Log("[Maps] Phase: vanilla cleanup");
        var defaultKeepObjects = new HashSet<string>
        {
            "GAME", "Remove When DOne", "RoundArtifactSpawner", "VoiceLogger",
            "Player(Clone)", "RoundSpawnerTools", "Spawns"
        };
        foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (obj.name == "PickupHolder(Clone)")
                obj.transform.position += Vector3.up * 3;
            else if (!(defaultKeepObjects.Contains(obj.name)
                       || config.extraKeepObjects.Contains(obj.name)
                       || obj.GetComponent<RoundSpawner>() != null
                       || obj.GetComponent<AmbienceHandler>() != null))
                Destroy(obj);
        }
    }

    private void SetupLightSpheres(MapConfig config)
    {
        ApiLog.Log("[Maps] Phase: light spheres");
        int count = 0;
        foreach (Renderer rend in FindObjectsOfType<Renderer>())
        {
            if (!rend.name.Contains("LightSphere"))
                continue;
            Material[] mats = rend.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                Shader? niceShader = Shader.Find(config.lightSphereShader);
                if (niceShader == null)
                    ApiLog.LogError($"[Maps] '{config.lightSphereShader}' not found!");
                else
                    mats[i].shader = niceShader;
                mats[i].SetColor("_Color", config.lightSphereColor);
            }
            rend.sharedMaterials = mats;
            Light? l = rend.gameObject.GetComponent<Light>();
            if (l != null)
                Level.currentLevel.lights.Add(l);
            count++;
        }
        ApiLog.Log($"[Maps] LightSpheres handled: {count}");
    }

    private void RetextureScene(MapConfig config, Material render)
    {
        ApiLog.Log("[Maps] Phase: retexture");
        int count = 0;
        foreach (Renderer rend in FindObjectsOfType<Renderer>())
        {
            if (rend.name.Contains("LightSphere"))
                continue;
            if (rend.transform.root.name == "Spawns" || rend.transform.root.name.Contains("(Clone)"))
                continue;
            Material[] mats = rend.sharedMaterials;
            for (int i = 0; i < mats.Length; i++)
            {
                mats[i] = render;
                mats[i].SetColor("_Color", config.retextureColor);
            }
            rend.sharedMaterials = mats;
            rend.gameObject.layer = 10;
            count++;
        }
        ApiLog.Log($"[Maps] Objects retextured: {count}");
    }

    private void CollectMarkers(MapConfig config, MapLoadContext ctx)
    {
        ApiLog.Log("[Maps] Phase: markers");
        int patrolAdded = 0;

        if (config.setupPatrolPoints && config.respectScenePatrolPoints)
        {
            foreach (PatrolPoint existing in FindObjectsOfType<PatrolPoint>())
            {
                if (IsExcludedPatrolObject(existing.gameObject))
                    continue;
                TryAddPatrolEntry(existing, config, ref patrolAdded);
            }
        }

        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.name.Contains(config.diveBellSpawnMarker))
            {
                ctx.DiveBellSpawns.Add(obj);
                StripMarkerVisuals(obj);
            }

            if (config.setupPatrolPoints && obj.name.Contains(config.patrolPointMarker))
            {
                PatrolPoint point = obj.GetComponent<PatrolPoint>() ?? obj.AddComponent<PatrolPoint>();
                TryAddPatrolEntry(point, config, ref patrolAdded, obj.name);
                try
                {
                    Destroy(obj.GetComponentInChildren<Renderer>());
                    Destroy(obj.GetComponentInChildren<Collider>());
                }
                catch { }
            }
        }

        foreach (var entry in _patrolEntries)
            ctx.PatrolPoints.Add(entry.Point);

        ApiLog.Log($"[Maps] Markers: {config.diveBellSpawnMarker}s={ctx.DiveBellSpawns.Count}, patrol={patrolAdded}");
        if (ctx.DiveBellSpawns.Count == 0)
            ApiLog.LogError($"[Maps] No {config.diveBellSpawnMarker} markers found! Players might spawn in the void.");
    }

    private static bool IsExcludedPatrolObject(GameObject obj)
    {
        return obj.transform.root.name == "Spawns" || obj.transform.root.name.Contains("(Clone)");
    }

    private bool TryAddPatrolEntry(PatrolPoint point, MapConfig config, ref int patrolAdded, string? markerName = null)
    {
        if (_patrolEntries.Any(e => e.Point == point))
            return false;

        if (!TryResolvePatrolGroups(point, config, markerName ?? point.gameObject.name, out HashSet<PatrolPoint.PatrolGroup> groups))
            return false;

        float weight = config.defaultPatrolSpawnWeight;
        if (markerName != null)
            TryParseSpawnWeight(markerName, ref weight);
        point.spawnWeight = weight;

        if (groups.Count == 1)
            point.group = groups.First();

        _patrolEntries.Add(new PatrolPointEntry { Point = point, Groups = groups });
        patrolAdded++;
        return true;
    }

    private static bool TryResolvePatrolGroups(
        PatrolPoint point,
        MapConfig config,
        string objectName,
        out HashSet<PatrolPoint.PatrolGroup> groups)
    {
        groups = new HashSet<PatrolPoint.PatrolGroup>();

        if (config.patrolGroupFromName
            && TryParsePatrolGroupFromName(objectName, config, out PatrolPoint.PatrolGroup parsed))
            groups.Add(parsed);
        else if (!config.patrolGroupFromName && config.defaultPatrolGroup.HasValue)
            groups.Add(config.defaultPatrolGroup.Value);
        else if (config.assignUnspecifiedToAllGroups)
        {
            foreach (PatrolPoint.PatrolGroup g in Enum.GetValues(typeof(PatrolPoint.PatrolGroup)))
                groups.Add(g);
        }
        else if (config.defaultPatrolGroup.HasValue)
            groups.Add(config.defaultPatrolGroup.Value);
        else if (config.respectScenePatrolPoints && point.group != default)
            groups.Add(point.group);
        else
        {
            ApiLog.LogWarning($"[Maps] Patrol point '{objectName}' has no group; skipped.");
            return false;
        }

        return groups.Count > 0;
    }

    private static bool TryParsePatrolGroupFromName(string objectName, MapConfig config, out PatrolPoint.PatrolGroup group)
    {
        group = default;
        if (!objectName.Contains(config.patrolPointMarker, StringComparison.OrdinalIgnoreCase))
            return false;

        string suffix = objectName;
        int markerIdx = objectName.IndexOf(config.patrolPointMarker, StringComparison.OrdinalIgnoreCase);
        if (markerIdx >= 0)
            suffix = objectName.Substring(markerIdx + config.patrolPointMarker.Length);

        if (suffix.StartsWith(config.patrolNameSeparator, StringComparison.Ordinal))
            suffix = suffix.Substring(config.patrolNameSeparator.Length);

        int weightIdx = suffix.LastIndexOf("_w", StringComparison.OrdinalIgnoreCase);
        if (weightIdx > 0)
            suffix = suffix.Substring(0, weightIdx);

        if (string.IsNullOrWhiteSpace(suffix))
            return false;

        return Enum.TryParse(suffix, true, out group);
    }

    private static void TryParseSpawnWeight(string objectName, ref float weight)
    {
        int idx = objectName.LastIndexOf("_w", StringComparison.OrdinalIgnoreCase);
        if (idx < 0 || idx + 2 >= objectName.Length)
            return;
        if (float.TryParse(objectName.Substring(idx + 2), out float parsed))
            weight = parsed;
    }

    private void SetupPatrolPoints(MapConfig config, MapLoadContext ctx)
    {
        ApiLog.Log("[Maps] Phase: patrol");
        int connectionsMade = 0;
        foreach (PatrolPointEntry pEntry in _patrolEntries)
        {
            foreach (PatrolPointEntry xEntry in _patrolEntries)
            {
                if (pEntry.Point == xEntry.Point)
                    continue;
                if (config.patrolGroupScopedConnections && !pEntry.Groups.Overlaps(xEntry.Groups))
                    continue;

                Vector3 pPos = pEntry.Point.transform.position + Vector3.up * 2;
                Vector3 xPos = xEntry.Point.transform.position + Vector3.up * 2;
                Vector3 diff = xPos - pPos;
                float distance = diff.magnitude;

                if (distance < config.patrolConnectionRadius
                    && !Physics.Raycast(pPos, diff.normalized, distance, config.patrolConnectionLayerMask))
                {
                    pEntry.Point.connectedPoints.Add(xEntry.Point);
                    connectionsMade++;
                }
            }
        }
        ApiLog.Log($"[Maps] Patrol connections: {connectionsMade}");

        var all = new Dictionary<PatrolPoint.PatrolGroup, List<PatrolPoint>>();
        foreach (PatrolPoint.PatrolGroup g in Enum.GetValues(typeof(PatrolPoint.PatrolGroup)))
            all[g] = new List<PatrolPoint>();

        foreach (PatrolPointEntry entry in _patrolEntries)
        {
            foreach (PatrolPoint.PatrolGroup g in entry.Groups)
                all[g].Add(entry.Point);
        }

        Level.currentLevel.patrolGroups = all;
    }

    private static void ApplyMonsterRemoval(MapConfig config)
    {
        ApiLog.Log("[Maps] Phase: monster removal");
        RoundSpawner? spawner = GameObject.FindObjectOfType<RoundSpawner>();
        if (spawner == null)
        {
            ApiLog.LogError("[Maps] RoundSpawner not found in scene!");
            return;
        }
        GameObject? roundRemoveHandler = GameObject.Find(config.roundRemoveHandlerName);
        if (roundRemoveHandler == null)
            return;
        Transform[] options = roundRemoveHandler.GetComponentsInChildren<Transform>();
        HashSet<string> remove = new HashSet<string>(options.Select(t => t.name));
        int beforeCount = spawner.possibleSpawns.Length;
        spawner.possibleSpawns = spawner.possibleSpawns.Where(s => !remove.Contains(s.name)).ToArray();
        ApiLog.Log($"[Maps] Monster removal: {beforeCount} -> {spawner.possibleSpawns.Length}");
    }

    private void LoadMultipliers(MapConfig config)
    {
        ApiLog.Log("[Maps] Phase: multipliers");
        GameObject? roundMultiplierHandler = GameObject.Find(config.roundMultiplierHandlerName);
        if (roundMultiplierHandler == null)
        {
            ApiLog.LogError($"[Maps] {config.roundMultiplierHandlerName} not found; multipliers stay at 1.");
            return;
        }
        foreach (GameObject mult in roundMultiplierHandler.GetComponentsInChildren<GameObject>())
        {
            if (string.IsNullOrEmpty(mult.name))
                continue;
            try
            {
                switch (mult.name[0])
                {
                    case 'M': multiplierMonster = float.Parse(mult.name.Substring(1)); break;
                    case 'T': multiplierTool = float.Parse(mult.name.Substring(1)); break;
                    case 'A': multiplierArtifact = float.Parse(mult.name.Substring(1)); break;
                }
            }
            catch (Exception e)
            {
                ApiLog.LogError($"[Maps] Error parsing multiplier '{mult.name}': {e.Message}");
            }
        }
        ApiLog.Log($"[Maps] Multipliers: Monster={multiplierMonster}, Tool={multiplierTool}, Artifact={multiplierArtifact}");
    }

    private static void SetupAmbience(MapConfig config)
    {
        ApiLog.Log("[Maps] Phase: ambience");
        AmbienceHandler? handle = GameObject.FindObjectOfType<AmbienceHandler>();
        GameObject? ambienceHolder = GameObject.Find(config.ambienceHolderName);
        if (handle == null || ambienceHolder == null)
            return;
        AudioSource? ambience = handle.gameObject.GetComponent<AudioSource>();
        AudioSource? customAmbience = ambienceHolder.GetComponent<AudioSource>();
        if (ambience == null || customAmbience == null)
        {
            ApiLog.LogError($"[Maps] AudioSource missing on AmbienceHandler or {config.ambienceHolderName}!");
            return;
        }
        Destroy(handle);
        ambience.clip = customAmbience.clip;
        ambience.pitch = customAmbience.pitch;
        ambience.loop = true;
        ambience.volume *= config.ambienceVolumeMultiplier;
        ambience.Play();
    }

    private static void TeleportToSpawn(DivingBell tmp, MapLoadContext ctx)
    {
        if (ctx.DiveBellSpawns.Count == 0)
            return;
        UnityEngine.Random.InitState(GameAPI.seed);
        GameObject spawn = ctx.DiveBellSpawns[UnityEngine.Random.Range(0, ctx.DiveBellSpawns.Count)];
        ApiLog.Log($"[Maps] Spawn: {spawn.name} at {spawn.transform.position}");
        tmp.transform.position = spawn.transform.position + Vector3.up;
        tmp.transform.rotation = spawn.transform.rotation;
        MethodInfo? teleportMethod = typeof(Player).GetMethod("Teleport", BindingFlags.Instance | BindingFlags.NonPublic);
        if (teleportMethod == null)
            ApiLog.LogError("[Maps] Player.Teleport method not found!");
        else
            teleportMethod.Invoke(Player.localPlayer, new object[] { spawn.transform.position + Vector3.up * 4, spawn.transform.rotation * Vector3.forward });
    }

    private static void StripMarkerVisuals(GameObject obj)
    {
        try
        {
            foreach (var renderer in obj.GetComponentsInChildren<Renderer>(true))
                Destroy(renderer);
            foreach (var collider in obj.GetComponentsInChildren<Collider>(true))
                Destroy(collider);
        }
        catch { }
    }
}
