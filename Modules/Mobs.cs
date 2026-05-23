using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

namespace DbsContentApi;

/// <summary>Spawn budget and rarity settings for a custom monster.</summary>
public class BudgetConfig
{
    /// <summary>Budget cost deducted when this monster is spawned.</summary>
    public int budgetCost = 1;

    /// <summary>Relative spawn rarity (lower = rarer).</summary>
    public float rarity = 0.01f;
}

/// <summary>Movement and physics settings for a custom monster's <see cref="PlayerController"/>.</summary>
public class ControllerConfig
{
    /// <summary>Horizontal movement force applied by the controller.</summary>
    public float movementForce = 9f;

    /// <summary>Force applied to keep the character upright.</summary>
    public float standForce = 25f;

    /// <summary>Gravity acceleration applied to the character.</summary>
    public float gravity = 80f;

    /// <summary>Maximum stamina pool.</summary>
    public float maxStamina = 100f;

    /// <summary>Rate at which stamina regenerates per second.</summary>
    public float staminaRegRate = 100f;

    /// <summary>Stamina threshold required before regeneration resumes.</summary>
    public float staminaReActivationThreshold = 100f;

    /// <summary>Initial upward impulse applied on jump.</summary>
    public float jumpImpulse = 7f;

    /// <summary>Additional jump force applied over time while jumping.</summary>
    public float jumpForceOverTime = 0.6f;

    /// <summary>Constant gravity multiplier applied each frame.</summary>
    public float constantGravity = 1f;
}

/// <summary>Core player component settings for a custom monster.</summary>
public class PlayerConfig
{
    /// <summary>Target capsule height for the character.</summary>
    public float targetHeight = 3f;

    /// <summary>Whether the character is AI-controlled.</summary>
    public bool ai = true;
}

/// <summary>Configuration for creating or updating a <see cref="RigCreator"/> on a monster prefab.</summary>
public class RigCreatorConfig
{
    /// <summary>
    /// Optional bodypart configuration for RigCreator.
    /// If null, any existing RigCreator configuration on the prefab is used.
    /// Required if RigCreator doesn't exist on the prefab.
    /// </summary>
    public List<RigCreatorBodypart>? bodyparts;

    /// <summary>Whether rig parts use gravity.</summary>
    public bool useGravity = false;

    /// <summary>Multiplier applied to each bodypart's mass.</summary>
    public float massMultiplier = 1f;

    /// <summary>Whether joints use target rotation constraints.</summary>
    public bool useTargetRotation = true;

    /// <summary>Spring strength for target rotation joints.</summary>
    public float targetRotationSpring = 300f;

    /// <summary>Drag factor applied to target rotation spring (multiplied by <see cref="targetRotationSpring"/>).</summary>
    public float targetRotationDragFactor = 0.05f;

    /// <summary>Smart-fill legs mode passed to <see cref="RigCreator.SmartFillLegs"/>.</summary>
    public int smartFillLegs = 0;

    /// <summary>Default layer index assigned to created rig colliders.</summary>
    public int setDefaultLayer = 0;
}

/// <summary>Ragdoll physics settings for a custom monster.</summary>
public class RagdollConfig
{
    /// <summary>Whether opposing forces are applied during ragdoll simulation.</summary>
    public bool addOpposingForce = false;

    /// <summary>Force magnitude applied to ragdoll parts.</summary>
    public float force = 60f;

    /// <summary>Torque magnitude applied to ragdoll parts.</summary>
    public float torque = 0.1f;

    /// <summary>Linear drag on ragdoll rigidbodies.</summary>
    public float drag = 0.92f;

    /// <summary>Angular drag on ragdoll rigidbodies.</summary>
    public float angularDrag = 0.9f;
}

/// <summary>Animation punch and movement multiplier settings on the RigCreator object.</summary>
public class MonsterAnimationValuesConfig
{
    /// <summary>Whether the right punch animation is enabled.</summary>
    public bool rightPunch = false;

    /// <summary>Whether the left punch animation is enabled.</summary>
    public bool leftPunch = false;

    /// <summary>Multiplier applied to movement animation speed.</summary>
    public float movementMultiplier = 1f;
}

/// <summary>Photon networking settings for a custom monster.</summary>
public class PhotonViewConfig
{
    /// <summary>Network synchronization mode for the PhotonView.</summary>
    public ViewSynchronization synchronization = ViewSynchronization.UnreliableOnChange;

    /// <summary>Whether the PhotonView observes the monster's <see cref="MonsterSyncer"/> component.</summary>
    public bool observeMonsterSyncer = true;
}

/// <summary>Base AI bot settings for a custom monster.</summary>
public class BotConfig
{
    /// <summary>Patrol groups this bot participates in.</summary>
    public List<PatrolPoint.PatrolGroup> patrolGroups = new List<PatrolPoint.PatrolGroup> { PatrolPoint.PatrolGroup.Bear, PatrolPoint.PatrolGroup.Dog };

    /// <summary>Attack behavior type index.</summary>
    public int attackType = 0;

    /// <summary>Turn velocity for the bot.</summary>
    public float turnVel = 0f;

    /// <summary>Multiplier applied to animation move speed.</summary>
    public float animMoveSpeedFactor = 1f;

    /// <summary>Whether the bot can be alerted by player actions.</summary>
    public bool alertable = true;
}

/// <summary>NavMeshAgent settings for a custom monster's bot child object.</summary>
public class NavMeshAgentConfig
{
    /// <summary>Maximum movement speed.</summary>
    public float speed = 3.5f;

    /// <summary>Acceleration rate.</summary>
    public float acceleration = 0f;

    /// <summary>Maximum turning speed in degrees per second.</summary>
    public float angularSpeed = 120f;

    /// <summary>Distance from target at which the agent stops.</summary>
    public float stoppingDistance = 0f;

    /// <summary>Agent radius for pathfinding.</summary>
    public float radius = 1f;

    /// <summary>Agent height for pathfinding.</summary>
    public float height = 2f;

    /// <summary>When <c>true</c>, uses the wide NavMesh agent type.</summary>
    public bool wide = false;

    /// <summary>Obstacle avoidance quality level.</summary>
    public ObstacleAvoidanceType obstacleAvoidanceType = ObstacleAvoidanceType.HighQualityObstacleAvoidance;
}

/// <summary>Chase/flee AI settings for <see cref="Bot_Chaser"/> behavior.</summary>
public class BotChaserConfig
{
    /// <summary>Seconds before the chaser becomes exhausted.</summary>
    public float exhaustionTime = 20f;

    /// <summary>Seconds the bot flees after exhausting.</summary>
    public float fleeForSeconds = 20f;

    /// <summary>Multiplier applied to exhaustion time while the target is hiding.</summary>
    public float hidingExhaustionMultiplier = 1f;

    /// <summary>Seconds without line of sight before the target is lost.</summary>
    public float timeToLoseTarget = 2f;

    /// <summary>Preferred distance to maintain from the target while chasing.</summary>
    public float targetDistance = 15f;

    /// <summary>Whether the bot backs away when too close to the target.</summary>
    public bool backUpIfTooClose = true;

    /// <summary>Whether the bot can rotate while standing still.</summary>
    public bool canRotateWhenStandingStill = true;

    /// <summary>Turn rate while chasing.</summary>
    public float chaseTurnRate = 6f;

    /// <summary>Turn rate while fleeing.</summary>
    public float fleeTurnRate = 6f;

    /// <summary>Turn rate while investigating.</summary>
    public float investigateTurnRate = 3f;

    /// <summary>Turn rate while patrolling.</summary>
    public float patrolTurnRate = 3f;

    /// <summary>Maximum detection range.</summary>
    public float maxRange = 70f;

    /// <summary>Maximum field-of-view angle for detection.</summary>
    public float maxAngle = 110f;

    /// <summary>Seconds of continuous visibility required to acquire a target.</summary>
    public float timeToSeeTarget = 1f;
}

/// <summary>Drag/pull AI settings for <see cref="Bot_Drag"/> behavior.</summary>
public class BotDragConfig
{
    /// <summary>Force applied when dragging the target.</summary>
    public float dragForce = 150f;

    /// <summary>Force applied at the hand attachment point.</summary>
    public float handForce = 150f;

    /// <summary>Maximum range at which dragging can begin.</summary>
    public float range = 2f;
}

/// <summary>Leap attack AI settings for <see cref="Bot_Knifo"/> behavior.</summary>
public class BotKnifoConfig
{
    /// <summary>Bodypart used as the main rig reference for jump attacks.</summary>
    public BodypartType mainRigBodyPart = BodypartType.Elbow_L;

    /// <summary>Distance at which the bot initiates a jump attack.</summary>
    public float targetDistance = 1f;

    /// <summary>Forward force applied during a jump attack.</summary>
    public float jumpForceForward = 15f;

    /// <summary>Upward force applied during a jump attack.</summary>
    public float jumpForceUp = 15f;
}

/// <summary>Toolkit Boy AI settings for <see cref="Bot_ToolkitBoy"/> behavior.</summary>
public class BotToolkitBoyConfig
{
    /// <summary>Preferred distance to maintain from the target.</summary>
    public float targetDistance = 2f;
}

/// <summary>
/// Master configuration for <see cref="Mobs.SetupCustomMonster"/>.
/// Only non-null sections and enabled flags are applied to the prefab.
/// </summary>
public class MobSetupConfig
{
    /// <summary>
    /// Visual + RigCreator configuration.
    /// - If null: no PlayerVisual/RigCreator setup is performed
    /// - If provided: intelligently sets up missing components based on what's already on the prefab
    /// </summary>
    public RigCreatorConfig? visualRig;

    /// <summary>Spawn budget configuration. Applied when non-null.</summary>
    public BudgetConfig? budget;

    /// <summary>Player controller configuration. Applied when non-null.</summary>
    public ControllerConfig? controller;

    /// <summary>When <c>true</c>, adds a <see cref="PlayerAnimRefHandler"/> component.</summary>
    public bool addAnimRefHandler;

    /// <summary>Player component configuration. Applied when non-null.</summary>
    public PlayerConfig? player;

    /// <summary>Ragdoll configuration. Applied when non-null.</summary>
    public RagdollConfig? ragdoll;

    /// <summary>PhotonView configuration. Applied when non-null.</summary>
    public PhotonViewConfig? photonView;

    /// <summary>Bot AI configuration. Applied when non-null.</summary>
    public BotConfig? bot;

    /// <summary>NavMeshAgent configuration for the bot child. Applied when non-null.</summary>
    public NavMeshAgentConfig? navMesh;

    /// <summary>Animation values configuration. Applied when non-null.</summary>
    public MonsterAnimationValuesConfig? monsterAnimationValues;

    /// <summary>When <c>true</c>, adds a <see cref="MonsterAnimationHandler"/> component.</summary>
    public bool addMonsterAnimationHandler;

    /// <summary>When <c>true</c>, adds a <see cref="MonsterSyncer"/> component.</summary>
    public bool addMonsterSyncer;

    /// <summary>When <c>true</c>, adds a <see cref="HeadFollower"/> to the HeadPosition child.</summary>
    public bool addHeadFollower;

    /// <summary>When <c>true</c>, adds a <see cref="PlayerGroundPositionTransform"/> to the PlayerGroundPos child.</summary>
    public bool addGroundPos;
}

/// <summary>
/// API for registering and configuring custom monster prefabs in Content Warning.
/// </summary>
public static class Mobs
{
    /// <summary>
    /// Registers a custom monster with the game.
    /// The prefab may come from an AssetBundle, be built in code, or be authored in the Unity editor.
    /// </summary>
    /// <param name="prefab">Monster prefab (already loaded or constructed).</param>
    /// <param name="monsterName">Display/logical name used during setup.</param>
    /// <param name="config">Component setup configuration.</param>
    /// <param name="material">Optional game material applied after setup.</param>
    /// <param name="postSetup">Optional callback invoked after setup, before registration.</param>
    /// <returns>The configured and registered monster prefab.</returns>
    public static GameObject RegisterMonster(
        GameObject prefab,
        string monsterName,
        MobSetupConfig config,
        GameMaterial? material = null,
        Action<GameObject>? postSetup = null)
    {
        if (prefab == null)
            throw new ArgumentNullException(nameof(prefab));

        SetupCustomMonster(prefab, monsterName, config);
        postSetup?.Invoke(prefab);
        if (material.HasValue)
            GameMaterials.ApplyOnLoad(prefab, material.Value, recursive: true);
        DbsContentApiPlugin.customMonsters.Add(prefab);
        return prefab;
    }

    /// <summary>
    /// Looks up a registered monster prefab by its Unity object name.
    /// </summary>
    /// <param name="prefabName">The Unity object name of the prefab to find.</param>
    /// <param name="prefab">The found prefab, or <c>null</c> if not registered.</param>
    /// <returns><c>true</c> if a matching prefab was found.</returns>
    public static bool TryGetRegisteredPrefab(string prefabName, out GameObject? prefab)
    {
        prefab = DbsContentApiPlugin.customMonsters.FirstOrDefault(m => m.name == prefabName);
        return prefab != null;
    }

    /// <summary>
    /// Configures a custom monster by adding specified components and setting up the rig.
    /// </summary>
    /// <param name="monster">The monster prefab root GameObject.</param>
    /// <param name="monsterName">Display/logical name used in logs.</param>
    /// <param name="config">Component setup configuration.</param>
    public static void SetupCustomMonster(GameObject monster, string monsterName, MobSetupConfig config)
    {
        ApiLog.Log($"Starting setup for custom monster: {monsterName}");

        if (config.budget != null)
        {
            ApiLog.Log($"  Setting up Budget component for {monsterName}");
            SetupBudget(monster, config.budget);
        }
        if (config.player != null)
        {
            ApiLog.Log($"  Setting up Player component for {monsterName}");
            SetupPlayer(monster, config.player);
        }
        if (config.ragdoll != null)
        {
            ApiLog.Log($"  Setting up Ragdoll component for {monsterName}");
            SetupRagdoll(monster, config.ragdoll);
        }
        if (config.addMonsterSyncer)
        {
            ApiLog.Log($"  Setting up MonsterSyncer component for {monsterName}");
            SetupMonsterSyncer(monster);
        }
        if (config.controller != null)
        {
            ApiLog.Log($"  Setting up Controller component for {monsterName}");
            SetupController(monster, config.controller);
        }
        if (config.addAnimRefHandler)
        {
            ApiLog.Log($"  Setting up AnimRefHandler component for {monsterName}");
            SetupAnimRefHandler(monster);
        }
        if (config.photonView != null)
        {
            ApiLog.Log($"  Setting up PhotonView component for {monsterName}");
            SetupPhotonView(monster, config.photonView);
        }
        if (config.bot != null)
        {
            ApiLog.Log($"  Setting up Bot component for {monsterName}");
            SetupBot(monster, monsterName, config.bot, config.navMesh);
        }

        // Visual + RigCreator setup (intelligently handles what's already on the prefab)
        if (config.visualRig != null)
        {
            ApiLog.Log($"  Setting up Visual and RigCreator for {monsterName}");
            SetupVisualAndRigCreator(monster, monsterName, config.visualRig);
        }

        if (config.addHeadFollower)
        {
            ApiLog.Log($"  Setting up HeadFollower component for {monsterName}");
            SetupHeadFollower(monster);
        }
        if (config.addGroundPos)
        {
            ApiLog.Log($"  Setting up GroundPositionTransform component for {monsterName}");
            SetupGroundPositionTransform(monster);
        }
        if (config.addMonsterAnimationHandler)
        {
            ApiLog.Log($"  Setting up MonsterAnimationHandler component for {monsterName}");
            SetupMonsterAnimationHandler(monster);
        }
        if (config.monsterAnimationValues != null)
        {
            ApiLog.Log($"  Setting up MonsterAnimationValues component for {monsterName}");
            SetupMonsterAnimationValues(monster, config.monsterAnimationValues);
        }

        ApiLog.Log($"Completed setup for custom monster: {monsterName}");
    }


    /// <summary>
    /// Setup MonsterAnimationValues component on the RigCreator GameObject.
    /// </summary>
    private static void SetupMonsterAnimationValues(GameObject monster, MonsterAnimationValuesConfig config)
    {
        var monsterRigCreator = monster.transform.Find("RigCreator");
        if (monsterRigCreator == null)
        {
            throw new System.Exception($"RigCreator not found for {monster.name}");
        }
        var monsterAnimationValues = monsterRigCreator.gameObject.AddComponent<MonsterAnimationValues>();
        monsterAnimationValues.rightPunch = config.rightPunch;
        monsterAnimationValues.leftPunch = config.leftPunch;
        monsterAnimationValues.movementMultiplier = config.movementMultiplier;
    }


    /// <summary>
    /// Finds the Bot child GameObject of a monster prefab.
    /// </summary>
    /// <param name="customMonsterPrefab">The monster prefab root GameObject.</param>
    /// <returns>The <c>Bot_{name}</c> child GameObject.</returns>
    public static GameObject GetBotChildObject(GameObject customMonsterPrefab)
    {
        string botChildName = $"Bot_{customMonsterPrefab.name}";
        Transform? botTransform = customMonsterPrefab.transform.Find(botChildName);
        if (botTransform == null)
        {
            throw new InvalidOperationException(
                $"Bot child '{botChildName}' not found on monster prefab '{customMonsterPrefab.name}'. " +
                "Ensure MobSetupConfig.bot is set so Mobs.RegisterMonster creates the bot child.");
        }

        GameObject botObject = botTransform.gameObject;
        ApiLog.Log($"  Retrieved Bot child object '{botObject.name}' for {customMonsterPrefab.name}");
        return botObject;
    }

    /// <summary>
    /// Replaces "World Optimized" shaders with the internal game shader from the "Zombe" monster.
    /// </summary>
    /// <param name="prefab">The prefab whose renderers should be updated.</param>
    public static void RestoreShaders(GameObject prefab)
    {
        ApiLog.Log($"  Restoring shaders for {prefab.name}");
        var renderers = prefab.GetComponentsInChildren<Renderer>();
        var zombePrefab = Resources.Load<GameObject>("Zombe");
        if (zombePrefab == null)
        {
            ApiLog.LogError($"  Zombe prefab not found, skipping shader restoration for {prefab.name}");
            return;
        }

        var zombeShader = zombePrefab.GetComponentInChildren<Renderer>().sharedMaterial.shader;
        int shaderReplacements = 0;
        foreach (var renderer in renderers)
        {
            foreach (var mat in renderer.sharedMaterials)
            {
                if (mat.shader.name == "World Optimized")
                {
                    mat.shader = zombeShader;
                    shaderReplacements++;
                }
            }
        }
        ApiLog.Log($"  Restored {shaderReplacements} shader(s) for {prefab.name}");
    }

    private static void SetupBudget(GameObject monster, BudgetConfig config)
    {
        var budget = monster.AddComponent<BudgetCost>();
        budget.budgetCost = config.budgetCost;
        budget.rarity = config.rarity;
        ApiLog.Log($"Budget configured: cost={config.budgetCost}, rarity={config.rarity}");
    }

    private static void SetupController(GameObject monster, ControllerConfig config)
    {
        var pc = monster.AddComponent<PlayerController>();
        pc.movementForce = config.movementForce;
        pc.standForce = config.standForce;
        pc.gravity = config.gravity;
        pc.maxStamina = config.maxStamina;
        pc.staminaRegRate = config.staminaRegRate;
        pc.staminaReActivationThreshold = config.staminaReActivationThreshold;
        pc.jumpImpulse = config.jumpImpulse;
        pc.jumpForceOverTime = config.jumpForceOverTime;
        pc.constantGravity = config.constantGravity;
        ApiLog.Log($"Controller configured: movementForce={config.movementForce}, maxStamina={config.maxStamina}");
    }

    private static void SetupAnimRefHandler(GameObject monster)
    {
        monster.AddComponent<PlayerAnimRefHandler>();
        ApiLog.Log($"AnimRefHandler component added");
    }

    private static void SetupPlayer(GameObject monster, PlayerConfig config)
    {
        var player = monster.AddComponent<Player>();
        player.ai = config.ai;
        player.input = new Player.PlayerInput();
        player.data = new Player.PlayerData();
        player.data.targetHeight = config.targetHeight;
        player.refs = new Player.PlayerRefs();
        ApiLog.Log($"Player configured: targetHeight={config.targetHeight}, ai={player.ai}");
    }

    private static void SetupRagdoll(GameObject monster, RagdollConfig config)
    {
        var pr = monster.AddComponent<PlayerRagdoll>();
        pr.addOpposingForce = config.addOpposingForce;
        pr.force = config.force;
        pr.torque = config.torque;
        pr.drag = config.drag;
        pr.angularDrag = config.angularDrag;
        ApiLog.Log($"Ragdoll configured: force={config.force}, torque={config.torque}, drag={config.drag}");
    }

    private static void SetupMonsterSyncer(GameObject monster)
    {
        var ms = monster.AddComponent<MonsterSyncer>();
        ms.applyData = true;
        ApiLog.Log($"MonsterSyncer component added (applyData={ms.applyData})");
    }

    private static void SetupVisualAndRigCreator(GameObject monster, string monsterName, RigCreatorConfig config)
    {
        // Find the Visual transform
        var visualTransform = monster.transform.Find("Visual");
        if (visualTransform == null)
        {
            throw new System.Exception($"Visual GameObject not found for {monsterName}. A 'Visual' child is required.");
        }

        // Check what already exists
        var existingPlayerVisual = visualTransform.GetComponent<PlayerVisual>();
        var rigCreatorTransform = monster.transform.Find("RigCreator");
        RigCreator? existingRigCreator = rigCreatorTransform?.GetComponent<RigCreator>();
        bool rigAlreadyCreated = rigCreatorTransform?.Find("Rig") != null;

        ApiLog.Log($"Visual/Rig status: PlayerVisual={(existingPlayerVisual != null ? "exists" : "missing")}, " +
                   $"RigCreator={(existingRigCreator != null ? "exists" : "missing")}, " +
                   $"Rig={(rigAlreadyCreated ? "created" : "not created")}");

        // Scenario 1: Both exist and rig is already created
        if (existingPlayerVisual != null && existingRigCreator != null && rigAlreadyCreated)
        {
            ApiLog.Log($"Scenario 1: Both PlayerVisual and RigCreator are fully configured, nothing to do");
            return;
        }

        // Scenario 2: RigCreator exists (and possibly rig is created) but PlayerVisual is missing
        if (existingRigCreator != null && existingPlayerVisual == null)
        {
            ApiLog.Log($"Scenario 2: RigCreator exists but PlayerVisual missing, adding PlayerVisual");
            var playerVisual = visualTransform.gameObject.AddComponent<PlayerVisual>();

            // If rig is already created, call SetTargets now
            if (rigAlreadyCreated)
            {
                ApiLog.Log($"Rig already created, calling SetTargets on PlayerVisual");
                playerVisual.SetTargets();
            }
            else
            {
                // If rig not created yet, we need to create it and wire up the event
                ApiLog.Log($"Rig not created yet, will call SetTargets after rig creation");
                existingRigCreator.createRigEvent = existingRigCreator.createRigEvent ?? new UnityEngine.Events.UnityEvent();
                existingRigCreator.createRigEvent.AddListener(() => VisualSetTargets(monster));

                ApiLog.Log($"Creating rig for existing RigCreator");
                CustomCreateRig(existingRigCreator);
            }
            return;
        }

        // Scenario 3 & 4: RigCreator doesn't exist (PlayerVisual may or may not exist)
        // If PlayerVisual doesn't exist, create it
        if (existingPlayerVisual == null)
        {
            ApiLog.Log($"Scenario 4: Neither PlayerVisual nor RigCreator exist, creating both");
            visualTransform.gameObject.AddComponent<PlayerVisual>();
        }
        else
        {
            ApiLog.Log($"Scenario 3: PlayerVisual exists but RigCreator missing, creating RigCreator");
        }

        // Now create and configure RigCreator
        if (config.bodyparts == null)
        {
            throw new System.Exception($"bodyparts configuration is required when RigCreator doesn't exist on the prefab for {monsterName}");
        }

        ApiLog.Log($"Creating RigCreator GameObject with {config.bodyparts.Count} bodyparts");
        var rigCreatorObject = new GameObject("RigCreator");
        rigCreatorObject.transform.SetParent(monster.transform);
        rigCreatorObject.transform.localPosition = Vector3.zero;
        rigCreatorObject.transform.localRotation = Quaternion.identity;
        var rigCreator = rigCreatorObject.AddComponent<RigCreator>();

        // Configure RigCreator
        rigCreator.source = visualTransform.gameObject;
        ApiLog.Log($"RigCreator source set to Visual GameObject");

        ApiLog.Log($"Setting up RigCreator resources");
        SetupRigCreatorResources(rigCreator);

        // Apply configuration
        ApiLog.Log($"Applying RigCreator configuration");
        rigCreator.useGravity = config.useGravity;
        rigCreator.massMultiplier = config.massMultiplier;
        rigCreator.useTargetRotation = config.useTargetRotation;
        rigCreator.targetRotationSpring = config.targetRotationSpring;
        rigCreator.targetRotationDragFactor = config.targetRotationDragFactor;
        rigCreator.smartFillLegs = config.smartFillLegs;
        rigCreator.setDefaultLayer = config.setDefaultLayer;
        rigCreator.bodyparts = config.bodyparts;

        // Wire up the createRigEvent to call SetTargets after rig creation
        rigCreator.createRigEvent = new UnityEngine.Events.UnityEvent();
        rigCreator.createRigEvent.AddListener(() => VisualSetTargets(monster));

        ApiLog.Log($"Creating rig for {monsterName}");
        CustomCreateRig(rigCreator);
        ApiLog.Log($"Rig creation completed for {monsterName}");
    }

    private static void VisualSetTargets(GameObject monster)
    {
        var visual = monster.transform.Find("Visual")?.gameObject;
        if (visual != null)
        {
            var playerVisual = visual.GetComponent<PlayerVisual>();
            if (playerVisual != null)
            {
                playerVisual.SetTargets();
                ApiLog.Log($"Visual targets set successfully");
            }
            else
            {
                ApiLog.LogError($"PlayerVisual component not found on Visual for {monster.name}");
            }
        }
        else
        {
            ApiLog.LogError($"Visual GameObject not found for {monster.name}");
        }
    }

    private static void SetupPhotonView(GameObject monster, PhotonViewConfig config)
    {
        var pv = monster.AddComponent<PhotonView>();
        pv.Synchronization = config.synchronization;
        if (config.observeMonsterSyncer)
        {
            var ms = monster.GetComponent<MonsterSyncer>();
            if (ms != null)
            {
                pv.ObservedComponents = new List<Component> { ms };
                ApiLog.Log($"PhotonView configured: synchronization={config.synchronization}, observing MonsterSyncer");
            }
            else
            {
                ApiLog.LogError($"MonsterSyncer not found for PhotonView observation on {monster.name}");
            }
        }
        else
        {
            ApiLog.Log($"PhotonView configured: synchronization={config.synchronization}");
        }
        pv.observableSearch = PhotonView.ObservableSearch.AutoFindAll;
    }

    private static void SetupHeadFollower(GameObject monster)
    {
        var headObj = monster.transform.Find("HeadPosition")?.gameObject;
        if (headObj != null)
        {
            headObj.AddComponent<HeadFollower>();
            ApiLog.Log($"HeadFollower component added to HeadPosition");
        }
        else
        {
            ApiLog.LogError($"HeadPosition not found for {monster.name}");
        }
    }

    private static void SetupGroundPositionTransform(GameObject monster)
    {
        var groundObj = monster.transform.Find("PlayerGroundPos")?.gameObject;
        if (groundObj != null)
        {
            groundObj.AddComponent<PlayerGroundPositionTransform>();
            ApiLog.Log($"PlayerGroundPositionTransform component added to PlayerGroundPos");
        }
        else
        {
            ApiLog.LogError($"PlayerGroundPos not found for {monster.name}");
        }
    }

    private static void SetupMonsterAnimationHandler(GameObject monster)
    {
        monster.AddComponent<MonsterAnimationHandler>();
        ApiLog.Log($"MonsterAnimationHandler component added");
    }

    private static void SetupBot(GameObject monster, string monsterName, BotConfig botConfig, NavMeshAgentConfig? navConfig)
    {
        ApiLog.Log($"Creating Bot GameObject 'Bot_{monsterName}' for {monsterName}");
        var botObject = new GameObject($"Bot_{monsterName}");
        botObject.transform.SetParent(monster.transform);
        botObject.transform.localPosition = Vector3.zero;
        botObject.transform.localRotation = Quaternion.identity;
        botObject.transform.localScale = new Vector3(1f, 1f, 1f);

        ApiLog.Log($"Adding PhotonView to Bot for {monsterName}");
        var pv = botObject.AddComponent<PhotonView>();
        pv.Synchronization = ViewSynchronization.UnreliableOnChange;
        pv.observableSearch = PhotonView.ObservableSearch.AutoFindAll;

        ApiLog.Log($"Configuring Bot component: patrolGroups={botConfig.patrolGroups.Count}, attackType={botConfig.attackType}, alertable={botConfig.alertable}");
        var bot = botObject.AddComponent<Bot>();
        bot.patrolGroups = botConfig.patrolGroups;
        bot.attackType = botConfig.attackType;
        bot.turnVel = botConfig.turnVel;
        bot.animMoveSpeedFactor = botConfig.animMoveSpeedFactor;
        bot.alertable = botConfig.alertable;

        bot.groundTransform = monster.transform.Find("PlayerGroundPos");
        bot.syncData = new Bot.SyncData { targetPlayerId = -1 };
        var centerTransformHipFollower = monster.transform.Find("Center");
        bot.centerTransform = centerTransformHipFollower;
        if (centerTransformHipFollower != null)
        {
            ApiLog.Log($"Bot centerTransform set to Hip");
        }
        else
        {
            ApiLog.LogError($"Center transform not found for Bot centerTransform on {monsterName}");
        }

        if (navConfig != null)
        {
            ApiLog.Log($"Configuring NavMeshAgent: speed={navConfig.speed}, angularSpeed={navConfig.angularSpeed}, radius={navConfig.radius}, height={navConfig.height}");
            var nav = botObject.AddComponent<NavMeshAgent>();
            nav.speed = navConfig.speed;
            nav.acceleration = navConfig.acceleration;
            nav.angularSpeed = navConfig.angularSpeed;
            nav.stoppingDistance = navConfig.stoppingDistance;
            nav.radius = navConfig.radius;
            nav.height = navConfig.height;
            nav.obstacleAvoidanceType = navConfig.obstacleAvoidanceType;
            if (navConfig.wide)
            {
                nav.agentTypeID = -1372625422;
            }
            else
            {
                nav.agentTypeID = 0;
            }
        }
        else
        {
            ApiLog.Log($"No NavMeshConfig provided, skipping NavMeshAgent setup");
        }

        ApiLog.Log($"Adding Bot_Nav_Navmesh and Bot_RagdollCharacter components");
        botObject.AddComponent<Bot_Nav_Navmesh>();
        botObject.AddComponent<Bot_RagdollCharacter>();
        ApiLog.Log($"Bot setup completed for {monsterName}");
    }

    /// <summary>Adds a <see cref="Bot_Zombie"/> component to the bot child object.</summary>
    /// <param name="botObject">The bot child GameObject (see <see cref="GetBotChildObject"/>).</param>
    public static void AddBotZombieComponent(GameObject botObject)
    {
        botObject.AddComponent<Bot_Zombie>();
        ApiLog.Log($"  Added Bot_Zombie component to {botObject.name}");
    }

    /// <summary>Adds a <see cref="Bot_ToolkitBoy"/> component to the bot child object.</summary>
    /// <param name="botObject">The bot child GameObject.</param>
    /// <param name="config">Optional behavior configuration; defaults are used when <c>null</c>.</param>
    public static void AddBotToolkitBoyComponent(GameObject botObject, BotToolkitBoyConfig? config = null)
    {
        config ??= new BotToolkitBoyConfig();
        var b = botObject.AddComponent<Bot_ToolkitBoy>();
        b.targetDistance = config.targetDistance;
        ApiLog.Log($"  Added Bot_ToolkitBoy component to {botObject.name} (targetDistance={config.targetDistance})");
    }

    /// <summary>Adds a <see cref="Bot_Knifo"/> component to the bot child object.</summary>
    /// <param name="botObject">The bot child GameObject.</param>
    /// <param name="config">Optional behavior configuration; defaults are used when <c>null</c>.</param>
    public static void AddBotKnifoComponent(GameObject botObject, BotKnifoConfig? config = null)
    {
        config ??= new BotKnifoConfig();
        var b = botObject.AddComponent<Bot_Knifo>();
        b.targetDistance = config.targetDistance;
        b.mainRig = config.mainRigBodyPart;
        b.jumpForceForward = config.jumpForceForward;
        b.jumpForceUp = config.jumpForceUp;
        b.jumpCurve = Resources.Load<GameObject>("Knifo")?.transform.Find("Bot_Knifo")?.GetComponent<Bot_Knifo>()?.jumpCurve;
        ApiLog.Log($"  Added Bot_Knifo component to {botObject.name} (mainRig={config.mainRigBodyPart})");
    }

    /// <summary>Adds a <see cref="Bot_Chaser"/> component to the bot child object.</summary>
    /// <param name="botObject">The bot child GameObject.</param>
    /// <param name="config">Optional behavior configuration; defaults are used when <c>null</c>.</param>
    /// <returns>The created <see cref="Bot_Chaser"/> instance.</returns>
    public static Bot_Chaser AddBotChaserComponent(GameObject botObject, BotChaserConfig? config = null)
    {
        config ??= new BotChaserConfig();
        var b = botObject.AddComponent<Bot_Chaser>();
        b.exhastionTime = config.exhaustionTime;
        b.fleeForSeconds = config.fleeForSeconds;
        b.hidingExhastionMultiplier = config.hidingExhaustionMultiplier;
        b.timeToLoseTarget = config.timeToLoseTarget;
        b.targetDistance = config.targetDistance;
        b.backUpIfTooClose = config.backUpIfTooClose;
        b.canRotateWhenStandingStill = config.canRotateWhenStandingStill;
        b.chaseTurnRate = config.chaseTurnRate;
        b.fleeTurnRate = config.fleeTurnRate;
        b.investigateTurnRate = config.investigateTurnRate;
        b.patrolTurnRate = config.patrolTurnRate;
        b.maxRange = config.maxRange;
        b.maxAngle = config.maxAngle;
        b.timeToSeeTarget = config.timeToSeeTarget;

        ApiLog.Log($"  Added Bot_Chaser component to {botObject.name} (maxRange={b.maxRange}, targetDistance={b.targetDistance})");
        return b;
    }

    /// <summary>Adds a <see cref="Bot_Drag"/> component to the bot child object.</summary>
    /// <param name="botObject">The bot child GameObject.</param>
    /// <param name="config">Optional behavior configuration; defaults are used when <c>null</c>.</param>
    public static void AddBotDragComponent(GameObject botObject, BotDragConfig? config = null)
    {
        config ??= new BotDragConfig();
        var b = botObject.AddComponent<Bot_Drag>();
        b.dragForce = config.dragForce;
        b.handForce = config.handForce;
        b.range = config.range;
        ApiLog.Log($"  Added Bot_Drag component to {botObject.name} (dragForce={b.dragForce}, range={b.range})");
    }

    private static void SetupRigCreatorResources(RigCreator rigCreator)
    {
        RigCreator? template = null;
        foreach (var rc in Resources.FindObjectsOfTypeAll<RigCreator>())
        {
            if (rc.boxColldier != null && rc.capsuleCol != null && rc.slipperyMat != null)
            {
                template = rc;
                ApiLog.Log($"  Found RigCreator template from existing instance");
                break;
            }
        }

        if (template == null)
        {
            ApiLog.Log($"  Searching for Zombe prefab as RigCreator template");
            var zombePrefab = Resources.Load<GameObject>("Zombe");
            if (zombePrefab != null)
            {
                template = zombePrefab.GetComponentInChildren<RigCreator>(true);
                if (template != null)
                {
                    ApiLog.Log($"  Found RigCreator template from Zombe prefab");
                }
            }
        }

        if (template != null)
        {
            rigCreator.boxColldier = template.boxColldier;
            rigCreator.capsuleCol = template.capsuleCol;
            rigCreator.sphereCol = template.sphereCol;
            rigCreator.slipperyMat = template.slipperyMat;
            ApiLog.Log($"  RigCreator resources copied from template");
        }
        else if (rigCreator.slipperyMat == null)
        {
            ApiLog.Log($"  Searching for slippery PhysicMaterial");
            foreach (var mat in Resources.FindObjectsOfTypeAll<UnityEngine.PhysicsMaterial>())
            {
                if (mat.name.ToLower().Contains("slippery"))
                {
                    rigCreator.slipperyMat = mat;
                    ApiLog.Log($"  Found slippery PhysicMaterial: {mat.name}");
                    break;
                }
            }
        }
        else
        {
            ApiLog.LogError($"  No RigCreator template found and no slippery material available");
        }
    }

    private static void CustomCreateRig(RigCreator rigCreator)
    {
        ApiLog.Log($"  SmartFillLegs()");
        rigCreator.SmartFillLegs();
        Transform transform = rigCreator.gameObject.transform.Find("Rig");
        if (transform)
        {
            ApiLog.Log($"  Destroying existing Rig GameObject");
            UnityEngine.Object.DestroyImmediate(transform.gameObject);
        }
        ApiLog.Log($"  Instantiating rig source");
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(rigCreator.source, rigCreator.gameObject.transform.position, rigCreator.gameObject.transform.rotation, rigCreator.gameObject.transform);
        gameObject.SetActive(true);
        gameObject.name = "Rig";
        ApiLog.Log($"  Clearing mesh and registering parts");
        rigCreator.ClearMesh(gameObject);
        rigCreator.RegisterParts();
        ApiLog.Log($"  Configuring rotations");
        rigCreator.ConfigRotations();
        ApiLog.Log($"  Adding rigs");
        rigCreator.AddRigs();
        int jointsCreated = 0;
        for (int i = 0; i < rigCreator.bodyparts.Count; i++)
        {
            if (rigCreator.bodyparts[i].rigObject && rigCreator.bodyparts[i].joint.hasJoint)
            {
                rigCreator.bodyparts[i].rigObject.AddComponent<RigCreatorJoint>().Init(rigCreator, rigCreator.bodyparts[i]);
                rigCreator.bodyparts[i].joint.SpawnJoint(rigCreator.bodyparts[i].rig, rigCreator.bodyparts[i].rigObject.transform.parent.GetComponentInParent<Rigidbody>(true), rigCreator.useTargetRotation ? rigCreator.targetRotationSpring : 0f, rigCreator.useTargetRotation ? (rigCreator.targetRotationSpring * rigCreator.targetRotationDragFactor) : 0f);
                jointsCreated++;
            }
        }
        ApiLog.Log($"  Created {jointsCreated} joints for {rigCreator.bodyparts.Count} bodyparts");
        ApiLog.Log($"  Adding collision and scripts");
        rigCreator.AddCollision();
        rigCreator.AddScripts();
        ApiLog.Log($"  Invoking createRigEvent");
        rigCreator.createRigEvent.Invoke();
    }

    /// <summary>
    /// Creates a <see cref="RigCreatorBodypart"/> entry for use in <see cref="RigCreatorConfig.bodyparts"/>.
    /// </summary>
    /// <param name="type">The bodypart type.</param>
    /// <param name="mass">Mass assigned to the bodypart rigidbody.</param>
    /// <param name="colType">Collider shape for the bodypart.</param>
    /// <param name="hasJoint">Whether the bodypart has a physics joint.</param>
    /// <param name="mat">Physics material applied to the collider.</param>
    /// <returns>A configured bodypart definition.</returns>
    public static RigCreatorBodypart CreatePart(BodypartType type, float mass, ColliderType colType, bool hasJoint = true, RigCreatorBodypart.ColliderMaterial mat = RigCreatorBodypart.ColliderMaterial.Default)
    {
        var part = new RigCreatorBodypart
        {
            partType = type,
            mass = mass,
            useMovementForceMultiplier = false,
            movementForceMultiplier = 0f,
            joint = new JointConfig
            {
                hasJoint = hasJoint,
                springMultiplier = 1f,
                dragMultiplier = 1f,
                minX = -45f,
                maxX = 45f,
                yAngle = 45f,
                zAngle = 45f
            }
        };

        part.colliders = new List<RigCreatorBodypart.RigCreatorColliderData> {
            new RigCreatorBodypart.RigCreatorColliderData
            {
                colliderType = colType,
                colliderScale = new Vector3(0.2f, 0.2f, 0.2f),
                colliderPosition = Vector3.zero,
                colliderRotation = Vector3.zero,
                physicsMaterial = mat,
                overrideLayer = 0
            }
        };
        return part;
    }

    /// <summary>
    /// Validates that a monster prefab has the components required for <see cref="Player.Start"/> to run without errors.
    /// Logs detailed errors for any missing requirements.
    /// </summary>
    /// <param name="prefab">The monster prefab to validate.</param>
    /// <param name="monsterName">Name used in log messages.</param>
    /// <returns><c>true</c> if all required components and hierarchy are present.</returns>
    public static bool ValidatePlayerPrefab(GameObject prefab, string monsterName)
    {
        bool ok = true;

        Player player = prefab.GetComponent<Player>();
        if (player == null)
        {
            ApiLog.LogError($"[ValidatePlayerPrefab] {monsterName}: Missing Player component on root GameObject.");
            return false;
        }

        if (player.input == null)
        {
            ApiLog.LogError($"[ValidatePlayerPrefab] {monsterName}: Player.input is null. Make sure SetupPlayer was called or assign a PlayerInput instance.");
            ok = false;
        }
        if (player.data == null)
        {
            ApiLog.LogError($"[ValidatePlayerPrefab] {monsterName}: Player.data is null. Make sure SetupPlayer was called or assign a PlayerData instance.");
            ok = false;
        }
        if (player.refs == null)
        {
            ApiLog.LogError($"[ValidatePlayerPrefab] {monsterName}: Player.refs is null. Make sure SetupPlayer was called or assign a PlayerRefs instance.");
            ok = false;
        }

        Transform rigCreator = prefab.transform.Find("RigCreator");
        if (rigCreator == null)
        {
            ApiLog.LogError($"[ValidatePlayerPrefab] {monsterName}: Child 'RigCreator' not found. Player.Start() accesses it via transform.Find(\"RigCreator\").");
            ok = false;
        }
        else
        {
            var rc = rigCreator.GetComponent<RigCreator>();
            if (rc == null)
            {
                ApiLog.LogError($"[ValidatePlayerPrefab] {monsterName}: 'RigCreator' found but has no RigCreator component. CustomCreateRig must have been run on this object.");
                ok = false;
            }
            else
            {
                var rig = rigCreator.Find("Rig");
                if (rig == null)
                {
                    ApiLog.LogError($"[ValidatePlayerPrefab] {monsterName}: RigCreator has no 'Rig' child. Did CustomCreateRig run on this prefab?");
                }
            }
        }

        void RequireOnRoot<T>() where T : Component
        {
            if (prefab.GetComponent<T>() == null)
            {
                ApiLog.LogError($"[ValidatePlayerPrefab] {monsterName}: Missing required component {typeof(T).Name} on root GameObject.");
                ok = false;
            }
        }

        RequireOnRoot<PlayerRagdoll>();
        RequireOnRoot<PlayerAnimRefHandler>();
        RequireOnRoot<PlayerAnimationHandler>();
        RequireOnRoot<PlayerItems>();
        RequireOnRoot<PlayerInteraction>();
        RequireOnRoot<PhotonView>();

        if (prefab.GetComponent<PlayerVisor>() == null)
        {
            ApiLog.LogError($"[ValidatePlayerPrefab] {monsterName}: PlayerVisor not found. This is optional but differs from the default player prefab.");
        }

        if (!ok)
        {
            ApiLog.LogError($"[ValidatePlayerPrefab] {monsterName}: Prefab validation FAILED. Player.Start() may throw NullReferenceException. See previous logs for details.");
        }
        else
        {
            ApiLog.Log($"[ValidatePlayerPrefab] {monsterName}: Prefab validation passed.");
        }

        return ok;
    }
}
