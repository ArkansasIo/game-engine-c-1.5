# Ethereal Veni Runtime

Ethereal Veni is the standalone .NET game-engine platform for **Ethereo Pandora**.

## Product identity
- Engine: Ethereal Veni 1.5
- Game: Ethereo Pandora
- Runtime: .NET 10, 64-bit
- Gameplay: RPG / MMORPG foundations with an authoritative-server design
- Renderer prototype: OpenTK / OpenGL on Windows
- Integrations: Unreal Engine 5 adapter and native runtime are independent targets

## Projects

| Project | Purpose | Target |
|---|---|---|
| Engine | Runtime-neutral engine and gameplay systems | .NET 10 |
| Game | Ethereo Pandora desktop prototype | Windows x64 |
| Editor | Standalone editor host | Windows x64 |
| Server | ASP.NET Core authoritative server host | .NET 10 |

## Build

From this directory:

```powershell
dotnet restore Engine/EtherealVeni.Engine.csproj
dotnet build Engine/EtherealVeni.Engine.csproj -c Release
dotnet build Server/EtherealVeni.Server.csproj -c Release
dotnet build Game/EthereoPandora.Game.csproj -c Release
dotnet build Editor/EtherealVeni.Editor.csproj -c Release
```

The Windows desktop applications target `net10.0-windows`. Build them on Windows for native execution. `EnableWindowsTargeting` permits restore/build from non-Windows CI agents when the required targeting packs are available.

## Architecture rules
1. Keep the Engine independent of WinForms, OpenTK, Unity, Unreal, and database providers.
2. Keep the Editor independent of the Game executable. Editor tooling talks to engine/runtime abstractions.
3. Keep client-submitted data untrusted. The Server owns authoritative combat, progression, inventory, and persistence decisions.
4. Keep stable IDs stable. Use explicit migration maps for schema or identity changes.
5. Keep licensed console SDKs and credentials out of the repository.

## Current gameplay modules
- Attributes and derived statistics
- Character progression and XP curve
- Damage calculation and combatant validation
- Item definitions and stack-aware inventory
- Data-driven quest objectives and quest state

## Current status
This repository contains a playable Windows 3D prototype and foundational .NET gameplay systems. It is not yet a production MMO. Authentication, persistence transactions, transport security, observability, anti-cheat, integration tests, and load testing remain production milestones.

See [Docs/3D_PROTOTYPE_STATUS.md](Docs/3D_PROTOTYPE_STATUS.md), [Docs/RPG_CORE.md](Docs/RPG_CORE.md), and [Engine/Gameplay/README.md](Engine/Gameplay/README.md).
