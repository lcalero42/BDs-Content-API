# Introduction

DbsContentApi is distributed as a Steam Workshop dependency mod. Your mod project references `DbsContentApi.dll` and uses the public `DbsContentApi` namespace to register custom content at runtime.

## Installation

1. Subscribe to the DbsContentApi Workshop item and copy `DbsContentApi.dll` into your mod project (see `CW_SDK/EXAMPLE.Directory.Build.props` for the `DbsContentApiDir` pattern).
2. Add a project reference to the DLL in your mod `.csproj`.
3. Ensure DbsContentApi is listed as a dependency in your Workshop manifest so it loads before your mod.

## Core modules

| Module | Purpose |
|--------|---------|
| `ContentLoader` | Load AssetBundles and register prefabs in the Photon pool |
| `Items` | Register custom shop items and categories |
| `Mobs` | Configure and register custom monsters |
| `Maps` | Register custom map scenes from AssetBundles |
| `GameMaterials` | Apply in-game materials to fix broken bundle shaders |
| `ObjectHelper` | Create content-trigger volumes for filming |
| `ContentEvents` | Register custom content event types |
| `CustomCommentRegistry` | Inject localized comment UI strings |
| `BaseCWInput` | Define custom keybinds with settings-menu integration |

Harmony patches under `DbsContentApi.Patches` are internal and not part of the mod-facing API.
