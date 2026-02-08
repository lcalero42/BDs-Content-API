using UnityEngine;
using Photon.Pun;
using Zorro.Core;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace DbsContentApi.Modules;

public class CustomShopItemCategory
{
    public byte index;
    public string name;
}

/// <summary>
/// API for creating and registering custom items in Content Warning.
/// Provides simplified methods for item setup, configuration, and registration.
/// </summary>
public static class Items
{
    public static List<CustomShopItemCategory> customCategories = new List<CustomShopItemCategory>();
    /// <summary>
    /// Loads a prefab from the provided AssetBundle.
    /// </summary>
    /// <param name="bundle">The AssetBundle containing the prefab.</param>
    /// <param name="prefabName">The name of the prefab to load.</param>
    /// <returns>The loaded GameObject prefab.</returns>
    /// <exception cref="Exception">Thrown if bundle is null or prefab not found.</exception>
    public static GameObject LoadPrefab(AssetBundle bundle, string prefabName)
    {
        if (bundle == null)
            throw new Exception("AssetBundle is null");

        GameObject prefab = bundle.LoadAsset<GameObject>(prefabName);
        if (prefab == null)
            throw new Exception($"{prefabName} not found in AssetBundle");

        return prefab;
    }

    public static byte RegisterCustomCategory(string categoryName)
    {
        byte index = (byte)(customCategories.Count + 20);
        customCategories.Add(new CustomShopItemCategory() { index = index, name = categoryName });
        return index;
    }

    /// <summary>
    /// Sets up a prefab with required components and materials.
    /// </summary>
    /// <param name="prefab">The prefab to setup.</param>
    /// <param name="prefabName">The name of the prefab.</param>
    /// <param name="matName">Name of the material to apply to the prefab.</param>
    public static void SetupPrefab(GameObject prefab, string prefabName, GameMaterialType mat)
    {
        GameMaterials.ApplyMaterial(prefab, mat);

        EnsureComponents(prefab, prefabName);
    }

    /// <summary>
    /// Ensures required components are present on the prefab.
    /// </summary>
    /// <param name="prefab">The prefab to check.</param>
    /// <param name="prefabName">The name of the prefab.</param>
    private static void EnsureComponents(GameObject prefab, string prefabName)
    {
        // Add ItemInstance component if missing
        if (prefab.GetComponent<ItemInstance>() == null)
        {
            prefab.AddComponent<ItemInstance>();
            Debug.Log("Added ItemInstance component");
        }
        EnsureHandGizmo(prefab);
    }

    /// <summary>
    /// Ensures a HandGizmo component exists on the prefab.
    /// </summary>
    /// <param name="prefab">The prefab to check.</param>
    public static void EnsureHandGizmo(GameObject prefab)
    {
        if (prefab.GetComponentInChildren<HandGizmo>(true) != null) return;

        GameObject handGizmoObj = new GameObject("HandGizmo");
        handGizmoObj.transform.SetParent(prefab.transform);
        handGizmoObj.AddComponent<HandGizmo>();

        // Add dummy child for HandGizmo.Start()
        GameObject dummyChild = new GameObject("GizmoVisual");
        dummyChild.transform.SetParent(handGizmoObj.transform);
        dummyChild.transform.localPosition = Vector3.zero;
        dummyChild.transform.localRotation = Quaternion.identity;

        Debug.Log("Added HandGizmo with dummy child");
    }

    /// <summary>
    /// Registers a prefab in the Photon network pool.
    /// </summary>
    /// <param name="prefab">The prefab to register.</param>
    public static void RegisterPrefabInPool(GameObject prefab)
    {
        if (PhotonNetwork.PrefabPool is DefaultPool defaultPool)
        {
            if (!defaultPool.ResourceCache.ContainsKey(prefab.name))
            {
                defaultPool.ResourceCache.Add(prefab.name, prefab);
            }
        }
    }

    /// <summary>
    /// Creates and configures a new Item ScriptableObject.
    /// </summary>
    /// <param name="bundle">The AssetBundle containing assets.</param>
    /// <param name="prefabName">The name of the prefab.</param>
    /// <param name="prefab">The item GameObject prefab.</param>
    /// <param name="price">The price of the item.</param>
    /// <param name="category">The shop category.</param>
    /// <param name="iconName">The name of the icon asset.</param>
    /// <param name="soundEffectName">The name of the sound effect asset.</param>
    /// <param name="holdPos">The holding position of the item.</param>
    /// <param name="displayName">The display name of the item.</param>
    /// <returns>The configured Item.</returns>
    public static Item CreateItem(AssetBundle bundle, string prefabName, GameObject prefab, int price, ShopItemCategory category,
                                                        string iconName, string soundEffectName, Vector3 holdPos, string displayName)
    {
        ItemDatabase db = SingletonAsset<ItemDatabase>.Instance;
        Item item = ScriptableObject.CreateInstance<Item>();

        SetupPhysicsSound(bundle, prefab, db, soundEffectName);
        SetupIcon(bundle, prefab, item, iconName);
        SetupItemBasics(item, prefabName, prefab, price, category, holdPos, displayName);

        return item;
    }

    /// <summary>
    /// Sets up the icon for an item.
    /// </summary>
    /// <param name="bundle">The AssetBundle containing the icon.</param>
    /// <param name="prefab">The item prefab.</param>
    /// <param name="item">The item to configure.</param>
    /// <param name="iconName">The name of the icon asset.</param>
    public static void SetupIcon(AssetBundle bundle, GameObject prefab, Item item, string iconName)
    {
        Sprite icon = bundle.LoadAsset<Sprite>(iconName);
        if (icon != null)
            item.icon = icon;
    }

    /// <summary>
    /// Sets up physics sound for an item prefab.
    /// </summary>
    /// <param name="bundle">The AssetBundle containing sound assets.</param>
    /// <param name="prefab">The item prefab.</param>
    /// <param name="db">The ItemDatabase instance.</param>
    /// <param name="soundEffectName">The name of the sound effect asset.</param>
    public static void SetupPhysicsSound(AssetBundle bundle, GameObject prefab, ItemDatabase db, string soundEffectName)
    {
        var ps = prefab.AddComponent<PhysicsSound>();
        AudioClip customImpactSound = bundle.LoadAsset<AudioClip>(soundEffectName);

        if (customImpactSound == null)
        {
            Debug.LogWarning("Custom impact sound not found, using fallback");
            ps.impactSounds = GetFallbackPhysicsSound(db);
        }
        else
        {
            ps.impactSounds = CreateCustomImpactSound(customImpactSound, db);
            Debug.Log("Custom impact sound loaded successfully!");
        }
    }

    /// <summary>
    /// Gets fallback physics sound from existing items.
    /// </summary>
    /// <param name="db">The ItemDatabase instance.</param>
    /// <returns>Array of SFX_Instance for fallback sounds.</returns>
    public static SFX_Instance[] GetFallbackPhysicsSound(ItemDatabase db)
    {
        var objectsField = GetObjectsField(db);
        var currentItems = GetItemsFromField(objectsField, db);
        return currentItems[0].itemObject.GetComponent<PhysicsSound>().impactSounds;
    }

    /// <summary>
    /// Creates a custom impact sound instance.
    /// </summary>
    /// <param name="customSound">The custom AudioClip.</param>
    /// <param name="db">The ItemDatabase instance.</param>
    /// <returns>Array containing the custom SFX_Instance.</returns>
    public static SFX_Instance[] CreateCustomImpactSound(AudioClip customSound, ItemDatabase db)
    {
        var objectsField = GetObjectsField(db);
        var currentItems = GetItemsFromField(objectsField, db);

        SFX_Instance templateSFX = currentItems[0].itemObject.GetComponent<PhysicsSound>()?.impactSounds?[0];
        if (templateSFX == null)
        {
            Debug.LogError("Could not find template SFX_Instance");
            return GetFallbackPhysicsSound(db);
        }

        SFX_Instance sfxInstance = ScriptableObject.Instantiate(templateSFX);
        sfxInstance.clips = new AudioClip[] { customSound };
        sfxInstance.settings.pitch = 1.0f;
        sfxInstance.settings.volume = 1.0f;
        return new SFX_Instance[] { sfxInstance };
    }

    /// <summary>
    /// Sets up basic item properties.
    /// </summary>
    /// <param name="item">The item to configure.</param>
    /// <param name="prefabName">The name of the prefab.</param>
    /// <param name="prefab">The item GameObject.</param>
    /// <param name="price">The price of the item.</param>
    /// <param name="category">The shop category.</param>
    /// <param name="holdPos">The holding position of the item.</param>
    /// <param name="displayName">The display name of the item.</param>
    public static void SetupItemBasics(Item item, string prefabName, GameObject prefab, int price, ShopItemCategory category, Vector3 holdPos, string displayName)
    {
        item.displayName = displayName;
        item.itemObject = prefab;
        item.persistentID = "unlistedentities." + item.displayName.ToLower();
        item.name = "unlistedentities." + item.displayName.ToLower();

        item.itemType = Item.ItemType.Tool;
        item.Category = category;

        item.mass = 0.5f;
        item.holdPos = holdPos;
        item.holdRotation = Vector3.zero;
        item.useAlternativeHoldingPos = false;
        item.useAlternativeHoldingRot = false;
        item.groundSizeMultiplier = 1f;
        item.groundMassMultiplier = 1f;

        item.purchasable = true;
        item.price = price;
        item.quantity = 1;
        item.spawnable = true;
        item.toolSpawnRarity = RARITY.common;
        item.toolBudgetCost = 1;
        item.budgetCost = 0;
        item.rarity = 1f;

        item.content = null;
        item.Tooltips = new List<ItemKeyTooltip>();
    }

    /// <summary>
    /// Gets the Objects field from ItemDatabase using reflection.
    /// </summary>
    /// <param name="db">The ItemDatabase instance.</param>
    /// <returns>The FieldInfo for the Objects field.</returns>
    /// <exception cref="Exception">Thrown if Objects field cannot be found.</exception>
    public static FieldInfo GetObjectsField(ItemDatabase db)
    {
        var objectsField = db.GetType().GetField("Objects", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        if (objectsField == null)
            objectsField = db.GetType().GetField("objects", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

        if (objectsField == null)
            throw new Exception("Could not find Objects field in ItemDatabase");

        return objectsField;
    }

    /// <summary>
    /// Gets the list of items from the Objects field.
    /// </summary>
    /// <param name="objectsField">The FieldInfo for the Objects field.</param>
    /// <param name="db">The ItemDatabase instance.</param>
    /// <returns>List of items.</returns>
    /// <exception cref="Exception">Thrown if field type is unexpected.</exception>
    public static List<Item> GetItemsFromField(FieldInfo objectsField, ItemDatabase db)
    {
        var objectsValue = objectsField.GetValue(db);

        if (objectsValue is List<Item> itemList)
            return itemList;
        else if (objectsValue is Item[] itemArray)
            return new List<Item>(itemArray);

        throw new Exception($"Objects field is type {objectsValue?.GetType()?.Name ?? "null"}, expected List<Item> or Item[]");
    }

    /// <summary>
    /// Copies icon from a template item.
    /// </summary>
    /// <param name="currentItems">The list of existing items.</param>
    /// <returns>Template item with icon or null.</returns>
    public static Item CopyIconFromTemplate(List<Item> currentItems)
    {
        foreach (Item existingItem in currentItems)
        {
            if (existingItem.icon != null && existingItem.purchasable)
            {
                Debug.Log($"Found template item with icon: {existingItem.displayName}");
                return existingItem;
            }
        }
        Debug.LogWarning("No template item found with icon!");
        return null;
    }

    /// <summary>
    /// Adds an item to the ItemDatabase.
    /// </summary>
    /// <param name="item">The item to add.</param>
    public static void AddItemToDatabase(Item item)
    {
        ItemDatabase db = SingletonAsset<ItemDatabase>.Instance;
        var objectsField = GetObjectsField(db);
        var currentItems = GetItemsFromField(objectsField, db);

        Item templateItem = CopyIconFromTemplate(currentItems);

        item.id = currentItems.Count > 0 ? (byte)(currentItems.Max(i => i.id) + 1) : (byte)0;
        currentItems.Add(item);

        objectsField.SetValue(db, currentItems);
        Debug.Log($"Item '{item.displayName}' registered with ID: {item.id}");
    }

    /// <summary>
    /// Sets all items in the database to free (price = 0).
    /// </summary>
    public static void SetAllItemsFree()
    {
        ItemDatabase db = SingletonAsset<ItemDatabase>.Instance;
        var objectsField = GetObjectsField(db);
        var currentItems = GetItemsFromField(objectsField, db);

        foreach (Item existingItem in currentItems)
        {
            if (existingItem != null)
                existingItem.price = 0;
        }

        objectsField.SetValue(db, currentItems);
    }

    /// <summary>
    /// Checks if an item with the same name already exists in the database.
    /// </summary>
    /// <param name="item">The item to check.</param>
    /// <returns>True if a duplicate exists, false otherwise.</returns>
    public static bool CheckDuplicateItem(Item item)
    {
        ItemDatabase db = SingletonAsset<ItemDatabase>.Instance;
        var objectsField = GetObjectsField(db);
        var currentItems = GetItemsFromField(objectsField, db);

        if (currentItems.Count == 0)
            return false;

        for (int i = 0; i < currentItems.Count; i++)
        {
            if (currentItems[i].displayName == item.displayName)
                return true;
        }
        return false;
    }

    /// <summary>
    /// Registers an item with the game. This is the main entry point for item registration.
    /// Handles loading, setup, registration, and database addition.
    /// </summary>
    /// <param name="bundle">The AssetBundle containing the item assets.</param>
    /// <param name="prefabName">The name of the item prefab.</param>
    /// <param name="displayName">The display name of the item.</param>
    /// <param name="price">The price of the item.</param>
    /// <param name="category">The shop category.</param>
    /// <param name="iconName">The name of the icon asset.</param>
    /// <param name="soundEffectName">The name of the sound effect asset.</param>
    /// <param name="matName">Name of the material to apply (default: M_Metal).</param>
    /// <param name="holdPos">Optional holding position of the item (default: (0.3, -0.3, 0.7)).</param>
    /// <param name="customBehaviourSetup">Optional callback to add custom behaviours to the prefab.</param>
	public static Item RegisterItem(
        AssetBundle bundle,
        string prefabName,
        string displayName,
        int price,
        ShopItemCategory category,
        string iconName,
        string soundEffectName,
        GameMaterialType mat,
        Vector3? holdPos = null,
        Action<GameObject, string> customBehaviourSetup = null)
    {
        Debug.Log($"Registering item: {prefabName}");

        GameObject prefab = LoadPrefab(bundle, prefabName);
        SetupPrefab(prefab, prefabName, mat);

        // Allow custom behaviour setup
        customBehaviourSetup?.Invoke(prefab, prefabName);

        RegisterPrefabInPool(prefab);

        // Use default value if not provided
        Vector3 actualHoldPos = holdPos ?? new Vector3(0.3f, -0.3f, 0.7f);

        Item item = CreateItem(bundle, prefabName, prefab, price, category, iconName, soundEffectName, actualHoldPos, displayName);

        if (!CheckDuplicateItem(item))
            AddItemToDatabase(item);

        return item;
    }
}