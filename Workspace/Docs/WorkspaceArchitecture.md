# [CORE] Workspace Architecture

Ethereal Veni uses a portable workspace model so the editor can provide Unity/Unreal-inspired productivity without copying either editor implementation.

## Layers

1. **Project workspace** — portable layout and project-level editor preferences.
2. **User workspace** — local panel state, recent documents, and window geometry.
3. **Runtime configuration** — graphics, audio, input, networking, and platform options.
4. **Cache** — disposable indexes, thumbnails, shader caches, and profiling captures.

## Runtime boundary

The workspace system must not become a dependency of the game runtime. Runtime code consumes validated project settings; editor-only layout state stays in the editor process.

## Input devices

The common input layer supports keyboard, mouse, gamepad/controller, touch, and platform-specific devices. Actions are named independently from physical bindings so a single gameplay action can have multiple bindings.

## Compatibility

Workspace files are versioned. A future schema migration may transform old workspace JSON into the current version without changing serialized asset IDs.
