# Ethereal Asset Format (EAF) — Blender bridge

EAF is the engine-owned interchange contract for Blender integration. It describes meshes, materials, skeletons, animation clips, cameras, lights, collision metadata, and scene references without requiring Blender at runtime.

## Pipeline

```text
Blender .blend
   -> export script
   -> EAF manifest/data
   -> Ethereal Veni importer
   -> runtime asset database
```

The bridge is intentionally data-oriented. Blender remains an authoring tool; Ethereal Veni owns runtime asset identity and loading.
