using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Photon.Pun;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace DbsContentApi.Modules;

/// <summary>
/// Data descriptor for a custom map.
/// </summary>
public class CustomMap
{
    public AssetBundle Bundle { get; }
    public string SceneName { get; } // exact path from bundle.GetAllScenePaths()
    public string DisplayName { get; }

    public CustomMap(AssetBundle bundle, string sceneName, string displayName)
    {
        Bundle = bundle;
        SceneName = sceneName;
        DisplayName = displayName;
    }
}

/// <summary>
/// MonoBehaviour responsible for loading and setting up a custom map scene.
/// Ported from CustomMapTest.LoadMap.
/// </summary>
public class CustomMapLoader : MonoBehaviour
{
    public CustomMap? Map { get; set; }

    public static List<PatrolPoint> points = new List<PatrolPoint>();

    public static float multiplierMonster = 1f;
    public static float multiplierTool = 1f;
    public static float multiplierArtifact = 1f;

    private IEnumerator Start()
    {
        if (Map == null)
        {
            Logger.LogError("CustomMapLoader: Map is null!");
            yield break;
        }

        multiplierMonster = 1f;
        multiplierTool = 1f;
        multiplierArtifact = 1f;

        // LevelToPlay < 3 are vanilla maps. This loader is only for 3+.
        if (SurfaceNetworkHandler.RoomStats.LevelToPlay < 3)
        {
            Logger.Log($"[Maps] LevelToPlay ({SurfaceNetworkHandler.RoomStats.LevelToPlay}) is vanilla. Enabling occlusion culling and exiting loader.");
            Camera.main.useOcclusionCulling = true;
            yield break;
        }

        Logger.Log($"[Maps] Loading custom map: {Map.DisplayName} (Scene: {Map.SceneName})");

        // Cleaning
        Camera.main.useOcclusionCulling = false;
        points.Clear();

        DivingBell tmp = GameObject.FindObjectOfType<DivingBell>();
        if (tmp == null)
        {
            Logger.LogError("[Maps] DivingBell not found in scene!");
            yield break;
        }

        Renderer bellRenderer = tmp.GetComponentInChildren<Renderer>();
        if (bellRenderer == null)
        {
            Logger.LogError("[Maps] DivingBell has no Renderer!");
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

        Logger.Log("[Maps] Cleaning up vanilla level objects...");
        foreach (GameObject obj in SceneManager.GetActiveScene().GetRootGameObjects())
        {
            if (obj.name == "PickupHolder(Clone)")
            {
                obj.transform.position += Vector3.up * 3;
            }
            else if (!(obj.name == "GAME" || obj.name == "Remove When DOne" || obj.name == "RoundArtifactSpawner" || obj.name == "VoiceLogger" || obj.name == "Player(Clone)" || obj.name == "RoundSpawnerTools" || obj.name == "Spawns" || obj.GetComponent<RoundSpawner>() != null || obj.GetComponent<AmbienceHandler>() != null))
            {
                Destroy(obj);
            }
        }

        // Load and Setup map
        Logger.Log($"[Maps] Asynchronously loading scene: {Map.SceneName}");
        AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(Map.SceneName, LoadSceneMode.Additive);
        if (asyncLoad == null)
        {
            Logger.LogError($"[Maps] Failed to start loading scene: {Map.SceneName}");
            yield break;
        }
        yield return asyncLoad;
        Logger.Log($"[Maps] Scene loaded: {Map.SceneName}");

        int lightSpheresHandled = 0;
        int objectsRetextured = 0;

        foreach (Renderer rend in FindObjectsOfType<Renderer>())
        {
            if (rend.name.Contains("LightSphere"))
            {
                Material[] mats = rend.sharedMaterials;

                for (int i = 0; i < mats.Length; i++)
                {
                    Shader niceShader = Shader.Find("NiceShader");
                    if (niceShader == null)
                    {
                        Logger.LogError("[Maps] 'NiceShader' not found!");
                    }
                    else
                    {
                        mats[i].shader = niceShader;
                    }
                    mats[i].SetColor("_Color", Color.white);
                }

                rend.sharedMaterials = mats;

                Light l = rend.gameObject.GetComponent<Light>();
                if (l != null) Level.currentLevel.lights.Add(l);
                lightSpheresHandled++;
            }
            else if (rend.transform.root.name != "Spawns" && !rend.transform.root.name.Contains("(Clone)"))
            {
                Material[] mats = rend.sharedMaterials;

                for (int i = 0; i < mats.Length; i++)
                {
                    mats[i] = render;
                    mats[i].SetColor("_Color", Color.gray);
                }

                rend.sharedMaterials = mats;
                rend.gameObject.layer = 10;
                objectsRetextured++;
            }
        }
        Logger.Log($"[Maps] Processed renderers. LightSpheres: {lightSpheresHandled}, Objects retextured: {objectsRetextured}");

        // Markers
        List<GameObject> spawns = new List<GameObject>();
        int patrolPointsAdded = 0;

        foreach (GameObject obj in FindObjectsOfType<GameObject>())
        {
            if (obj.name.Contains("DiveBellSpawn"))
            {
                spawns.Add(obj);
                try
                {
                    Destroy(obj.GetComponentInChildren<Renderer>());
                    Destroy(obj.GetComponentInChildren<Collider>());
                }
                catch { }
            }
            if (obj.name.Contains("PatrolPoint"))
            {
                PatrolPoint point = obj.AddComponent<PatrolPoint>();
                points.Add(point);
                patrolPointsAdded++;
                try
                {
                    Destroy(obj.GetComponentInChildren<Renderer>());
                    Destroy(obj.GetComponentInChildren<Collider>());
                }
                catch { }
            }
        }
        Logger.Log($"[Maps] Markers found. DiveBellSpawns: {spawns.Count}, PatrolPoints: {patrolPointsAdded}");

        if (spawns.Count == 0)
        {
            Logger.LogError("[Maps] No DiveBellSpawn markers found in custom map! Players might spawn in the void.");
        }

        int layer = 1 << 10;

        // Patrol Points connection
        Logger.Log("[Maps] Connecting patrol points...");
        int connectionsMade = 0;
        foreach (PatrolPoint p in points)
        {
            foreach (PatrolPoint x in points)
            {
                if (p == x) continue;

                Vector3 pPos = p.transform.position + (Vector3.up * 2);
                Vector3 xPos = x.transform.position + (Vector3.up * 2);

                Vector3 diff = xPos - pPos;
                float distance = diff.magnitude;

                if (distance < 64f && !Physics.Raycast(pPos, diff.normalized, distance, layer))
                {
                    p.connectedPoints.Add(x);
                    connectionsMade++;
                }
            }
        }
        Logger.Log($"[Maps] Patrol point connections made: {connectionsMade}");

        Dictionary<PatrolPoint.PatrolGroup, List<PatrolPoint>> all = new Dictionary<PatrolPoint.PatrolGroup, List<PatrolPoint>>();
        all.Add(PatrolPoint.PatrolGroup.Ant, points);
        all.Add(PatrolPoint.PatrolGroup.Bear, points);
        all.Add(PatrolPoint.PatrolGroup.Bird, points);
        all.Add(PatrolPoint.PatrolGroup.Cat, points);
        all.Add(PatrolPoint.PatrolGroup.Dog, points);
        all.Add(PatrolPoint.PatrolGroup.Fish, points);
        all.Add(PatrolPoint.PatrolGroup.Wolf, points);
        Level.currentLevel.patrolGroups = all;

        // Monster Removal
        RoundSpawner spawner = FindObjectOfType<RoundSpawner>();
        if (spawner == null)
        {
            Logger.LogError("[Maps] RoundSpawner not found in scene!");
        }
        else
        {
            GameObject roundRemoveHandler = GameObject.Find("RoundRemoveHandler");
            if (roundRemoveHandler != null)
            {
                Transform[] options = roundRemoveHandler.GetComponentsInChildren<Transform>();
                HashSet<string> remove = new HashSet<string>(options.Cast<Transform>().Select(t => t.name));
                int beforeCount = spawner.possibleSpawns.Length;
                spawner.possibleSpawns = spawner.possibleSpawns.Where(s => !remove.Contains(s.name)).ToArray();
                Logger.Log($"[Maps] Monster removal applied. Spawns before: {beforeCount}, after: {spawner.possibleSpawns.Length}");
            }
        }

        // Multipliers — match CustomMapTest: scan full subtree (not only direct children)
        GameObject roundMultiplierHandler = GameObject.Find("RoundMultiplierHandler");
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
                    Logger.LogError($"[Maps] Error parsing multiplier from object '{mult.name}': {e.Message}");
                }
            }
            Logger.Log($"[Maps] Multipliers loaded: Monster={multiplierMonster}, Tool={multiplierTool}, Artifact={multiplierArtifact}");
        }
        else
        {
            Logger.LogError("[Maps] RoundMultiplierHandler not found; budgets stay at default multipliers (1).");
        }

        // Ambience
        AmbienceHandler handle = GameObject.FindObjectOfType<AmbienceHandler>();
        GameObject ambienceHolder = GameObject.Find("AmbienceHolder");
        if (handle != null && ambienceHolder != null)
        {
            Logger.Log("[Maps] Setting up custom ambience.");
            AudioSource ambience = handle.gameObject.GetComponent<AudioSource>();
            AudioSource customAmbience = ambienceHolder.GetComponent<AudioSource>();

            if (ambience != null && customAmbience != null)
            {
                Destroy(handle);
                ambience.clip = customAmbience.clip;
                ambience.pitch = customAmbience.pitch;
                ambience.loop = true;
                ambience.volume *= 6;
                ambience.Play();
            }
            else
            {
                Logger.LogError("[Maps] AudioSource missing on AmbienceHandler or AmbienceHolder!");
            }
        }

        UnityEngine.Random.InitState(GameAPI.seed);
        if (spawns.Count > 0)
        {
            GameObject spawn = spawns[UnityEngine.Random.Range(0, spawns.Count)];
            Logger.Log($"[Maps] Selecting spawn point: {spawn.name} at {spawn.transform.position}");

            tmp.transform.position = spawn.transform.position + Vector3.up;
            tmp.transform.rotation = spawn.transform.rotation;

            MethodInfo teleportMethod = typeof(Player).GetMethod("Teleport", BindingFlags.Instance | BindingFlags.NonPublic);
            if (teleportMethod == null)
            {
                Logger.LogError("[Maps] Player.Teleport method not found!");
            }
            else
            {
                Logger.Log("[Maps] Teleporting player to spawn.");
                teleportMethod.Invoke(Player.localPlayer, new object[] { spawn.transform.position + (Vector3.up * 4), spawn.transform.rotation * Vector3.forward });
            }
        }

        Logger.Log("[Maps] Waiting for all players to join...");
        while (PhotonNetwork.PlayerList.Length != PlayerHandler.instance.players.Count)
        {
            yield return null;
        }

        yield return null;

        Logger.Log("[Maps] Map setup complete. Level is ready.");
        Level.currentLevel.levelIsReady = true;
        tmp.locked = false;

        MethodInfo setupFinishedMethod = typeof(Level).GetMethod("SetupFinished", BindingFlags.Instance | BindingFlags.NonPublic);
        if (setupFinishedMethod == null)
        {
            Logger.LogError("[Maps] Level.SetupFinished method not found!");
        }
        else
        {
            setupFinishedMethod.Invoke(Level.currentLevel, null);
        }
    }
}
