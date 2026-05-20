# Getting started

This walkthrough mirrors the `CW_SDK` example mod. Call registration methods from your plugin constructor or `Awake`/`Start` — the API applies them at the correct game lifecycle points via internal Harmony patches.

DbsContentApi does not provide logging. Copy `CW_SDK/EXAMPLE.Logger.cs.txt` to `Logger.cs` in your mod, set your namespace and prefix, and add to `GlobalUsings.cs`:

```csharp
global using DbsContentApi;
global using Logger = YourModNamespace.Logger;
```

## 1. Load assets

```csharp
var bundle = ContentLoader.LoadAssetBundle(typeof(MyPlugin).Assembly, "mymod");
var itemPrefab = ContentLoader.LoadPrefabFromBundle(bundle, "MyItem.prefab");
```

## 2. Register maps and monsters

```csharp
var scenePath = Maps.FindScenePath(bundle, "MyMap");
Maps.RegisterMap(bundle, scenePath!, "My Custom Map");

Mobs.RegisterMonster(monsterPrefab, "MyMonster", new MobSetupConfig
{
    budget = new BudgetConfig(),
    bot = new BotConfig(),
    navMesh = new NavMeshAgentConfig()
});
```

## 3. Defer item registration until the shop is ready

```csharp
Items.DeferRegistration(() =>
{
    Items.RegisterItem(itemPrefab, new ItemConfig
    {
        displayName = "My Item",
        price = 50,
        icon = bundle.LoadAsset<Sprite>("icon.png")
    });
});
```

## 4. Fix materials and optional API flags

```csharp
GameMaterials.ApplyOnLoad(itemPrefab, GameMaterial.M_Metal, recursive: true);

DbsContentApiPlugin.SetModdedMobsOnly(false);
DbsContentApiPlugin.SetAllItemsFree(false);
```

## 5. Content triggers

Use `DbsContentApiPlugin.TemporaryContentTriggerPrefab` with `ObjectHelper` to spawn filming trigger volumes:

```csharp
ObjectHelper.CreateTemporaryTriggerObject(120, DbsContentApiPlugin.TemporaryContentTriggerPrefab!);
```

See the [API reference](../api/DbsContentApi.yml) for full member documentation.
