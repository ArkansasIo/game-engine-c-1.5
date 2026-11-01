# UE5 Game Menus

The interface has two layers:

| Layer | Code | Draws |
|---|---|---|
| **MMO interface (Slate)** | `Source/EtherealVeni/UI/` | Micro menu bar, Game Menu, cascading dropdowns and sub-menus, Options window, key bindings, Help & Support, confirmation popups, messages |
| **HUD (Canvas)** | `Source/EtherealVeni/GZ/GZHUD.*` | Status frame, minimap, target frame, chat, quick bar, and the game windows (Character, Inventory, Skills, Manufacture, Market, Town, Quests, World Map, Community, How to Play) |

`AGZHUD::BeginPlay` attaches the Slate layer. Once it is attached, the HUD stops drawing its own menu bar, and window hotkeys plus Escape become rebindable key bindings. Without it, the HUD's original launcher and fixed hotkeys still work (see *Canvas fallback* at the end).

Nothing here needs UMG assets. Panel tabs can still host a UMG widget (`FEVPanelTab::WidgetClass`).

## Menu tree

```
Micro menu (bottom right)               left-click            right-click
  Character Info  [C] ................. Character window      -> unit.self
  Inventory [I] / Skills [K] / Manufacture [U] / Market [B]
  Quest Log [L]
  World Map [M] ....................... World Map window      -> minimap
  Town Hall [N] / Community & Guilds [J] / How to Play [H]
  Help & Support [Shift+H] ............ Help & Support panel
  Game Menu [F10 / Esc] ............... Game Menu             -> system

Game Menu (Esc)
  Options ............................. Options window
  Interface  >  (system.interface)
  Key Bindings ........................ Options > Key Bindings
  Help & Support ...................... Getting Started | Key Reference | Report a Bug
  Log Out ............................. confirm -> LogoutMap or RestartLevel
  Exit Game ........................... confirm -> quit
  Return to Game

system (right-click the Game Menu button)
  Options  >  Gameplay  >  Controls, Interface, Combat, Social, Accessibility
              Graphics  >  Display, Quality
              Audio     >  Volume
              Key Bindings
  Interface  >  [x] Menu Bar, Zone & Clock Banner, Minimap, Target Frame, Chat Window,
                    Floating Combat Text, Tooltips
                Menu Scale >  (o) 80% 90% 100% 110% 125% 140%
                HUD Scale  >  (o) 80% ... 140%
                More Interface Options...
  Key Bindings / Close All Windows / Reset Window Positions / Hide Interface / Take Screenshot
  Help & Support / Log Out / Exit Game

unit.self (right-click your portrait)
  Character Info / Inventory / Skills
  Status  >  (o) Online  Away  Busy
  Cancel

unit.target (right-click the target frame, or Shift+F10)
  Attack / Inspect
  Target Marker  >  (o) Star Circle Diamond Triangle Moon Square Cross Skull | None
  Clear Target / Cancel

minimap (right-click the minimap)
  Tracking: [x] Townsfolk & Merchants  [x] Monsters  [x] Gathering Nodes
  Zoom  >  (o) Near  Normal  Far
  Open World Map

chat (right-click the chat window)
  Show Channel  >  (o) All System Combat NPC Quest Town
  [x] System Messages in "All"
  Clear Chat Log / Hide Chat Window
```

The Options sub-menus are generated from the Options category tree, so the two can't drift apart.

## Dropdown behaviour

- **Opening sub-menus:** hovering a sub-menu row opens it beside the row. Near the screen edge it flips to the left and is clamped vertically.
- **Row types:** checkbox rows keep the menu open. Radio and action rows close it.
- **Shortcuts:** the right column shows the action's current key binding.
- **Closing:** clicking anywhere outside closes every level. Esc closes dropdowns first, then the popup, then the top panel, then the top HUD window. With nothing open, Esc shows the Game Menu.
- **Keyboard:** Up/Down select, Right/Enter open a sub-menu, Left goes back a level, Enter/Space activate.

## Options

Options has a category tree on the left and the selected page on the right. The bottom buttons are **Defaults** (current category), **Okay**, **Cancel** and **Apply**. Edits stay pending until applied.

| Category | Settings | Wired to |
|---|---|---|
| Gameplay > Controls | Click to Move, Auto-Target, Camera Rotation Step | `AGZIsoCharacter` |
| Gameplay > Interface | Menu & Window Scale, HUD Scale, menu bar / clock / target / chat / minimap visibility, minimap tracking + zoom, tooltips | Slate root DPI scale, `AGZHUD` |
| Gameplay > Combat | Floating Combat Text, Marker Above Target | `AGZHUD` |
| Gameplay > Social | Status (shown next to your name), System messages in "All" | `AGZHUD` |
| Gameplay > Accessibility | HUD text shadows, HUD panel opacity | `AGZHUD` |
| Graphics > Display | Display mode, resolution, VSync, max frame rate, render scale, gamma | `UGameUserSettings`, `r.ScreenPercentage`, `GEngine->DisplayGamma` |
| Graphics > Quality | Overall preset + 10 scalability groups | `UGameUserSettings` |
| Audio > Volume | Enable sound, master, music, effects, sound in background | audio device primary volume, `UGZAudioSubsystem`, `FApp` |
| Key Bindings | Two slots per command; click and press, right-click clears, Esc cancels | saved immediately |

Values are saved to `GameUserSettings.ini` in section `[/Script/EtherealVeni.EVUI]`, for example `ui.scale=1.0` or `Bind.window.character=C;None`. Graphics values are read from and written to `UGameUserSettings` directly.

Rebinding refuses keys the character reads every frame: W A S D, arrows, Q, E, Space, Tab, G, T, P and F1-F5. Binding a chord that another command already uses unbinds that command and shows a message.

## API: `UEVUISubsystem` (one per local player)

Get it with `UEVUISubsystem::Get(PlayerController)` in C++, or **Get MMO Interface** in Blueprint. Everything is addressed by `FName` id.

| Area | Calls |
|---|---|
| Actions | `RegisterAction(Id, Execute, IsChecked, IsEnabled)` (C++), `ExecuteAction`, `IsActionChecked/Enabled`, event `OnAction` |
| Menus | `RegisterMenu(FEVMenuDefinition)`, `AddMenuItem(Menu, Item, InsertAfter)`, `RemoveMenuItem`, `SetMenuItemEnabled`, `OpenContextMenu(Menu, Context)`, `OpenMenuAt`, `CloseMenus`, `GetMenuContext`, event `OnMenuItemActivated` |
| Panels | `RegisterPanel(FEVPanelDefinition)` (also creates action `panel.<id>`), `RegisterPanelContent(Panel, Tab, Factory)` (C++), `Open/Close/TogglePanel`, `SetPanelTab`, `ResetPanelPositions`, event `OnPanelChanged` |
| Micro menu | `RegisterMicroButton(FEVMicroButton)` |
| Options | `RegisterOptionsCategory`, `RegisterSetting`, `RegisterSettingApplier` / `RegisterSettingReader` (C++), `Get/SetSetting*`, `OpenOptions(Category)`, event `OnSettingChanged` |
| Bindings | `RegisterKeyBinding`, `Get/SetBinding`, `ClearBinding`, `ResetAllBindings`, `BeginKeyCapture`, `RegisterReservedKey` (C++) |
| Popups | `ShowConfirmation(Title, Message, ConfirmActionId, Label)`, `ShowConfirmationNative` (C++), `ShowMessage(Text, Color)` |
| Input | `HandleEscape`, `RegisterEscapeHandler` (C++), `SetInterfaceVisible` |

Menu item kinds (`EEVMenuItemKind`):

- `Action`: runs `ActionId`.
- `Submenu`: opens `SubmenuId`.
- `Toggle`: flips the bool setting `SettingKey`, or runs `ActionId` (checked state comes from the action).
- `Radio`: sets `SettingKey` to `RadioValue`, or runs `ActionId`.
- `Separator` and `Header`: layout only.

A menu `Title` may contain `{Context}`, which is replaced by the string passed to `OpenContextMenu`, such as the target's name.

Panels dock into `Left`, `Center` or `Right`. Opening a panel replaces the one already in that area. With `bReturnToPrevious`, closing it brings the replaced panel back (Options opened from the Game Menu returns to the Game Menu). `Free` panels never replace anything. Every panel can be dragged by its title bar.

### Example: add a "Mail" window with a sub-menu entry

```cpp
UEVUISubsystem* UI = UEVUISubsystem::Get(PC);

FEVPanelDefinition Mail;
Mail.PanelId = "mail";
Mail.Title = INVTEXT("Mailbox");
Mail.Area = EEVPanelArea::Left;
FEVPanelTab Inbox;  Inbox.TabId = "inbox"; Inbox.Label = INVTEXT("Inbox");
FEVPanelTab Send;   Send.TabId = "send";   Send.Label = INVTEXT("Send Mail");   // or Send.WidgetClass = <UMG class>
Mail.Tabs = {Inbox, Send};
UI->RegisterPanel(Mail);                                   // creates action "panel.mail"
UI->RegisterPanelContent("mail", "inbox", []{ return SNew(STextBlock).Text(INVTEXT("No mail.")); });

UI->AddMenuItem("system", FEVMenuItem::MakeAction("mail", INVTEXT("Mailbox"), "panel.mail"), "keybindings");

FEVKeyBindingDefinition Key;
Key.ActionId = "panel.mail";
Key.Label = INVTEXT("Mailbox");
Key.Group = INVTEXT("Interface Panels");
Key.DefaultPrimary = FEVKeyChord(EKeys::Y);
UI->RegisterKeyBinding(Key);
```

In Blueprint: build an `FEVPanelDefinition` whose tabs set `WidgetClass` to your UMG widget, call **Register Panel**, then **Add Menu Item**. Bind **On Action** to handle custom action ids that have no C++ handler.

`EVUIDefaultContent.cpp` registers everything listed above and is the reference for new content.

## Source files

| File | Purpose |
|---|---|
| `UI/EVUITypes.h` | Data structs and enums |
| `UI/EVUISubsystem.*` | The API, settings and binding persistence, Escape and hotkey routing (Slate input pre-processor) |
| `UI/EVUIDefaultContent.*` | This game's menus, panels, settings, bindings and actions |
| `UI/EVUIStyle.*` | Palette, fonts, brushes, widget styles |
| `UI/SEVRoot.*` | Viewport overlay: panel layer, micro menu, messages, popup, dropdown stack |
| `UI/SEVMenu.*` | Cascading dropdown level |
| `UI/SEVPanels.*` | Window frame with tabs, Game Menu, micro menu, confirmation popup |
| `UI/SEVOptions.*` | Options window and Key Bindings page |

## Canvas fallback (`AGZHUD`)

If the Slate layer isn't attached, `AGZHUD` keeps its original behaviour:

- **Menu bar:** the canvas draws its own bottom-right menu bar.
- **Hotkeys:** C, I, K, U, B, N, L, M, J and H toggle windows, and Esc/F10 opens the canvas launcher.
- **Launcher contents:** Character, World, Crafting & Trade, Social, Help and System groups, driven by `GetGZMenuRegistry()`.

Its Blueprint API (`OpenWindow`, `ToggleWindow`, `GetMenuEntries`, `ActivateMenuEntryById`, `OnWindowStateChanged` …) is unchanged. The HUD also exposes `CloseTopmostWindow`, `CloseAllWindows`, chat filter and target marker accessors, which the MMO interface uses.
