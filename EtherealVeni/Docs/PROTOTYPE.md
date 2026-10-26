# Ethereal Veni + Ethereo Pandora Prototype

The repository now contains a runnable Windows desktop editor and playable 3D prototype.

## Projects

- `Engine/EtherealVeni.Engine.csproj` — engine/runtime API layer.
- `Editor/EtherealVeni.Editor.csproj` — dock-style editor workspace.
- `Game/EthereoPandora.Game.csproj` — playable Ethereo Pandora runtime.
- `Server/EtherealVeni.Server.csproj` — authoritative-server project.

## Editor

Run the editor project to open the Ethereal Veni workspace. It includes:

- Scene hierarchy
- 3D scene viewport
- Entity inspector
- Console/log panel
- Play, pause, stop, save, build and profiler controls
- Ethereo Pandora scene preview

Select an entity in the hierarchy to inspect its prototype component data. Press **PLAY** to launch the playable game window.

## Game prototype

The game runtime is a Windows Forms software-rendered 3D prototype so it has no external rendering SDK dependency. It currently provides:

- Perspective 3D terrain projection
- Procedural hills and vegetation
- Player character
- Multiple enemy archetypes
- Enemy pursuit AI
- Player movement
- Mouse camera rotation
- Basic melee/energy attack
- Health and mana
- Respawning enemies
- World-map panel
- Inventory menu
- Quest log
- Pause/menu overlay
- RPG/MMORPG-style HUD

### Controls

| Input | Action |
|---|---|
| `WASD` | Move |
| Right mouse drag | Rotate camera |
| Mouse wheel | Camera pitch |
| Left mouse / `Space` | Attack |
| `I` | Inventory |
| `Q` | Quest log |
| `Esc` | Pause/menu |

## Build

Requires the .NET 10 SDK and Windows desktop targeting support.

```powershell
dotnet restore EtherealVeni/EtherealVeni.sln
dotnet build EtherealVeni/EtherealVeni.sln -c Release

dotnet run --project EtherealVeni/Editor/EtherealVeni.Editor.csproj -c Release
```

To launch the game directly:

```powershell
dotnet run --project EtherealVeni/Game/EthereoPandora.Game.csproj -c Release
```

This prototype intentionally uses a software renderer for portability and fast iteration. The architecture can later replace the viewport backend with Direct3D 12, Vulkan, OpenGL, or a dedicated renderer while retaining the editor/game systems.
