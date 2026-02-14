using System.Collections.Generic;
using UnityEngine;
using Zorro.Core;

namespace DbsContentApi.Modules;

public enum ImpactSoundType
{
    None,
    /// <summary>
    /// Bouncy plastic object
    /// </summary>
    PlasticBounce1,
    PlasticBounce2,
    /// <summary>
    /// For small plastic or cardboard containers that contain smaller objects inside of it
    /// </summary>
    PlasticBounce3,
    /// <summary>
    /// For small plastic or cardboard containers that contain smaller objects inside of it
    /// </summary>
    PlasticBounce4,
    /// <summary>
    /// For brittle platistic/metal electronic objects
    /// </summary>
    PlasticBounce5,
    /// <summary>
    /// Impactful metal/plastic sound, high pitched, like dropping a large metal sheet on the ground
    /// </summary>
    PlasticBounce6,
    /// <summary>
    /// Like dropping shoes on the ground
    /// </summary>
    BombBounce1,
    /// <summary>
    /// Neutral impact sound for soft objects
    /// </summary>
    BombBounce2,
    CDBounce1,
    CDBounce2,
    /// <summary>
    /// For large and heavy metal containers
    /// </summary>
    ContainerBounce1,
    /// <summary>
    /// For large and heavy metal containers
    /// </summary>
    ContainerBounce2,
    ShroomBounce1,
    /// <summary>
    /// Can bounce sound, for small and brittle metal containers
    /// </summary>
    CanBounce,
    /// <summary>
    /// Can bounce sound, for small and brittle metal containers
    /// </summary>
    CanBounce1,
    /// <summary>
    /// Greasy sound
    /// </summary>
    Burger1,
    /// <summary>
    /// Greasy sound
    /// </summary>
    Burger2
}

/// <summary>
/// Utility to scan items in the ItemDatabase for PhysicsSound components and extract impactSounds.
/// </summary>
public class ImpactSoundScanner
{
    public static Dictionary<string, SFX_Instance> FoundImpactSounds = new Dictionary<string, SFX_Instance>();

    /// <summary>
    /// Scans all items in the ItemDatabase for PhysicsSound components and logs/caches impactSounds.
    /// </summary>
    public static void ScanImpactSounds()
    {
        Debug.Log("[ImpactSoundScanner] Starting scan via ItemDatabase...");

        ItemDatabase db = SingletonAsset<ItemDatabase>.Instance;
        if (db == null)
        {
            Debug.LogError("[ImpactSoundScanner] ItemDatabase instance not found!");
            return;
        }

        // Using the helper methods from Items.cs to safely access the items list
        var objectsField = Items.GetObjectsField(db);
        var items = Items.GetItemsFromField(objectsField, db);

        if (items == null || items.Count == 0)
        {
            Debug.LogWarning("[ImpactSoundScanner] No items found in ItemDatabase.");
            return;
        }

        foreach (Item item in items)
        {
            if (item == null) continue;

            string itemName = item.displayName ?? item.name ?? "Unknown Item";

            if (item.itemObject == null)
            {
                Debug.Log($"[ImpactSoundScanner] Skipping item '{itemName}': No itemObject (prefab) assigned.");
                continue;
            }

            PhysicsSound[] physicsSounds = item.itemObject.GetComponentsInChildren<PhysicsSound>(true);
            if (physicsSounds.Length == 0)
            {
                // Debug.Log($"[ImpactSoundScanner] Item '{itemName}' has no PhysicsSound components.");
                continue;
            }

            Debug.Log($"[ImpactSoundScanner] Checking item '{itemName}' ({physicsSounds.Length} PhysicsSound components)...");

            foreach (var ps in physicsSounds)
            {
                if (ps.impactSounds == null) continue;

                foreach (var sfx in ps.impactSounds)
                {
                    if (sfx == null) continue;

                    string sfxName = sfx.name;
                    if (!FoundImpactSounds.ContainsKey(sfxName))
                    {
                        FoundImpactSounds.Add(sfxName, sfx);
                        Debug.Log($"[ImpactSoundScanner] >>> NEW SOUND FOUND: '{sfxName}' (Source: {itemName})");
                    }
                    else
                    {
                        // Optional: log duplicates if you want to see where else they appear
                        // Debug.Log($"[ImpactSoundScanner] Duplicate sound '{sfxName}' found on '{itemName}' (already cached)");
                    }
                }
            }
        }

        Debug.Log($"[ImpactSoundScanner] Scan complete: {FoundImpactSounds.Count} unique impact sounds cached.");
    }

    // --- Association Logic (similar to GameMaterials.cs) ---

    public static Dictionary<ImpactSoundType, SFX_Instance> ImpactSounds = new Dictionary<ImpactSoundType, SFX_Instance>();

    /// <summary>
    /// Map of [Item Display Name] -> [SFX Name] -> [ImpactSoundType]
    /// We use Display Name as the primary key for the item in the database.
    /// </summary>
    private static readonly Dictionary<string, Dictionary<string, ImpactSoundType>> ItemExtractionMap = new Dictionary<string, Dictionary<string, ImpactSoundType>>
    {
        { "Boom Mic", new Dictionary<string, ImpactSoundType> {
            { "SFX Plastic Bounce 3", ImpactSoundType.PlasticBounce3 },
            { "SFX Plastic Bounce 4", ImpactSoundType.PlasticBounce4 }
        }},
        { "Camera", new Dictionary<string, ImpactSoundType> {
            { "SFX Plastic Bounce 5", ImpactSoundType.PlasticBounce5 }
        }},
        { "Defibrilator", new Dictionary<string, ImpactSoundType> {
            { "SFX Bomb Bounce 1", ImpactSoundType.BombBounce1 },
            { "SFX Bomb Bounce 2", ImpactSoundType.BombBounce2 }
        }},
        { "Disc", new Dictionary<string, ImpactSoundType> {
            { "SFX CD Bounce 1", ImpactSoundType.CDBounce1 },
            { "SFX CD Bounce 2", ImpactSoundType.CDBounce2 }
        }},
        { "Goo Ball", new Dictionary<string, ImpactSoundType> {
            { "SFX Plastic Bounce 6", ImpactSoundType.PlasticBounce6 }
        }},
        { "FredGull", new Dictionary<string, ImpactSoundType> {
            { "SFX Can Bounce", ImpactSoundType.CanBounce },
            { "SFX Can Bounce 1", ImpactSoundType.CanBounce1 }
        }},
        { "SirMonsterBurger", new Dictionary<string, ImpactSoundType> {
            { "SFX Burger 1", ImpactSoundType.Burger1 },
            { "SFX Burger 2", ImpactSoundType.Burger2 }
        }},
        // Items with empty display names (extracted from logs)
        { "", new Dictionary<string, ImpactSoundType> {
            { "SFX Plastic Bounce 1", ImpactSoundType.PlasticBounce1 },
            { "SFX Plastic Bounce 2", ImpactSoundType.PlasticBounce2 },
            { "SFX Container Bounce 1", ImpactSoundType.ContainerBounce1 },
            { "SFX Container Bounce 2", ImpactSoundType.ContainerBounce2 },
            { "SFX Shroom Bounce 1", ImpactSoundType.ShroomBounce1 }
        }}
    };

    public static void InitImpactSounds()
    {
        Debug.Log("[ImpactSoundScanner] Initializing Impact Sounds association...");
        ItemDatabase db = SingletonAsset<ItemDatabase>.Instance;
        if (db == null)
        {
            Debug.LogError("[ImpactSoundScanner] Failed to initialize: ItemDatabase is null!");
            return;
        }

        var objectsField = Items.GetObjectsField(db);
        var items = Items.GetItemsFromField(objectsField, db);

        int itemsProcessed = 0;
        int soundsAssociated = 0;

        foreach (Item item in items)
        {
            if (item == null || item.itemObject == null) continue;

            string itemName = item.displayName ?? "";

            if (ItemExtractionMap.TryGetValue(itemName, out var targetSounds))
            {
                itemsProcessed++;
                PhysicsSound[] physicsSounds = item.itemObject.GetComponentsInChildren<PhysicsSound>(true);
                foreach (var ps in physicsSounds)
                {
                    if (ps.impactSounds == null) continue;
                    foreach (var sfx in ps.impactSounds)
                    {
                        if (sfx == null) continue;
                        if (targetSounds.TryGetValue(sfx.name, out ImpactSoundType type))
                        {
                            if (!ImpactSounds.ContainsKey(type))
                            {
                                ImpactSounds.Add(type, sfx);
                                soundsAssociated++;
                                Debug.Log($"[ImpactSoundScanner] Associated '{sfx.name}' with ImpactSoundType.{type} (from item '{itemName}')");
                            }
                        }
                    }
                }
            }
        }

        Debug.Log($"[ImpactSoundScanner] Association phase complete. Processed {itemsProcessed} mapped items, associated {soundsAssociated} unique sounds.");
        ValidateExtraction();
    }

    /// <summary>
    /// Checks if all defined ImpactSoundTypes (except None) were successfully extracted.
    /// </summary>
    private static void ValidateExtraction()
    {
        Debug.Log("[ImpactSoundScanner] Starting extraction validation...");
        int missingCount = 0;
        int totalTypes = 0;

        foreach (ImpactSoundType type in System.Enum.GetValues(typeof(ImpactSoundType)))
        {
            if (type == ImpactSoundType.None) continue;
            totalTypes++;

            if (!ImpactSounds.ContainsKey(type))
            {
                Debug.LogWarning($"[ImpactSoundScanner] VALIDATION FAILURE: Missing sound for type '{type}'");
                missingCount++;
            }
        }

        if (missingCount == 0)
        {
            Debug.Log($"[ImpactSoundScanner] VALIDATION SUCCESS: All {totalTypes} impact sound types are correctly associated.");
        }
        else
        {
            Debug.LogError($"[ImpactSoundScanner] VALIDATION FAILED: {missingCount} / {totalTypes} sounds were not found. Check ItemExtractionMap and logs above.");
        }
    }

    /// <summary>
    /// Gets a copy of a cached impact sound by its type.
    /// Returns a new instance so modifications do not affect other items.
    /// </summary>
    /// <param name="type">The impact sound type to retrieve.</param>
    /// <returns>A copy of the SFX_Instance if found; otherwise null.</returns>
    public static SFX_Instance GetImpactSound(ImpactSoundType type)
    {
        if (!ImpactSounds.TryGetValue(type, out SFX_Instance sfx) || sfx == null)
            return null!;
        return Object.Instantiate(sfx);
    }

    /// <summary>
    /// Gets an array of impact sounds from multiple types.
    /// </summary>
    /// <param name="types">The impact sound types to retrieve.</param>
    /// <returns>Array of SFX_Instance objects.</returns>
    public static SFX_Instance[] GetImpactSounds(params ImpactSoundType[] types)
    {
        List<SFX_Instance> sounds = new List<SFX_Instance>();
        foreach (var type in types)
        {
            var sound = GetImpactSound(type);
            if (sound != null) sounds.Add(sound);
        }
        return sounds.ToArray();
    }
}
