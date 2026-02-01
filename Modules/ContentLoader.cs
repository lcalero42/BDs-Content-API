using System.IO;
using UnityEngine;
using BepInEx;
using Photon.Pun;

namespace DbsContentApi.Modules;

/// <summary>
/// Utility class for loading assets from AssetBundles.
/// </summary>
public static class ContentLoader
{
    /// <summary>
    /// Loads an AssetBundle from the plugin's directory.
    /// </summary>
    /// <param name="pluginInfo">The BepInPlugin information for path resolution.</param>
    /// <param name="bundleName">The filename of the bundle to load.</param>
    /// <returns>The loaded AssetBundle.</returns>
    /// <exception cref="System.Exception">Thrown if the bundle cannot be found or loaded.</exception>
    public static AssetBundle LoadAssetBundle(PluginInfo pluginInfo, string bundleName)
    {
        string bundlePath = Path.Combine(Path.GetDirectoryName(pluginInfo.Location), bundleName);
        AssetBundle bundle = AssetBundle.LoadFromFile(bundlePath);
        if (bundle == null) throw new System.Exception($"Failed to load AssetBundle: {bundleName} at path: {bundlePath}");
        return bundle;
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

