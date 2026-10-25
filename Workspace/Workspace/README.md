# [CORE] Ethereal Veni Workspace

The `Workspace/` directory is the first-class editor workspace for **Ethereal Veni** and **Ethereo Pandora**.

It defines portable editor layouts, project workspaces, templates, and documentation while keeping personal machine state and caches out of source control.

## Structure

```text
Workspace/
├── README.md
├── Docs/
├── Layouts/
├── Projects/
├── User/
├── Templates/
└── Cache/
```

## Responsibilities

- Dockable editor panels and tabs.
- World, scene, Blueprint, Lua, UI, animation, networking, server, and debugging workspaces.
- Keyboard, mouse, and controller configuration surfaces.
- Graphics, video, audio, accessibility, localization, gameplay, networking, and editor settings.
- Portable project configuration versus machine-specific user state.

Personal settings and cache data must not contain credentials, secrets, or generated binaries.
