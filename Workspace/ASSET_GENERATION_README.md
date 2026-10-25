# Goonzu Asset Generation Guide

This guide shows how to replace the 393 placeholder text files with actual AI-generated PNG sprites for your medieval GoonZu MMORPG.

## 🎨 Available Tools

### 1. `generate_ai_assets.py` - Python Script for AI Generation
- **Purpose**: Automated generation using AI APIs
- **Supports**: Recraft AI, Scenario AI (with API keys)
- **Output**: Direct PNG generation

### 2. `batch_processor.sh` - Batch Processing Helper
- **Purpose**: Organize prompts for manual generation
- **Features**: Create batches, validate progress, replace files

### 3. Exported Files
- `goonzu_prompts.json` - All prompts in JSON format
- `goonzu_prompts.csv` - All prompts in CSV format for spreadsheet processing

## 🚀 Quick Start - Manual Generation

### Step 1: Choose Your AI Platform
- **Recraft AI** (recraft.ai) - Best for anime-style sprites
- **Scenario AI** (scenario.gg) - Great for game assets
- **Midjourney** (Discord) - High quality, artistic
- **DALL-E** (ChatGPT) - Accessible, good results
- **Stable Diffusion** - Free, customizable

### Step 2: Use the Batch Processor
```bash
./batch_processor.sh
```
Choose option 3 to create batch files in `batch_files/` directory.

### Step 3: Generate Images
For each batch file:
1. Open your AI platform
2. Copy prompts from batch files
3. Generate images with these settings:
   - **Style**: Anime/Cartoon
   - **Resolution**: 512x512 or 1024x1024
   - **Format**: PNG with transparent background

### Step 4: Replace Placeholders
```bash
./batch_processor.sh
```
Choose option 5, then run:
```bash
./replace_placeholders.sh
```

## 🔧 Advanced Usage - API Integration

### Using Recraft AI
```bash
python3 generate_ai_assets.py --platform recraft --api-key YOUR_API_KEY
```

### Using Scenario AI
```bash
python3 generate_ai_assets.py --platform scenario --api-key YOUR_API_KEY
```

### Custom API Integration
Edit `generate_ai_assets.py` to add support for other AI platforms.

## 📊 Asset Categories (393 Total)

| Category | Count | Description |
|----------|-------|-------------|
| Characters | 186 | Player classes, NPCs, all races/genders |
| Buildings | 32 | Castles, shops, temples, houses |
| Weapons | 26 | Swords, bows, staffs, exotic weapons |
| Armor | 21 | Helmets, armor sets, accessories |
| Consumables | 15 | Potions, food, scrolls |
| Materials | 10 | Ores, woods, crafting materials |
| Creatures | 28 | Dragons, monsters, mounts |
| UI Elements | 29 | Panels, icons, bars |
| Tiles | 21 | Terrain, floors, textures |
| Effects | 15 | Spells, particles, auras |
| Furniture | 10 | Interior decorations |

## 🎯 Prompt Structure

Each prompt includes:
- **Medieval fantasy theme** with GoonZu styling
- **Anime-inspired MMORPG art style**
- **Bright colors, clean outlines, soft shading**
- **Hand-painted look, game-ready assets**
- **Transparent backgrounds** for sprites

Example prompt:
```
"male human warrior, medieval fantasy character, detailed armor and clothing, heroic pose, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"
```

## 🔍 Quality Control

### Validation Script
```bash
./batch_processor.sh
# Choose option 4 to check conversion progress
```

### Unity Import
Once PNGs are generated:
1. Unity automatically imports with correct sprite settings
2. Check `Assets/GoonzuGame/` folder
3. Use the AI Asset Tools in Unity for additional processing

## 🛠️ Troubleshooting

### Common Issues
- **API Rate Limits**: Add delays between requests
- **Image Quality**: Adjust prompts or use higher resolution
- **Style Consistency**: Use consistent AI model/settings

### Manual Processing
If API generation fails, use the batch files for manual processing with any AI tool.

## 📈 Progress Tracking

Monitor your progress:
```bash
# Check current status
find Assets/GoonzuGame -name "*.png" | wc -l  # Generated PNGs
find Assets/GoonzuGame -name "*.txt" | wc -l  # Remaining placeholders
```

## 🎮 Final Result

After completion, you'll have:
- ✅ 393 authentic medieval GoonZu sprites
- ✅ Consistent anime-inspired MMORPG style
- ✅ Ready for Unity import
- ✅ Complete asset library for your game

## 🤝 Need Help?

- Check the prompts in `goonzu_prompts.json`
- Use batch files for organized processing
- Start with character assets for testing
- Scale up to full library generation

Happy game developing! 🗡️⚔️🏰