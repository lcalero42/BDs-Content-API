---
_layout: landing
---

# DbsContentApi

**DbsContentApi** is the shared modding API for [Content Warning](https://store.steampowered.com/app/2881650/Content_Warning/). It provides registration helpers for custom items, monsters, maps, materials, content events, and localized comments.

Consumer mods reference the published `DbsContentApi.dll` from the Steam Workshop dependency and call the static APIs under the `DbsContentApi` namespace.

## Quick links

| Topic | Description |
|-------|-------------|
| [Introduction](docs/introduction.md) | Overview and dependency setup |
| [Getting started](docs/getting-started.md) | End-to-end mod registration workflow |
| [API reference](api/DbsContentApi.yml) | Generated API documentation |

## Generate documentation locally

```powershell
dotnet build -c Release
dotnet docfx metadata docfx.json
dotnet docfx build docfx.json
dotnet docfx serve _site
```
