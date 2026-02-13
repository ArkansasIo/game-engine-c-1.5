-- Expanded GoonZu MMORPG SQL Schema: Crafting, Trading, Economy, Guilds, Parties

-- Crafting Recipes
CREATE TABLE crafting_recipes (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT NOT NULL,
    output_item_id INTEGER NOT NULL,
    output_quantity INTEGER DEFAULT 1,
    required_level INTEGER DEFAULT 1,
    FOREIGN KEY(output_item_id) REFERENCES items(id)
);

CREATE TABLE crafting_ingredients (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    recipe_id INTEGER NOT NULL,
    item_id INTEGER NOT NULL,
    quantity INTEGER DEFAULT 1,
    FOREIGN KEY(recipe_id) REFERENCES crafting_recipes(id),
    FOREIGN KEY(item_id) REFERENCES items(id)
);

-- Trading (Player-to-Player)
CREATE TABLE trades (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    seller_id INTEGER NOT NULL,
    buyer_id INTEGER,
    status TEXT DEFAULT 'open',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    completed_at DATETIME,
    FOREIGN KEY(seller_id) REFERENCES players(id),
    FOREIGN KEY(buyer_id) REFERENCES players(id)
);

CREATE TABLE trade_items (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    trade_id INTEGER NOT NULL,
    item_id INTEGER NOT NULL,
    quantity INTEGER DEFAULT 1,
    FOREIGN KEY(trade_id) REFERENCES trades(id),
    FOREIGN KEY(item_id) REFERENCES items(id)
);

-- Economy/Market
CREATE TABLE market_listings (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    seller_id INTEGER NOT NULL,
    item_id INTEGER NOT NULL,
    price INTEGER NOT NULL,
    quantity INTEGER DEFAULT 1,
    status TEXT DEFAULT 'active',
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(seller_id) REFERENCES players(id),
    FOREIGN KEY(item_id) REFERENCES items(id)
);

CREATE TABLE market_price_history (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    item_id INTEGER NOT NULL,
    price INTEGER NOT NULL,
    quantity INTEGER,
    sold_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(item_id) REFERENCES items(id)
);

-- Guilds
CREATE TABLE guilds (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    name TEXT UNIQUE NOT NULL,
    leader_id INTEGER NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(leader_id) REFERENCES players(id)
);

CREATE TABLE guild_members (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    guild_id INTEGER NOT NULL,
    player_id INTEGER NOT NULL,
    role TEXT DEFAULT 'member',
    joined_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(guild_id) REFERENCES guilds(id),
    FOREIGN KEY(player_id) REFERENCES players(id)
);

-- Parties
CREATE TABLE parties (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    leader_id INTEGER NOT NULL,
    created_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(leader_id) REFERENCES players(id)
);

CREATE TABLE party_members (
    id INTEGER PRIMARY KEY AUTOINCREMENT,
    party_id INTEGER NOT NULL,
    player_id INTEGER NOT NULL,
    joined_at DATETIME DEFAULT CURRENT_TIMESTAMP,
    FOREIGN KEY(party_id) REFERENCES parties(id),
    FOREIGN KEY(player_id) REFERENCES players(id)
);
