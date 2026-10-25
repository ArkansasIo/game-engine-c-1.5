# [CORE] Editor Source Documentation

The editor source is intentionally separated from the game runtime.

| Source | Purpose |
|---|---|
| `EditorApplication.cs` | Editor lifecycle, panels, and workspace session |
| `ViewportPanels.cs` | World viewport, camera modes, hierarchy, inspector, assets |
| `BlueprintEditor.cs` | Native Ethereal Veni visual scripting graph |

## Camera modes

The viewport supports isometric, third-person, over-the-shoulder, free, and cinematic modes. These are editor previews of runtime camera contracts.

## Source links

Each source path above is relative to `Editor/`. GitHub renders the paths as source navigation links in the repository.
