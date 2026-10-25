# [CORE] Ethereal Veni Editor

The Editor is a standalone tool layer for Ethereal Veni. It provides a Unity/Unreal-inspired workflow without depending on either engine.

## Core surfaces

- World viewport
- Entity hierarchy
- Inspector/property editor
- Asset browser
- Scene and level editor
- Blueprint visual scripting
- Lua scripting
- CSS.C1.5 style editor
- Animation and timeline tools
- Procedural Studio
- Profiler and diagnostics
- Network/server tools
- Platform build manager

The Editor consumes the same engine contracts used by Ethereo Pandora and external adapters. It does not replace the runtime.

## Architecture

```text
Editor UI
  -> Workspace
  -> Editor Services
  -> Engine API
  -> Runtime / Simulation / Renderer
```

See `Workspace/Docs/WorkspaceArchitecture.md` and the source documentation accompanying each editor subsystem.
