# Asset bundles

DbsContentApi loads bundles from disk with `ContentLoader`. Unity authoring stays in your CWSDK / Unity project; runtime loading is always from the mod install folder.

## File layout

```
Workshop/content/2881650/<your_mod_id>/
  MyMod.dll
  my_mod              ← prefabs, sprites, audio (no extension)
  my_map              ← optional scene bundle
```

The bundle **filename** (without extension) must match the string you pass to `LoadAssetBundle`:

[!code-csharp[Load bundle](../snippets/QuickStart.cs?name=LoadBundle)]

> [!WARNING]
> `LoadAssetBundle` returns `null` on failure. Always check before registering content — a null bundle should abort `Init()`, not cascade into null reference errors.

## One bundle vs many

| Pattern | Used by | When |
|---------|---------|------|
| Single bundle | `UnlistedEntities` | All prefabs in one file |
| Mod + map bundles | `CW_SDK`, `RegionsExpanded` | Separate scene bundle for maps |

Scenes inside a map bundle are resolved with `Maps.FindScenePath` — the path rarely equals the display name.

## Loading prefabs and assets

```csharp
GameObject prefab = ContentLoader.LoadPrefabFromBundle(bundle, "MyItem.prefab");
Sprite icon = bundle.LoadAsset<Sprite>("my_icon");
SFX_Instance sfx = bundle.LoadAsset<SFX_Instance>("MySoundSfx");
```

`LoadPrefabFromBundle` throws if the asset name is wrong. Catch at your `Init()` boundary or validate names during development.

## Photon pool

Networked prefabs must be in Photon's prefab pool. `Items.RegisterItem` and monster setup call `ContentLoader.RegisterPrefabInPhotonPool` for you. If you spawn custom networked objects outside those APIs, register them manually.

## Shader fix is separate

Loading a bundle does **not** fix materials. Bundle shaders are usually incompatible with the live game — see [Fix materials](../tutorials/fix-materials.md).

## Build pipeline

1. Author prefabs/scenes in Unity (CWSDK project in the monorepo).
2. Build AssetBundles to an output folder.
3. Copy bundles next to your mod DLL for local testing.
4. Include bundles in your Workshop upload.

`CW_SDK/EXAMPLE.config.json` documents deploy paths for local iteration.

> [!TIP]
> Keep bundle name constants (`"my_mod"`, `"example_scene"`) in one place — typos here are the #1 load failure.
