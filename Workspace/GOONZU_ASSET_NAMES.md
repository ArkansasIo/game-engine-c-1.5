# Goonzu Asset Naming Convention

## 📋 **Complete Asset Library (393 Assets)**

### **🏰 Buildings (32)**
- `apothecary.png`, `armory.png`, `bakery.png`, `barracks.png`
- `blacksmith_shop.png`, `bridge.png`, `castle.png`, `castle_tower.png`
- `chapel.png`, `dungeon_entrance.png`, `farmhouse.png`, `fountain.png`
- `gatehouse.png`, `guild_hall.png`, `inn.png`, `jeweler.png`
- `keep.png`, `library.png`, `lumberyard.png`, `market_stall.png`
- `mill.png`, `mine_entrance.png`, `obelisk.png`, `portal.png`
- `stable.png`, `tailor_shop.png`, `tavern.png`, `temple.png`
- `town_house.png`, `tower.png`, `watchtower.png`, `windmill.png`

### **⚔️ Weapons (26)**
- `battleaxe.png`, `bow.png`, `broadsword.png`, `claymore.png`
- `crossbow.png`, `dagger.png`, `flail.png`, `halberd.png`
- `katana.png`, `longbow.png`, `longsword.png`, `mace.png`
- `magic_staff.png`, `morningstar.png`, `quarterstaff.png`, `rapier.png`
- `scimitar.png`, `scythe.png`, `shortbow.png`, `sling.png`
- `spear.png`, `throwing_knife.png`, `warhammer.png`, `whip.png`

### **🛡️ Armor (21)**
- `belt.png`, `boots.png`, `buckler.png`, `chainmail_armor.png`
- `chainmail_helmet.png`, `cloak.png`, `cuirass.png`, `gauntlets.png`
- `greaves.png`, `knight_armor.png`, `leather_armor.png`, `leather_helmet.png`
- `mage_robe.png`, `pauldrons.png`, `plate_helmet.png`, `robe.png`
- `shield.png`, `surcoat.png`, `tower_shield.png`, `vambraces.png`

### **🧪 Consumables (15)**
- `agility_potion.png`, `ale.png`, `apple.png`, `bread.png`
- `cheese.png`, `elixir.png`, `health_potion.png`, `herb_bundle.png`
- `intelligence_potion.png`, `magic_crystal.png`, `mana_potion.png`
- `rune_stone.png`, `scroll.png`, `strength_potion.png`, `wine.png`

### **🔧 Materials (10)**
- `adamantite_ore.png`, `bone.png`, `cloth_bolt.png`, `crystal.png`
- `gemstone.png`, `gold_ore.png`, `iron_ore.png`, `leather_hide.png`
- `mithril_ore.png`, `wood_log.png`

### **🐉 Creatures (28)**
- `angel.png`, `beast.png`, `centaur.png`, `demon.png`
- `dire_wolf.png`, `dragon.png`, `elemental.png`, `fairy.png`
- `giant_spider.png`, `goblin.png`, `golem.png`, `griffin.png`
- `harpy.png`, `mermaid.png`, `minotaur.png`, `ogre.png`
- `orc_warrior.png`, `phoenix.png`, `skeleton_warrior.png`, `troll.png`
- `unicorn.png`, `vampire.png`, `war_horse.png`, `werewolf.png`
- `zombie.png`

### **🎨 UI Elements (29)**

#### **Icons (16):**
- `arrow_icon.png`, `chat_bubble.png`, `chest_icon.png`, `enemy_icon.png`
- `experience_star.png`, `friend_icon.png`, `gold_coin.png`, `health_heart.png`
- `key_icon.png`, `lock_icon.png`, `mana_crystal.png`, `map_marker.png`
- `potion_icon.png`, `quest_scroll.png`, `shield_icon.png`, `sword_icon.png`

#### **Panels (9):**
- `character_panel.png`, `crafting_panel.png`, `guild_panel.png`, `inventory_panel.png`
- `map_panel.png`, `quest_panel.png`, `settings_panel.png`, `skill_panel.png`, `trade_panel.png`

#### **Bars (4):**
- `experience_bar.png`, `health_bar.png`, `mana_bar.png`, `stamina_bar.png`

### **🪄 Effects (15)**
- `buff_aura.png`, `debuff_curse.png`, `dust_cloud.png`, `explosion.png`
- `fire_spell.png`, `heal_spell.png`, `ice_spell.png`, `level_up_burst.png`
- `lightning_spell.png`, `magic_circle.png`, `poison_cloud.png`, `shield_barrier.png`
- `smoke_puff.png`, `sparkle_effect.png`, `teleport_flash.png`

### **🪑 Furniture (10)**
- `anvil.png`, `bed.png`, `bookshelf.png`, `cauldron.png`
- `chest.png`, `forge.png`, `stone_throne.png`, `wardrobe.png`
- `wooden_chair.png`, `wooden_table.png`

### **🗺️ Tiles (21)**
- `carpet_blue.png`, `carpet_red.png`, `cobblestone.png`, `crystal_floor.png`
- `dirt_path.png`, `dirt_road.png`, `forest_floor.png`, `grass_field.png`
- `ice.png`, `lava_rock.png`, `marble_floor.png`, `mosaic_floor.png`
- `mud.png`, `sand_dune.png`, `snow_ground.png`, `stone_brick.png`
- `tile_floor.png`, `water_surface.png`, `wood_floor.png`

### **👥 Characters (186)**
**Player Classes (160):** All combinations of:
- **Races (8):** `human`, `elf`, `dwarf`, `orc`, `halfling`, `gnome`, `half-elf`, `tiefling`
- **Genders (2):** `male`, `female`
- **Classes (10):** `warrior`, `mage`, `rogue`, `cleric`, `paladin`, `ranger`, `bard`, `necromancer`, `druid`, `monk`

**Format:** `[race]_[gender]_[class].png`
**Examples:** `human_male_warrior.png`, `elf_female_mage.png`, `dwarf_male_paladin.png`

**NPCs (20):** 10 types × 2 genders
- `merchant_female.png`, `merchant_male.png`
- `blacksmith_female.png`, `blacksmith_male.png`
- `guard_female.png`, `guard_male.png`
- `innkeeper_female.png`, `innkeeper_male.png`
- `priest_female.png`, `priest_male.png`
- `mayor_female.png`, `mayor_male.png`
- `farmer_female.png`, `farmer_male.png`
- `hunter_female.png`, `hunter_male.png`
- `beggar_female.png`, `beggar_male.png`
- `noble_female.png`, `noble_male.png`

## 📁 **Directory Structure**
```
Assets/GoonzuGame/
├── Characters/
│   ├── [race]_[gender]_[class].png (160 files)
│   └── NPCs/
│       └── [type]_[gender].png (20 files)
├── Buildings/
│   └── [building_type].png (32 files)
├── Items/
│   ├── Weapons/
│   │   └── [weapon_type].png (26 files)
│   ├── Armor/
│   │   └── [armor_type].png (21 files)
│   ├── Consumables/
│   │   └── [item_type].png (15 files)
│   └── Materials/
│       └── [material_type].png (10 files)
├── Creatures/
│   └── [creature_type].png (28 files)
├── UI/
│   ├── Icons/
│   │   └── [icon_name]_icon.png (16 files)
│   ├── Panels/
│   │   └── [panel_name]_panel.png (9 files)
│   └── Bars/
│       └── [bar_name]_bar.png (4 files)
├── Effects/
│   └── [effect_type].png (15 files)
├── Furniture/
│   └── [furniture_type].png (10 files)
└── Tiles/
    └── [tile_type].png (21 files)
```

## 🎨 **Naming Conventions**
- **Snake_case:** All filenames use lowercase with underscores
- **Descriptive:** Names clearly indicate the asset type and function
- **Consistent:** Same patterns across all categories
- **Unity-friendly:** Compatible with Unity's asset naming conventions

## 📊 **Asset Statistics**
- **Total Assets:** 393
- **Categories:** 11 main categories
- **Subcategories:** 25 subdirectories
- **File Format:** PNG (after AI generation)
- **Style:** Medieval GoonZu fantasy with anime-inspired MMORPG aesthetics

This comprehensive asset library provides everything needed for a complete medieval GoonZu MMORPG, from character sprites to UI elements and environmental assets.