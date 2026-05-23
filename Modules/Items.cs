using UnityEngine;
using Photon.Pun;
using Zorro.Core;
using System;
using System.Reflection;
using System.Collections.Generic;
using System.Linq;

namespace DbsContentApi;

/// <summary>
/// Describes a custom shop category registered via <see cref="Items.RegisterCustomCategory"/>.
/// </summary>
public class CustomShopItemCategory
{
    /// <summary>Internal category index assigned by the API (starts at 20).</summary>
    public byte index;

    /// <summary>Display name shown in the shop UI.</summary>
    public string name;
}

/// <summary>
/// Configuration for a custom item passed to <see cref="Items.RegisterItem"/>.
/// </summary>
public class ItemConfig
{
    /// <summary>Display name shown in the shop and inventory.</summary>
    public string displayName = "";

    /// <summary>
    /// Persistent identifier used for saves and networking.
    /// When <c>null</c>, defaults to <c>unknown_mod.{displayName}</c> (lowercase, spaces removed).
    /// Set an explicit mod-scoped ID for production mods.
    /// </summary>
    public string? persistentId = null;

    /// <summary>Shop price in in-game currency.</summary>
    public int price = 100;

    /// <summary>Shop category tab the item appears under.</summary>
    public ShopItemCategory category = ShopItemCategory.Gadgets;

    /// <summary>Whether the item can be purchased from the shop.</summary>
    public bool purchasable = true;

    /// <summary>Number of units sold per purchase.</summary>
    public int quantity = 1;

    /// <summary>Item type classification used by the game.</summary>
    public Item.ItemType itemType = Item.ItemType.Tool;

    /// <summary>Physics mass of the item prefab.</summary>
    public float mass = 0.5f;

    /// <summary>Multiplier applied to the item's ground collision size.</summary>
    public float groundSizeMultiplier = 1f;

    /// <summary>Multiplier applied to the item's ground collision mass.</summary>
    public float groundMassMultiplier = 1f;

    /// <summary>Whether the item can spawn in the world during rounds.</summary>
    public bool spawnable = true;

    /// <summary>Spawn rarity tier when the item appears as a world pickup.</summary>
    public RARITY toolSpawnRarity = RARITY.common;

    /// <summary>Budget cost when spawned as a tool pickup.</summary>
    public int toolBudgetCost = 1;

    /// <summary>General spawn budget cost.</summary>
    public int budgetCost = 0;

    /// <summary>Relative spawn rarity weight.</summary>
    public float rarity = 1f;

    /// <summary>Local position offset when the item is held in the player's hand.</summary>
    public Vector3 holdPos = new Vector3(0.3f, -0.3f, 0.7f);

    /// <summary>Local rotation offset when the item is held in the player's hand.</summary>
    public Vector3 holdRot = Vector3.zero;

    /// <summary>Whether to use <see cref="alternativeHoldPos"/> instead of <see cref="holdPos"/>.</summary>
    public bool useAlternativeHoldPos = false;

    /// <summary>Whether to use <see cref="alternativeHoldRot"/> instead of <see cref="holdRot"/>.</summary>
    public bool useAlternativeHoldRot = false;

    /// <summary>Alternative local hold position for special hold animations.</summary>
    public Vector3 alternativeHoldPos = new Vector3(0.3f, -0.3f, 0.7f);

    /// <summary>Alternative local hold rotation for special hold animations.</summary>
    public Vector3 alternativeHoldRot = Vector3.zero;

    /// <summary>
    /// Shop/inventory icon sprite. Assign before calling <see cref="Items.RegisterItem"/>
    /// (e.g. via <c>bundle.LoadAsset&lt;Sprite&gt;</c>).
    /// </summary>
    public Sprite? icon = null;

    /// <summary>
    /// Obsolete: load a <see cref="Sprite"/> and assign <see cref="icon"/> instead.
    /// </summary>
    [Obsolete("Assign config.icon yourself after loading from a bundle or other source. RegisterItem no longer loads icons from AssetBundles.")]
    public string? iconName = null;

    /// <summary>Custom impact sounds applied to the item's <see cref="PhysicsSound"/> component.</summary>
    public SFX_Instance[]? impactSounds = null;

    /// <summary>
    /// Impact sound types resolved automatically via <see cref="ImpactSoundScanner"/>.
    /// Used when <see cref="impactSounds"/> is not set.
    /// </summary>
    public ImpactSoundType[]? impactSoundTypes = null;

    /// <summary>Key/tooltip entries shown in the item description UI.</summary>
    public List<ItemKeyTooltip> tooltips = new();
}

/// <summary>
/// API for creating and registering custom items in Content Warning.
/// Provides simplified methods for item setup, configuration, and registration.
/// </summary>
public static class Items
{
    /// <summary>All custom shop categories registered by mods via <see cref="RegisterCustomCategory"/>.</summary>
    public static List<CustomShopItemCategory> customCategories = new List<CustomShopItemCategory>();

    /// <summary>
    /// Registers a custom shop category and returns it as a ShopItemCategory.
    /// </summary>
    /// <param name="categoryName">The name of the category.</param>
    /// <returns>The registered ShopItemCategory.</returns>
    public static ShopItemCategory RegisterCustomCategory(string categoryName)
    {
        byte index = (byte)(customCategories.Count + 20);
        customCategories.Add(new CustomShopItemCategory() { index = index, name = categoryName });
        return (ShopItemCategory)index;
    }

    /// <summary>
    /// Defers the registration of items until the shop and item database are ready.
    /// Always call <see cref="RegisterItem"/> from inside this callback.
    /// </summary>
    /// <param name="callback">The callback to execute for registration.</param>
    /// <example>
    /// <code>
    /// Items.DeferRegistration(() =>
    /// {
    ///     GameObject prefab = ContentLoader.LoadPrefabFromBundle(bundle, "MyItem.prefab");
    ///     Items.RegisterItem(prefab, new ItemConfig { displayName = "My Item", price = 50 });
    /// });
    /// </code>
    /// </example>
    public static void DeferRegistration(Action callback)
    {
        DbsContentApiPlugin.customItemsRegistrationCallbacks.Add(callback);
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
            ApiLog.Log("Added ItemInstance component");
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

        ApiLog.Log("Added HandGizmo with dummy child");
    }

    /// <summary>
    /// Registers an item with the game. This is the main entry point for item registration.
    /// The prefab may come from an AssetBundle, be built in code, or be authored in the Unity editor.
    /// </summary>
    /// <param name="prefab">The item GameObject prefab (already configured).</param>
    /// <param name="config">The configuration for the item.</param>
    /// <returns>The registered Item.</returns>
    public static Item RegisterItem(GameObject prefab, ItemConfig config)
    {
        if (prefab == null)
            throw new ArgumentNullException(nameof(prefab));

        ApiLog.Log($"Registering item: {config.displayName} (Prefab: {prefab.name})");

        SetupPrefab(prefab);
        ContentLoader.RegisterPrefabInPhotonPool(prefab);

        Item item = ScriptableObject.CreateInstance<Item>();

        // Setup sounds
        SFX_Instance[]? impactSounds = config.impactSounds;
        if (impactSounds == null && config.impactSoundTypes != null)
        {
            impactSounds = ImpactSoundScanner.GetImpactSounds(config.impactSoundTypes);
        }
        
        if (impactSounds != null && impactSounds.Length > 0)
        {
            SetupPhysicsSound(prefab, impactSounds);
        }

        if (config.icon != null)
        {
            item.icon = config.icon;
        }
        else if (!string.IsNullOrEmpty(config.iconName))
        {
            ApiLog.LogWarning(
                $"Item '{config.displayName}': config.iconName is obsolete. Load a Sprite and set config.icon before calling RegisterItem.");
        }

        // Setup basics
        item.displayName = config.displayName;
        item.itemObject = prefab;

        string pId = config.persistentId ?? ("unknown_mod." + item.displayName.ToLower().Replace(" ", ""));
        item.persistentID = pId;
        item.PersistentID = GuidHelper.ToDeterministicGuid(pId);
        item.name = pId;

        item.itemType = config.itemType;
        item.Category = config.category;

        item.mass = config.mass;
        item.holdPos = config.holdPos;
        item.holdRotation = config.holdRot;
        item.useAlternativeHoldingPos = config.useAlternativeHoldPos;
        item.useAlternativeHoldingRot = config.useAlternativeHoldRot;
        item.alternativeHoldPos = config.alternativeHoldPos;
        item.alternativeHoldRot = config.alternativeHoldRot;
        item.groundSizeMultiplier = config.groundSizeMultiplier;
        item.groundMassMultiplier = config.groundMassMultiplier;

        item.purchasable = config.purchasable;
        item.price = config.price;
        item.quantity = config.quantity;
        item.spawnable = config.spawnable;
        item.toolSpawnRarity = config.toolSpawnRarity;
        item.toolBudgetCost = config.toolBudgetCost;
        item.budgetCost = config.budgetCost;
        item.rarity = config.rarity;

        item.content = null;
        item.Tooltips = config.tooltips ?? new List<ItemKeyTooltip>();

        if (!CheckDuplicateItem(item))
        {
            AddItemToDatabase(item);
        }

        return item;
    }

    /// <summary>
    /// Sets up the icon for an item.
    /// </summary>
    /// <param name="bundle">The AssetBundle containing the icon.</param>
    /// <param name="item">The item to configure.</param>
    /// <param name="iconName">The name of the icon asset.</param>
    public static void SetupIcon(AssetBundle bundle, Item item, string iconName)
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
        var ps = prefab.GetComponent<PhysicsSound>();
        if (ps == null)
        {
            ps = prefab.AddComponent<PhysicsSound>();
        }
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
            ApiLog.LogError("Could not find template SFX_Instance");
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
            ApiLog.LogError("Could not find template SFX_Instance");
            return GetFallbackPhysicsSound(db);
        }

        SFX_Instance sfxInstance = ScriptableObject.Instantiate(templateSFX);
        sfxInstance.clips = customSounds;
        sfxInstance.settings.pitch = 1.0f;
        sfxInstance.settings.volume = 1.0f;
        return new SFX_Instance[] { sfxInstance };
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
    /// Finds an item in the database whose prefab (itemObject) has a component of type T.
    /// Used e.g. to get the GooBall item and steal its ticking AudioLoop for custom items.
    /// </summary>
    /// <typeparam name="T">Component type on the item's prefab (e.g. ItemGooBall).</typeparam>
    /// <returns>The first Item whose itemObject has T, or null.</returns>
    public static Item? GetItemByPrefabComponent<T>() where T : Component
    {
        ItemDatabase db = SingletonAsset<ItemDatabase>.Instance;
        FieldInfo objectsField = GetObjectsField(db);
        List<Item> currentItems = GetItemsFromField(objectsField, db);
        foreach (Item item in currentItems)
        {
            if (item?.itemObject != null && item.itemObject.GetComponent<T>() != null)
                return item;
        }
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

        item.id = currentItems.Count > 0 ? (byte)(currentItems.Max(i => i.id) + 1) : (byte)0;
        currentItems.Add(item);

        objectsField.SetValue(db, currentItems);
        ApiLog.Log($"Item '{item.displayName}' registered with ID: {item.id}");
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
            {
                existingItem.price = 0;
            }
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
    /// Finds an item by its ID in the ItemDatabase.
    /// </summary>
    /// <param name="id">The item ID to find.</param>
    /// <returns>The Item if found, null otherwise.</returns>
    public static Item? GetItemByID(byte id)
    {
        ItemDatabase db = SingletonAsset<ItemDatabase>.Instance;
        FieldInfo objectsField = GetObjectsField(db);
        List<Item> currentItems = GetItemsFromField(objectsField, db);
        return currentItems.FirstOrDefault(i => i != null && i.id == id);
    }
}
