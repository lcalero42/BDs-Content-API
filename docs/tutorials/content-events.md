# Content events & filming

Hook custom monsters and items into Content Warning's filming and comment systems.

**API reference:** [ContentEvents](xref:DbsContentApi.ContentEvents) · [CustomCommentRegistry](xref:DbsContentApi.CustomCommentRegistry)

## Register content events

[!code-csharp[Register event](../snippets/TutorialSnippets.cs?name=ContentEvent)]

Each registered event gets an ID of `2000 + registrationIndex`.

> [!WARNING]
> **Never reorder** registered events after release. IDs are positional — inserting an event at the top shifts every later ID.

`UnlistedEntities` registers all events in a fixed block before deferred item registration:

```csharp
ContentEvents.RegisterEvent(new PopitExplosionContentEvent());
ContentEvents.RegisterEvent(new BatHitMonsterContentEvent());
// ... always append new events at the end in updates
```

Resolve an ID at runtime:

```csharp
ushort id = ContentEvents.GetEventID(nameof(PopitExplosionContentEvent));
```

## Custom comment strings

Register localized comment UI text keyed to your content events:

```csharp
CustomCommentRegistry.Register(
    "Content_Teapot_0",
    new CustomComment("en", "Is that a teapot?"));
```

See `UnlistedEntities/CustomContent/CustomCommentsBootstrap.cs` for a full set.

## Temporary filming triggers

Spawn invisible trigger volumes for testing or level scripting:

```csharp
ObjectHelper.CreateTemporaryTriggerObject(
    frameCount: 50,
    DbsContentApiPlugin.TemporaryContentTriggerPrefab!);
```

The first argument is how many **Update frames** the trigger stays active, not a world-space radius.

## Wiring monsters to filming

Attach content providers in `Mobs.RegisterMonster` `postSetup`:

```csharp
postSetup: go =>
{
    go.AddComponent<MyMonsterContentProvider>();
}
```

Register the associated `ContentEvent` during `Init()` — not in `postSetup` — so IDs stay consistent:

```csharp
ContentEvents.RegisterEvent(new MyMonsterContentEvent());
```

> [!WARNING]
> Content event IDs are assigned as `2000 + registrationIndex` across **every mod** that calls `RegisterEvent`. If two clients load mods in a different order, or one client is missing a mod, event IDs will not match and filming moments can desync. DbsContentApi does not enforce a global mod load order today; keeping registration order stable within your mod and documenting Workshop dependencies is the best mitigation for now.

## Related

- [Add a monster](add-monster.md)
- [Multiplayer rules](../concepts/multiplayer.md)
