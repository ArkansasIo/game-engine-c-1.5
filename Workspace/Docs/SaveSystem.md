# Save System

This document describes the save/load slot system for the game engine.

## Save Slots
- 5 save slots available
- Each slot stores game state in a file (e.g., `save_slot_1.dat`)

## Features
- Save game to any slot
- Load game from any slot
- Current slot tracking
- List available slots

## Example Usage
```
engine.SaveGame(0); // Save to slot 1
engine.LoadGame(0); // Load from slot 1
engine.ListSaveSlots(); // Show all slots
```

## Customization
Save slot count and file naming can be adjusted.