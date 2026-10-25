# Workspace Architecture

Ethereal Veni uses a portable workspace model to provide Unity/Unreal-inspired productivity without copying either editor implementation.

## Layers

1. Project workspace: portable project and editor configuration.
2. Layout: dock/panel arrangement and active tools.
3. User profile: local preferences and device bindings.
4. Cache: disposable machine-local data.

The runtime engine remains independent from the editor workspace.
