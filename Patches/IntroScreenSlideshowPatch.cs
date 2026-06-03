// using System;
// using System.Collections.Generic;
// using System.Reflection;
// using HarmonyLib;
// using UnityEngine;
// using UnityEngine.EventSystems;
// using UnityEngine.InputSystem;
// using UnityEngine.UI;
// using Zorro.ControllerSupport;

// namespace DbsContentApi;

// /// <summary>
// /// Opt-in pre-roll slideshow support for mods that want a short API-branded intro before the vanilla intro.
// /// </summary>
// public static class IntroScreenSlideshow
// {
//     private const string IntroScreenBdPrefabName = "IntroScreenBD.prefab";

//     private static GameObject? _introScreenBdPrefab;
//     private static bool _requested;

//     /// <summary>
//     /// Requests that the bundled IntroScreenBD prefab play before the vanilla intro screen.
//     /// Safe to call from multiple mods; the slideshow will only be inserted once.
//     /// </summary>
//     public static void Request()
//     {
//         if (_requested)
//         {
//             ApiLog.Log("[IntroScreenBD] Slideshow was requested again; keeping the existing request.");
//             return;
//         }

//         _requested = true;
//         ApiLog.Log("[IntroScreenBD] Slideshow requested.");
//     }

//     internal static bool IsRequested => _requested;

//     internal static bool TryGetPrefab(out GameObject prefab)
//     {
//         if (_introScreenBdPrefab != null)
//         {
//             ApiLog.Log("[IntroScreenBD] Using cached IntroScreenBD prefab.");
//             prefab = _introScreenBdPrefab;
//             return true;
//         }

//         prefab = null!;

//         if (DbsContentApiPlugin.ApiAssetBundle == null)
//         {
//             ApiLog.LogError("[IntroScreenBD] Cannot load prefab because the API asset bundle is missing.");
//             return false;
//         }

//         try
//         {
//             ApiLog.Log($"[IntroScreenBD] Loading {IntroScreenBdPrefabName} from API asset bundle.");
//             _introScreenBdPrefab =
//                 ContentLoader.LoadPrefabFromBundle(DbsContentApiPlugin.ApiAssetBundle, IntroScreenBdPrefabName);
//             prefab = _introScreenBdPrefab;
//             ApiLog.Log($"[IntroScreenBD] Loaded prefab '{prefab.name}'.");
//             return true;
//         }
//         catch (Exception e)
//         {
//             ApiLog.LogError($"[IntroScreenBD] Failed to load {IntroScreenBdPrefabName}: {e.Message}");
//             return false;
//         }
//     }
// }

// [HarmonyPatch(typeof(IntroScreenAnimator))]
// internal static class IntroScreenAnimatorPatch
// {
//     private static readonly FieldInfo? HasPlayedField =
//         AccessTools.Field(typeof(IntroScreenAnimator), "m_hasPlayed");

//     private static bool _insertedThisSession;

//     [HarmonyPatch("Start")]
//     [HarmonyPrefix]
//     public static bool StartPrefix(IntroScreenAnimator __instance)
//     {
//         ApiLog.Log(
//             $"[IntroScreenBD] IntroScreenAnimator.Start intercepted on '{__instance.gameObject.name}'. " +
//             $"requested={IntroScreenSlideshow.IsRequested}, inserted={_insertedThisSession}, hasPlayed={HasVanillaIntroPlayed()}.");

//         if (IntroScreenBdCoordinator.IsResumedVanillaIntro(__instance))
//         {
//             ApiLog.Log("[IntroScreenBD] Suppressing vanilla Start during BD handoff so replay guard does not deactivate it.");
//             return false;
//         }

//         if (IntroScreenBdCoordinator.IsBdIntroObject(__instance.gameObject))
//         {
//             // The BD prefab carries vanilla references for value transfer only.
//             ApiLog.Log("[IntroScreenBD] Suppressing vanilla IntroScreenAnimator on BD prefab instance.");
//             __instance.enabled = false;
//             return false;
//         }

//         if (!IntroScreenSlideshow.IsRequested || _insertedThisSession || HasVanillaIntroPlayed())
//         {
//             ApiLog.Log("[IntroScreenBD] Letting vanilla intro start normally.");
//             return true;
//         }

//         if (!IntroScreenSlideshow.TryGetPrefab(out GameObject prefab))
//         {
//             ApiLog.LogWarning("[IntroScreenBD] Prefab unavailable; letting vanilla intro start normally.");
//             return true;
//         }

//         _insertedThisSession = true;
//         ApiLog.Log("[IntroScreenBD] Inserting BD slideshow before vanilla intro.");

//         if (!IntroScreenBdCoordinator.TryPlayBeforeVanilla(__instance, prefab))
//         {
//             ApiLog.LogError("[IntroScreenBD] Failed to start custom intro; falling back to vanilla intro.");
//             return true;
//         }

//         return false;
//     }

//     internal static void MarkVanillaIntroPlayed()
//     {
//         if (HasPlayedField == null)
//         {
//             ApiLog.LogError("[IntroScreenBD] Could not find IntroScreenAnimator.m_hasPlayed; vanilla intro may replay unexpectedly.");
//             return;
//         }

//         HasPlayedField?.SetValue(null, true);
//         ApiLog.Log("[IntroScreenBD] Marked vanilla intro as played before handoff.");
//     }

//     private static bool HasVanillaIntroPlayed()
//     {
//         return HasPlayedField?.GetValue(null) is true;
//     }
// }

// internal static class IntroScreenBdCoordinator
// {
//     private const string IntroScreenBdName = "IntroScreenBD";
//     private static readonly HashSet<int> ResumedVanillaIntroInstanceIds = new();

//     internal static bool TryPlayBeforeVanilla(IntroScreenAnimator vanillaIntro, GameObject prefab)
//     {
//         GameObject vanillaObject = vanillaIntro.gameObject;
//         Transform? parent = vanillaObject.transform.parent;
//         ApiLog.Log(
//             $"[IntroScreenBD] Instantiating '{prefab.name}' next to vanilla intro '{vanillaObject.name}'. " +
//             $"parent='{(parent != null ? parent.name : "<none>")}'.");

//         GameObject bdObject = UnityEngine.Object.Instantiate(prefab, parent, false);
//         bdObject.name = IntroScreenBdName;

//         if (!TryInstallController(bdObject, vanillaIntro, out IntroScreenBdAnimator controller))
//         {
//             UnityEngine.Object.Destroy(bdObject);
//             return false;
//         }

//         PauseVanilla(vanillaIntro);
//         controller.Play(() => ResumeVanilla(vanillaIntro));
//         ApiLog.Log("[IntroScreenBD] BD slideshow is now playing.");
//         return true;
//     }

//     internal static bool IsBdIntroObject(GameObject gameObject)
//     {
//         return gameObject.name.StartsWith(IntroScreenBdName, StringComparison.Ordinal);
//     }

//     internal static bool IsResumedVanillaIntro(IntroScreenAnimator intro)
//     {
//         return ResumedVanillaIntroInstanceIds.Contains(intro.GetInstanceID());
//     }

//     private static bool TryInstallController(
//         GameObject bdObject,
//         IntroScreenAnimator vanillaIntro,
//         out IntroScreenBdAnimator controller)
//     {
//         controller = null!;

//         IntroScreenAnimator source = bdObject.GetComponent<IntroScreenAnimator>();
//         if (source == null)
//         {
//             ApiLog.LogError("[IntroScreenBD] Prefab is missing IntroScreenAnimator for value transfer.");
//             return false;
//         }

//         ApiLog.Log("[IntroScreenBD] Found prefab IntroScreenAnimator; copying values into custom controller.");
//         source.enabled = false;
//         controller = bdObject.AddComponent<IntroScreenBdAnimator>();
//         controller.CopyFrom(source, vanillaIntro);
//         UnityEngine.Object.Destroy(source);
//         ApiLog.Log("[IntroScreenBD] Disabled and destroyed prefab vanilla IntroScreenAnimator after value transfer.");
//         return true;
//     }

//     private static void PauseVanilla(IntroScreenAnimator vanillaIntro)
//     {
//         ApiLog.Log($"[IntroScreenBD] Pausing vanilla intro '{vanillaIntro.gameObject.name}'.");
//         vanillaIntro.gameObject.SetActive(false);
//     }

//     private static void ResumeVanilla(IntroScreenAnimator vanillaIntro)
//     {
//         if (vanillaIntro == null)
//         {
//             ApiLog.LogWarning("[IntroScreenBD] Vanilla intro was destroyed before handoff; cannot resume it.");
//             return;
//         }

//         ApiLog.Log($"[IntroScreenBD] Resuming vanilla intro '{vanillaIntro.gameObject.name}'.");
//         IntroScreenAnimatorPatch.MarkVanillaIntroPlayed();
//         ResumedVanillaIntroInstanceIds.Add(vanillaIntro.GetInstanceID());

//         vanillaIntro.skipping = false;
//         ResetVanillaVisuals(vanillaIntro);

//         vanillaIntro.gameObject.SetActive(true);

//         if (vanillaIntro.m_animator != null)
//         {
//             vanillaIntro.m_animator.enabled = true;
//             vanillaIntro.m_animator.Rebind();
//             vanillaIntro.m_animator.Update(0f);
//         }
//         else
//         {
//             ApiLog.LogWarning("[IntroScreenBD] Vanilla intro has no animator during resume.");
//         }

//         ApiLog.Log("[IntroScreenBD] Vanilla intro reactivated.");
//     }

//     private static void ResetVanillaVisuals(IntroScreenAnimator vanillaIntro)
//     {
//         if (vanillaIntro.m_canvasGroup != null)
//         {
//             vanillaIntro.m_canvasGroup.alpha = 1f;
//             vanillaIntro.m_canvasGroup.blocksRaycasts = true;
//             vanillaIntro.m_canvasGroup.interactable = true;
//         }
//         else
//         {
//             ApiLog.LogWarning("[IntroScreenBD] Vanilla intro has no canvas group during resume.");
//         }

//         if (vanillaIntro.m_image != null)
//         {
//             Color color = vanillaIntro.m_image.color;
//             color.a = 1f;
//             vanillaIntro.m_image.color = color;
//         }
//         else
//         {
//             ApiLog.LogWarning("[IntroScreenBD] Vanilla intro has no image during resume; vanilla may destroy itself immediately.");
//         }

//         if (vanillaIntro.m_audioSource != null)
//         {
//             vanillaIntro.m_audioSource.volume = 1f;
//             vanillaIntro.m_audioSource.Stop();
//             vanillaIntro.m_audioSource.Play();
//         }
//         else
//         {
//             ApiLog.LogWarning("[IntroScreenBD] Vanilla intro has no audio source during resume.");
//         }
//     }
// }

// internal sealed class IntroScreenBdAnimator : MonoBehaviour
// {
//     private const float FadeCompleteAlpha = 0.01f;
//     private const float FadeCompleteVolume = 0.01f;

//     private InputActionReference? _skipAction;
//     private Button? _hiddenSkipButton;
//     private Graphic? _image;
//     private Animator? _animator;
//     private CanvasGroup? _canvasGroup;
//     private AudioSource? _audioSource;
//     private AudioSource? _ambience;
//     private Action? _onFinished;
//     private bool _finished;
//     private bool _skipping;
//     private bool _loggedMissingImage;
//     private bool _loggedMissingCanvasGroup;
//     private bool _loggedMissingAudioSource;
//     private bool _loggedAnimatorComplete;

//     internal void CopyFrom(IntroScreenAnimator source, IntroScreenAnimator vanillaIntro)
//     {
//         _skipAction = source.m_skipAction ?? vanillaIntro.m_skipAction;
//         _hiddenSkipButton = source.m_hiddenSkipButton;
//         _image = source.m_image;
//         _animator = source.m_animator;
//         _canvasGroup = source.m_canvasGroup;
//         _audioSource = source.m_audioSource;
//         _ambience = source.m_ambience ?? vanillaIntro.m_ambience;

//         ApiLog.Log(
//             "[IntroScreenBD] Value transfer complete. " +
//             $"skipAction={Describe(_skipAction)}, hiddenSkipButton={Describe(_hiddenSkipButton)}, " +
//             $"image={Describe(_image)}, animator={Describe(_animator)}, canvasGroup={Describe(_canvasGroup)}, " +
//             $"audioSource={Describe(_audioSource)}, ambience={Describe(_ambience)}.");

//         if (_image == null)
//         {
//             ApiLog.LogWarning("[IntroScreenBD] Prefab IntroScreenAnimator.m_image is null; using CanvasGroup alpha for finish/skip visibility checks.");
//         }

//         if (_canvasGroup == null)
//         {
//             ApiLog.LogWarning("[IntroScreenBD] Prefab IntroScreenAnimator.m_canvasGroup is null; fade/finish behavior may not match vanilla.");
//         }

//         if (_audioSource == null)
//         {
//             ApiLog.LogWarning("[IntroScreenBD] Prefab IntroScreenAnimator.m_audioSource is null; finish checks will be visual-only.");
//         }

//         if (_hiddenSkipButton != null)
//         {
//             _hiddenSkipButton.onClick.AddListener(Skip);
//         }
//         else
//         {
//             ApiLog.LogWarning("[IntroScreenBD] Prefab IntroScreenAnimator.m_hiddenSkipButton is null; clickable/gamepad skip button will not work.");
//         }
//     }

//     internal void Play(Action onFinished)
//     {
//         _onFinished = onFinished;
//         ApiLog.Log("[IntroScreenBD] Custom BD controller Play called.");

//         if (_animator != null)
//         {
//             _animator.enabled = true;
//         }
//         else
//         {
//             ApiLog.LogWarning("[IntroScreenBD] BD controller has no animator to enable.");
//         }

//         if (_canvasGroup != null)
//         {
//             _canvasGroup.alpha = 1f;
//             _canvasGroup.blocksRaycasts = true;
//             _canvasGroup.interactable = true;
//         }
//         else
//         {
//             ApiLog.LogWarning("[IntroScreenBD] BD controller has no canvas group to initialize.");
//         }
//     }

//     private void OnDestroy()
//     {
//         if (_hiddenSkipButton != null)
//         {
//             _hiddenSkipButton.onClick.RemoveListener(Skip);
//         }

//         ApiLog.Log("[IntroScreenBD] BD controller destroyed.");
//     }

//     private void Update()
//     {
//         if (_finished)
//         {
//             return;
//         }

//         if (!_skipping && HasAnimatorCompleted(out string animatorCompletionReason))
//         {
//             BeginFadeOut(animatorCompletionReason);
//         }

//         if (_skipping)
//         {
//             ApplyFadeOut();
//         }

//         if (HasCompletedFadeOut(out string completionReason))
//         {
//             Finish(completionReason);
//             return;
//         }

//         if (_canvasGroup != null && _canvasGroup.alpha < 0.5f)
//         {
//             _canvasGroup.blocksRaycasts = false;
//             _canvasGroup.interactable = false;
//         }

//         if (_ambience != null && _canvasGroup != null)
//         {
//             _ambience.volume = (1f - _canvasGroup.alpha) * 0.1f;
//         }

//         UpdateGamepadSkipSelection();

//         if (Input.GetKeyDown(KeyCode.Escape) ||
//             Input.GetKeyDown(KeyCode.Space) ||
//             (_skipAction?.action?.WasPressedThisFrame() ?? false))
//         {
//             Skip();
//         }
//     }

//     private bool HasCompletedFadeOut(out string reason)
//     {
//         reason = string.Empty;
//         bool imageGone = IsVisualFadeComplete();
//         bool audioDone = _audioSource == null || !_audioSource.isPlaying;

//         if (_audioSource == null && !_loggedMissingAudioSource)
//         {
//             _loggedMissingAudioSource = true;
//             ApiLog.LogWarning("[IntroScreenBD] Completion check has no audio source; treating audio as done.");
//         }

//         if (!imageGone || !audioDone)
//         {
//             return false;
//         }

//         reason = $"visualComplete={imageGone}, audioDone={audioDone}";
//         return true;
//     }

//     private bool HasAnimatorCompleted(out string reason)
//     {
//         reason = string.Empty;

//         if (_animator == null || !_animator.enabled)
//         {
//             return false;
//         }

//         if (_animator.layerCount <= 0)
//         {
//             ApiLog.LogWarning("[IntroScreenBD] Animator has no layers; starting fallback fade.");
//             reason = "animator has no layers";
//             return true;
//         }

//         AnimatorStateInfo state = _animator.GetCurrentAnimatorStateInfo(0);
//         if (_animator.IsInTransition(0) || state.normalizedTime < 1f)
//         {
//             return false;
//         }

//         if (!_loggedAnimatorComplete)
//         {
//             _loggedAnimatorComplete = true;
//             ApiLog.Log(
//                 $"[IntroScreenBD] Animator state completed. shortNameHash={state.shortNameHash}, " +
//                 $"normalizedTime={state.normalizedTime:0.00}.");
//         }

//         reason = $"animator state complete ({state.normalizedTime:0.00})";
//         return true;
//     }

//     private void ApplyFadeOut()
//     {
//         if (_canvasGroup != null)
//         {
//             _canvasGroup.alpha = Mathf.Lerp(_canvasGroup.alpha, 0f, Time.unscaledDeltaTime * 10f);
//             if (_canvasGroup.alpha <= FadeCompleteAlpha)
//             {
//                 _canvasGroup.alpha = 0f;
//             }
//         }

//         if (_image != null)
//         {
//             Color color = _image.color;
//             color.a = Mathf.Lerp(color.a, 0f, Time.unscaledDeltaTime * 10f);
//             if (color.a <= FadeCompleteAlpha)
//             {
//                 color.a = 0f;
//             }

//             _image.color = color;
//         }

//         if (_audioSource != null)
//         {
//             _audioSource.volume = Mathf.Lerp(_audioSource.volume, 0f, Time.unscaledDeltaTime * 2f);
//             if (_audioSource.volume <= FadeCompleteVolume)
//             {
//                 _audioSource.volume = 0f;
//                 _audioSource.Stop();
//             }
//         }
//     }

//     private bool IsVisualFadeComplete()
//     {
//         if (_image != null)
//         {
//             return _image.color.a <= FadeCompleteAlpha;
//         }

//         if (_canvasGroup != null)
//         {
//             if (!_loggedMissingImage)
//             {
//                 _loggedMissingImage = true;
//                 ApiLog.LogWarning("[IntroScreenBD] m_image is null during completion check; using CanvasGroup alpha instead.");
//             }

//             return _canvasGroup.alpha <= FadeCompleteAlpha;
//         }

//         if (!_loggedMissingCanvasGroup)
//         {
//             _loggedMissingCanvasGroup = true;
//             ApiLog.LogWarning("[IntroScreenBD] Both m_image and m_canvasGroup are null; treating visual fade as complete.");
//         }

//         return true;
//     }

//     private void UpdateGamepadSkipSelection()
//     {
//         if (_hiddenSkipButton == null)
//         {
//             return;
//         }

//         bool isGamepad = InputHandler.GetCurrentUsedInputScheme() == InputScheme.Gamepad;
//         bool canSkip = IsVisibleEnoughForSkip() && !_skipping;
//         GameObject buttonObject = _hiddenSkipButton.gameObject;

//         if (isGamepad && canSkip)
//         {
//             buttonObject.SetActive(true);
//             if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject != buttonObject)
//             {
//                 EventSystem.current.SetSelectedGameObject(buttonObject);
//             }

//             return;
//         }

//         buttonObject.SetActive(false);
//         if (EventSystem.current != null && EventSystem.current.currentSelectedGameObject == buttonObject)
//         {
//             EventSystem.current.SetSelectedGameObject(null);
//         }
//     }

//     private void Skip()
//     {
//         if (_finished || _skipping)
//         {
//             ApiLog.Log($"[IntroScreenBD] Skip ignored. finished={_finished}, skipping={_skipping}.");
//             return;
//         }

//         BeginFadeOut("skip requested");
//     }

//     private void BeginFadeOut(string reason)
//     {
//         if (_skipping)
//         {
//             return;
//         }

//         ApiLog.Log($"[IntroScreenBD] Starting fade out: {reason}.");
//         _skipping = true;
//         if (_animator != null)
//         {
//             _animator.enabled = false;
//         }
//         else
//         {
//             ApiLog.LogWarning("[IntroScreenBD] Skip requested but animator is null.");
//         }
//     }

//     private bool IsVisibleEnoughForSkip()
//     {
//         if (_image != null)
//         {
//             return _image.color.a > 0.5f;
//         }

//         if (_canvasGroup != null)
//         {
//             return _canvasGroup.alpha > 0.5f;
//         }

//         return true;
//     }

//     private void Finish(string reason)
//     {
//         _finished = true;
//         ApiLog.Log($"[IntroScreenBD] BD slideshow finished ({reason}); handing off to vanilla intro.");

//         if (_ambience != null)
//         {
//             _ambience.volume = 0f;
//         }

//         Action? onFinished = _onFinished;
//         _onFinished = null;
//         onFinished?.Invoke();
//         Destroy(gameObject);
//     }

//     private static string Describe(UnityEngine.Object? value)
//     {
//         return value != null ? value.name : "<null>";
//     }
// }
