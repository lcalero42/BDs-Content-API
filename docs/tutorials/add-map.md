# Add a custom map

Ship a playable scene from an AssetBundle and register it for round selection.

**API reference:** [Maps](xref:DbsContentApi.Maps) · [MapConfig](xref:DbsContentApi.MapConfig)

## 1. Build a scene bundle

Export your Unity scene into a dedicated bundle (or combined map bundle). File sits next to your mod DLL with no extension — same rules as [Asset bundles](../concepts/asset-bundles.md).

## 2. Find the scene path

Bundle paths rarely match your display name. Use `Maps.FindScenePath`:

[!code-csharp[Register map](../snippets/TutorialSnippets.cs?name=Map)]

`RegionsExpanded` falls back to the filename without extension if the first hint fails.

## 3. Pass a stable `mapId`

Always set `mapId` explicitly:

```csharp
mapId: "mymod.my_scene"
```

> [!WARNING]
> All clients must register maps in the **same order**. See [Multiplayer rules](../concepts/multiplayer.md).

## 4. Scene markers (authoring contract)

Your Unity scene should include:

| Marker | Purpose |
|--------|---------|
| `DiveBellSpawn*` | Diving bell spawn points |
| `PatrolPoint*` or `PatrolPoint_Dog` | Patrol routing (optional `_w2` weight suffix) |
| `RoundRemoveHandler` | Optional spawn filtering |
| `RoundMultiplierHandler` | Optional spawn multipliers |

The API runs a load pipeline (retexture, patrol wiring, ambience, etc.). Customize via `MapConfig` or `MapLifecycleHooks` when you need to skip phases.

## 5. Force selection from custom UI

```csharp
Maps.SetSelectedMapId("mymod.my_scene");
```

Used by `RegionsExpanded` when the Diving Bell map screen picks a destination.

## 6. Dev flag: modded maps only

```csharp
DbsContentApiPlugin.SetModdedMapsOnly(true);
```

Restricts selectable maps to API-registered ones — useful when testing a single custom scene.

## Related

- [Multiplayer rules](../concepts/multiplayer.md)
- [SDK overview](../concepts/overview.md)
