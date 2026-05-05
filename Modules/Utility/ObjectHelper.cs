using UnityEngine;

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
