---
uid: DbsContentApi.GameMaterials
remarks: |
  ## Tutorial
  
  [Fix materials](~/docs/tutorials/fix-materials.md)
  
  `InitMaterials()` runs when the shop opens — not at plugin load — so all in-game materials are available before caching. Use `ApplyMaterial` in deferred item registration; use `ApplyOnLoad` (or `Mobs.RegisterMonster(material:)`) for content registered earlier at plugin load.
---
