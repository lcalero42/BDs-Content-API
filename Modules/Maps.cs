using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DbsContentApi;

/// <summary>
/// Configuration for how a custom map scene is loaded and integrated into a round.
/// </summary>
public class MapConfig
{
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

/// <summary>
/// Data descriptor for a custom map.
/// </summary>
public class CustomMap
{
    /// <summary>Asset bundle containing the map scene.</summary>
    public AssetBundle Bundle { get; }

    /// <summary>Exact scene path from <see cref="AssetBundle.GetAllScenePaths"/>.</summary>
    public string SceneName { get; }

    /// <summary>Human-readable name used in logs and map selection.</summary>
    public string DisplayName { get; }

    /// <summary>Load-time configuration for scene setup.</summary>
    public MapConfig Config { get; }

    /// <summary>
    /// Creates a custom map descriptor.
    /// </summary>
    /// <param name="bundle">Asset bundle containing the scene.</param>
    /// <param name="sceneName">Exact scene path within the bundle.</param>
    /// <param name="displayName">Display name for logging and selection.</param>
    /// <param name="config">Optional load configuration; defaults are used when <c>null</c>.</param>
    public CustomMap(AssetBundle bundle, string sceneName, string displayName, MapConfig? config = null)
    {
        Bundle = bundle;
        SceneName = sceneName;
        DisplayName = displayName;
        Config = config ?? new MapConfig();
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
    /// <param name="bundle">Asset bundle containing the map scene.</param>
    /// <param name="sceneName">Exact scene path within the bundle.</param>
    /// <param name="displayName">Display name for logging and selection.</param>
    /// <param name="config">Optional load configuration.</param>
    /// <returns>The registered <see cref="CustomMap"/> instance.</returns>
    public static CustomMap RegisterMap(AssetBundle bundle, string sceneName, string displayName, MapConfig? config = null)
    {
        var map = new CustomMap(bundle, sceneName, displayName, config);
        DbsContentApiPlugin.RegisterCustomMap(map);
        return map;
    }

    /// <summary>
    /// Finds a scene path in an AssetBundle that contains the given hint.
    /// </summary>
    /// <param name="bundle">The asset bundle to search.</param>
    /// <param name="hint">Case-insensitive substring to match against scene paths.</param>
    /// <returns>The first matching scene path, or <c>null</c> if none found.</returns>
    public static string? FindScenePath(AssetBundle bundle, string hint)
    {
        return bundle.GetAllScenePaths().FirstOrDefault(p => p.Contains(hint, System.StringComparison.OrdinalIgnoreCase));
    }
}

/// <summary>
/// MonoBehaviour responsible for loading and setting up a custom map scene.
/// </summary>
internal class CustomMapLoader : MonoBehaviour
{
    public CustomMap? Map { get; set; }

    private List<PatrolPoint> _points = new List<PatrolPoint>();

    public static float multiplierMonster = 1f;
    public static float multiplierTool = 1f;
    public static float multiplierArtifact = 1f;

    private IEnumerator Start()
    {
        if (Map == null)
        {
            ApiLog.LogError("CustomMapLoader: Map is null!");
            yield break;
        }

        var config = Map.Config;

        multiplierMonster = 1f;
        multiplierTool = 1f;
        multiplierArtifact = 1f;

        // LevelToPlay < 3 are vanilla maps. This loader is only for 3+.
        if (SurfaceNetworkHandler.RoomStats.LevelToPlay < 3)
        {
            ApiLog.Log($"[Maps] LevelToPlay ({SurfaceNetworkHandler.RoomStats.LevelToPlay}) is vanilla. Enabling occlusion culling and exiting loader.");
            Camera.main.useOcclusionCulling = true;
            yield break;
        }

        ApiLog.Log($"[Maps] Loading custom map: {Map.DisplayName} (Scene: {Map.SceneName})");

        // Cleaning
        Camera.main.useOcclusionCulling = false;
        _points.Clear();

        DivingBell tmp = GameObject.FindObjectOfType<DivingBell>();
        if (tmp == null)
        {
            ApiLog.LogError("[Maps] DivingBell not found in scene!");
            yield break;
        }

        Renderer bellRenderer = tmp.GetComponentInChildren<Renderer>();
        if (bellRenderer == null)
        {
            ApiLog.LogError("[Maps] DivingBell has no Renderer!");
            yield break;
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

        tmp.locked = true;

        ApiLog.Log("[Maps] Cleaning up vanilla level objects...");
        var defaultKeepObjects = new HashSet<string> { "GAME", "Remove When DOne", "RoundArtifactSpawner", "VoiceLogger", "Player(Clone)", "RoundSpawnerTools", "Spawns" };
        foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (obj.name == "PickupHolder(Clone)")
            {
                obj.transform.position += Vector3.up * 3;
            }
            else if (!(defaultKeepObjects.Contains(obj.name) || config.extraKeepObjects.Contains(obj.name) || obj.GetComponent<RoundSpawner>() != null || obj.GetComponent<AmbienceHandler>() != null))
            {
                Destroy(obj);
            }
        }

        // Load and Setup map
        ApiLog.Log($"[Maps] Asynchronously loading scene: {Map.SceneName}");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Map.SceneName, LoadSceneMode.Additive);
        if (asyncLoad == null)
        {
            ApiLog.LogError($"[Maps] Failed to start loading scene: {Map.SceneName}");
            yield break;
        }
        yield return asyncLoad;
        ApiLog.Log($"[Maps] Scene loaded: {Map.SceneName}");

        int lightSpheresHandled = 0;
        int objectsRetextured = 0;

        foreach (Renderer rend in FindObjectsOfType<Renderer>())
        {
            if (config.setupLightSpheres && rend.name.Contains("LightSphere"))
            {
                Material[] mats = rend.sharedMaterials;

                for (int i = 0; i < mats.Length; i++)
                {
                    Shader niceShader = Shader.Find(config.lightSphereShader);
                    if (niceShader == null)
                    {
                        ApiLog.LogError($"[Maps] '{config.lightSphereShader}' not found!");
                    }
                    else
                    {
                        mats[i].shader = niceShader;
                    }
                    mats[i].SetColor("_Color", config.lightSphereColor);
                }

                rend.sharedMaterials = mats;

                Light l = rend.gameObject.GetComponent<Light>();
                if (l != null) Level.currentLevel.lights.Add(l);
                lightSpheresHandled++;
            }
            else if (config.retextureScene && rend.transform.root.name != "Spawns" && !rend.transform.root.name.Contains("(Clone)"))
            {
                Material[] mats = rend.sharedMaterials;

                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = render;
                    mats[i].SetColor("_Color", config.retextureColor);
                }

                rend.sharedMaterials = mats;
                rend.gameObject.layer = 10;
                objectsRetextured++;
            }
        }
        ApiLog.Log($"[Maps] Processed renderers. LightSpheres: {lightSpheresHandled}, Objects retextured: {objectsRetextured}");

        // Markers
        List<GameObject> spawns = new List<GameObject>();
        int patrolPointsAdded = 0;

        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.name.Contains(config.diveBellSpawnMarker))
            {
                spawns.Add(obj);
                try
                {
                    var renderers = obj.GetComponentsInChildren<Renderer>(true);
                    foreach (var renderer in renderers)
                    {
                        Destroy(renderer);
                    }
                    var colliders = obj.GetComponentsInChildren<Collider>(true);
                    foreach (var collider in colliders)
                    {
                        Destroy(collider);
                    }
                }
                catch { }
            }
            if (config.setupPatrolPoints && obj.name.Contains(config.patrolPointMarker))
            {
                PatrolPoint point = obj.AddComponent<PatrolPoint>();
                _points.Add(point);
                patrolPointsAdded++;
                try
                {
                    Destroy(obj.GetComponentInChildren<Renderer>());
                    Destroy(obj.GetComponentInChildren<Collider>());
                }
                catch { }
            }
        }
        ApiLog.Log($"[Maps] Markers found. {config.diveBellSpawnMarker}s: {spawns.Count}, {config.patrolPointMarker}s: {patrolPointsAdded}");

        if (spawns.Count == 0)
        {
            ApiLog.LogError($"[Maps] No {config.diveBellSpawnMarker} markers found in custom map! Players might spawn in the void.");
        }

        // Patrol Points connection
        if (config.setupPatrolPoints)
        {
            ApiLog.Log("[Maps] Connecting patrol points...");
            int connectionsMade = 0;
            foreach (PatrolPoint p in _points)
            {
                foreach (PatrolPoint x in _points)
                {
                    if (p == x) continue;

                    Vector3 pPos = p.transform.position + (Vector3.up * 2);
                    Vector3 xPos = x.transform.position + (Vector3.up * 2);

                    Vector3 diff = xPos - pPos;
                    float distance = diff.magnitude;

                    if (distance < config.patrolConnectionRadius && !Physics.Raycast(pPos, diff.normalized, distance, config.patrolConnectionLayerMask))
                    {
                        p.connectedPoints.Add(x);
                        connectionsMade++;
                    }
                }
            }
            ApiLog.Log($"[Maps] Patrol point connections made: {connectionsMade}");

            Dictionary<PatrolPoint.PatrolGroup, List<PatrolPoint>> all = new Dictionary<PatrolPoint.PatrolGroup, List<PatrolPoint>>();
            all.Add(PatrolPoint.PatrolGroup.Ant, _points);
            all.Add(PatrolPoint.PatrolGroup.Bear, _points);
            all.Add(PatrolPoint.PatrolGroup.Bird, _points);
            all.Add(PatrolPoint.PatrolGroup.Cat, _points);
            all.Add(PatrolPoint.PatrolGroup.Dog, _points);
            all.Add(PatrolPoint.PatrolGroup.Fish, _points);
            all.Add(PatrolPoint.PatrolGroup.Wolf, _points);
            Level.currentLevel.patrolGroups = all;
        }

        // Monster Removal
        if (config.applyMonsterRemoval)
        {
            RoundSpawner spawner = FindObjectOfType<RoundSpawner>();
            if (spawner == null)
            {
                ApiLog.LogError("[Maps] RoundSpawner not found in scene!");
            }
            else
            {
                GameObject roundRemoveHandler = GameObject.Find(config.roundRemoveHandlerName);
                if (roundRemoveHandler != null)
                {
                    Transform[] options = roundRemoveHandler.GetComponentsInChildren<Transform>();
                    HashSet<string> remove = new HashSet<string>(options.Cast<Transform>().Select(t => t.name));
                    int beforeCount = spawner.possibleSpawns.Length;
                    spawner.possibleSpawns = spawner.possibleSpawns.Where(s => !remove.Contains(s.name)).ToArray();
                    ApiLog.Log($"[Maps] Monster removal applied. Spawns before: {beforeCount}, after: {spawner.possibleSpawns.Length}");
                }
            }
        }

        // Multipliers
        if (config.setupMultipliers)
        {
            GameObject roundMultiplierHandler = GameObject.Find(config.roundMultiplierHandlerName);
            if (roundMultiplierHandler != null)
            {
                foreach (GameObject mult in roundMultiplierHandler.GetComponentsInChildren<GameObject>())
                {
                    if (string.IsNullOrEmpty(mult.name)) continue;
                    try
                    {
                        switch (mult.name[0])
                        {
                            case 'M': multiplierMonster = float.Parse(mult.name.Substring(1)); break;
                            case 'T': multiplierTool = float.Parse(mult.name.Substring(1)); break;
                            case 'A': multiplierArtifact = float.Parse(mult.name.Substring(1)); break;
                        }
                    }
                    catch (System.Exception e)
                    {
                        ApiLog.LogError($"[Maps] Error parsing multiplier from object '{mult.name}': {e.Message}");
                    }
                }
                ApiLog.Log($"[Maps] Multipliers loaded: Monster={multiplierMonster}, Tool={multiplierTool}, Artifact={multiplierArtifact}");
            }
            else
            {
                ApiLog.LogError($"[Maps] {config.roundMultiplierHandlerName} not found; budgets stay at default multipliers (1).");
            }
        }

        // Ambience
        if (config.setupAmbience)
        {
            AmbienceHandler handle = GameObject.FindObjectOfType<AmbienceHandler>();
            GameObject ambienceHolder = GameObject.Find(config.ambienceHolderName);
            if (handle != null && ambienceHolder != null)
            {
                ApiLog.Log("[Maps] Setting up custom ambience.");
                AudioSource ambience = handle.gameObject.GetComponent<AudioSource>();
                AudioSource customAmbience = ambienceHolder.GetComponent<AudioSource>();

                if (ambience != null && customAmbience != null)
                {
                    Destroy(handle);
                    ambience.clip = customAmbience.clip;
                    ambience.pitch = customAmbience.pitch;
                    ambience.loop = true;
                    ambience.volume *= config.ambienceVolumeMultiplier;
                    ambience.Play();
                }
                else
                {
                    ApiLog.LogError($"[Maps] AudioSource missing on AmbienceHandler or {config.ambienceHolderName}!");
                }
            }
        }

        UnityEngine.Random.InitState(GameAPI.seed);
        if (spawns.Count > 0)
        {
            GameObject spawn = spawns[UnityEngine.Random.Range(0, spawns.Count)];
            ApiLog.Log($"[Maps] Selecting spawn point: {spawn.name} at {spawn.transform.position}");

            tmp.transform.position = spawn.transform.position + Vector3.up;
            tmp.transform.rotation = spawn.transform.rotation;

            MethodInfo teleportMethod = typeof(Player).GetMethod("Teleport", BindingFlags.Instance | BindingFlags.NonPublic);
            if (teleportMethod == null)
            {
                ApiLog.LogError("[Maps] Player.Teleport method not found!");
            }
            else
            {
                ApiLog.Log("[Maps] Teleporting player to spawn.");
                teleportMethod.Invoke(Player.localPlayer, new object[] { spawn.transform.position + (Vector3.up * 4), spawn.transform.rotation * Vector3.forward });
            }
        }

        ApiLog.Log("[Maps] Waiting for all players to join...");
        while (PhotonNetwork.PlayerList.Length != PlayerHandler.instance.players.Count)
        {
            yield return null;
        }

        yield return null;

        ApiLog.Log("[Maps] Map setup complete. Level is ready.");
        Level.currentLevel.levelIsReady = true;
        tmp.locked = false;

        MethodInfo setupFinishedMethod = typeof(Level).GetMethod("SetupFinished", BindingFlags.Instance | BindingFlags.NonPublic);
        if (setupFinishedMethod == null)
        {
            ApiLog.LogError("[Maps] Level.SetupFinished method not found!");
        }
        else
        {
            setupFinishedMethod.Invoke(Level.currentLevel, null);
        }
    }
}
