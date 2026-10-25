# Goonzu MMORPG Game Engine

A complete Unity-based medieval fantasy MMORPG framework with comprehensive game systems, asset management, and gameplay mechanics.

## Overview

This project provides a fully functional game engine for a medieval fantasy MMORPG called "Goonzu". The engine includes all major game systems needed for a complete RPG experience, utilizing 393+ game assets with proper animations, AI behaviors, and interactive gameplay.

## Features

### Core Systems
- **Asset Management**: Centralized system for loading and managing 393+ game assets
- **Character System**: Races, classes, stats, equipment, and progression
- **Animation System**: Sprite-based animations with effects and character states
- **Combat System**: Turn-based/AI combat with damage calculations and effects
- **Quest System**: Dynamic quest generation with objectives, rewards, and chains
- **Item System**: Weapons, armor, consumables, materials with rarity and stats
- **NPC System**: Interactive NPCs with dialogue, trading, and AI behaviors
- **Building System**: Interactive buildings with functions and states
- **World System**: Procedural terrain generation with biomes and zones
- **Save/Load System**: Complete game persistence with JSON/binary formats
- **UI System**: Comprehensive interface for all game interactions

### Game Assets (393+)
- **Characters**: 8 races × 6 classes × 4 directions × 3 animation states = 576 character sprites
- **Creatures**: 20+ monster types with animations and behaviors
- **Items**: Weapons, armor, accessories, consumables, materials
- **Buildings**: Castles, shops, temples, houses with interactive functions
- **Terrain**: Grass, stone, water, forest, mountain tiles
- **Effects**: Combat effects, magic spells, environmental effects
- **UI Elements**: Panels, bars, icons, buttons, tooltips

## Architecture

### Manager Classes
- `GoonzuGameManager`: Main game controller and state management
- `GoonzuAssetManager`: Asset loading and sprite management
- `GoonzuUIManager`: UI panel and interface management
- `GoonzuCombatManager`: Combat logic and battle management
- `GoonzuQuestManager`: Quest generation and tracking
- `GoonzuItemManager`: Item database and generation
- `GoonzuWorldManager`: World generation and zone management
- `GoonzuSaveLoadManager`: Game persistence

### Game Objects
- `GoonzuCharacter`: Player character with stats and equipment
- `GoonzuCreature`: Monsters with AI behaviors and loot
- `GoonzuNPC`: Interactive NPCs with dialogue and trading
- `GoonzuBuilding`: Interactive buildings with functions
- `GoonzuItem`: Items with stats, rarity, and properties

## Getting Started

### Prerequisites
- Unity 2020.3 or later
- .NET Framework 4.7.1 or later

### Installation
1. Clone or download the project
2. Open in Unity Editor
3. The main scene is located at `Assets/GoonzuGame/Scenes/MainScene.unity`
4. Press Play to start the game

### Project Structure
```
Assets/GoonzuGame/
├── Scripts/           # All game logic scripts
├── Sprites/           # Game asset sprites
├── Prefabs/           # Game object prefabs
├── Scenes/            # Unity scenes
├── Resources/         # Runtime-loaded assets
└── UI/               # UI prefabs and assets
```

## Game Systems Documentation

### Character System
Characters have 8 races and 6 classes with unique stat bonuses:

**Races:**
- Human: Balanced stats
- Elf: +Dexterity, +Intelligence
- Dwarf: +Constitution, +Strength
- Orc: +Strength, +Constitution
- Undead: +Wisdom, special abilities
- Demon: +Intelligence, +Charisma
- Angel: +Wisdom, +Charisma
- Beast: +Dexterity, +Constitution

**Classes:**
- Warrior: High strength, melee combat
- Mage: High intelligence, magic spells
- Rogue: High dexterity, stealth
- Priest: High wisdom, healing/support
- Ranger: Balanced, ranged attacks
- Paladin: Strength/wisdom, tank/support

### Combat System
- Turn-based combat with AI opponents
- Damage types: Physical, Magic, Fire, Ice, Lightning, Poison
- Critical hits, blocking, and special abilities
- Experience and loot rewards

### Quest System
- Dynamic quest generation
- Multiple quest types: Kill, Collect, Deliver, Explore, Craft, Talk, Defend, Escort
- Quest chains and prerequisites
- Experience and item rewards

### World System
- Procedural world generation
- Multiple biomes: Forest, Desert, Mountain, Plains, Swamp, Coastal
- Interactive buildings and NPCs
- Day/night cycle

### Item System
- 5 item types: Weapons, Armor, Accessories, Consumables, Materials
- 5 rarity levels: Common, Uncommon, Rare, Epic, Legendary
- Stat bonuses and special effects
- Crafting and trading systems

## Controls

### Gameplay
- **WASD/Arrow Keys**: Move character
- **Mouse**: Interact with objects/NPCs
- **Space**: Attack/Interact
- **I**: Open inventory
- **J**: Open quest log
- **M**: Open world map
- **Tab**: Character sheet
- **Escape**: Pause menu

### Debug (F1 to enable)
- **F2**: Add random item
- **F3**: Level up player
- **F4**: Teleport to random location
- **F5**: Quick save
- **F9**: Quick load

## API Reference

### Key Methods

#### GoonzuGameManager
```csharp
public void StartNewGame()
public void LoadGame(string saveName)
public void CreateCharacter(string name, CharacterRace race, CharacterClass charClass)
public GoonzuCharacter GetPlayer()
```

#### GoonzuCharacter
```csharp
public void InitializeCharacter(string name, CharacterRace race, CharacterClass charClass)
public void GainExperience(int amount)
public void AddItemToInventory(GoonzuItem item)
public void EquipItem(GoonzuItem item)
```

#### GoonzuQuestManager
```csharp
public void AcceptQuest(string questID)
public void CompleteQuest(string questID)
public List<GoonzuQuest> GetActiveQuests()
public void UpdateQuestProgress(QuestType type, string targetName, int amount = 1)
```

#### GoonzuItemManager
```csharp
public GoonzuItem GenerateRandomItem(int level = 1, ItemType? forcedType = null)
public GoonzuItem GetItemByID(string itemID)
public List<GoonzuItem> GetItemsByType(ItemType type)
```

## Customization

### Adding New Assets
1. Add sprite to `Assets/GoonzuGame/Sprites/` in appropriate subfolder
2. Update `GoonzuAssetManager` asset loading paths if needed
3. Add asset references to relevant manager classes

### Creating New Quests
```csharp
GoonzuQuest quest = new GoonzuQuest("quest_id", "Quest Name", "Description", "Giver", QuestDifficulty.Medium);
quest.AddObjective(new QuestObjective("Kill 5 goblins", QuestType.Kill, 5, "Goblin"));
questManager.AddQuest(quest);
```

### Adding New Items
```csharp
GoonzuItem item = new GoonzuItem();
item.itemType = ItemType.Weapon;
item.itemName = "Custom Sword";
item.damage = 15;
// ... set other properties
itemManager.AddItemToDatabase(item);
```

## Performance

### Optimization Features
- Object pooling for projectiles and effects
- Efficient sprite batching
- Zone-based world loading
- Asset streaming for large worlds
- UI element pooling

### Recommended Settings
- Target FPS: 60
- Max active objects: 1000
- Texture compression: Enabled
- Mipmap: Enabled

## Known Issues

- Some asset sprites may need manual adjustment for proper animation timing
- World generation can create isolated areas (future pathfinding improvements planned)
- Large worlds may experience performance issues on low-end hardware

## Future Development

### Planned Features
- Multiplayer networking
- Advanced AI behaviors
- Crafting and profession systems
- Weather and environmental effects
- Achievement system
- Guild and alliance systems
- Mount and pet systems
- Dungeon generation
- Magic spell system expansion

### Content Expansion
- Additional character races and classes
- New creature types and behaviors
- Expanded quest content
- New biomes and zones
- Seasonal events
- Player housing

## Contributing

To contribute to the project:
1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Test thoroughly
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.

## Credits

- Game design and programming by AI Assistant
- Asset creation and organization
- Unity framework and documentation

## Support

For support or questions:
- Check the documentation first
- Review the code comments
- Create an issue on the repository
- Join the community Discord

---

**Version**: 1.0.0
**Unity Version**: 2020.3+
**Last Updated**: 2024