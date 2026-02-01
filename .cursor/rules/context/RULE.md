---
alwaysApply: true
---

# Content Warning Modding Context

This document provides architectural and technical context for AI agents working on the Content Warning mod project.

## Project Overview
- **Game**: "Content Warning" by Landfall Games.
- **Disassembled Code (`./decomp`)**: Contains the original game's logic (Assembly-CSharp.dll).
- **Mod Project (`./`)**: A BepInEx-based plugin for extending or modifying game behavior.

## Technical Stack
- **Engine**: Unity 2022.3.10f1.
- **Scripting**: C# (Targeting `.NET Standard 2.1`).
- **Modding Framework**: BepInEx 5.
- **Patching Library**: Harmony (used for runtime method hooking/injection).
- **Networking**: Photon Pun 2 / Photon Fusion.
- **Core Game Framework**: `Zorro.Core` (internal framework used by the game).

## Core Game Architecture

### Monsters (Bots)
- **Base Class**: `Bot` (inherits from `MonoBehaviour`). Handles AI movement, targeting, and state.
- **Handler**: `BotHandler` manages active bot instances.
- **Spawning**: `RoundSpawner` controls when and where monsters spawn based on a budget system.
- **Filming System**:
    - `ContentProvider`: Attached to monsters/objects to define what happens when they are filmed.
    - `ContentEvent`: Represents a specific event captured on film (e.g., "Monster saw player").
    - `IBudgetCost`: Interface used by the spawner to determine the "cost" of spawning a specific entity.

### Items
- **Base Class**: `Item` (inherits from `ScriptableObject`).
- **Database**: `ItemDatabase` contains all items available in the game.
- **Registration**: Custom items must be registered to be recognized by the game's shops and inventory system.

### Networking
- The game uses **Photon Pun 2**. Many components rely on `PhotonView` for synchronization.
- Custom objects must be added to the `PhotonNetwork.PrefabPool` to be spawnable over the network.

## Mod Architecture & Conventions

Use the central Log, LogError and LogWarning functions defined in the mod root.

### Entry Point
- `DbsContentApi.cs`: The main plugin class inheriting from `BaseUnityPlugin`.
- Uses `[BepInPlugin]` and `[ContentWarningPlugin]` attributes.

### ContentLib (Internal Mod Modules)
The project includes a set of helper modules under `Modules/` (often referred to as ContentLib):
- **`BundleHelper.cs`**: Utilities for loading Unity AssetBundles (embedded or from disk).
- **`ContentLoader.cs`**: Handles Photon prefab registration and Harmony patches for `RoundSpawner` to inject custom content.
- **`Monsters.cs`**: Provides `RegisterMonster` to add custom bot prefabs to the game's spawning pool.

### Patching Patterns
- **Harmony**: Used extensively to hook into game methods.
- **Prefix/Postfix**: Standard Harmony patterns for modifying behavior before or after game methods execution.
- **AccessTools**: Used to access private fields or methods in the disassembled code.

## Key Classes to Watch (in `./decomp`)
- `Bot`: The base for all monster AI.
- `BotHandler`: Global manager for monsters.
- `RoundSpawner`: Logic for level generation and monster spawning.
- `Player`: The local and networked player representation.
- `Item`: ScriptableObject definition for all items.
- `ContentProvider`: The bridge between world objects and the filming/score mechanic.

## Development Rules
- **AssetBundles**: Custom prefabs should be loaded from AssetBundles. Shaders often need fixing (`ContentLoader.FixMaterials`) because they don't always transfer correctly from bundles to the game environment.
- **Network Sync**: Always ensure custom monsters have a `PhotonView` and are registered in the prefab pool.
- **Harmony Patches**: When patching, use the `[HarmonyPatch]` attribute on static classes/methods. Prefer `Postfix` for adding behavior and `Prefix` for overriding or cancelling original behavior.

## Common Pitfalls
- **Missing `IBudgetCost`**: Custom monsters *must* have a component implementing `IBudgetCost` (like `BudgetCost.cs` in the game) on their prefab, or the `RoundSpawner` patch will throw a `NullReferenceException`.
- **Shader Compatibility**: Materials on prefabs from AssetBundles often appear pink or invisible. Use `ContentLoader.FixMaterials` or `Monsters.UseMonsterMaterial` to swap shaders to those already loaded in the game.
- **Photon View ID**: Ensure custom networked objects have a `PhotonView` with an ID that doesn't conflict, or let the game/Photon handle ID assignment during instantiation.
- **AssetBundle Names**: Ensure the resource name used in `LoadEmbeddedAssetBundle` matches the one defined in the `.csproj` (`<EmbeddedResource Include="..." />`).