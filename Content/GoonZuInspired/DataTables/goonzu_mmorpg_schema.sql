-- GoonZu MMORPG SQL Database Schema
-- This schema covers core RPG/MMORPG entities: players, items, weapons, armor, monsters, zones, quests, skills, inventory, and more.

CREATE TABLE players (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    username TEXT UNIQUE NOT NULL,
    password_hash TEXT NOT NULL,
    email TEXT,
    level INTEGER DEFAULT 1,
    exp INTEGER DEFAULT 0,
    gold INTEGER DEFAULT 0,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP
);

CREATE TABLE characters (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    player_id INTEGER NOT NULL,
    name TEXT NOT NULL,
    class TEXT,
    race TEXT,
    gender TEXT,
    level INTEGER DEFAULT 1,
    exp INTEGER DEFAULT 0,
    hp INTEGER DEFAULT 100,
    mp INTEGER DEFAULT 50,
    zone_id INTEGER,
    x REAL,
    y REAL,
    z REAL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(player_id) REFERENCES players(id)
);

CREATE TABLE items (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    type TEXT,
    rarity TEXT,
    description TEXT,
    icon TEXT,
    value INTEGER DEFAULT 0
);

CREATE TABLE weapons (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    item_id INTEGER NOT NULL,
    weapon_type TEXT,
    attack INTEGER,
    magic_attack INTEGER,
    durability INTEGER,
    FOREIGN KEY(item_id) REFERENCES items(id)
);

CREATE TABLE armor (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    item_id INTEGER NOT NULL,
    armor_type TEXT,
    defense INTEGER,
    magic_defense INTEGER,
    durability INTEGER,
    FOREIGN KEY(item_id) REFERENCES items(id)
);

CREATE TABLE inventory (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    character_id INTEGER NOT NULL,
    item_id INTEGER NOT NULL,
    quantity INTEGER DEFAULT 1,
    equipped BOOLEAN DEFAULT 0,
    FOREIGN KEY(character_id) REFERENCES characters(id),
    FOREIGN KEY(item_id) REFERENCES items(id)
);

CREATE TABLE monsters (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    type TEXT,
    level INTEGER,
    hp INTEGER,
    attack INTEGER,
    defense INTEGER,
    exp_reward INTEGER,
    zone_id INTEGER,
    FOREIGN KEY(zone_id) REFERENCES zones(id)
);

CREATE TABLE zones (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    description TEXT,
    map_asset TEXT
);

CREATE TABLE quests (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    description TEXT,
    required_level INTEGER DEFAULT 1,
    reward_exp INTEGER DEFAULT 0,
    reward_gold INTEGER DEFAULT 0
);

CREATE TABLE quest_progress (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    character_id INTEGER NOT NULL,
    quest_id INTEGER NOT NULL,
    status TEXT DEFAULT 'active',
    progress INTEGER DEFAULT 0,
    FOREIGN KEY(character_id) REFERENCES characters(id),
    FOREIGN KEY(quest_id) REFERENCES quests(id)
);

CREATE TABLE skills (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    description TEXT,
    skill_type TEXT,
    power INTEGER,
    mana_cost INTEGER
);

CREATE TABLE character_skills (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    character_id INTEGER NOT NULL,
    skill_id INTEGER NOT NULL,
    level INTEGER DEFAULT 1,
    FOREIGN KEY(character_id) REFERENCES characters(id),
    FOREIGN KEY(skill_id) REFERENCES skills(id)
);

-- Add more tables as needed for crafting, trading, economy, guilds, parties, etc.
