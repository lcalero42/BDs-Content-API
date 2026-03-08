using System.IO;
using UnityEngine;
using BepInEx;
using Photon.Pun;
using System.Reflection;

namespace DbsContentApi.Modules;

/// <summary>
/// Utility class for loading assets from AssetBundles.
/// </summary>
public static class ContentLoader
{
    /// <summary>
    /// Loads an AssetBundle from the plugin's directory.
    /// </summary>
    /// <param name="bundleName">The filename of the bundle to load.</param>
    /// <returns>The loaded AssetBundle.</returns>
    public static AssetBundle LoadAssetBundle(string assemblyLocation, string bundleName)
    {
        // Get the directory where the current DLL is located
        string directory = Path.GetDirectoryName(assemblyLocation);

        // Combine directory with the bundle filename
        string bundlePath = Path.Combine(directory, bundleName);

        // Load the bundle
        AssetBundle myBundle = AssetBundle.LoadFromFile(bundlePath);

        if (myBundle == null)
        {
            Debug.LogError($"Failed to load AssetBundle: {bundleName} at path: {bundlePath}");
            return null;
        }

        return myBundle;
    }

    /// <summary>
    /// Extracts a prefab GameObject from a loaded AssetBundle.
    /// </summary>
    /// <param name="bundle">The AssetBundle containing the prefab.</param>
    /// <param name="prefabName">The name of the prefab asset.</param>
    /// <returns>The loaded GameObject prefab.</returns>
    /// <exception cref="System.Exception">Thrown if the prefab asset is not found.</exception>
    public static GameObject LoadPrefabFromBundle(AssetBundle bundle, string prefabName)
    {
        GameObject prefab = bundle.LoadAsset<GameObject>(prefabName);
        if (prefab == null) throw new System.Exception($"Failed to load Prefab: {prefabName} from bundle: {bundle.name}");
        return prefab;
    }

    /// <summary>
    /// Registers a prefab in the PhotonNetwork prefab pool.
    /// </summary>
    /// <param name="prefab">The prefab to register.</param>
    public static void RegisterPrefabInPhotonPool(GameObject prefab)
    {
        if (PhotonNetwork.PrefabPool is DefaultPool defaultPool && !defaultPool.ResourceCache.ContainsKey(prefab.name))
        {
            defaultPool.ResourceCache.Add(prefab.name, prefab);
        }
    }
}

