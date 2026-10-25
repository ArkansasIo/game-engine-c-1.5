# [CORE] Ethereal Veni Workspace

The `Workspace/` directory defines the persistent, portable editor workspace model for **Ethereal Veni** and **Ethereo Pandora**.

## Purpose

Workspace files describe editor layout, active panels, input devices, and user-facing engine settings without storing machine-specific editor cache or credentials.

## Structure

```text
Workspace/
├── README.md
├── Docs/
│   ├── README.md
│   ├── WorkspaceArchitecture.md
│   ├── WorkspaceLayouts.md
│   └── WorkspaceConfiguration.md
├── Layouts/
│   ├── Default.workspace.json
│   ├── GameDevelopment.workspace.json
│   ├── Blueprint.workspace.json
│   ├── WorldBuilding.workspace.json
│   ├── UI.workspace.json
│   ├── Animation.workspace.json
│   ├── Networking.workspace.json
│   ├── Server.workspace.json
│   └── Debugging.workspace.json
├── Projects/
│   ├── EtherealVeni.workspace.json
│   └── EthereoPandora.workspace.json
├── Templates/
│   ├── EngineWorkspace.template.json
│   ├── GameWorkspace.template.json
│   ├── BlueprintWorkspace.template.json
│   └── PluginWorkspace.template.json
└── Cache/
    └── .gitkeep
```

## Design rules

- Workspace templates and default layouts are version-controlled.
- Personal editor state and machine-local caches are not committed.
- Keyboard, mouse, controller, graphics, video, audio, accessibility, networking, and editor preferences are represented by versioned configuration schemas.
- Core engine configuration remains independent of Unity and Unreal SDKs.
- Platform-specific controls may extend the common input action model without changing gameplay APIs.

## Workspace vs. settings

A workspace answers **where editor tools are placed**. Settings answer **how the engine and editor behave**. Project settings are portable; user settings may be machine-specific.
