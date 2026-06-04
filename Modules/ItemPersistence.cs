using System;
using System.Collections.Generic;
using System.Linq;

namespace DbsContentApi;

/// <summary>
/// Lets mods append extra <see cref="SavedInventoryItem"/> entries when the game serializes inventory for a save.
/// Loaded saves spawn these via the vanilla inventory spawn path (pickups at spawn points).
/// </summary>
public static class ItemPersistence
{
    private static readonly List<Func<IEnumerable<Item?>>> InventoryItemProviders = new();

    /// <summary>
    /// Registers a callback invoked during <c>SaveLoadHandler.SerializeInventoryItems</c>.
    /// Return <see cref="Item"/> definitions to persist as inventory pickups on load (not equipped state).
    /// </summary>
    public static void RegisterInventoryItemProvider(Func<IEnumerable<Item?>> provider)
    {
        if (provider == null)
            throw new ArgumentNullException(nameof(provider));

        InventoryItemProviders.Add(provider);
    }

    internal static SavedInventoryItem[] AppendProviderItems(SavedInventoryItem[] vanillaResult)
    {
        if (InventoryItemProviders.Count == 0)
            return vanillaResult;

        List<SavedInventoryItem> combined = vanillaResult?.ToList() ?? new List<SavedInventoryItem>();
        int addedCount = 0;

        foreach (Func<IEnumerable<Item?>> provider in InventoryItemProviders)
        {
            try
            {
                foreach (Item? item in provider())
                {
                    if (item == null)
                        continue;

                    combined.Add(new SavedInventoryItem(item));
                    addedCount++;
                }
            }
            catch (Exception e)
            {
                ApiLog.LogError($"[ItemPersistence] Provider failed: {e}");
            }
        }

        if (addedCount > 0)
            ApiLog.Log($"[ItemPersistence] Added {addedCount} extra inventory item(s) to save.");

        return combined.ToArray();
    }
}
