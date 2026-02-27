using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Photon.Pun;

namespace DbsContentApi.Modules;

public class BudgetConfig
{
    public int budgetCost = 1;
    public float rarity = 0.01f;
}

public class ControllerConfig
{
    public float movementForce = 9f;
    public float standForce = 25f;
    public float gravity = 80f;
    public float maxStamina = 100f;
    public float staminaRegRate = 100f;
    public float staminaReActivationThreshold = 100f;
    public float jumpImpulse = 7f;
    public float jumpForceOverTime = 0.6f;
    public float constantGravity = 1f;
}

public class PlayerConfig
{
    public float targetHeight = 3f;
}

public class RigCreatorConfig
{
    /// <summary>
    /// Optional bodypart configuration for RigCreator.
    /// If null, any existing RigCreator configuration on the prefab is used.
    /// Required if RigCreator doesn't exist on the prefab.
    /// </summary>
    public List<RigCreatorBodypart>? bodyparts;

    // RigCreator settings (used when creating or configuring RigCreator)
    public bool useGravity = false;
    public float massMultiplier = 1f;
    public bool useTargetRotation = true;
    public float targetRotationSpring = 300f;
    public float targetRotationDragFactor = 0.05f;
    public int smartFillLegs = 0;
    public int setDefaultLayer = 0;
}

public class RagdollConfig
{
    public bool addOpposingForce = false;
    public float force = 60f;
    public float torque = 0.1f;
    public float drag = 0.92f;
    public float angularDrag = 0.9f;
}

public class MonsterAnimationValuesConfig
{
    public bool rightPunch = false;
    public bool leftPunch = false;
    public float movementMultiplier = 1f;
}

public class PhotonViewConfig
{
    public ViewSynchronization synchronization = ViewSynchronization.UnreliableOnChange;
    public bool observeMonsterSyncer = true;
}

public class BotConfig
{
    public string monsterName = "";
    public List<PatrolPoint.PatrolGroup> patrolGroups = new List<PatrolPoint.PatrolGroup> { PatrolPoint.PatrolGroup.Bear, PatrolPoint.PatrolGroup.Dog };
    public int attackType = 0;
    public float turnVel = 0f;
    public float animMoveSpeedFactor = 1f;
    public bool alertable = true;
}

public class NavMeshAgentConfig
{
    public float speed = 3.5f;
    public float acceleration = 0f;
    public float angularSpeed = 120f;
    public float stoppingDistance = 0f;
    public float radius = 1f;
    public float height = 2f;
    public int obstacleAvoidanceType = (int)ObstacleAvoidanceType.HighQualityObstacleAvoidance;
}
public class MobSetupConfig
{
    /// <summary>
    /// Visual + RigCreator configuration.
    /// - If null: no PlayerVisual/RigCreator setup is performed
    /// - If provided: intelligently sets up missing components based on what's already on the prefab
    /// </summary>
    public RigCreatorConfig? visualRig;

    public BudgetConfig? budget;
    public ControllerConfig? controller;
    public bool addAnimRefHandler;
    public PlayerConfig? player;
    public RagdollConfig? ragdoll;
    public PhotonViewConfig? photonView;
    public BotConfig? bot;
    public NavMeshAgentConfig? navMesh;
    public MonsterAnimationValuesConfig? monsterAnimationValues;
    public bool addMonsterAnimationHandler;
    public bool addMonsterSyncer;
    public bool addHeadFollower;
    public bool addGroundPos;
}

public class Mobs
{
    /// <summary>
    /// Configures a custom monster by adding specified components and setting up the rig.
    /// </summary>
    public static void SetupCustomMonster(GameObject monster, string monsterName, MobSetupConfig config)
    {
        Logger.Log($"Starting setup for custom monster: {monsterName}");

        if (config.budget != null)
        {
            Logger.Log($"  Setting up Budget component for {monsterName}");
            SetupBudget(monster, config.budget);
        }
        if (config.player != null)
        {
            Logger.Log($"  Setting up Player component for {monsterName}");
            SetupPlayer(monster, config.player);
        }
        if (config.ragdoll != null)
        {
            Logger.Log($"  Setting up Ragdoll component for {monsterName}");
            SetupRagdoll(monster, config.ragdoll);
        }
        if (config.addMonsterSyncer)
        {
            Logger.Log($"  Setting up MonsterSyncer component for {monsterName}");
            SetupMonsterSyncer(monster);
        }
        if (config.controller != null)
        {
            Logger.Log($"  Setting up Controller component for {monsterName}");
            SetupController(monster, config.controller);
        }
        if (config.addAnimRefHandler)
        {
            Logger.Log($"  Setting up AnimRefHandler component for {monsterName}");
            SetupAnimRefHandler(monster);
        }
        if (config.photonView != null)
        {
            Logger.Log($"  Setting up PhotonView component for {monsterName}");
            SetupPhotonView(monster, config.photonView);
        }
        if (config.bot != null)
        {
            Logger.Log($"  Setting up Bot component for {monsterName}");
            SetupBot(monster, config.bot, config.navMesh);
        }

        // Visual + RigCreator setup (intelligently handles what's already on the prefab)
        if (config.visualRig != null)
        {
            Logger.Log($"  Setting up Visual and RigCreator for {monsterName}");
            SetupVisualAndRigCreator(monster, monsterName, config.visualRig);
        }

        if (config.addHeadFollower)
        {
            Logger.Log($"  Setting up HeadFollower component for {monsterName}");
            SetupHeadFollower(monster);
        }
        if (config.addGroundPos)
        {
            Logger.Log($"  Setting up GroundPositionTransform component for {monsterName}");
            SetupGroundPositionTransform(monster);
        }
        if (config.addMonsterAnimationHandler)
        {
            Logger.Log($"  Setting up MonsterAnimationHandler component for {monsterName}");
            SetupMonsterAnimationHandler(monster);
        }
        if (config.monsterAnimationValues != null)
        {
            Logger.Log($"  Setting up MonsterAnimationValues component for {monsterName}");
            SetupMonsterAnimationValues(monster, config.monsterAnimationValues);
        }

        Logger.Log($"Completed setup for custom monster: {monsterName}");
    }


    /// <summary>
    /// Setup MonsterAnimationValues component on the RigCreator GameObject.
    /// </summary>
    /// <param name="monster">The monster GameObject.</param>
    /// <param name="config">The MonsterAnimationValuesConfig configuration.</param>
    /// <exception cref="System.Exception">Thrown if RigCreator not found.</exception>
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
    public static GameObject GetBotChildObject(GameObject customMonsterPrefab)
    {
        var botObject = customMonsterPrefab.transform.Find($"Bot_{customMonsterPrefab.name}").gameObject;
        Logger.Log($"  Retrieved Bot child object '{botObject.name}' for {customMonsterPrefab.name}");
        return botObject;
    }

    /// <summary>
    /// Replaces "World Optimized" shaders with the internal game shader from the "Zombe" monster.
    /// </summary>
    public static void RestoreShaders(GameObject prefab)
    {
        Logger.Log($"  Restoring shaders for {prefab.name}");
        var renderers = prefab.GetComponentsInChildren<Renderer>();
        var zombePrefab = Resources.Load<GameObject>("Zombe");
        if (zombePrefab == null)
        {
            Logger.LogWarning($"  Zombe prefab not found, skipping shader restoration for {prefab.name}");
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
        Logger.Log($"  Restored {shaderReplacements} shader(s) for {prefab.name}");
    }

    private static void SetupBudget(GameObject monster, BudgetConfig config)
    {
        var budget = monster.AddComponent<BudgetCost>();
        budget.budgetCost = config.budgetCost;
        budget.rarity = config.rarity;
        Logger.Log($"Budget configured: cost={config.budgetCost}, rarity={config.rarity}");
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
        Logger.Log($"Controller configured: movementForce={config.movementForce}, maxStamina={config.maxStamina}");
    }

    private static void SetupAnimRefHandler(GameObject monster)
    {
        monster.AddComponent<PlayerAnimRefHandler>();
        Logger.Log($"AnimRefHandler component added");
    }

    private static void SetupPlayer(GameObject monster, PlayerConfig config)
    {
        var player = monster.AddComponent<Player>();
        player.ai = true;
        player.input = new Player.PlayerInput();
        player.data = new Player.PlayerData();
        player.data.targetHeight = config.targetHeight;
        player.refs = new Player.PlayerRefs();
        Logger.Log($"Player configured: targetHeight={config.targetHeight}, ai={player.ai}");
    }

    private static void SetupRagdoll(GameObject monster, RagdollConfig config)
    {
        var pr = monster.AddComponent<PlayerRagdoll>();
        pr.addOpposingForce = config.addOpposingForce;
        pr.force = config.force;
        pr.torque = config.torque;
        pr.drag = config.drag;
        pr.angularDrag = config.angularDrag;
        Logger.Log($"Ragdoll configured: force={config.force}, torque={config.torque}, drag={config.drag}");
    }

    private static void SetupMonsterSyncer(GameObject monster)
    {
        var ms = monster.AddComponent<MonsterSyncer>();
        ms.applyData = true;
        Logger.Log($"MonsterSyncer component added (applyData={ms.applyData})");
    }

    /// <summary>
    /// Intelligently sets up PlayerVisual and RigCreator based on what's already on the prefab.
    /// Handles 4 scenarios:
    /// 1. Both RigCreator and PlayerVisual exist → do nothing (already configured)
    /// 2. RigCreator exists but not PlayerVisual → add PlayerVisual and call SetTargets
    /// 3. PlayerVisual exists but not RigCreator → create RigCreator, configure it, and create rig
    /// 4. Neither exists → create PlayerVisual, then same as scenario 3
    /// </summary>
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

        Logger.Log($"Visual/Rig status: PlayerVisual={(existingPlayerVisual != null ? "exists" : "missing")}, " +
                   $"RigCreator={(existingRigCreator != null ? "exists" : "missing")}, " +
                   $"Rig={(rigAlreadyCreated ? "created" : "not created")}");

        // Scenario 1: Both exist and rig is already created
        if (existingPlayerVisual != null && existingRigCreator != null && rigAlreadyCreated)
        {
            Logger.Log($"Scenario 1: Both PlayerVisual and RigCreator are fully configured, nothing to do");
            return;
        }

        // Scenario 2: RigCreator exists (and possibly rig is created) but PlayerVisual is missing
        if (existingRigCreator != null && existingPlayerVisual == null)
        {
            Logger.Log($"Scenario 2: RigCreator exists but PlayerVisual missing, adding PlayerVisual");
            var playerVisual = visualTransform.gameObject.AddComponent<PlayerVisual>();

            // If rig is already created, call SetTargets now
            if (rigAlreadyCreated)
            {
                Logger.Log($"Rig already created, calling SetTargets on PlayerVisual");
                playerVisual.SetTargets();
            }
            else
            {
                // If rig not created yet, we need to create it and wire up the event
                Logger.Log($"Rig not created yet, will call SetTargets after rig creation");
                existingRigCreator.createRigEvent = existingRigCreator.createRigEvent ?? new UnityEngine.Events.UnityEvent();
                existingRigCreator.createRigEvent.AddListener(() => VisualSetTargets(monster));

                Logger.Log($"Creating rig for existing RigCreator");
                CustomCreateRig(existingRigCreator);
            }
            return;
        }

        // Scenario 3 & 4: RigCreator doesn't exist (PlayerVisual may or may not exist)
        // If PlayerVisual doesn't exist, create it
        if (existingPlayerVisual == null)
        {
            Logger.Log($"Scenario 4: Neither PlayerVisual nor RigCreator exist, creating both");
            visualTransform.gameObject.AddComponent<PlayerVisual>();
        }
        else
        {
            Logger.Log($"Scenario 3: PlayerVisual exists but RigCreator missing, creating RigCreator");
        }

        // Now create and configure RigCreator
        if (config.bodyparts == null)
        {
            throw new System.Exception($"bodyparts configuration is required when RigCreator doesn't exist on the prefab for {monsterName}");
        }

        Logger.Log($"Creating RigCreator GameObject with {config.bodyparts.Count} bodyparts");
        var rigCreatorObject = new GameObject("RigCreator");
        rigCreatorObject.transform.SetParent(monster.transform);
        rigCreatorObject.transform.localPosition = Vector3.zero;
        rigCreatorObject.transform.localRotation = Quaternion.identity;
        var rigCreator = rigCreatorObject.AddComponent<RigCreator>();

        // Configure RigCreator
        rigCreator.source = visualTransform.gameObject;
        Logger.Log($"RigCreator source set to Visual GameObject");

        Logger.Log($"Setting up RigCreator resources");
        SetupRigCreatorResources(rigCreator);

        // Apply configuration
        Logger.Log($"Applying RigCreator configuration");
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

        Logger.Log($"Creating rig for {monsterName}");
        CustomCreateRig(rigCreator);
        Logger.Log($"Rig creation completed for {monsterName}");
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
                Logger.Log($"Visual targets set successfully");
            }
            else
            {
                Logger.LogWarning($"PlayerVisual component not found on Visual for {monster.name}");
            }
        }
        else
        {
            Logger.LogWarning($"Visual GameObject not found for {monster.name}");
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
                Logger.Log($"PhotonView configured: synchronization={config.synchronization}, observing MonsterSyncer");
            }
            else
            {
                Logger.LogWarning($"MonsterSyncer not found for PhotonView observation on {monster.name}");
            }
        }
        else
        {
            Logger.Log($"PhotonView configured: synchronization={config.synchronization}");
        }
        pv.observableSearch = PhotonView.ObservableSearch.AutoFindAll;
    }

    private static void SetupHeadFollower(GameObject monster)
    {
        var headObj = monster.transform.Find("HeadPosition")?.gameObject;
        if (headObj != null)
        {
            headObj.AddComponent<HeadFollower>();
            Logger.Log($"HeadFollower component added to HeadPosition");
        }
        else
        {
            Logger.LogWarning($"HeadPosition not found for {monster.name}");
        }
    }

    private static void SetupGroundPositionTransform(GameObject monster)
    {
        var groundObj = monster.transform.Find("PlayerGroundPos")?.gameObject;
        if (groundObj != null)
        {
            groundObj.AddComponent<PlayerGroundPositionTransform>();
            Logger.Log($"PlayerGroundPositionTransform component added to PlayerGroundPos");
        }
        else
        {
            Logger.LogWarning($"PlayerGroundPos not found for {monster.name}");
        }
    }

    private static void SetupMonsterAnimationHandler(GameObject monster)
    {
        monster.AddComponent<MonsterAnimationHandler>();
        Logger.Log($"MonsterAnimationHandler component added");
    }

    private static void SetupBot(GameObject monster, BotConfig botConfig, NavMeshAgentConfig? navConfig)
    {
        Logger.Log($"Creating Bot GameObject 'Bot_{botConfig.monsterName}' for {botConfig.monsterName}");
        var botObject = new GameObject($"Bot_{botConfig.monsterName}");
        botObject.transform.SetParent(monster.transform);
        botObject.transform.localPosition = Vector3.zero;
        botObject.transform.localRotation = Quaternion.identity;

        Logger.Log($"Adding PhotonView to Bot for {botConfig.monsterName}");
        var pv = botObject.AddComponent<PhotonView>();
        pv.Synchronization = ViewSynchronization.UnreliableOnChange;
        pv.observableSearch = PhotonView.ObservableSearch.AutoFindAll;

        Logger.Log($"Configuring Bot component: patrolGroups={botConfig.patrolGroups.Count}, attackType={botConfig.attackType}, alertable={botConfig.alertable}");
        var bot = botObject.AddComponent<Bot>();
        bot.patrolGroups = botConfig.patrolGroups;
        bot.attackType = botConfig.attackType;
        bot.turnVel = botConfig.turnVel;
        bot.animMoveSpeedFactor = botConfig.animMoveSpeedFactor;
        bot.alertable = botConfig.alertable;

        var hip = monster.transform.Find("Visual")?.Find(botConfig.monsterName)?.Find("Armature")?.Find("Hip");
        bot.centerTransform = hip;
        bot.groundTransform = monster.transform.Find("PlayerGroundPos");
        bot.syncData = new Bot.SyncData { targetPlayerId = -1 };
        if (hip != null)
        {
            Logger.Log($"Bot centerTransform set to Hip");
        }
        else
        {
            Logger.LogWarning($"Hip transform not found for Bot centerTransform on {botConfig.monsterName}");
        }

        if (navConfig != null)
        {
            Logger.Log($"Configuring NavMeshAgent: speed={navConfig.speed}, angularSpeed={navConfig.angularSpeed}, radius={navConfig.radius}, height={navConfig.height}");
            var nav = botObject.AddComponent<NavMeshAgent>();
            nav.speed = navConfig.speed;
            nav.acceleration = navConfig.acceleration;
            nav.angularSpeed = navConfig.angularSpeed;
            nav.stoppingDistance = navConfig.stoppingDistance;
            nav.radius = navConfig.radius;
            nav.height = navConfig.height;
            nav.obstacleAvoidanceType = (ObstacleAvoidanceType)navConfig.obstacleAvoidanceType;
        }
        else
        {
            Logger.Log($"No NavMeshConfig provided, skipping NavMeshAgent setup");
        }

        Logger.Log($"Adding Bot_Nav_Navmesh and Bot_RagdollCharacter components");
        botObject.AddComponent<Bot_Nav_Navmesh>();
        botObject.AddComponent<Bot_RagdollCharacter>();
        Logger.Log($"Bot setup completed for {botConfig.monsterName}");
    }

    public static void AddBotZombieComponent(GameObject botObject)
    {
        botObject.AddComponent<Bot_Zombie>();
        Logger.Log($"  Added Bot_Zombie component to {botObject.name}");
    }

    public static void AddBotToolkitBoyComponent(GameObject botObject, float targetDistance = 2f)
    {
        var b = botObject.AddComponent<Bot_ToolkitBoy>();
        b.targetDistance = targetDistance;
        Logger.Log($"  Added Bot_ToolkitBoy component to {botObject.name} (targetDistance={targetDistance})");
    }

    public static void AddBotKnifoComponent(GameObject botObject, BodypartType mainRigBodyPart)
    {
        var b = botObject.AddComponent<Bot_Knifo>();
        b.targetDistance = 1f;
        b.mainRig = mainRigBodyPart;
        b.jumpForceForward = 15f;
        b.jumpForceUp = 15f;
        b.jumpCurve = Resources.Load<GameObject>("Knifo")?.transform.Find("Bot_Knifo")?.GetComponent<Bot_Knifo>()?.jumpCurve;
        Logger.Log($"  Added Bot_Knifo component to {botObject.name} (mainRig={mainRigBodyPart})");
    }

    public static void AddBotChaserComponent(GameObject botObject)
    {
        var b = botObject.AddComponent<Bot_Chaser>();
        b.exhastionTime = 20f;
        b.fleeForSeconds = 20f;
        b.hidingExhastionMultiplier = 1f;
        b.timeToLoseTarget = 2f;
        b.targetDistance = 3f;
        b.backUpIfTooClose = true;
        b.useWorldMoveInChase = true;
        b.canRotateWhenStandingStill = true;
        b.chaseTurnRate = 6f;
        b.fleeTurnRate = 6f;
        b.investigateTurnRate = 3f;
        b.patrolTurnRate = 3f;
        b.maxRange = 70f;
        b.maxAngle = 110f;
        b.timeToSeeTarget = 1f;
        Logger.Log($"  Added Bot_Chaser component to {botObject.name} (maxRange={b.maxRange}, targetDistance={b.targetDistance})");
    }

    public static void AddBotDragComponent(GameObject botObject)
    {
        var b = botObject.AddComponent<Bot_Drag>();
        b.dragForce = 150f;
        b.handForce = 150f;
        b.range = 2f;
        Logger.Log($"  Added Bot_Drag component to {botObject.name} (dragForce={b.dragForce}, range={b.range})");
    }

    private static void SetupRigCreatorResources(RigCreator rigCreator)
    {
        RigCreator? template = null;
        foreach (var rc in Resources.FindObjectsOfTypeAll<RigCreator>())
        {
            if (rc.boxColldier != null && rc.capsuleCol != null && rc.slipperyMat != null)
            {
                template = rc;
                Logger.Log($"  Found RigCreator template from existing instance");
                break;
            }
        }

        if (template == null)
        {
            Logger.Log($"  Searching for Zombe prefab as RigCreator template");
            var zombePrefab = Resources.Load<GameObject>("Zombe");
            if (zombePrefab != null)
            {
                template = zombePrefab.GetComponentInChildren<RigCreator>(true);
                if (template != null)
                {
                    Logger.Log($"  Found RigCreator template from Zombe prefab");
                }
            }
        }

        if (template != null)
        {
            rigCreator.boxColldier = template.boxColldier;
            rigCreator.capsuleCol = template.capsuleCol;
            rigCreator.sphereCol = template.sphereCol;
            rigCreator.slipperyMat = template.slipperyMat;
            Logger.Log($"  RigCreator resources copied from template");
        }
        else if (rigCreator.slipperyMat == null)
        {
            Logger.Log($"  Searching for slippery PhysicMaterial");
            foreach (var mat in Resources.FindObjectsOfTypeAll<UnityEngine.PhysicsMaterial>())
            {
                if (mat.name.ToLower().Contains("slippery"))
                {
                    rigCreator.slipperyMat = mat;
                    Logger.Log($"  Found slippery PhysicMaterial: {mat.name}");
                    break;
                }
            }
        }
        else
        {
            Logger.LogWarning($"  No RigCreator template found and no slippery material available");
        }
    }

    /// <summary>
    /// Copy of the original RigCreator:CreateRig method, the only change being that it
    /// passes True to the GetComponentInParent. This is essential because it won't find
    /// the RigidBody in the parent components otherwise. This is because prefabs are inactive gameObjects.
    /// </summary>
    private static void CustomCreateRig(RigCreator rigCreator)
    {
        Logger.Log($"  SmartFillLegs()");
        rigCreator.SmartFillLegs();
        Transform transform = rigCreator.gameObject.transform.Find("Rig");
        if (transform)
        {
            Logger.Log($"  Destroying existing Rig GameObject");
            UnityEngine.Object.DestroyImmediate(transform.gameObject);
        }
        Logger.Log($"  Instantiating rig source");
        GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(rigCreator.source, rigCreator.gameObject.transform.position, rigCreator.gameObject.transform.rotation, rigCreator.gameObject.transform);
        gameObject.SetActive(true);
        gameObject.name = "Rig";
        Logger.Log($"  Clearing mesh and registering parts");
        rigCreator.ClearMesh(gameObject);
        rigCreator.RegisterParts();
        Logger.Log($"  Configuring rotations");
        rigCreator.ConfigRotations();
        Logger.Log($"  Adding rigs");
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
        Logger.Log($"  Created {jointsCreated} joints for {rigCreator.bodyparts.Count} bodyparts");
        Logger.Log($"  Adding collision and scripts");
        rigCreator.AddCollision();
        rigCreator.AddScripts();
        Logger.Log($"  Invoking createRigEvent");
        rigCreator.createRigEvent.Invoke();
    }

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
    /// Validates that a prefab has the minimum setup required for the vanilla
    /// <see cref="Player"/> component so that its Awake/Start/Update code paths
    /// do not throw NullReferenceException. Intended to be called on custom
    /// monsters that use a Player-based rig.
    /// </summary>
    /// <param name="prefab">The monster prefab.</param>
    /// <param name="monsterName">Name used for logging.</param>
    /// <returns>True if the prefab passes all critical checks, otherwise false.</returns>
    public static bool ValidatePlayerPrefab(GameObject prefab, string monsterName)
    {
        bool ok = true;

        Player player = prefab.GetComponent<Player>();
        if (player == null)
        {
            Logger.LogError($"[ValidatePlayerPrefab] {monsterName}: Missing Player component on root GameObject.");
            return false;
        }

        // Player.Start expects these backing objects to be non-null.
        if (player.input == null)
        {
            Logger.LogError($"[ValidatePlayerPrefab] {monsterName}: Player.input is null. Make sure SetupPlayer was called or assign a PlayerInput instance.");
            ok = false;
        }
        if (player.data == null)
        {
            Logger.LogError($"[ValidatePlayerPrefab] {monsterName}: Player.data is null. Make sure SetupPlayer was called or assign a PlayerData instance.");
            ok = false;
        }
        if (player.refs == null)
        {
            Logger.LogError($"[ValidatePlayerPrefab] {monsterName}: Player.refs is null. Make sure SetupPlayer was called or assign a PlayerRefs instance.");
            ok = false;
        }

        // Children used directly in Player.Start()
        // Player.Start() assigns refs.rigRoot from this transform and then calls DoInits,
        // which in turn rely on that rig root to already contain the physics rig created
        // by RigCreator. The AnimationRig object is created later by PlayerAnimRefHandler.DoInit,
        // so we deliberately do NOT require it here.
        Transform rigCreator = prefab.transform.Find("RigCreator");
        if (rigCreator == null)
        {
            Logger.LogError($"[ValidatePlayerPrefab] {monsterName}: Child 'RigCreator' not found. Player.Start() accesses it via transform.Find(\"RigCreator\").");
            ok = false;
        }
        else
        {
            var rc = rigCreator.GetComponent<RigCreator>();
            if (rc == null)
            {
                Logger.LogError($"[ValidatePlayerPrefab] {monsterName}: 'RigCreator' found but has no RigCreator component. CustomCreateRig must have been run on this object.");
                ok = false;
            }
            else
            {
                // Heuristic: after CustomCreateRig there should be a 'Rig' child with Bodyparts.
                var rig = rigCreator.Find("Rig");
                if (rig == null)
                {
                    Logger.LogWarning($"[ValidatePlayerPrefab] {monsterName}: RigCreator has no 'Rig' child. Did CustomCreateRig run on this prefab?");
                }
            }
        }

        // Components required by DoInits() and general runtime logic.
        void RequireOnRoot<T>() where T : Component
        {
            if (prefab.GetComponent<T>() == null)
            {
                Logger.LogError($"[ValidatePlayerPrefab] {monsterName}: Missing required component {typeof(T).Name} on root GameObject.");
                ok = false;
            }
        }

        RequireOnRoot<PlayerRagdoll>();
        RequireOnRoot<PlayerAnimRefHandler>();
        RequireOnRoot<PlayerAnimationHandler>();
        RequireOnRoot<PlayerItems>();
        RequireOnRoot<PlayerInteraction>();
        RequireOnRoot<PhotonView>();

        // Optional but strongly recommended for parity with the base player.
        if (prefab.GetComponent<PlayerVisor>() == null)
        {
            Logger.LogWarning($"[ValidatePlayerPrefab] {monsterName}: PlayerVisor not found. This is optional but differs from the default player prefab.");
        }

        if (!ok)
        {
            Logger.LogError($"[ValidatePlayerPrefab] {monsterName}: Prefab validation FAILED. Player.Start() may throw NullReferenceException. See previous logs for details.");
        }
        else
        {
            Logger.Log($"[ValidatePlayerPrefab] {monsterName}: Prefab validation passed.");
        }

        return ok;
    }
}
