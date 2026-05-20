# DbsContentApi

Shared modding API for [Content Warning](https://store.steampowered.com/app/2881650/Content_Warning/). Provides registration helpers for custom items, monsters, maps, materials, content events, and localized comments.

## Documentation

API reference and guides are generated with [DocFX](https://dotnet.github.io/docfx/).

```powershell
# One-shot build + generate (from this directory)
./generate-docs.ps1

# Preview locally
./generate-docs.ps1 -Serve
# or: dotnet docfx serve _site
```

Manual steps:

```powershell
dotnet tool restore
dotnet build -c Release
dotnet docfx metadata docfx.json
dotnet docfx build docfx.json
```

Output is written to `_site/`. Generated API YAML lives in `api/` (gitignored; regenerated on each run).

## Consumer mods

Reference the published `DbsContentApi.dll` from the Steam Workshop dependency. See `CW_SDK/` in the monorepo for an example mod project.
