# Add a monster

Register a custom monster with AI, networking, and spawn budget. Pattern from `CW_SDK` and `UnlistedEntities/CustomContent/Mobs`.

**API reference:** [Mobs](xref:DbsContentApi.Mobs) · [MobSetupConfig](xref:DbsContentApi.MobSetupConfig)

## 1. Load the prefab

Your monster prefab needs a rig the SDK can configure (or an existing `RigCreator` setup):

```csharp
GameObject prefab = ContentLoader.LoadPrefabFromBundle(bundle, "MyMonster.prefab");
```

## 2. Build `MobSetupConfig`

Enable the standard component stack for most humanoid monsters:

[!code-csharp[Mob config](../snippets/TutorialSnippets.cs?name=Monster&highlight=5-18)]

| Config block | Controls |
|--------------|----------|
| `BudgetConfig` | Spawn cost and rarity weight |
| `ControllerConfig` | Movement, gravity, jump |
| `NavMeshAgentConfig` | Agent size, speed; set `wide = true` for large mobs |
| `addMonsterSyncer` etc. | Networking and animation handlers |

> [!TIP]
> Set `monsterAnimationValues.rightPunch = false` when your rig does not support punch animations (`UnlistedEntities` Reaper/Teapot pattern).

## 3. Register with material and post-setup

[!code-csharp[Register monster](../snippets/TutorialSnippets.cs?name=Monster&highlight=20-27)]

- **`material:`** queues a shader fix via `GameMaterials.ApplyOnLoad` (applied when the shop opens)
- **`postSetup`** runs after rig/bot creation — attach attacks and content providers here

## 4. Bot components go on the bot child

```csharp
GameObject bot = Mobs.GetBotChildObject(monsterRoot);
Mobs.AddBotChaserComponent(bot, new BotChaserConfig { targetDistance = 1.5f });

var melee = bot.AddComponent<Attack_Melee>();
melee.damage = 50f;
melee.range = 2.5f;
```

> [!WARNING]
> Adding `Bot_Chaser` or `Attack_*` to the **root** prefab instead of the bot child is a common AI failure.

## 5. Test spawning

During development:

```csharp
DbsContentApiPlugin.SetModdedMobsOnly(true);
```

This restricts the round spawner to API-registered monsters only.

## Related

- [Content events & filming](content-events.md) — monster filming moments
- [Fix materials](fix-materials.md)
- [Multiplayer rules](../concepts/multiplayer.md)
