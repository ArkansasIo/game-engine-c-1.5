PRAGMA foreign_keys = ON;

CREATE TABLE IF NOT EXISTS player_skill (
    player_id INTEGER NOT NULL,
    skill_id TEXT NOT NULL,
    basic_level INTEGER NOT NULL DEFAULT 0 CHECK (basic_level BETWEEN 0 AND 30),
    experience_level INTEGER NOT NULL DEFAULT 0 CHECK (experience_level BETWEEN 0 AND 70),
    PRIMARY KEY (player_id, skill_id)
);

CREATE TABLE IF NOT EXISTS item_market_listing (
    listing_id INTEGER PRIMARY KEY,
    seller_id INTEGER NOT NULL,
    town_id TEXT NOT NULL,
    item_id TEXT NOT NULL,
    quantity INTEGER NOT NULL CHECK (quantity > 0),
    unit_price INTEGER NOT NULL CHECK (unit_price >= 0),
    created_at TEXT NOT NULL DEFAULT CURRENT_TIMESTAMP,
    expires_at TEXT
);

CREATE INDEX IF NOT EXISTS idx_market_item_price ON item_market_listing(item_id, unit_price);

CREATE TABLE IF NOT EXISTS town (
    town_id TEXT PRIMARY KEY,
    name TEXT NOT NULL,
    mayor_player_id INTEGER,
    tax_rate REAL NOT NULL DEFAULT 0.05 CHECK (tax_rate BETWEEN 0.0 AND 1.0),
    treasury INTEGER NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS election (
    election_id INTEGER PRIMARY KEY,
    town_id TEXT NOT NULL REFERENCES town(town_id),
    opens_at TEXT NOT NULL,
    closes_at TEXT NOT NULL,
    status TEXT NOT NULL DEFAULT 'open'
);

CREATE TABLE IF NOT EXISTS election_vote (
    election_id INTEGER NOT NULL REFERENCES election(election_id),
    voter_player_id INTEGER NOT NULL,
    candidate_player_id INTEGER NOT NULL,
    weight INTEGER NOT NULL DEFAULT 1 CHECK (weight > 0),
    PRIMARY KEY (election_id, voter_player_id)
);

CREATE TABLE IF NOT EXISTS recipe (
    recipe_id TEXT PRIMARY KEY,
    output_item_id TEXT NOT NULL,
    output_quantity INTEGER NOT NULL CHECK (output_quantity > 0),
    profession TEXT NOT NULL,
    required_skill INTEGER NOT NULL DEFAULT 0,
    base_success REAL NOT NULL CHECK (base_success BETWEEN 0.0 AND 1.0)
);

CREATE TABLE IF NOT EXISTS recipe_input (
    recipe_id TEXT NOT NULL REFERENCES recipe(recipe_id),
    item_id TEXT NOT NULL,
    quantity INTEGER NOT NULL CHECK (quantity > 0),
    PRIMARY KEY (recipe_id, item_id)
);

CREATE VIEW IF NOT EXISTS market_average_price AS
SELECT item_id,
       SUM(quantity) AS supply,
       CAST(SUM(quantity * unit_price) AS REAL) / NULLIF(SUM(quantity), 0) AS average_price
FROM item_market_listing
GROUP BY item_id;
