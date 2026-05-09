using UnityEngine;
using UnityEngine.SceneManagement;

namespace DbsContentApi.Modules.Utility;

/// <summary>
/// Helper class for managing temporary game objects.
/// </summary>
public static class ObjectHelper
{
    /// <summary>
    /// Layer for short-lived proxies used by <see cref="ContentPolling"/> raycasts.
    /// Solid colliders are required (polling ignores triggers); these layers avoid blocking players.
    /// </summary>
    private static int ResolveContentPollProxyLayer()
    {
        int layer = LayerMask.NameToLayer("Interactible_NoCollide");
        if (layer >= 0)
            return layer;
        layer = LayerMask.NameToLayer("Ignore Raycast");
        if (layer >= 0)
            return layer;
        return HelperFunctions.GetLayer(HelperFunctions.LayerType.Terrain);
    }

    private static void SetLayerRecursively(GameObject go, int layer)
    {
        go.layer = layer;
        Transform transform = go.transform;
        for (int i = 0; i < transform.childCount; i++)
            SetLayerRecursively(transform.GetChild(i).gameObject, layer);
    }

    /// <summary>
    /// Creates a temporary GameObject that will automatically disable itself after a frame budget.
    /// </summary>
    /// <param name="frameCount">How many Update frames the object should remain active.</param>
    /// <returns>The created GameObject.</returns>
    public static GameObject CreateTemporaryTriggerObject(int frameCount, GameObject prefab)
    {
        GameObject tempObj = Object.Instantiate(prefab);
        TimedDestruction timedDestruction = tempObj.AddComponent<TimedDestruction>();
        timedDestruction.frames = frameCount;
        SetLayerRecursively(tempObj, ResolveContentPollProxyLayer());
        return tempObj;
    }

    /// <summary>
    /// Creates a permanent Trigger GameObject
    /// </summary>
    /// <returns>The created GameObject.</returns>
    public static GameObject CreatePermanentTriggerObject(GameObject prefab)
    {
        GameObject tempObj = Object.Instantiate(prefab);
        SetLayerRecursively(tempObj, ResolveContentPollProxyLayer());
        return tempObj;
    }

    /// <summary>
    /// Finds all MeshRenderers and SkinnedMeshRenderers on <paramref name="go"/> and its children,
    /// then swaps each material's shader to match the current scene:
    /// <list type="bullet">
    ///   <item>SurfaceScene — replaces the "World" shader with "NiceShader"</item>
    ///   <item>Old world    — replaces the "NiceShader" shader with "World"</item>
    /// </list>
    /// Materials that already use the correct shader are left untouched.
    /// </summary>
    public static void FixShadersForCurrentScene(GameObject go)
    {
        bool isSurface = SceneManager.GetActiveScene().name == "SurfaceScene";

        string fromShaderName = isSurface ? "World" : "NiceShader";
        string toShaderName   = isSurface ? "NiceShader" : "World";

        Shader toShader = Shader.Find(toShaderName);
        if (toShader == null)
        {
            Logger.LogError($"[ObjectHelper] Could not find shader \"{toShaderName}\".");
            return;
        }

        foreach (Renderer renderer in go.GetComponentsInChildren<Renderer>(true))
        {
            Material[] mats = renderer.materials;
            bool changed = false;
            for (int i = 0; i < mats.Length; i++)
            {
                if (mats[i] != null && mats[i].shader != null &&
                    mats[i].shader.name == fromShaderName)
                {
                    mats[i].shader = toShader;
                    changed = true;
                }
            }
            if (changed)
                renderer.materials = mats;
        }
    }

    /// <summary>
    /// Creates a permanent Trigger GameObject attached to a bone with randomization.
    /// </summary>
    public static GameObject CreateAttachedTriggerObject(GameObject prefab, Transform parent, int frameLifetime = -1)
    {
        GameObject trigger = CreatePermanentTriggerObject(prefab);
        trigger.transform.SetParent(parent);

        // Random Y rotation
        float randomYRot = Random.Range(0f, 360f);
        trigger.transform.localRotation = Quaternion.Euler(0f, randomYRot, 0f);

        // Random Y offset from -0.5 to 0.5
        float randomYOffset = Random.Range(-0.5f, 0.5f);
        trigger.transform.localPosition = new Vector3(0f, randomYOffset, 0f);

        if (frameLifetime > 0)
        {
            TimedDestruction timedDestruction = trigger.AddComponent<TimedDestruction>();
            timedDestruction.frames = frameLifetime;
        }

        return trigger;
    }
}
