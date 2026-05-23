# Multiplayer rules

Custom maps and some spawn behaviour depend on **every client agreeing** on registration order and IDs. Single-player testing can hide bugs that break multiplayer lobbies.

## Mod load order

Several DbsContentApi features assign positional IDs at runtime — custom map indices, content event IDs (`2000 + index`), and custom shop category indices. **The order in which mods load therefore matters.**

All players in a lobby should run the **same mod set in the same load order**. If one client loads mods A → B → C and another loads B → A → C, map selection, filming event IDs, and shop categories can desync even when everyone has the same files installed.

> [!IMPORTANT]
> Consistent mod load order across modders and players is essential for reliable multiplayer. **DbsContentApi does not provide a load-order enforcement mechanism today.** Workshop dependency declarations help, but they are not a full solution. We may revisit this — for example by moving back to BepInEx-style dependency ordering — in a future update.

Treat unstable load order as a known limitation when shipping multiplayer content.

## Custom maps

### Stable `mapId`

Always pass an explicit `mapId` when registering:

```csharp
Maps.RegisterMap(bundle, scenePath, "Factory", mapId: "mymod.factory");
```

Convention: `"yourmod.scene_name"` in lowercase. `RegionsExpanded` uses:

```csharp
$"regionsexpanded.{sceneName.ToLowerInvariant()}"
```

### Registration order is the network protocol

The game maps custom map index to `LevelToPlay = 3 + registrationIndex`. **All clients must register maps in the same order.**

> [!WARNING]
> Never insert a new map before existing ones in a published mod — clients with different order desync. Append new maps at the end, or use a breaking major version.

### Selecting a map from custom UI

When bypassing vanilla map selection, set the ID before the round starts:

```csharp
Maps.SetSelectedMapId("mymod.factory");
Maps.SetSelectedMapId(null); // clear selection
```

## Custom monsters

Monster prefabs are networked through Photon. Use `Mobs.RegisterMonster` so the SDK adds `PhotonView`, sync handlers, and pool registration consistently.

Budget and rarity (`BudgetConfig`) affect spawn selection on the host — tune for gameplay, not per-client.

## Shop items

Once registered, items use the game's normal shop sync. Deferred registration runs on each client when the shop initializes; ensure every client has the same mod and DbsContentApi version.

## Content event IDs

`ContentEvents.RegisterEvent` assigns IDs as `2000 + registrationIndex`. **Never reorder** registered events after release — saved content and triggers reference IDs by index.

> [!IMPORTANT]
> Register all content events in a fixed order during `Init()` before items or monsters that reference them. Because IDs are positional across **all loaded mods**, mod load order also affects event IDs — see [Mod load order](#mod-load-order).

## Testing checklist

- [ ] Same mod set and load order on all clients
- [ ] Maps registered in identical order on both
- [ ] New maps appended, not inserted
- [ ] `mapId` strings are unique across all loaded mods
- [ ] Content events registered in stable order

See [Add a map](../tutorials/add-map.md) for scene marker requirements.
