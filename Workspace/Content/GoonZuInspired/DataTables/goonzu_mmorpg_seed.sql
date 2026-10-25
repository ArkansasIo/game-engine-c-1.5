-- Example SQL data for GoonZu MMORPG schema

-- Players
INSERT INTO players (username, password_hash, email) VALUES ('hero1', 'hash1', 'hero1@email.com');
INSERT INTO players (username, password_hash, email) VALUES ('hero2', 'hash2', 'hero2@email.com');

-- Characters
INSERT INTO characters (player_id, name, class, race, gender, level, exp, hp, mp, zone_id, x, y, z) VALUES (1, 'Aldous', 'Warrior', 'Human', 'M', 10, 1500, 300, 100, 1, 10, 20, 0);
INSERT INTO characters (player_id, name, class, race, gender, level, exp, hp, mp, zone_id, x, y, z) VALUES (2, 'Lina', 'Mage', 'Elf', 'F', 8, 900, 180, 200, 2, 5, 15, 0);

-- Items
INSERT INTO items (name, type, rarity, description, icon, value) VALUES ('Iron Sword', 'Weapon', 'Common', 'A basic iron sword.', 'itm_sword_iron_01', 100);
INSERT INTO items (name, type, rarity, description, icon, value) VALUES ('Leather Armor', 'Armor', 'Common', 'Simple leather armor.', 'itm_armor_leather_01', 80);
INSERT INTO items (name, type, rarity, description, icon, value) VALUES ('Health Potion', 'Consumable', 'Common', 'Restores HP.', 'itm_potion_hp_01', 10);

-- Weapons
INSERT INTO weapons (item_id, weapon_type, attack, magic_attack, durability) VALUES (1, 'Sword', 15, 0, 100);

-- Armor
INSERT INTO armor (item_id, armor_type, defense, magic_defense, durability) VALUES (2, 'Light', 5, 2, 80);

-- Inventory
INSERT INTO inventory (character_id, item_id, quantity, equipped) VALUES (1, 1, 1, 1);
INSERT INTO inventory (character_id, item_id, quantity, equipped) VALUES (1, 3, 5, 0);
INSERT INTO inventory (character_id, item_id, quantity, equipped) VALUES (2, 2, 1, 1);

-- Monsters
INSERT INTO monsters (name, type, level, hp, attack, defense, exp_reward, zone_id) VALUES ('Slime', 'Beast', 2, 30, 5, 1, 10, 1);
INSERT INTO monsters (name, type, level, hp, attack, defense, exp_reward, zone_id) VALUES ('Bandit', 'Humanoid', 5, 80, 12, 4, 30, 2);

-- Zones
INSERT INTO zones (name, description, map_asset) VALUES ('Beginner Village', 'A peaceful starting area.', 'map_town_01');
INSERT INTO zones (name, description, map_asset) VALUES ('Dark Forest', 'A dangerous forest full of monsters.', 'map_field_01');

-- Quests
INSERT INTO quests (name, description, required_level, reward_exp, reward_gold) VALUES ('First Steps', 'Defeat 3 slimes.', 1, 100, 20);
INSERT INTO quests (name, description, required_level, reward_exp, reward_gold) VALUES ('Bandit Menace', 'Defeat the bandit leader.', 5, 300, 100);

-- Quest Progress
INSERT INTO quest_progress (character_id, quest_id, status, progress) VALUES (1, 1, 'active', 1);
INSERT INTO quest_progress (character_id, quest_id, status, progress) VALUES (2, 2, 'active', 0);

-- Skills
INSERT INTO skills (name, description, skill_type, power, mana_cost) VALUES ('Slash', 'A basic sword attack.', 'Active', 10, 0);
INSERT INTO skills (name, description, skill_type, power, mana_cost) VALUES ('Fireball', 'A basic fire spell.', 'Active', 20, 10);

-- Character Skills
INSERT INTO character_skills (character_id, skill_id, level) VALUES (1, 1, 2);
INSERT INTO character_skills (character_id, skill_id, level) VALUES (2, 2, 1);
