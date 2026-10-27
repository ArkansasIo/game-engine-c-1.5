#!/bin/bash

# Goonzu Asset Generator - Command Line Version
# This script generates placeholder assets based on the prompts in the GoonzuGame folder

echo "=== Medieval Goonzu Asset Generator ==="
echo "Generating 200+ placeholder assets for Goonzu-style MMORPG"
echo

# Function to create a simple colored PNG placeholder
create_placeholder_png() {
    local filepath="$1"
    local prompt="$2"
    local dirname=$(dirname "$filepath")

    # Create directory if it doesn't exist
    mkdir -p "$dirname"

    # Create a simple colored PNG using ImageMagick if available, otherwise create a text file
    if command -v convert &> /dev/null; then
        # Create a 64x64 colored PNG
        convert -size 64x64 xc:"rgb($((RANDOM % 256)),$((RANDOM % 256)),$((RANDOM % 256)))" "$filepath"
        echo "✓ Created PNG: $filepath"
    else
        # Fallback: create a text file indicating what the PNG should be
        echo "# PNG Placeholder: $(basename "$filepath")" > "${filepath%.png}.txt"
        echo "# This should be replaced with actual AI-generated PNG" >> "${filepath%.png}.txt"
        echo "# Prompt: $prompt" >> "${filepath%.png}.txt"
        echo "✓ Created text placeholder: ${filepath%.png}.txt (ImageMagick not available)"
    fi
}

# Counter for total assets
total_assets=0

# Generate character assets (50 total)
echo "Generating character assets..."
classes=("warrior" "mage" "rogue" "cleric" "paladin" "ranger" "bard" "necromancer" "druid" "monk")
races=("human" "elf" "dwarf" "orc" "halfling" "gnome" "half-elf" "tiefling")
genders=("male" "female")

for class in "${classes[@]}"; do
    for race in "${races[@]}"; do
        for gender in "${genders[@]}"; do
            name="${race}_${gender}_${class}"
            prompt="${gender} ${race} ${class}, medieval fantasy character, detailed armor and clothing, heroic pose, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"
            create_placeholder_png "Assets/GoonzuGame/Characters/${name}.png" "$prompt"
            ((total_assets++))
        done
    done
done

# Additional NPC variations (20 total)
npcs=("merchant" "blacksmith" "guard" "innkeeper" "priest" "mayor" "farmer" "hunter" "beggar" "noble")
for npc in "${npcs[@]}"; do
    for gender in "${genders[@]}"; do
        name="${npc}_${gender}"
        prompt="${gender} medieval ${npc} NPC, detailed clothing and accessories, friendly pose, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"
        create_placeholder_png "Assets/GoonzuGame/Characters/NPCs/${name}.png" "$prompt"
        ((total_assets++))
    done
done

# Generate building assets (30 total)
echo "Generating building assets..."
buildings=("town_house" "castle" "tower" "keep" "blacksmith_shop" "tavern" "temple" "library" "market_stall" "stable" "barracks" "guild_hall" "apothecary" "bakery" "tailor_shop" "jeweler" "armory" "inn" "chapel" "watchtower" "gatehouse" "bridge" "mill" "farmhouse" "lumberyard" "mine_entrance" "dungeon_entrance" "portal" "obelisk" "fountain")
for building in "${buildings[@]}"; do
    prompt="medieval fantasy ${building//_/ }, detailed stone and wood architecture, ornate details, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset"
    create_placeholder_png "Assets/GoonzuGame/Buildings/${building}.png" "$prompt"
    ((total_assets++))
done

# Generate weapon assets (25 total)
echo "Generating weapon assets..."
weapons=("broadsword" "longsword" "dagger" "battleaxe" "warhammer" "mace" "staff" "wand" "bow" "crossbow" "spear" "halberd" "scimitar" "rapier" "claymore" "flail" "morningstar" "quarterstaff" "shortbow" "longbow" "throwing_knife" "sling" "whip" "katana" "scythe")
for weapon in "${weapons[@]}"; do
    prompt="medieval fantasy ${weapon//_/ }, ornate design with gemstones, detailed metalwork, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"
    create_placeholder_png "Assets/GoonzuGame/Items/Weapons/${weapon}.png" "$prompt"
    ((total_assets++))
done

# Generate armor assets (20 total)
echo "Generating armor assets..."
armors=("plate_helmet" "chainmail_helmet" "leather_helmet" "wizard_hat" "knight_armor" "chainmail_armor" "leather_armor" "robe" "cloak" "shield" "buckler" "tower_shield" "gauntlets" "boots" "belt" "pauldrons" "vambraces" "greaves" "cuirass" "surcoat")
for armor in "${armors[@]}"; do
    prompt="medieval fantasy ${armor//_/ }, ornate design with engravings, detailed metal and leather work, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"
    create_placeholder_png "Assets/GoonzuGame/Items/Armor/${armor}.png" "$prompt"
    ((total_assets++))
done

# Generate consumable assets (15 total)
echo "Generating consumable assets..."
consumables=("health_potion" "mana_potion" "strength_potion" "agility_potion" "intelligence_potion" "bread" "cheese" "apple" "wine" "ale" "herb_bundle" "magic_crystal" "scroll" "rune_stone" "elixir")
for item in "${consumables[@]}"; do
    prompt="medieval fantasy ${item//_/ }, detailed bottle or item design, glowing magical effects, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"
    create_placeholder_png "Assets/GoonzuGame/Items/Consumables/${item}.png" "$prompt"
    ((total_assets++))
done

# Generate material assets (10 total)
echo "Generating material assets..."
materials=("iron_ore" "gold_ore" "mithril_ore" "adamantite_ore" "wood_log" "leather_hide" "cloth_bolt" "gemstone" "bone" "crystal")
for material in "${materials[@]}"; do
    prompt="medieval fantasy ${material//_/ }, raw material resource, detailed texture, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"
    create_placeholder_png "Assets/GoonzuGame/Items/Materials/${material}.png" "$prompt"
    ((total_assets++))
done

# Generate creature assets (25 total)
echo "Generating creature assets..."
creatures=("dragon" "griffin" "phoenix" "unicorn" "war_horse" "dire_wolf" "giant_spider" "troll" "ogre" "goblin" "orc_warrior" "skeleton_warrior" "zombie" "vampire" "werewolf" "minotaur" "centaur" "harpy" "mermaid" "golem" "elemental" "demon" "angel" "fairy" "beast")
for creature in "${creatures[@]}"; do
    prompt="medieval fantasy ${creature//_/ }, detailed creature design, magical aura, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"
    create_placeholder_png "Assets/GoonzuGame/Creatures/${creature}.png" "$prompt"
    ((total_assets++))
done

# Generate UI assets (25 total)
echo "Generating UI assets..."
ui_panels=("inventory_panel" "character_panel" "skill_panel" "quest_panel" "map_panel" "trade_panel" "crafting_panel" "guild_panel" "settings_panel")
for panel in "${ui_panels[@]}"; do
    prompt="medieval fantasy ${panel//_/ }, ornate parchment border, detailed frame design, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"
    create_placeholder_png "Assets/GoonzuGame/UI/Panels/${panel}.png" "$prompt"
    ((total_assets++))
done

ui_icons=("sword_icon" "shield_icon" "potion_icon" "gold_coin" "mana_crystal" "health_heart" "experience_star" "quest_scroll" "map_marker" "chat_bubble" "friend_icon" "enemy_icon" "chest_icon" "key_icon" "lock_icon" "arrow_icon")
for icon in "${ui_icons[@]}"; do
    prompt="medieval fantasy ${icon//_/ }, detailed icon design, symbolic representation, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"
    create_placeholder_png "Assets/GoonzuGame/UI/Icons/${icon}.png" "$prompt"
    ((total_assets++))
done

ui_bars=("health_bar" "mana_bar" "experience_bar" "stamina_bar")
for bar in "${ui_bars[@]}"; do
    prompt="medieval fantasy ${bar//_/ }, ornate frame with liquid fill, detailed design, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"
    create_placeholder_png "Assets/GoonzuGame/UI/Bars/${bar}.png" "$prompt"
    ((total_assets++))
done

# Generate tile assets (20 total)
echo "Generating tile assets..."
tiles=("cobblestone" "stone_brick" "dirt_road" "grass_field" "forest_floor" "sand_dune" "snow_ground" "lava_rock" "water_surface" "wood_floor" "carpet_red" "carpet_blue" "marble_floor" "tile_floor" "mosaic_floor" "dirt_path" "gravel" "mud" "ice" "crystal_floor")
for tile in "${tiles[@]}"; do
    prompt="medieval fantasy ${tile//_/ } tile, seamless texture pattern, detailed surface, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset"
    create_placeholder_png "Assets/GoonzuGame/Tiles/${tile}.png" "$prompt"
    ((total_assets++))
done

# Generate effect assets (15 total)
echo "Generating effect assets..."
effects=("fire_spell" "ice_spell" "lightning_spell" "heal_spell" "poison_cloud" "smoke_puff" "dust_cloud" "sparkle_effect" "magic_circle" "explosion" "teleport_flash" "shield_barrier" "buff_aura" "debuff_curse" "level_up_burst")
for effect in "${effects[@]}"; do
    prompt="medieval fantasy ${effect//_/ }, magical particle effect, glowing energy, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"
    create_placeholder_png "Assets/GoonzuGame/Effects/${effect}.png" "$prompt"
    ((total_assets++))
done

# Generate furniture assets (10 total)
echo "Generating furniture assets..."
furniture=("wooden_chair" "stone_throne" "wooden_table" "bookshelf" "bed" "chest" "wardrobe" "anvil" "forge" "cauldron")
for item in "${furniture[@]}"; do
    prompt="medieval fantasy ${item//_/ }, detailed wood and metal construction, ornate design, anime-inspired fantasy MMORPG style, bright colors, clean outlines, soft shading, hand-painted look, game-ready asset, transparent background"
    create_placeholder_png "Assets/GoonzuGame/Furniture/${item}.png" "$prompt"
    ((total_assets++))
done

echo
echo "=== Asset Generation Complete ==="
echo "Generated $total_assets placeholder assets in Assets/GoonzuGame/"
echo "In Unity, these will be automatically imported with correct sprite settings."
echo "Replace with actual AI-generated assets from Recraft AI, Scenario AI, etc."
echo
echo "Asset count by category:"
echo "- Characters: 160 (10 classes × 8 races × 2 genders)"
echo "- NPCs: 20 (10 types × 2 genders)"
echo "- Buildings: 30"
echo "- Weapons: 25"
echo "- Armor: 20"
echo "- Consumables: 15"
echo "- Materials: 10"
echo "- Creatures: 25"
echo "- UI Panels: 9"
echo "- UI Icons: 16"
echo "- UI Bars: 4"
echo "- Tiles: 20"
echo "- Effects: 15"
echo "- Furniture: 10"
echo "- Total: $total_assets assets"
echo
echo "To generate more assets, use the Goonzu Asset Generator tool in Unity:"
echo "Tools > AI Asset Tools > Goonzu Asset Generator"