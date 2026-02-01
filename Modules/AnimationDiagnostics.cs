// using System.Collections.Generic;
// using System.Text;
// using UnityEngine;

// namespace DbsContentApi.Modules;

// /// <summary>
// /// Comprehensive diagnostic component for checking animator and animation setup.
// /// Attach this to any monster prefab to diagnose animation issues.
// /// </summary>
// public class AnimationDiagnostics : MonoBehaviour
// {
//     [Header("Diagnostic Settings")]
//     [Tooltip("Enable continuous logging every frame")]
//     public bool continuousLogging = false;

//     [Tooltip("Interval between diagnostic reports (seconds)")]
//     public float reportInterval = 5f;

//     [Tooltip("Compare with MainCharacter if available")]
//     public bool compareWithMainCharacter = true;

//     private float lastReportTime = 0f;
//     private Player? player;
//     private PlayerAnimRefHandler? animRefHandler;
//     private Animator? animator;
//     private StringBuilder diagnosticReport = new StringBuilder();

//     private void Start()
//     {
//         Logger.Log("=== Animation Diagnostics Started ===");
//         RunFullDiagnostics();
//     }

//     private void Update()
//     {
//         if (continuousLogging)
//         {
//             RunFullDiagnostics();
//         }
//         else if (Time.time - lastReportTime >= reportInterval)
//         {
//             RunFullDiagnostics();
//             lastReportTime = Time.time;
//         }
//     }

//     /// <summary>
//     /// Runs a complete diagnostic check of the animation system
//     /// </summary>
//     public void RunFullDiagnostics()
//     {
//         diagnosticReport.Clear();
//         diagnosticReport.AppendLine("=== ANIMATION DIAGNOSTICS REPORT ===");
//         diagnosticReport.AppendLine($"GameObject: {gameObject.name}");
//         diagnosticReport.AppendLine($"Time: {Time.time:F2}");
//         diagnosticReport.AppendLine();

//         // Show quick summary of playing animations first
//         CheckPlayingAnimationsSummary();

//         CheckPlayerComponent();
//         CheckPlayerRefs();
//         CheckAnimator();
//         CheckAnimationController();
//         CheckPlayerAnimRefHandler();
//         CheckRigRoot();
//         CheckAnimationRig();
//         CheckSyncedTransforms();
//         CheckVisualTargets();
//         CheckAnimationState();
//         CheckAnimationParameters();

//         if (compareWithMainCharacter)
//         {
//             CompareWithMainCharacter();
//         }

//         Logger.Log(diagnosticReport.ToString());
//     }

//     private void CheckPlayingAnimationsSummary()
//     {
//         diagnosticReport.AppendLine("=== QUICK SUMMARY: CURRENTLY PLAYING ANIMATIONS ===");

//         // Find the AnimationRig animator first (most important for visual)
//         Transform? animationRig = transform.Find("AnimationRig");
//         Animator? primaryAnimator = null;
//         string primarySource = "None";

//         if (animationRig != null)
//         {
//             primaryAnimator = animationRig.GetComponent<Animator>();
//             if (primaryAnimator != null && primaryAnimator.enabled && primaryAnimator.isInitialized)
//             {
//                 primarySource = "AnimationRig (Visual Driver)";
//             }
//         }

//         // Fallback to RigCreator animator
//         if (primaryAnimator == null)
//         {
//             Transform rigCreator = transform.Find("RigCreator");
//             if (rigCreator != null)
//             {
//                 primaryAnimator = rigCreator.GetComponent<Animator>();
//                 if (primaryAnimator != null && primaryAnimator.enabled && primaryAnimator.isInitialized)
//                 {
//                     primarySource = "RigCreator";
//                 }
//             }
//         }

//         // Fallback to root animator
//         if (primaryAnimator == null && animator != null && animator.enabled && animator.isInitialized)
//         {
//             primaryAnimator = animator;
//             primarySource = "Root GameObject";
//         }

//         if (primaryAnimator == null)
//         {
//             diagnosticReport.AppendLine("❌ No active Animator found - animations cannot play!");
//             diagnosticReport.AppendLine();
//             return;
//         }

//         diagnosticReport.AppendLine($"Primary Animator: {primarySource}");
//         diagnosticReport.AppendLine($"  - Enabled: {primaryAnimator.enabled}");
//         diagnosticReport.AppendLine($"  - Initialized: {primaryAnimator.isInitialized}");
//         diagnosticReport.AppendLine($"  - Speed: {primaryAnimator.speed:F2}");
//         diagnosticReport.AppendLine($"  - Controller: {(primaryAnimator.runtimeAnimatorController != null ? primaryAnimator.runtimeAnimatorController.name : "NULL")}");
//         diagnosticReport.AppendLine();

//         bool foundPlayingAnimation = false;

//         for (int i = 0; i < primaryAnimator.layerCount; i++)
//         {
//             AnimatorStateInfo stateInfo = primaryAnimator.GetCurrentAnimatorStateInfo(i);
//             AnimatorClipInfo[] clipInfo = primaryAnimator.GetCurrentAnimatorClipInfo(i);

//             if (clipInfo != null && clipInfo.Length > 0)
//             {
//                 foundPlayingAnimation = true;
//                 string layerName = primaryAnimator.GetLayerName(i);
//                 float layerWeight = primaryAnimator.GetLayerWeight(i);

//                 diagnosticReport.AppendLine($"🎬 Layer {i} ({layerName}) [Weight: {layerWeight:F2}]:");
//                 foreach (var clip in clipInfo)
//                 {
//                     if (clip.clip != null)
//                     {
//                         float currentTime = clip.clip.length * stateInfo.normalizedTime;
//                         float progress = stateInfo.normalizedTime * 100f;
//                         diagnosticReport.AppendLine($"   ▶ PLAYING: {clip.clip.name}");
//                         diagnosticReport.AppendLine($"      Progress: {progress:F1}% ({currentTime:F2}s / {clip.clip.length:F2}s)");
//                         diagnosticReport.AppendLine($"      Weight: {clip.weight:F2} | Speed: {stateInfo.speed:F2} | Loop: {clip.clip.isLooping}");
//                     }
//                 }
//                 diagnosticReport.AppendLine();
//             }
//         }

//         if (!foundPlayingAnimation)
//         {
//             diagnosticReport.AppendLine("⚠ WARNING: No animations are currently playing!");
//             diagnosticReport.AppendLine("  Check animation parameters and controller setup.");
//             diagnosticReport.AppendLine();
//         }

//         diagnosticReport.AppendLine();
//     }

//     private void CheckPlayerComponent()
//     {
//         diagnosticReport.AppendLine("--- Player Component Check ---");
//         player = GetComponent<Player>();

//         if (player == null)
//         {
//             diagnosticReport.AppendLine("❌ Player component NOT FOUND");
//             return;
//         }

//         diagnosticReport.AppendLine("✓ Player component found");
//         diagnosticReport.AppendLine($"  - AI: {player.ai}");
//         diagnosticReport.AppendLine($"  - IsLocal: {player.IsLocal}");
//         diagnosticReport.AppendLine($"  - Dead: {player.data?.dead ?? false}");
//         diagnosticReport.AppendLine();
//     }

//     private void CheckPlayerRefs()
//     {
//         diagnosticReport.AppendLine("--- Player Refs Check ---");

//         if (player == null)
//         {
//             diagnosticReport.AppendLine("❌ Cannot check refs - Player is null");
//             diagnosticReport.AppendLine();
//             return;
//         }

//         if (player.refs == null)
//         {
//             diagnosticReport.AppendLine("❌ player.refs is NULL");
//             diagnosticReport.AppendLine();
//             return;
//         }

//         diagnosticReport.AppendLine("✓ player.refs exists");
//         diagnosticReport.AppendLine($"  - rigRoot: {(player.refs.rigRoot != null ? $"✓ ({player.refs.rigRoot.name})" : "❌ NULL")}");
//         diagnosticReport.AppendLine($"  - animator: {(player.refs.animator != null ? $"✓ ({player.refs.animator.name})" : "❌ NULL")}");
//         diagnosticReport.AppendLine($"  - animatorTransform: {(player.refs.animatorTransform != null ? $"✓" : "❌ NULL")}");
//         diagnosticReport.AppendLine($"  - animRefHandler: {(player.refs.animRefHandler != null ? $"✓" : "❌ NULL")}");
//         diagnosticReport.AppendLine($"  - ragdoll: {(player.refs.ragdoll != null ? $"✓" : "❌ NULL")}");
//         diagnosticReport.AppendLine($"  - items: {(player.refs.items != null ? $"✓" : "❌ NULL")}");
//         diagnosticReport.AppendLine($"  - controller: {(player.refs.controller != null ? $"✓" : "❌ NULL")}");
//         diagnosticReport.AppendLine();
//     }

//     private void CheckAnimator()
//     {
//         diagnosticReport.AppendLine("--- Animator Check ---");

//         // Check root animator
//         animator = GetComponent<Animator>();
//         if (animator == null)
//         {
//             diagnosticReport.AppendLine("❌ Animator NOT FOUND on root GameObject");
//         }
//         else
//         {
//             diagnosticReport.AppendLine($"✓ Animator found on root: {animator.name}");
//             diagnosticReport.AppendLine($"  - Enabled: {animator.enabled}");
//             diagnosticReport.AppendLine($"  - IsInitialized: {animator.isInitialized}");
//             diagnosticReport.AppendLine($"  - HasTransformHierarchy: {animator.hasTransformHierarchy}");
//             diagnosticReport.AppendLine($"  - UpdateMode: {animator.updateMode}");
//             diagnosticReport.AppendLine($"  - CullingMode: {animator.cullingMode}");
//         }

//         // Check RigCreator animator
//         Transform rigCreator = transform.Find("RigCreator");
//         if (rigCreator != null)
//         {
//             Animator rigAnimator = rigCreator.GetComponent<Animator>();
//             if (rigAnimator != null)
//             {
//                 diagnosticReport.AppendLine($"✓ Animator found on RigCreator: {rigAnimator.name}");
//                 diagnosticReport.AppendLine($"  - Enabled: {rigAnimator.enabled}");
//                 diagnosticReport.AppendLine($"  - IsInitialized: {rigAnimator.isInitialized}");
//                 diagnosticReport.AppendLine($"  - RuntimeAnimatorController: {(rigAnimator.runtimeAnimatorController != null ? "✓" : "❌ NULL")}");
//             }
//             else
//             {
//                 diagnosticReport.AppendLine("⚠ RigCreator exists but has NO Animator component");
//             }
//         }
//         else
//         {
//             diagnosticReport.AppendLine("⚠ RigCreator GameObject NOT FOUND (this is expected if shouldCreateRig=false)");
//         }

//         // Check AnimationRig animator (created by PlayerAnimRefHandler)
//         Transform animationRig = transform.Find("AnimationRig");
//         if (animationRig != null)
//         {
//             Animator animRigAnimator = animationRig.GetComponent<Animator>();
//             if (animRigAnimator != null)
//             {
//                 diagnosticReport.AppendLine($"✓ Animator found on AnimationRig: {animRigAnimator.name}");
//                 diagnosticReport.AppendLine($"  - Enabled: {animRigAnimator.enabled}");
//                 diagnosticReport.AppendLine($"  - IsInitialized: {animRigAnimator.isInitialized}");
//                 diagnosticReport.AppendLine($"  - RuntimeAnimatorController: {(animRigAnimator.runtimeAnimatorController != null ? "✓" : "❌ NULL")}");
//             }
//             else
//             {
//                 diagnosticReport.AppendLine("⚠ AnimationRig exists but has NO Animator component");
//             }
//         }
//         else
//         {
//             diagnosticReport.AppendLine("⚠ AnimationRig GameObject NOT FOUND (created by PlayerAnimRefHandler.DoInit())");
//         }

//         diagnosticReport.AppendLine();
//     }

//     private void CheckAnimationController()
//     {
//         diagnosticReport.AppendLine("--- Animation Controller Check ---");

//         Animator? targetAnimator = null;

//         // Find the active animator
//         if (animator != null && animator.enabled)
//         {
//             targetAnimator = animator;
//         }
//         else
//         {
//             Transform rigCreator = transform.Find("RigCreator");
//             if (rigCreator != null)
//             {
//                 targetAnimator = rigCreator.GetComponent<Animator>();
//             }
//         }

//         if (targetAnimator == null)
//         {
//             diagnosticReport.AppendLine("❌ No active Animator found to check");
//             diagnosticReport.AppendLine();
//             return;
//         }

//         RuntimeAnimatorController controller = targetAnimator.runtimeAnimatorController;
//         if (controller == null)
//         {
//             diagnosticReport.AppendLine("❌ RuntimeAnimatorController is NULL");
//             diagnosticReport.AppendLine();
//             return;
//         }

//         diagnosticReport.AppendLine($"✓ RuntimeAnimatorController found: {controller.name}");
//         diagnosticReport.AppendLine($"  - Type: {controller.GetType().Name}");

//         // Try to get animation clips (may not be accessible at runtime)
//         try
//         {
//             var controllerType = controller.GetType();
//             var animationClipsProperty = controllerType.GetProperty("animationClips");
//             if (animationClipsProperty != null)
//             {
//                 var clips = animationClipsProperty.GetValue(controller) as AnimationClip[];
//                 if (clips != null)
//                 {
//                     diagnosticReport.AppendLine($"  - Animation Clips: {clips.Length}");
//                     if (clips.Length > 0 && clips.Length <= 20) // Limit output
//                     {
//                         diagnosticReport.AppendLine("    Clips:");
//                         foreach (var clip in clips)
//                         {
//                             if (clip != null)
//                             {
//                                 diagnosticReport.AppendLine($"      - {clip.name} ({clip.length:F2}s)");
//                             }
//                         }
//                     }
//                 }
//             }
//         }
//         catch (System.Exception ex)
//         {
//             diagnosticReport.AppendLine($"  - Could not access animation clips: {ex.Message}");
//         }

//         diagnosticReport.AppendLine();
//     }

//     private void CheckPlayerAnimRefHandler()
//     {
//         diagnosticReport.AppendLine("--- PlayerAnimRefHandler Check ---");

//         animRefHandler = GetComponent<PlayerAnimRefHandler>();
//         if (animRefHandler == null)
//         {
//             diagnosticReport.AppendLine("❌ PlayerAnimRefHandler component NOT FOUND");
//             diagnosticReport.AppendLine();
//             return;
//         }

//         diagnosticReport.AppendLine("✓ PlayerAnimRefHandler found");

//         // Check if DoInit was called (by checking if syncedTransforms is populated)
//         if (animRefHandler.syncedTransforms == null)
//         {
//             diagnosticReport.AppendLine("❌ syncedTransforms is NULL (DoInit may not have been called)");
//         }
//         else
//         {
//             diagnosticReport.AppendLine($"✓ syncedTransforms count: {animRefHandler.syncedTransforms.Count}");
//             if (animRefHandler.syncedTransforms.Count == 0)
//             {
//                 diagnosticReport.AppendLine("⚠ syncedTransforms is empty (no synced transforms found)");
//             }
//             else
//             {
//                 diagnosticReport.AppendLine("  Synced Transforms:");
//                 for (int i = 0; i < animRefHandler.syncedTransforms.Count; i++)
//                 {
//                     var synced = animRefHandler.syncedTransforms[i];
//                     diagnosticReport.AppendLine($"    [{i}] Target: {(synced.target != null ? synced.target.name : "NULL")}, " +
//                                                $"Follower: {(synced.follower != null ? synced.follower.name : "NULL")}");
//                 }
//             }
//         }

//         diagnosticReport.AppendLine();
//     }

//     private void CheckRigRoot()
//     {
//         diagnosticReport.AppendLine("--- RigRoot Check ---");

//         if (player == null || player.refs == null)
//         {
//             diagnosticReport.AppendLine("❌ Cannot check rigRoot - Player or refs is null");
//             diagnosticReport.AppendLine();
//             return;
//         }

//         if (player.refs.rigRoot == null)
//         {
//             diagnosticReport.AppendLine("❌ player.refs.rigRoot is NULL");
//             diagnosticReport.AppendLine("  This is CRITICAL - PlayerAnimRefHandler requires rigRoot to function!");
//             diagnosticReport.AppendLine("  Expected: GameObject named 'RigCreator' as child of root");

//             // Check if RigCreator exists but wasn't assigned
//             Transform rigCreator = transform.Find("RigCreator");
//             if (rigCreator != null)
//             {
//                 diagnosticReport.AppendLine($"  ⚠ RigCreator GameObject EXISTS but was not assigned to player.refs.rigRoot!");
//                 diagnosticReport.AppendLine($"     This should be set in Player.Start() or SetupCustomMonster");
//             }
//         }
//         else
//         {
//             diagnosticReport.AppendLine($"✓ player.refs.rigRoot found: {player.refs.rigRoot.name}");
//             diagnosticReport.AppendLine($"  - Active: {player.refs.rigRoot.activeSelf}");
//             diagnosticReport.AppendLine($"  - ActiveInHierarchy: {player.refs.rigRoot.activeInHierarchy}");

//             // Check for required components
//             Animator rigAnimator = player.refs.rigRoot.GetComponent<Animator>();
//             diagnosticReport.AppendLine($"  - Has Animator: {rigAnimator != null}");
//             if (rigAnimator != null)
//             {
//                 diagnosticReport.AppendLine($"    - Enabled: {rigAnimator.enabled}");
//                 diagnosticReport.AppendLine($"    - Controller: {(rigAnimator.runtimeAnimatorController != null ? "✓" : "❌ NULL")}");
//             }
//         }

//         diagnosticReport.AppendLine();
//     }

//     private void CheckAnimationRig()
//     {
//         diagnosticReport.AppendLine("--- AnimationRig Check ---");

//         Transform animationRig = transform.Find("AnimationRig");
//         if (animationRig == null)
//         {
//             diagnosticReport.AppendLine("❌ AnimationRig GameObject NOT FOUND");
//             diagnosticReport.AppendLine("  This is created by PlayerAnimRefHandler.SpawnAnimRef()");
//             diagnosticReport.AppendLine("  If missing, PlayerAnimRefHandler.DoInit() may not have been called");
//         }
//         else
//         {
//             diagnosticReport.AppendLine($"✓ AnimationRig found: {animationRig.name}");
//             diagnosticReport.AppendLine($"  - Active: {animationRig.gameObject.activeSelf}");
//             diagnosticReport.AppendLine($"  - ActiveInHierarchy: {animationRig.gameObject.activeInHierarchy}");

//             Animator animRigAnimator = animationRig.GetComponent<Animator>();
//             if (animRigAnimator != null)
//             {
//                 diagnosticReport.AppendLine($"  - Has Animator: ✓");
//                 diagnosticReport.AppendLine($"    - Enabled: {animRigAnimator.enabled}");
//                 diagnosticReport.AppendLine($"    - Controller: {(animRigAnimator.runtimeAnimatorController != null ? "✓" : "❌ NULL")}");
//             }
//             else
//             {
//                 diagnosticReport.AppendLine("  - Has Animator: ❌");
//             }

//             // Check if renderers are disabled (expected behavior)
//             Renderer[] renderers = animationRig.GetComponentsInChildren<Renderer>();
//             int disabledCount = 0;
//             foreach (var renderer in renderers)
//             {
//                 if (!renderer.enabled) disabledCount++;
//             }
//             diagnosticReport.AppendLine($"  - Renderers: {renderers.Length} total, {disabledCount} disabled (expected)");
//         }

//         diagnosticReport.AppendLine();
//     }

//     private void CheckSyncedTransforms()
//     {
//         diagnosticReport.AppendLine("--- Synced Transforms Check ---");

//         if (animRefHandler == null)
//         {
//             diagnosticReport.AppendLine("❌ Cannot check - PlayerAnimRefHandler is null");
//             diagnosticReport.AppendLine();
//             return;
//         }

//         if (animRefHandler.syncedTransforms == null || animRefHandler.syncedTransforms.Count == 0)
//         {
//             diagnosticReport.AppendLine("⚠ No synced transforms found");
//             diagnosticReport.AppendLine("  This means PlayerAnimRefHandler could not find transforms tagged 'Sync'");
//             diagnosticReport.AppendLine("  Check that rigRoot has child transforms with 'Sync' tag");
//         }
//         else
//         {
//             diagnosticReport.AppendLine($"✓ Found {animRefHandler.syncedTransforms.Count} synced transforms");

//             int validCount = 0;
//             int nullTargetCount = 0;
//             int nullFollowerCount = 0;

//             foreach (var synced in animRefHandler.syncedTransforms)
//             {
//                 if (synced.target != null && synced.follower != null)
//                 {
//                     validCount++;
//                 }
//                 else
//                 {
//                     if (synced.target == null) nullTargetCount++;
//                     if (synced.follower == null) nullFollowerCount++;
//                 }
//             }

//             diagnosticReport.AppendLine($"  - Valid: {validCount}");
//             if (nullTargetCount > 0) diagnosticReport.AppendLine($"  - ❌ NULL Targets: {nullTargetCount}");
//             if (nullFollowerCount > 0) diagnosticReport.AppendLine($"  - ❌ NULL Followers: {nullFollowerCount}");
//         }

//         diagnosticReport.AppendLine();
//     }

//     private void CheckVisualTargets()
//     {
//         diagnosticReport.AppendLine("--- Visual Targets Check ---");

//         Transform visual = transform.Find("Visual");
//         if (visual == null)
//         {
//             diagnosticReport.AppendLine("❌ Visual GameObject NOT FOUND");
//             diagnosticReport.AppendLine();
//             return;
//         }

//         PlayerVisual playerVisual = visual.GetComponent<PlayerVisual>();
//         if (playerVisual == null)
//         {
//             diagnosticReport.AppendLine("⚠ PlayerVisual component NOT FOUND on Visual GameObject");
//         }
//         else
//         {
//             diagnosticReport.AppendLine("✓ PlayerVisual found");
//             // Note: followerConfig might be private, so we can't check it directly
//             diagnosticReport.AppendLine("  (followerConfig details may be private)");
//         }

//         diagnosticReport.AppendLine();
//     }

//     private void CheckAnimationState()
//     {
//         diagnosticReport.AppendLine("--- CURRENTLY PLAYING ANIMATIONS ---");

//         // Check all possible animators
//         List<(Animator anim, string source)> animatorsToCheck = new List<(Animator, string)>();

//         // Root animator
//         if (animator != null && animator.enabled && animator.isInitialized)
//         {
//             animatorsToCheck.Add((animator, "Root GameObject"));
//         }

//         // RigCreator animator
//         Transform rigCreator = transform.Find("RigCreator");
//         if (rigCreator != null)
//         {
//             Animator rigAnimator = rigCreator.GetComponent<Animator>();
//             if (rigAnimator != null && rigAnimator.enabled && rigAnimator.isInitialized)
//             {
//                 animatorsToCheck.Add((rigAnimator, "RigCreator"));
//             }
//         }

//         // AnimationRig animator (the one that actually drives the visual)
//         Transform animationRig = transform.Find("AnimationRig");
//         if (animationRig != null)
//         {
//             Animator animRigAnimator = animationRig.GetComponent<Animator>();
//             if (animRigAnimator != null && animRigAnimator.enabled && animRigAnimator.isInitialized)
//             {
//                 animatorsToCheck.Add((animRigAnimator, "AnimationRig (Visual Driver)"));
//             }
//         }

//         if (animatorsToCheck.Count == 0)
//         {
//             diagnosticReport.AppendLine("❌ No initialized/enabled Animators found");
//             diagnosticReport.AppendLine();
//             return;
//         }

//         bool anyAnimationPlaying = false;

//         foreach (var (targetAnimator, sourceName) in animatorsToCheck)
//         {
//             diagnosticReport.AppendLine($"\n[{sourceName}]");

//             if (targetAnimator.layerCount == 0)
//             {
//                 diagnosticReport.AppendLine("  ⚠ Animator has no layers");
//                 continue;
//             }

//             for (int i = 0; i < targetAnimator.layerCount; i++)
//             {
//                 AnimatorStateInfo stateInfo = targetAnimator.GetCurrentAnimatorStateInfo(i);
//                 AnimatorClipInfo[] clipInfo = targetAnimator.GetCurrentAnimatorClipInfo(i);

//                 string layerName = targetAnimator.GetLayerName(i);
//                 float layerWeight = targetAnimator.GetLayerWeight(i);

//                 diagnosticReport.AppendLine($"  Layer {i} ({layerName}) [Weight: {layerWeight:F2}]:");
//                 diagnosticReport.AppendLine($"    - State Hash: {stateInfo.fullPathHash}");
//                 diagnosticReport.AppendLine($"    - Normalized Time: {stateInfo.normalizedTime:F2} / {stateInfo.length:F2}s");
//                 diagnosticReport.AppendLine($"    - Speed: {stateInfo.speed:F2} (Animator Speed: {targetAnimator.speed:F2})");
//                 diagnosticReport.AppendLine($"    - Looping: {stateInfo.loop}");

//                 // Try to get state name from hash
//                 string stateName = GetStateNameFromHash(targetAnimator, stateInfo.fullPathHash, i);
//                 if (!string.IsNullOrEmpty(stateName))
//                 {
//                     diagnosticReport.AppendLine($"    - State Name: {stateName}");
//                 }

//                 // Show currently playing clips
//                 if (clipInfo != null && clipInfo.Length > 0)
//                 {
//                     anyAnimationPlaying = true;
//                     diagnosticReport.AppendLine($"    - 🎬 ACTIVE CLIPS ({clipInfo.Length}):");
//                     foreach (var clip in clipInfo)
//                     {
//                         if (clip.clip != null)
//                         {
//                             float clipTime = clip.clip.length * stateInfo.normalizedTime;
//                             diagnosticReport.AppendLine($"      ▶ {clip.clip.name}");
//                             diagnosticReport.AppendLine($"        Weight: {clip.weight:F2} | Length: {clip.clip.length:F2}s | Time: {clipTime:F2}s");
//                             diagnosticReport.AppendLine($"        Frame Rate: {clip.clip.frameRate} | Is Loop: {clip.clip.isLooping}");
//                         }
//                     }
//                 }
//                 else
//                 {
//                     diagnosticReport.AppendLine($"    - ⚠ No active clips on this layer");
//                 }

//                 // Show transition info
//                 if (stateInfo.normalizedTime < 1.0f && stateInfo.normalizedTime > 0.0f)
//                 {
//                     float remainingTime = (1.0f - stateInfo.normalizedTime) * stateInfo.length;
//                     diagnosticReport.AppendLine($"    - Progress: {stateInfo.normalizedTime * 100:F1}% | Remaining: {remainingTime:F2}s");
//                 }
//             }
//         }

//         if (!anyAnimationPlaying)
//         {
//             diagnosticReport.AppendLine("\n⚠ WARNING: No animations are currently playing on any animator!");
//             diagnosticReport.AppendLine("  This could indicate:");
//             diagnosticReport.AppendLine("  - Animator is not receiving input/parameters");
//             diagnosticReport.AppendLine("  - Animation Controller has no default state");
//             diagnosticReport.AppendLine("  - Animator is paused or not updating");
//         }

//         diagnosticReport.AppendLine();
//     }

//     /// <summary>
//     /// Attempts to get the state name from the animator hash
//     /// </summary>
//     private string GetStateNameFromHash(Animator animator, int hash, int layerIndex)
//     {
//         try
//         {
//             // Try common state names
//             string[] commonStateNames = { "Idle", "Walk", "Run", "Attack", "Death", "Hit", "Jump", "Fall" };
//             foreach (string stateName in commonStateNames)
//             {
//                 if (animator.HasState(layerIndex, Animator.StringToHash(stateName)))
//                 {
//                     AnimatorStateInfo testInfo = animator.GetCurrentAnimatorStateInfo(layerIndex);
//                     if (testInfo.fullPathHash == hash)
//                     {
//                         return stateName;
//                     }
//                 }
//             }
//         }
//         catch
//         {
//             // Ignore errors
//         }

//         return "";
//     }

//     private void CheckAnimationParameters()
//     {
//         diagnosticReport.AppendLine("--- Animation Parameters Check ---");

//         Animator? targetAnimator = null;

//         if (animator != null && animator.enabled)
//         {
//             targetAnimator = animator;
//         }
//         else
//         {
//             Transform rigCreator = transform.Find("RigCreator");
//             if (rigCreator != null)
//             {
//                 targetAnimator = rigCreator.GetComponent<Animator>();
//             }
//         }

//         if (targetAnimator == null || !targetAnimator.isInitialized)
//         {
//             diagnosticReport.AppendLine("❌ No initialized Animator found");
//             diagnosticReport.AppendLine();
//             return;
//         }

//         if (targetAnimator.parameterCount == 0)
//         {
//             diagnosticReport.AppendLine("⚠ Animator has no parameters");
//             diagnosticReport.AppendLine();
//             return;
//         }

//         diagnosticReport.AppendLine($"✓ Found {targetAnimator.parameterCount} parameters:");

//         for (int i = 0; i < targetAnimator.parameterCount; i++)
//         {
//             AnimatorControllerParameter param = targetAnimator.GetParameter(i);
//             string valueStr = "";

//             switch (param.type)
//             {
//                 case AnimatorControllerParameterType.Float:
//                     valueStr = targetAnimator.GetFloat(param.name).ToString("F2");
//                     break;
//                 case AnimatorControllerParameterType.Int:
//                     valueStr = targetAnimator.GetInteger(param.name).ToString();
//                     break;
//                 case AnimatorControllerParameterType.Bool:
//                     valueStr = targetAnimator.GetBool(param.name).ToString();
//                     break;
//                 case AnimatorControllerParameterType.Trigger:
//                     valueStr = "Trigger";
//                     break;
//             }

//             diagnosticReport.AppendLine($"  - {param.name} ({param.type}): {valueStr}");
//         }

//         diagnosticReport.AppendLine();
//     }

//     private void CompareWithMainCharacter()
//     {
//         diagnosticReport.AppendLine("--- Comparison with MainCharacter ---");

//         GameObject? mainChar = DbsContentApi.customMonsters.Find(m => m.name == "MainCharacter");
//         if (mainChar == null)
//         {
//             diagnosticReport.AppendLine("⚠ MainCharacter prefab not available for comparison");
//             diagnosticReport.AppendLine();
//             return;
//         }

//         diagnosticReport.AppendLine($"Comparing with: {mainChar.name}");

//         // Compare setup
//         bool thisHasRigCreator = transform.Find("RigCreator") != null;
//         bool mainCharHasRigCreator = mainChar.transform.Find("RigCreator") != null;

//         diagnosticReport.AppendLine($"RigCreator:");
//         diagnosticReport.AppendLine($"  - This: {(thisHasRigCreator ? "✓" : "❌")}");
//         diagnosticReport.AppendLine($"  - MainCharacter: {(mainCharHasRigCreator ? "✓" : "❌")}");

//         Player mainCharPlayer = mainChar.GetComponent<Player>();
//         if (mainCharPlayer != null && mainCharPlayer.refs != null)
//         {
//             diagnosticReport.AppendLine($"MainCharacter rigRoot: {(mainCharPlayer.refs.rigRoot != null ? "✓" : "❌")}");
//         }

//         PlayerAnimRefHandler mainCharAnimRef = mainChar.GetComponent<PlayerAnimRefHandler>();
//         if (mainCharAnimRef != null)
//         {
//             diagnosticReport.AppendLine($"MainCharacter PlayerAnimRefHandler: ✓");
//             diagnosticReport.AppendLine($"  - Synced Transforms: {mainCharAnimRef.syncedTransforms?.Count ?? 0}");
//         }
//         else
//         {
//             diagnosticReport.AppendLine($"MainCharacter PlayerAnimRefHandler: ❌");
//         }

//         diagnosticReport.AppendLine();
//     }

//     /// <summary>
//     /// Force run diagnostics and output to console
//     /// </summary>
//     [ContextMenu("Run Diagnostics Now")]
//     public void RunDiagnosticsNow()
//     {
//         RunFullDiagnostics();
//     }
// }
