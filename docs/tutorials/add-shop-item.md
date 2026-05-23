# Add a shop item

Step-by-step recipe for a purchasable custom item. Based on patterns in `CW_SDK` and `UnlistedEntities`.

**API reference:** [Items](xref:DbsContentApi.Items) · [ItemConfig](xref:DbsContentApi.ItemConfig)

## 1. Create a shop category (optional)

Vanilla tabs (`ShopItemCategory.Gadgets`, etc.) work out of the box. For a custom tab:

[!code-csharp[Custom category](../snippets/TutorialSnippets.cs?name=ShopItem&highlight=2-3)]

## 2. Defer registration

[!code-csharp[Defer](../snippets/TutorialSnippets.cs?name=ShopItem&highlight=5-6)]

> [!WARNING]
> Skipping `DeferRegistration` is the most common reason custom items never appear in the shop.

## 3. Prepare the prefab

Inside the deferred callback:

1. Load the prefab from your bundle
2. Add a behaviour component for use logic (`BasicUsableItem`, or your own `MonoBehaviour`)
3. Fix materials (see [Fix materials](fix-materials.md))
4. Call `Items.RegisterItem`

[!code-csharp[Register](../snippets/TutorialSnippets.cs?name=ShopItem&highlight=8-28)]

## 4. `ItemConfig` fields you'll use most

| Field | Purpose |
|-------|---------|
| `displayName` | Shop and inventory label |
| `price` | Shop cost (overridden when `SetAllItemsFree(true)`) |
| `category` | Vanilla or custom tab |
| `icon` | Load `Sprite` from bundle **before** `RegisterItem` |
| `holdPos` / `holdRot` | Hand attachment |
| `impactSoundTypes` | Auto-resolved via `ImpactSoundScanner` |
| `persistentId` | Optional stable save ID; defaults to `unknown_mod.{displayName}` if omitted — set your own mod-scoped ID in production |

## 5. Store the returned `Item` if needed

```csharp
Item? myItem = Items.RegisterItem(prefab, config);
```

`UnlistedEntities` keeps references like `JumpingBootsItem` for cross-system lookups.

## Advanced: reuse vanilla prefabs

Clone vanilla projectiles or explosions instead of authoring from scratch:

```csharp
Item? bomb = Items.GetItemByPrefabComponent<BombItem>();
GameObject explosion = Object.Instantiate(bomb!.itemObject.GetComponent<BombItem>()!.explosion);
Object.DontDestroyOnLoad(explosion);
// customize, wire into your item behaviour
```

See `CW_SDK/Modules/CustomContent.cs` (`GetDogLaserProjectileSource`) for a full gun example.

## Related

- [Quick start](../quick-start.md)
- [Registration lifecycle](../concepts/lifecycle.md)
- [Fix materials](fix-materials.md)
