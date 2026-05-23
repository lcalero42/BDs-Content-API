# Installation

DbsContentApi is distributed as a Steam Workshop **dependency mod**. Your mod references `DbsContentApi.dll` and uses the public `DbsContentApi` namespace to register custom content at runtime.

> [!TIP]
> Ready to code? Skip straight to [Quick start](quick-start.md) for a 5-minute Hello World item.

## 1. Subscribe and reference the DLL

1. Subscribe to the DbsContentApi Workshop item.
2. Copy or reference `DbsContentApi.dll` from the Workshop install (see `CW_SDK/ExampleMod.csproj` for the `DbsContentApiDir` pattern).
3. Add a `<Reference Include="DbsContentApi">` entry in your mod `.csproj` — do **not** ship the DLL inside your mod; list it as a Workshop dependency.

## 2. Declare the dependency

Ensure DbsContentApi loads **before** your mod in the Workshop manifest. Without this, registration APIs are unavailable at plugin load.

## 3. Add global usings

```csharp
global using DbsContentApi;
global using Logger = YourModNamespace.Logger;
```

DbsContentApi does not ship a mod logger. Copy `CW_SDK/Logger.cs` or use your own `Debug.Log` wrapper.

## 4. Project references

Your mod still needs standard Content Warning references (`Assembly-CSharp`, `BepInEx`, `UnityEngine`, Photon, etc.). See `CW_SDK/ExampleMod.csproj` for a complete template.

## What's in the box

| Module | Purpose | Learn more |
|--------|---------|------------|
| `ContentLoader` | Load AssetBundles and Photon pool registration | [Asset bundles](concepts/asset-bundles.md) |
| `Items` | Shop items and custom categories | [Add a shop item](tutorials/add-shop-item.md) |
| `Mobs` | Custom monsters with AI and networking | [Add a monster](tutorials/add-monster.md) |
| `Maps` | Custom playable scenes | [Add a map](tutorials/add-map.md) |
| `GameMaterials` | Fix broken bundle shaders | [Fix materials](tutorials/fix-materials.md) |
| `ContentEvents` | Filming / content trigger events | [Content events](tutorials/content-events.md) |
| `CustomCommentRegistry` | Localized comment UI strings | [Content events](tutorials/content-events.md) |
| `ObjectHelper` | Temporary filming trigger volumes | [API overview](api-overview.md) |
| `BaseCWInput` | Custom keybinds in settings menu | [API overview](api-overview.md) |

> [!IMPORTANT]
> Harmony patches under `DbsContentApi.Patches` are **internal** and excluded from the public API reference.

## Documentation map

| Page | Audience |
|------|----------|
| [Home](index.md) | Everyone — start here |
| [Quick start](quick-start.md) | First mod in 5 minutes |
| [Complete guide](getting-started.md) | Full walkthrough with real mod patterns |
| [Concepts](concepts/overview.md) | How the SDK hooks into the game |
| [Tutorials](tutorials/add-shop-item.md) | Focused recipes |
| [API overview](api-overview.md) | Grouped type index |

## Example mod

The monorepo includes `CW_SDK/` — a working reference with maps, monsters, shop items, and deploy scripts. Larger production patterns live in `UnlistedEntities` and `RegionsExpanded`.
