# Fix materials

Unity AssetBundle shaders usually render pink or flat in Content Warning. `GameMaterials` swaps in working in-game materials at runtime.

**API reference:** [GameMaterials](xref:DbsContentApi.GameMaterials)

## When materials become available

DbsContentApi calls `GameMaterials.InitMaterials()` when the **shop screen loads** (`ShopViewScreen.Awake`), not at plugin startup.

That timing is intentional: scanning `Resources` too early misses many materials that the game has not loaded yet. All clients visit the shop during a normal session, so every player caches the same material set before rounds where custom content appears.

> [!IMPORTANT]
> Monster materials registered via `Mobs.RegisterMonster(material: ...)` use `ApplyOnLoad` and are applied when the shop opens. Custom items registered inside `Items.DeferRegistration` run in the same shop hook, so `ApplyMaterial` works immediately there.

## Quick fix: whole prefab

[!code-csharp[Apply material](../snippets/QuickStart.cs?name=ApplyMaterial)]

Use `ApplyMaterial` inside your deferred item callback (shop is already open). For monsters, pass `material:` to `Mobs.RegisterMonster` — the SDK queues `ApplyOnLoad` until `InitMaterials` runs.

## Per-child assignment

When only some meshes need fixing:

[!code-csharp[Batch materials](../snippets/TutorialSnippets.cs?name=Materials)]

Paths are transform paths from the prefab root (`"Item/Mesh"`, `"Item/boxy/FrontTexture"`, etc.).

## Per-renderer submesh indices

`RegionsExpanded` assigns different materials per submesh:

```csharp
GameMaterials.SetMaterialAt(renderer, 0, DescriptiveMaterial.BROWN_1);
GameMaterials.SetMaterialAt(renderer, 1, DescriptiveMaterial.YELLOW_1);
```

## Custom textures

Clone a base game material and swap the albedo/variation texture:

```csharp
Texture2D tex = bundle.LoadAsset<Texture2D>("FrontTexBoxRadar");
Material mat = GameMaterials.CloneMaterial(GameMaterial.M_ThePlan_1, tex, "M_FrontTextBoxRadar");
renderer.material = mat;
```

## `GameMaterial` vs `DescriptiveMaterial`

| Enum | Use when |
|------|----------|
| `GameMaterial` | You want a specific in-game material asset (`M_Metal`, `M_Monster`, `M_Apple_1`, …) |
| `DescriptiveMaterial` | You want a semantic color preset (`WHITE_1`, `BROWN_1`, …) mapped internally |

Browse both enums in the API reference — materials are cached the first time the shop opens.

## ApplyOnLoad vs ApplyMaterial

| Method | When to use |
|--------|-------------|
| `ApplyMaterial` | Immediate swap — call from deferred item registration or any code that runs after the shop has opened |
| `ApplyOnLoad` | Queue a swap until `InitMaterials` completes — used internally when monsters register at plugin load |

Do not call `InitMaterials()` yourself unless you have a specific reason; the shop patch owns that lifecycle point.

> [!TIP]
> If a prefab still looks wrong, inspect child paths in Unity and use `GameMaterials.Batch` rather than recursive apply on the root.

## Related

- [Add a shop item](add-shop-item.md)
- [Add a monster](add-monster.md)
- [Registration lifecycle](../concepts/lifecycle.md)
