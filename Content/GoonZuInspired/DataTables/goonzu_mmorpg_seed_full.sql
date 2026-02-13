-- GoonZu + DnD5e inspired MMORPG seed data

-- Players
INSERT INTO players (username, password_hash, email) VALUES ('goonzu', 'hash_gz', 'goonzu@mmorpg.com');
INSERT INTO players (username, password_hash, email) VALUES ('dndhero', 'hash_dnd', 'dnd@rpg.com');

-- Characters
INSERT INTO characters (player_id, name, class, race, gender, level, exp, hp, mp, zone_id, x, y, z) VALUES (1, 'Mayor Kim', 'Politician', 'Human', 'M', 50, 100000, 1000, 500, 1, 0, 0, 0);
INSERT INTO characters (player_id, name, class, race, gender, level, exp, hp, mp, zone_id, x, y, z) VALUES (2, 'Elminster', 'Wizard', 'Elf', 'M', 20, 20000, 300, 1000, 2, 10, 10, 0);

-- Items
INSERT INTO items (name, type, rarity, description, icon, value) VALUES ('Goonzu Sword', 'Weapon', 'Legendary', 'The legendary sword of the Mayor.', 'itm_sword_goonzu', 10000);
INSERT INTO items (name, type, rarity, description, icon, value) VALUES ('Bag of Holding', 'Bag', 'Rare', 'A magical bag from DnD.', 'itm_bag_holding', 5000);
INSERT INTO items (name, type, rarity, description, icon, value) VALUES ('Potion of Healing', 'Consumable', 'Common', 'Heals 50 HP.', 'itm_potion_heal', 50);
INSERT INTO items (name, type, rarity, description, icon, value) VALUES ('Scroll of Fireball', 'Consumable', 'Uncommon', 'Casts Fireball spell.', 'itm_scroll_fireball', 200);

-- Weapons
INSERT INTO weapons (item_id, weapon_type, attack, magic_attack, durability) VALUES (1, 'Sword', 100, 50, 999);

-- Armor
INSERT INTO items (name, type, rarity, description, icon, value) VALUES ('Plate Armor', 'Armor', 'Rare', 'Heavy armor for warriors.', 'itm_armor_plate', 2000);
INSERT INTO armor (item_id, armor_type, defense, magic_defense, durability) VALUES (5, 'Heavy', 50, 10, 500);

-- Inventory
INSERT INTO inventory (character_id, item_id, quantity, equipped) VALUES (1, 1, 1, 1);
INSERT INTO inventory (character_id, item_id, quantity, equipped) VALUES (2, 2, 1, 1);
INSERT INTO inventory (character_id, item_id, quantity, equipped) VALUES (2, 4, 3, 0);

-- Monsters
INSERT INTO monsters (name, type, level, hp, attack, defense, exp_reward, zone_id) VALUES ('Slime', 'Beast', 1, 20, 3, 1, 5, 1);
INSERT INTO monsters (name, type, level, hp, attack, defense, exp_reward, zone_id) VALUES ('Goblin', 'Humanoid', 2, 30, 5, 2, 10, 2);
INSERT INTO monsters (name, type, level, hp, attack, defense, exp_reward, zone_id) VALUES ('Red Dragon', 'Dragon', 20, 2000, 100, 50, 1000, 3);

-- Zones
INSERT INTO zones (name, description, map_asset) VALUES ('GoonZu Town', 'The bustling economic center.', 'map_town_goonzu');
INSERT INTO zones (name, description, map_asset) VALUES ('Forgotten Forest', 'A mysterious DnD forest.', 'map_forest_dnd');
INSERT INTO zones (name, description, map_asset) VALUES ('Dragon Lair', 'Home of the Red Dragon.', 'map_lair_dragon');

-- Quests
INSERT INTO quests (name, description, required_level, reward_exp, reward_gold) VALUES ('Become Mayor', 'Win the mayoral election.', 40, 10000, 5000);
INSERT INTO quests (name, description, required_level, reward_exp, reward_gold) VALUES ('Slay the Dragon', 'Defeat the Red Dragon.', 15, 20000, 10000);

-- Quest Progress
INSERT INTO quest_progress (character_id, quest_id, status, progress) VALUES (1, 1, 'completed', 1);
INSERT INTO quest_progress (character_id, quest_id, status, progress) VALUES (2, 2, 'active', 0);

-- Skills (DnD5e + GoonZu)
INSERT INTO skills (name, description, skill_type, power, mana_cost) VALUES ('Persuasion', 'Convince NPCs and players.', 'Passive', 0, 0);
INSERT INTO skills (name, description, skill_type, power, mana_cost) VALUES ('Fireball', 'Cast a powerful fireball.', 'Active', 100, 20);
INSERT INTO skills (name, description, skill_type, power, mana_cost) VALUES ('Leadership', 'Boost party stats.', 'Passive', 0, 0);
INSERT INTO skills (name, description, skill_type, power, mana_cost) VALUES ('Sneak Attack', 'Deal extra damage from stealth.', 'Active', 50, 5);

-- Character Skills
INSERT INTO character_skills (character_id, skill_id, level) VALUES (1, 1, 5);
INSERT INTO character_skills (character_id, skill_id, level) VALUES (2, 2, 3);
INSERT INTO character_skills (character_id, skill_id, level) VALUES (1, 3, 2);
INSERT INTO character_skills (character_id, skill_id, level) VALUES (2, 4, 1);

-- Crafting Recipes
INSERT INTO crafting_recipes (name, output_item_id, output_quantity, required_level) VALUES ('Sword of GoonZu', 1, 1, 40);
INSERT INTO crafting_ingredients (recipe_id, item_id, quantity) VALUES (1, 5, 2); -- 2x Plate Armor
INSERT INTO crafting_ingredients (recipe_id, item_id, quantity) VALUES (1, 3, 5); -- 5x Potion of Healing

-- Trading
INSERT INTO trades (seller_id, status) VALUES (1, 'open');
INSERT INTO trade_items (trade_id, item_id, quantity) VALUES (1, 1, 1);

-- Market Listings
INSERT INTO market_listings (seller_id, item_id, price, quantity, status) VALUES (1, 1, 15000, 1, 'active');
INSERT INTO market_price_history (item_id, price, quantity) VALUES (1, 15000, 1);

-- Guilds
INSERT INTO guilds (name, leader_id) VALUES ('GoonZu Guild', 1);
INSERT INTO guilds (name, leader_id) VALUES ('DnD Adventurers', 2);
INSERT INTO guild_members (guild_id, player_id, role) VALUES (1, 1, 'leader');
INSERT INTO guild_members (guild_id, player_id, role) VALUES (2, 2, 'leader');
INSERT INTO guild_members (guild_id, player_id, role) VALUES (2, 1, 'member');

-- Parties
INSERT INTO parties (leader_id) VALUES (2);
INSERT INTO party_members (party_id, player_id) VALUES (1, 2);
INSERT INTO party_members (party_id, player_id) VALUES (1, 1);
