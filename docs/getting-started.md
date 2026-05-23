# Complete guide

End-to-end walkthrough for maps, monsters, and shop items. Patterns match real mods in this repo — especially `CW_SDK/Modules/CustomContent.cs`, plus `RegionsExpanded` and `UnlistedEntities`.

**New here?** Start with [Quick start](quick-start.md) (5 min) or [Home](index.md). For a single task, use [Tutorials](tutorials/add-shop-item.md). For types and members, see [API overview](api-overview.md).

> [!TIP]
> This page is the long-form reference. Tutorials cover the same topics in shorter, task-focused recipes.

## What you build

A typical Content Warning mod that uses DbsContentApi has three layers:

| Layer | Responsibility |
|-------|----------------|
| **Plugin entry** (`ExampleMod.cs`) | Set optional API flags, call `CustomContent.Init()` |
| **Content registry** (`CustomContent.cs`) | Load bundles, register maps/mobs/items |
| **Feature modules** (`CustomItems.cs`, `CustomMobs.cs`, …) | Optional split when a mod grows large |

Asset bundles are built in Unity, shipped **next to your mod DLL** (no file extension), and loaded at runtime with `ContentLoader`.

## Project setup checklist

1. Reference `DbsContentApi.dll` in your `.csproj` (see `CW_SDK/ExampleMod.csproj` for the `DbsContentApiDir` pattern).
2. List DbsContentApi as a Workshop dependency so it loads before your mod.
3. Ship your bundles beside your DLL in the Workshop folder — same layout you use locally when testing.
4. Add a global using in `GlobalUsings.cs`:

```csharp
global using DbsContentApi;
```

DbsContentApi does **not** ship a mod logger. Copy the pattern from `CW_SDK/Logger.cs` (or your own wrapper) and alias it if you like:

```csharp
global using Logger = YourModNamespace.Logger;
```

## When to call registration

Call registration from your plugin constructor (or a static `Init()` invoked from there). DbsContentApi applies registrations at the correct game lifecycle points through internal Harmony patches — you do not need to wait for `Awake`/`Start` yourself.

```csharp
[ContentWarningPlugin(MyPluginInfo.PLUGIN_GUID, MyPluginInfo.PLUGIN_VERSION, false)]
public class ExampleMod
{
    static ExampleMod() => Instance = new ExampleMod();

    public ExampleMod()
    {
        // Optional dev/test flags — see "Development flags" below
        DbsContentApiPlugin.SetModdedMobsOnly(true);
        DbsContentApiPlugin.SetAllItemsFree(true);

        CustomContent.Init();
    }

    public static ExampleMod Instance { get; }
}
```

Keep the plugin constructor lightweight. Heavy prefab wiring belongs in `CustomContent` or deferred item callbacks.

## Recommended `CustomContent` shape

Most mods use a static `CustomContent` class as the single entry point:

```csharp
public static class CustomContent
{
    private const string BundleNameMod = "example_mod";
    private const string BundleNameMap = "example_scene";

    public static AssetBundle? BundleMod { get; private set; }
    public static AssetBundle? BundleMap { get; private set; }

    public static void Init()
    {
        BundleMod = ContentLoader.LoadAssetBundle(Assembly.GetExecutingAssembly(), BundleNameMod);
        if (BundleMod == null) { Logger.LogError($"Failed to load '{BundleNameMod}'."); return; }

        BundleMap = ContentLoader.LoadAssetBundle(Assembly.GetExecutingAssembly(), BundleNameMap);
        if (BundleMap == null) { Logger.LogError($"Failed to load '{BundleNameMap}'."); return; }

        RegisterMap();
        RegisterMob();
        RegisterItems();
    }
}
```

Larger mods (`UnlistedEntities`, `RegionsExpanded`) split items and mobs into `CustomItems.Setup(bundle)` and `CustomMobs.Setup(bundle)` but follow the same `Init()` → load bundles → register flow.

---

## 1. Load asset bundles

Bundles are loaded from the directory containing your mod assembly:

```csharp
// Preferred — resolves the assembly location for you
AssetBundle? bundle = ContentLoader.LoadAssetBundle(Assembly.GetExecutingAssembly(), "my_mod");

// Alternative — pass the DLL path explicitly (UnlistedEntities uses this)
AssetBundle bundle = ContentLoader.LoadAssetBundle(
    Assembly.GetExecutingAssembly().Location, "unlisted_entities_ab");
```

**File layout:** if your DLL is `MyMod.dll`, the bundle file must be `MyMod/my_mod` (filename only, no `.assetbundle` extension).

Load prefabs and other assets from the bundle:

```csharp
GameObject prefab = ContentLoader.LoadPrefabFromBundle(bundle, "MyItem.prefab");
Sprite icon = bundle.LoadAsset<Sprite>("my_icon");
SFX_Instance sfx = bundle.LoadAsset<SFX_Instance>("MySoundSfx");
```

`LoadPrefabFromBundle` throws if the asset name is wrong — wrap registration in null checks and try/catch at your `Init()` boundary so one bad asset does not crash the whole mod.

You can use separate bundles for scenes vs prefabs (`example_mod` + `example_scene` in CW_SDK), or a single bundle for everything.

---

## 2. Register maps

### Find the scene path

Scene paths inside a bundle rarely match the display name exactly. Use `Maps.FindScenePath`:

```csharp
string? scenePath = Maps.FindScenePath(bundleMap, "TestScene");
if (scenePath == null)
{
    Logger.LogWarning("Scene not found in bundle.");
    return;
}
```

`RegionsExpanded` tries the scene name, then the filename without extension, and skips entries that are not present in the bundle.

### Register with a stable `mapId`

```csharp
Maps.RegisterMap(
    bundleMap,
    scenePath,
    "Example Map",
    mapId: "example_mod.example_map");
```

**Multiplayer contract:** all clients must register custom maps in the **same order**. The game maps registration index to `LevelToPlay` (`3 + index`). Use a stable, unique `mapId` (convention: `"yourmod.scene_name"`) so selection survives reordering in your source code.

`RegionsExpanded` generates IDs with a helper:

```csharp
public static string MapIdForScene(string sceneName)
    => $"regionsexpanded.{sceneName.ToLowerInvariant()}";
```

To force a specific map from custom UI code, call `Maps.SetSelectedMapId(mapId)` (see `RegionsExpanded/Patches/NewMapToPlayPatch.cs`).

### Map authoring notes

Custom maps expect standard scene markers (see comment block at top of `Maps.cs`):

- `DiveBellSpawn*` — diving bell spawn points
- `PatrolPoint*` or `PatrolPoint_Dog` — patrol routing (optional `_w2` spawn weight suffix)
- Optional `RoundRemoveHandler`, `RoundMultiplierHandler` for spawn filtering

Pass a `MapConfig` or `MapLifecycleHooks` when you need to skip pipeline phases or run custom setup after load. Defaults work for many first maps.

---

## 3. Register monsters

`Mobs.RegisterMonster` adds the standard monster component stack (controller, ragdoll, PhotonView, bot child, nav mesh, sync handlers, etc.) from a `MobSetupConfig`.

### Minimal example

```csharp
var config = new MobSetupConfig
{
    budget = new BudgetConfig { budgetCost = 1, rarity = 1f },
    controller = new ControllerConfig(),
    player = new PlayerConfig(),
    ragdoll = new RagdollConfig(),
    photonView = new PhotonViewConfig(),
    bot = new BotConfig(),
    navMesh = new NavMeshAgentConfig { height = 2f, radius = 1f, speed = 3.5f },
    addMonsterSyncer = true,
    addAnimRefHandler = true,
    addMonsterAnimationHandler = true,
    addHeadFollower = true,
    addGroundPos = true,
};

GameObject prefab = ContentLoader.LoadPrefabFromBundle(bundle, "DefaultCharacter.prefab");

Mobs.RegisterMonster(prefab, "DefaultCharacter", config,
    material: GameMaterial.M_Monster,
    postSetup: go =>
    {
        GameObject bot = Mobs.GetBotChildObject(go);
        Mobs.AddBotChaserComponent(bot, new BotChaserConfig { targetDistance = 1.5f });

        var melee = bot.AddComponent<Attack_Melee>();
        melee.damage = 50f;
        melee.range = 2.5f;
        // ... tune other Attack_* fields
    });
```

### Key points

- **`postSetup`** runs after the API finishes rig/bot setup — attach AI attacks, content providers, and extra components here.
- **`Mobs.GetBotChildObject`** returns the child object the API created for bot AI. Chase/drag/knife components go on that child, not the root.
- **`material:`** queues a material swap via `GameMaterials.ApplyOnLoad` — applied when the shop opens and `InitMaterials` runs (see [Fix materials](tutorials/fix-materials.md)).
- Tune **`BudgetConfig`** (`budgetCost`, `rarity`) to control spawn frequency. `UnlistedEntities` uses higher `budgetCost` for stronger monsters.
- Disable punch animations when your rig does not support them: `monsterAnimationValues = new MonsterAnimationValuesConfig { rightPunch = false, leftPunch = false }`.

Register content events during `Init()` in a fixed order. Event IDs depend on registration order across **all loaded mods** — see [Multiplayer rules](concepts/multiplayer.md).

---

## 4. Register shop items

Shop items must be registered **after** the game's item database is ready. Always defer:

```csharp
public static ShopItemCategory TestCategory;

private static void RegisterItems()
{
    TestCategory = Items.RegisterCustomCategory("Test Category");

    Items.DeferRegistration(() =>
    {
        RegisterTestItem();
        RegisterTestGun();
    });
}
```

Inside the deferred callback, load prefabs, add behaviour components, fix materials, then call `Items.RegisterItem`.

### Basic item

```csharp
GameObject prefab = ContentLoader.LoadPrefabFromBundle(bundle, "TestItem.prefab");
prefab.AddComponent<BasicUsableItem>();
GameMaterials.ApplyMaterial(prefab, GameMaterial.M_Apple_1, deepApply: true);

Items.RegisterItem(prefab, new ItemConfig
{
    displayName = "Test Item",
    price = 100,
    category = TestCategory,
    icon = bundle.LoadAsset<Sprite>("example_icon"),
    holdPos = new Vector3(0.3f, -0.3f, 0.7f),
    impactSoundTypes = new[] { ImpactSoundType.PlasticBounce1 },
});
```

### What `RegisterItem` does for you

- Ensures `ItemInstance` and `HandGizmo` exist on the prefab
- Registers the prefab in the Photon prefab pool
- Creates the `Item` ScriptableObject with shop metadata, hold offsets, spawn settings, etc.
- Returns the registered `Item` — store it if other systems need a reference (`UnlistedEntities` keeps `JumpingBootsItem`, etc.)

### `ItemConfig` fields you will use most

| Field | Purpose |
|-------|---------|
| `displayName` | Shop and inventory name |
| `price` | Shop cost (overridden when `SetAllItemsFree(true)`) |
| `category` | Vanilla tab or custom category from `RegisterCustomCategory` |
| `icon` | Load from bundle **before** calling `RegisterItem` |
| `holdPos` / `holdRot` | Hand attachment offsets |
| `useAlternativeHoldPos` / `alternativeHoldPos` | Secondary hold pose (guns often use this) |
| `impactSoundTypes` | Auto-resolved impact sounds via `ImpactSoundScanner` |
| `persistentId` | Optional stable save/network ID; defaults to `unknown_mod.{displayName}` if omitted — set your own mod-scoped ID in production |

### Custom categories

```csharp
ShopItemCategory weapons = Items.RegisterCustomCategory("Weapons");
```

Category indices start at 20 and increment per mod registration order.

### Complex items (guns, explosives, equipables)

Real mods typically:

1. Load the item prefab from the bundle
2. Add a custom `MonoBehaviour` for use logic
3. Build or clone projectile/explosion prefabs at runtime
4. Apply materials per child path
5. Register content events **before** deferred item registration if the item triggers them

`CW_SDK` clones the vanilla Dog laser projectile for a test gun. `UnlistedEntities` clones the vanilla `BombItem` explosion for custom AOE templates:

```csharp
Item? bombItem = Items.GetItemByPrefabComponent<BombItem>();
GameObject explosion = Object.Instantiate(bombItem.itemObject.GetComponent<BombItem>().explosion);
// customize, DontDestroyOnLoad, wire into your item behaviour
```

---

## 5. Fix materials

Unity bundle materials often render incorrectly in Content Warning. Use `GameMaterials` to swap in working in-game shaders.

| Method | When to use |
|--------|-------------|
| `GameMaterials.ApplyMaterial(root, GameMaterial.M_Metal, deepApply: true)` | Items inside deferred registration (shop is open) |
| `GameMaterials.ApplyOnLoad(root, material, recursive: true)` | Monsters at plugin load; applied when shop opens |
| `GameMaterials.Batch(root).At("Item/Mesh", GameMaterial.M_Metal)` | Per-child-path assignment |
| `GameMaterials.CloneMaterial(baseMat, texture, "MyMatName")` | Custom textures on a base game material (`RegionsExpanded` upgrade box fronts) |

`DescriptiveMaterial` (`WHITE_1`, `BROWN_1`, …) is an alternative enum for common color presets.

Example from `UnlistedEntities`:

```csharp
GameMaterials.Batch(prefab)
    .At("Item/PopIt", DescriptiveMaterial.WHITE_1, deep: true);
```

Example from `RegionsExpanded` — different materials on submeshes:

```csharp
GameMaterials.SetMaterialAt(cubeRenderer, 0, DescriptiveMaterial.BROWN_1);
GameMaterials.SetMaterialAt(cubeRenderer, 1, DescriptiveMaterial.YELLOW_1);
```

Apply materials **before** `Items.RegisterItem` or inside `postSetup` for monsters (the API applies the `material:` parameter automatically).

---

## 6. Content events and filming triggers

Register custom `ContentEvent` subclasses so the filming/comment system can reference them. **Registration order is fixed** — IDs are `2000 + index`. Never reorder registered events after release.

```csharp
ContentEvents.RegisterEvent(new PopitExplosionContentEvent());
ContentEvents.RegisterEvent(new BatHitMonsterContentEvent());
// register all events before items/mobs that reference them
```

Spawn temporary trigger volumes with the API prefab:

```csharp
ObjectHelper.CreateTemporaryTriggerObject(
    frameCount: 50,
    DbsContentApiPlugin.TemporaryContentTriggerPrefab!);
```

The first argument is how many **Update frames** the trigger stays active, not a world-space radius.

---

## 7. Custom comment strings

Register localized comment lines for the in-game comment UI:

```csharp
CustomCommentRegistry.Register(
    "Content_Teapot_0",
    new CustomComment("en", "Is that a teapot?"));
```

Call registration during `Init()` (see `UnlistedEntities/CustomContent/CustomCommentsBootstrap.cs`). Keys must match what your `ContentEvent` / content provider expects.

---

## 8. Development flags

Optional helpers on `DbsContentApiPlugin`, typically gated behind your mod's `DEBUG_MODE`:

```csharp
DbsContentApiPlugin.SetModdedMobsOnly(true);   // Only API-registered monsters spawn
DbsContentApiPlugin.SetModdedMapsOnly(true);   // Only API-registered maps are selectable
DbsContentApiPlugin.SetAllItemsFree(true);     // Shop prices become 0
```

`CW_SDK` enables all three for content testing. Production mods usually leave these off or only enable `SetAllItemsFree` during development.

---

## End-to-end minimal mod

Putting it together — this is the smallest useful mod skeleton:

```csharp
public static class CustomContent
{
    public static void Init()
    {
        var bundle = ContentLoader.LoadAssetBundle(Assembly.GetExecutingAssembly(), "my_mod");
        if (bundle == null) return;

        var mapBundle = ContentLoader.LoadAssetBundle(Assembly.GetExecutingAssembly(), "my_map");
        if (mapBundle != null)
        {
            string? path = Maps.FindScenePath(mapBundle, "MyScene");
            if (path != null)
                Maps.RegisterMap(mapBundle, path, "My Map", mapId: "mymod.my_map");
        }

        var mobPrefab = ContentLoader.LoadPrefabFromBundle(bundle, "MyMonster.prefab");
        Mobs.RegisterMonster(mobPrefab, "MyMonster", new MobSetupConfig
        {
            budget = new BudgetConfig(),
            bot = new BotConfig(),
            navMesh = new NavMeshAgentConfig(),
        }, material: GameMaterial.M_Monster);

        var category = Items.RegisterCustomCategory("My Items");
        Items.DeferRegistration(() =>
        {
            var item = ContentLoader.LoadPrefabFromBundle(bundle, "MyItem.prefab");
            GameMaterials.ApplyMaterial(item, GameMaterial.M_Metal, deepApply: true);
            Items.RegisterItem(item, new ItemConfig
            {
                displayName = "My Item",
                price = 50,
                category = category,
                icon = bundle.LoadAsset<Sprite>("icon"),
            });
        });
    }
}
```

---

## Troubleshooting

| Symptom | Likely cause |
|---------|----------------|
| Bundle fails to load | File not beside DLL, wrong filename (no extension), or wrong bundle name constant |
| Prefab throws on load | Asset name in bundle does not match string passed to `LoadPrefabFromBundle` |
| Item missing from shop | Registration not inside `Items.DeferRegistration`, or `Init()` returned early |
| Map works solo, wrong map in MP | Maps registered in different order on different clients, or duplicate `mapId` |
| Monster invisible / pink | Missing `GameMaterial` apply; bundle shader incompatible with game |
| Content event ID mismatch | Events registered in different order, or new event inserted before existing ones |
| Bot does nothing | Attack/chase components added to root instead of `Mobs.GetBotChildObject` result |

Enable your mod logger and watch for DbsContentApi's internal `[DbsContentApi]` lines in the Unity console when diagnosing registration failures.

---

## Example mods in this repo

| Mod | What to copy |
|-----|----------------|
| `CW_SDK` | Full walkthrough — map + mob + two shop items, projectile cloning, dev flags |
| `UnlistedEntities` | Large item/mob split, content events, comment registry, vanilla prefab reuse |
| `RegionsExpanded` | Multiple maps, custom map IDs, material cloning, upgrade shop category |

---

## Next steps

- [Introduction](introduction.md) — installation and module index
- [API overview](api-overview.md) — grouped type index with member links
- `CW_SDK/EXAMPLE.config.json` — local deploy paths for bundles and Workshop folder
