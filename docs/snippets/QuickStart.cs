using System.Reflection;
using DbsContentApi;
using UnityEngine;

namespace DocsSnippets;

/// <summary>
/// DocFX snippet source — patterns mirror CW_SDK/Modules/CustomContent.cs.
/// </summary>
public static class QuickStart
{
    #region PluginEntry

    // Called from your [ContentWarningPlugin] constructor:
    public static void PluginEntry()
    {
        CustomContentInit();
    }

    #endregion

    #region LoadBundle

    public static void CustomContentInit()
    {
        AssetBundle? bundle = ContentLoader.LoadAssetBundle(
            Assembly.GetExecutingAssembly(), "my_mod");

        if (bundle == null)
        {
            Debug.LogError("Bundle not found beside mod DLL.");
            return;
        }

        RegisterItems(bundle);
    }

    #endregion

    #region RegisterItem

    public static void RegisterItems(AssetBundle bundle)
    {
        Items.DeferRegistration(() =>
        {
            GameObject prefab = ContentLoader.LoadPrefabFromBundle(bundle, "MyItem.prefab");
            ApplyMaterial(prefab);
            RegisterMyItem(bundle, prefab);
        });
    }

    public static void RegisterMyItem(AssetBundle bundle, GameObject prefab)
    {
        Items.RegisterItem(prefab, new ItemConfig
        {
            displayName = "Hello World Item",
            price = 50,
            icon = bundle.LoadAsset<Sprite>("my_icon"),
            holdPos = new Vector3(0.3f, -0.3f, 0.7f),
        });
    }

    #endregion

    #region ApplyMaterial

    public static void ApplyMaterial(GameObject prefab)
    {
        GameMaterials.ApplyMaterial(prefab, GameMaterial.M_Metal, deepApply: true);
    }

    #endregion
}
