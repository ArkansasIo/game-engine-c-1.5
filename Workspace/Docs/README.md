# Project Overview

This project is a modular 2D MMORPG engine inspired by GoonZu, featuring a classic MMO UI, player-driven economy, and extensible systems for Unity.

## Folders
- `Assets/GameEngine/` — Core engine scripts and systems
- `Assets/uUI/` — Modular MMO UI system
- `Assets/AIAssetTools/` — Scripts for managing AI-generated assets
- `Assets/GoonzuGame/` — Game-specific assets organized by type (Characters, Buildings, Items, UI, etc.)
- `Docs/` — Game design and planning documents
- `Docs/AIAssetPlatforms/` — Documentation for AI asset generation tools
- `UML/` — UML diagrams (Mermaid format)

## Getting Started
1. Open in Unity.
2. Review the GoonzuUI scripts and connect to your prefabs.
3. See `GameDesignDocument.md` for design details.
4. Use AI tools to generate assets (see `Docs/AIAssetPlatforms/` and asset prompts below).
5. Place generated assets in `Assets/GoonzuGame/` folders.
6. Use `Assets/AIAssetTools/AssetOrganizer.cs` to organize assets.

## AI Asset Pipeline Tools

The project includes comprehensive Unity Editor tools for managing AI-generated assets:

### AI Toolbox (`Tools > AI Asset Tools > Toolbox`)
Central hub for all AI tools:
- Quick access to all pipeline tools
- One-click project setup
- Core asset generation
- Documentation access

### Pipeline Demo (`Tools > AI Asset Tools > Pipeline Demo`)
Interactive demonstrations of all key features:
- Automatic Import: PNGs placed in folders get auto-configured
- Batch Generation: Multiple assets generated with API simulation
- Asset Validation: Checks import success and sprite creation
- Sprite Atlases: Performance optimization for UI and icons
- Category Detection: Smart file naming and path assignment
- Rate Limiting: Prevents API overload during batch generation

### Pipeline Validator (`Tools > AI Asset Tools > Pipeline Validator`)
Comprehensive validation system:
- Tests all pipeline features individually
- Validates automatic import settings
- Checks batch generation capabilities
- Verifies asset validation systems
- Confirms sprite atlas creation
- Tests category detection logic
- Validates rate limiting implementation

### Goonzu Asset Generator (`Tools > AI Asset Tools > Goonzu Asset Generator`)
Specialized medieval Goonzu asset creation:
- **2D Assets**: Characters, buildings, items, creatures, UI, tiles
- **3D Assets**: Models for props, weapons, architecture
- **Authentic Style**: Medieval fantasy with Goonzu anime-inspired aesthetics
- **Batch Generation**: Create multiple assets per category
- **Auto-Categorization**: Smart placement in correct folders

**Generated Asset Categories:**
- Characters: Adventurers, NPCs, guards, merchants
- Buildings: Houses, shops, castles, churches
- Items: Weapons, armor, materials, consumables
- Creatures: Mounts (horses, camels) and monsters (goblins, wolves)
- UI: Panels, icons, bars, windows
- Tiles: Terrain textures (grass, stone, water)
- 3D Models: Props, furniture, architecture

## Quick Start Asset Pipeline

1. **Setup Project**: Run `Tools > AI Asset Tools > Asset Pipeline` and click "Setup Project"
2. **Generate Sample Assets**: Run `./generate_goonzu_assets.sh` to create placeholder assets
3. **Generate Assets**: Use the placeholder .txt files in `Assets/GoonzuGame/` folders as prompts for AI tools
4. **Import Assets**: Place generated PNGs in appropriate folders - they'll auto-import with correct settings
5. **Optimize**: Run "Import & Optimize" in the Asset Pipeline tool

## Generated Assets

The project now includes **27 placeholder assets** across all categories:

- **Characters (4)**: Medieval adventurer, merchant, blacksmith, town guard
- **Buildings (4)**: Town house, blacksmith shop, market stall, castle tower
- **Items (4)**: Broadsword, knight helmet, iron ore, health potion
- **Creatures (3)**: War horse, goblin, orc warrior
- **UI (4)**: Inventory panel, gold coin icon, sword icon, health bar
- **Tiles (3)**: Cobblestone, grass field, dirt road

Each placeholder contains the exact AI prompt needed to generate the real asset.

## Asset Categories

- **Characters/**: Player sprites and NPCs (64x64, pixel perfect)
- **Buildings/**: Town structures (256-512px, isometric)
- **Tiles/**: Terrain textures (64x64, seamless)
- **Items/**: Weapons, armor, materials, consumables (64x64, UI)
- **Creatures/**: Mounts and monsters (64x64, pixel perfect)
- **UI/**: Interface elements (panels, icons, bars, windows)

## Asset Generation
Use the 100+ prompts listed below to generate GoonZu-style assets with Recraft AI or other tools. The `Assets/GoonzuGame/` folder is pre-organized with placeholder files containing the exact prompts to use.

---
Add more documentation as your project grows.

## AI Asset Generation Prompts

Below is a 100+ ready-to-use prompt library for Recraft AI (or Leonardo, Stable Diffusion, Midjourney) to generate a complete GoonZu-style MMORPG asset set.

All prompts are optimized for:

✔ consistent fantasy MMO style
✔ UI readability
✔ inventory icon clarity
✔ isometric & top-down gameplay
✔ economy & crafting systems

**STYLE BASE (append to prompts)**

Use this to keep outputs consistent:

anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background

### Player Characters (10)

1. male fantasy adventurer, light armor, standing pose, {style}
2. female fantasy adventurer, leather armor, {style}
3. young merchant player character, travel clothing, {style}
4. warrior class hero, sword and shield, {style}
5. crafter character with tool belt, {style}
6. novice villager player character, simple clothing, {style}
7. elite knight player armor set, heroic stance, {style}
8. fantasy ranger character, cloak and bow, {style}
9. traveling trader character, backpack and goods, {style}
10. beginner starter character outfit, {style}

### NPCs & Town Citizens (15)

11. friendly town merchant NPC, medieval clothing, {style}
12. blacksmith NPC holding hammer, apron, {style}
13. town guard soldier with spear and shield, {style}
14. village farmer NPC with straw hat, {style}
15. elderly town elder NPC, wise appearance, {style}
16. traveling potion seller NPC, {style}
17. banker NPC inside fantasy town, {style}
18. guild master NPC, decorated robe, {style}
19. market trader selling vegetables, {style}
20. horse stable keeper NPC, {style}
21. fisher NPC holding fishing rod, {style}
22. tavern keeper NPC, friendly face, {style}
23. traveling bard NPC with lute, {style}
24. town child NPC playful pose, {style}
25. city official NPC formal attire, {style}

### Buildings & Town Structures (15)

26. isometric medieval town house, timber frame, {style}
27. fantasy blacksmith shop building, {style}
28. isometric market stall with colorful canopy, {style}
29. fantasy tavern building exterior, {style}
30. town warehouse storage building, {style}
31. small medieval chapel building, {style}
32. potion shop building with bottles sign, {style}
33. stable building for mounts, {style}
34. town hall building fantasy style, {style}
35. player marketplace booth, {style}
36. medieval crafting workshop building, {style}
37. lumber mill building fantasy village, {style}
38. stone watchtower defense building, {style}
39. city gate entrance structure, {style}
40. guild hall building ornate design, {style}

### Tilesets & Terrain (12)

41. seamless grass tile texture, top-down, {style}
42. dirt road tile seamless texture, {style}
43. stone plaza tile medieval town, {style}
44. wooden floor tile texture, {style}
45. sand terrain tile, {style}
46. forest ground tile leaves and grass, {style}
47. cobblestone street tile, {style}
48. farmland soil tile texture, {style}
49. snowy ground tile fantasy style, {style}
50. swamp mud tile texture, {style}
51. cliff rock edge tile, {style}
52. water edge shoreline tile, {style}

### Weapons & Equipment Icons (15)

53. fantasy sword inventory icon, {style}
54. steel shield icon RPG equipment, {style}
55. battle axe icon fantasy weapon, {style}
56. wooden training sword icon, {style}
57. longbow weapon icon, {style}
58. magic staff icon glowing crystal, {style}
59. iron helmet armor icon, {style}
60. leather armor chest piece icon, {style}
61. knight plate armor icon gold trim, {style}
62. boots armor icon fantasy style, {style}
63. gloves armor icon RPG equipment, {style}
64. ring accessory icon fantasy gem, {style}
65. necklace accessory icon glowing gem, {style}
66. cape cloak equipment icon, {style}
67. tool hammer crafting equipment icon, {style}

### Crafting Materials & Resources (10)

68. iron ore resource icon, {style}
69. wood logs crafting material icon, {style}
70. leather crafting material icon, {style}
71. herbs bundle resource icon, {style}
72. gold coin currency icon, {style}
73. silver ingot resource icon, {style}
74. gemstone crafting resource icon, {style}
75. wheat bundle farming resource icon, {style}
76. fish food resource icon, {style}
77. coal crafting resource icon, {style}

### Consumables & Potions (8)

78. red health potion bottle icon, {style}
79. blue mana potion bottle icon, {style}
80. stamina potion green bottle icon, {style}
81. buff potion glowing vial icon, {style}
82. antidote potion icon, {style}
83. food bread consumable icon, {style}
84. roasted meat food item icon, {style}
85. cheese food consumable icon, {style}

### Creatures & Mounts (8)

86. cute fantasy horse mount side view, {style}
87. donkey pack mount RPG style, {style}
88. fantasy ox mount for transport, {style}
89. small slime monster enemy, {style}
90. goblin creature enemy cartoon style, {style}
91. forest wolf enemy creature, {style}
92. friendly farm chicken animal, {style}
93. fantasy cow livestock animal, {style}

### UI & HUD Elements (12)

94. fantasy MMORPG inventory window frame, {style}
95. RPG hotbar skill bar UI frame, {style}
96. ornate fantasy health and mana bars, {style}
97. character stats panel fantasy UI, {style}
98. crafting window interface panel, {style}
99. marketplace trading interface panel, {style}
100. quest log window fantasy UI frame, {style}
101. dialogue box UI fantasy frame, {style}
102. minimap frame fantasy style UI, {style}
103. buff debuff icon frames UI, {style}
104. party member frame UI fantasy style, {style}
105. guild interface window frame, {style}

**Recraft AI Settings (IMPORTANT)**

Use:

✔ Transparent background ON
✔ Vector or PNG output
✔ Consistent palette
✔ Batch generation for icon sets
