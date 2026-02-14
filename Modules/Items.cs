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
    public static byte RegisterCustomCategory(string categoryName)
    {
        byte index = (byte)(customCategories.Count + 20);
        customCategories.Add(new CustomShopItemCategory() { index = index, name = categoryName });
        return index;
    }

    /// <summary>
    /// Sets up a prefab with required components.
    /// </summary>
    /// <param name="prefab">The prefab to setup.</param>
    public static void SetupPrefab(GameObject prefab)
    {
        EnsureComponents(prefab);
    }

    /// <summary>
    /// Ensures required components are present on the prefab.
    /// </summary>
    /// <param name="prefab">The prefab to check.</param>
    private static void EnsureComponents(GameObject prefab)
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
    /// <param name="prefab">The item GameObject prefab.</param>
    /// <param name="price">The price of the item.</param>
    /// <param name="category">The shop category.</param>
    /// <param name="iconName">The name of the icon asset.</param>
    /// <param name="impactSounds">The impact sounds for the item's PhysicsSound component.</param>
    /// <param name="holdPos">The holding position of the item.</param>
    /// <param name="displayName">The display name of the item.</param>
    /// <returns>The configured Item.</returns>
    public static Item CreateItem(AssetBundle bundle, GameObject prefab, int price, ShopItemCategory category,
                                                        string iconName, SFX_Instance[] impactSounds, Vector3 holdPos, Vector3 holdRot, string displayName)
    {
        Item item = ScriptableObject.CreateInstance<Item>();

        SetupPhysicsSound(prefab, impactSounds);
        SetupIcon(bundle, prefab, item, iconName);
        SetupItemBasics(item, prefab, price, category, holdPos, holdRot, displayName);

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
    /// <param name="prefab">The item prefab.</param>
    /// <param name="impactSounds">The impact sounds to apply.</param>
    public static void SetupPhysicsSound(GameObject prefab, SFX_Instance[] impactSounds)
    {
        var ps = prefab.AddComponent<PhysicsSound>();
        ps.impactSounds = impactSounds;
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
    /// Creates a custom impact sound instance from an AudioClip.
    /// </summary>
    /// <param name="customSound">The custom AudioClip.</param>
    /// <returns>Array containing the custom SFX_Instance.</returns>
    public static SFX_Instance[] CreateSFXInstanceFromClip(AudioClip customSound)
    {
        ItemDatabase db = SingletonAsset<ItemDatabase>.Instance;
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
    /// Creates a custom impact sound instance from multiple AudioClips.
    /// </summary>
    /// <param name="customSounds">The custom AudioClips.</param>
    /// <returns>Array containing the custom SFX_Instance.</returns>
    public static SFX_Instance[] CreateImpactSoundFromClips(params AudioClip[] customSounds)
    {
        ItemDatabase db = SingletonAsset<ItemDatabase>.Instance;
        var objectsField = GetObjectsField(db);
        var currentItems = GetItemsFromField(objectsField, db);

        SFX_Instance templateSFX = currentItems[0].itemObject.GetComponent<PhysicsSound>()?.impactSounds?[0];
        if (templateSFX == null)
        {
            Debug.LogError("Could not find template SFX_Instance");
            return GetFallbackPhysicsSound(db);
        }

        SFX_Instance sfxInstance = ScriptableObject.Instantiate(templateSFX);
        sfxInstance.clips = customSounds;
        sfxInstance.settings.pitch = 1.0f;
        sfxInstance.settings.volume = 1.0f;
        return new SFX_Instance[] { sfxInstance };
    }

    /// <summary>
    /// Sets up basic item properties.
    /// </summary>
    /// <param name="item">The item to configure.</param>
    /// <param name="prefab">The item GameObject.</param>
    /// <param name="price">The price of the item.</param>
    /// <param name="category">The shop category.</param>
    /// <param name="holdPos">The holding position of the item.</param>
    /// <param name="displayName">The display name of the item.</param>
    public static void SetupItemBasics(Item item, GameObject prefab, int price, ShopItemCategory category, Vector3 holdPos, Vector3 holdRot, string displayName)
    {
        item.displayName = displayName;
        item.itemObject = prefab;
        item.persistentID = "unlistedentities." + item.displayName.ToLower();
        item.name = "unlistedentities." + item.displayName.ToLower();

        item.itemType = Item.ItemType.Tool;
        item.Category = category;

        item.mass = 0.5f;
        item.holdPos = holdPos;
        item.holdRotation = holdRot;
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
    /// <param name="prefab">The item GameObject prefab.</param>
    /// <param name="displayName">The display name of the item.</param>
    /// <param name="price">The price of the item.</param>
    /// <param name="category">The shop category.</param>
    /// <param name="iconName">The name of the icon asset.</param>
    /// <param name="impactSounds">The impact sounds for the item's PhysicsSound component.</param>
    /// <param name="holdPos">Optional holding position of the item (default: (0.3, -0.3, 0.7)).</param>
	public static Item RegisterItem(
        AssetBundle bundle,
        GameObject prefab,
        string displayName,
        int price,
        ShopItemCategory category,
        string iconName,
        SFX_Instance[] impactSounds,
        Vector3? holdPos = null,
        Vector3? holdRot = null
        )
    {
        Debug.Log($"Registering item: {prefab.name}");

        SetupPrefab(prefab);

        RegisterPrefabInPool(prefab);

        // Use default value if not provided
        Vector3 actualHoldPos = holdPos ?? new Vector3(0.3f, -0.3f, 0.7f);
        Vector3 actualHoldRot = holdRot ?? Vector3.zero;
        Item item = CreateItem(bundle, prefab, price, category, iconName, impactSounds, actualHoldPos, actualHoldRot, displayName);

        if (!CheckDuplicateItem(item))
            AddItemToDatabase(item);

        return item;
    }
}