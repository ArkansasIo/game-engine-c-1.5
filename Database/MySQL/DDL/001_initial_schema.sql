-- Game Engine C 1.5
-- MySQL DDL: initial authoritative game schema
-- 64-bit application/server architecture; database identifiers use BIGINT where appropriate.

CREATE DATABASE IF NOT EXISTS game_engine
    CHARACTER SET utf8mb4
    COLLATE utf8mb4_0900_ai_ci;

USE game_engine;

CREATE TABLE IF NOT EXISTS accounts (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    username VARCHAR(64) NOT NULL,
    email VARCHAR(320) NOT NULL,
    password_hash VARBINARY(255) NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uq_accounts_username (username),
    UNIQUE KEY uq_accounts_email (email)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS players (
    id BINARY(16) NOT NULL,
    account_id BIGINT UNSIGNED NULL,
    username VARCHAR(64) NOT NULL,
    level INT UNSIGNED NOT NULL DEFAULT 1,
    experience BIGINT UNSIGNED NOT NULL DEFAULT 0,
    gold BIGINT NOT NULL DEFAULT 0,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uq_players_username (username),
    KEY ix_players_account_id (account_id),
    CONSTRAINT fk_players_account FOREIGN KEY (account_id) REFERENCES accounts(id) ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS characters (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    player_id BINARY(16) NOT NULL,
    name VARCHAR(64) NOT NULL,
    class_id VARCHAR(64) NOT NULL,
    level INT UNSIGNED NOT NULL DEFAULT 1,
    experience BIGINT UNSIGNED NOT NULL DEFAULT 0,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uq_characters_name (name),
    KEY ix_characters_player_id (player_id),
    CONSTRAINT fk_characters_player FOREIGN KEY (player_id) REFERENCES players(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS quests (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    quest_key VARCHAR(128) NOT NULL,
    title VARCHAR(255) NOT NULL,
    quest_type VARCHAR(32) NOT NULL,
    description TEXT NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_quests_key (quest_key)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS quest_progress (
    character_id BIGINT UNSIGNED NOT NULL,
    quest_id BIGINT UNSIGNED NOT NULL,
    state VARCHAR(32) NOT NULL DEFAULT 'NotStarted',
    progress_json JSON NULL,
    started_at DATETIME NULL,
    completed_at DATETIME NULL,
    PRIMARY KEY (character_id, quest_id),
    CONSTRAINT fk_quest_progress_character FOREIGN KEY (character_id) REFERENCES characters(id) ON DELETE CASCADE,
    CONSTRAINT fk_quest_progress_quest FOREIGN KEY (quest_id) REFERENCES quests(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS items (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    item_key VARCHAR(128) NOT NULL,
    display_name VARCHAR(255) NOT NULL,
    rarity VARCHAR(32) NOT NULL DEFAULT 'Common',
    item_type VARCHAR(64) NOT NULL,
    level_requirement INT UNSIGNED NOT NULL DEFAULT 0,
    stack_size INT UNSIGNED NOT NULL DEFAULT 1,
    metadata JSON NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_items_key (item_key)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS inventory_items (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    character_id BIGINT UNSIGNED NOT NULL,
    item_id BIGINT UNSIGNED NOT NULL,
    quantity BIGINT UNSIGNED NOT NULL DEFAULT 1,
    slot_index INT NULL,
    metadata JSON NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    KEY ix_inventory_character (character_id),
    KEY ix_inventory_item (item_id),
    CONSTRAINT fk_inventory_character FOREIGN KEY (character_id) REFERENCES characters(id) ON DELETE CASCADE,
    CONSTRAINT fk_inventory_item FOREIGN KEY (item_id) REFERENCES items(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS loot_tables (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    loot_key VARCHAR(128) NOT NULL,
    display_name VARCHAR(255) NOT NULL,
    rolls INT UNSIGNED NOT NULL DEFAULT 1,
    metadata JSON NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_loot_tables_key (loot_key)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS loot_entries (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    loot_table_id BIGINT UNSIGNED NOT NULL,
    item_id BIGINT UNSIGNED NOT NULL,
    weight DECIMAL(18,6) NOT NULL DEFAULT 1.0,
    min_quantity BIGINT UNSIGNED NOT NULL DEFAULT 1,
    max_quantity BIGINT UNSIGNED NOT NULL DEFAULT 1,
    guaranteed TINYINT(1) NOT NULL DEFAULT 0,
    PRIMARY KEY (id),
    KEY ix_loot_entries_table (loot_table_id),
    CONSTRAINT fk_loot_entries_table FOREIGN KEY (loot_table_id) REFERENCES loot_tables(id) ON DELETE CASCADE,
    CONSTRAINT fk_loot_entries_item FOREIGN KEY (item_id) REFERENCES items(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS parties (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    leader_character_id BIGINT UNSIGNED NOT NULL,
    max_members INT UNSIGNED NOT NULL DEFAULT 4,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    CONSTRAINT fk_parties_leader FOREIGN KEY (leader_character_id) REFERENCES characters(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS party_members (
    party_id BIGINT UNSIGNED NOT NULL,
    character_id BIGINT UNSIGNED NOT NULL,
    joined_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (party_id, character_id),
    CONSTRAINT fk_party_members_party FOREIGN KEY (party_id) REFERENCES parties(id) ON DELETE CASCADE,
    CONSTRAINT fk_party_members_character FOREIGN KEY (character_id) REFERENCES characters(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS guilds (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    guild_key VARCHAR(64) NOT NULL,
    name VARCHAR(128) NOT NULL,
    leader_character_id BIGINT UNSIGNED NOT NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (id),
    UNIQUE KEY uq_guilds_key (guild_key),
    UNIQUE KEY uq_guilds_name (name),
    CONSTRAINT fk_guilds_leader FOREIGN KEY (leader_character_id) REFERENCES characters(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS guild_members (
    guild_id BIGINT UNSIGNED NOT NULL,
    character_id BIGINT UNSIGNED NOT NULL,
    role_name VARCHAR(64) NOT NULL DEFAULT 'Member',
    joined_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (guild_id, character_id),
    CONSTRAINT fk_guild_members_guild FOREIGN KEY (guild_id) REFERENCES guilds(id) ON DELETE CASCADE,
    CONSTRAINT fk_guild_members_character FOREIGN KEY (character_id) REFERENCES characters(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS story_progress (
    character_id BIGINT UNSIGNED NOT NULL,
    story_key VARCHAR(128) NOT NULL,
    node_key VARCHAR(128) NULL,
    state_json JSON NOT NULL,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (character_id, story_key),
    CONSTRAINT fk_story_progress_character FOREIGN KEY (character_id) REFERENCES characters(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS world_events (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    event_key VARCHAR(128) NOT NULL,
    state VARCHAR(32) NOT NULL DEFAULT 'Scheduled',
    starts_at DATETIME NULL,
    ends_at DATETIME NULL,
    state_json JSON NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_world_events_key (event_key)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS transactions (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    character_id BIGINT UNSIGNED NOT NULL,
    transaction_type VARCHAR(64) NOT NULL,
    currency_amount BIGINT NOT NULL DEFAULT 0,
    item_id BIGINT UNSIGNED NULL,
    quantity BIGINT UNSIGNED NULL,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    metadata JSON NULL,
    PRIMARY KEY (id),
    KEY ix_transactions_character_created (character_id, created_at),
    CONSTRAINT fk_transactions_character FOREIGN KEY (character_id) REFERENCES characters(id) ON DELETE CASCADE,
    CONSTRAINT fk_transactions_item FOREIGN KEY (item_id) REFERENCES items(id) ON DELETE SET NULL
) ENGINE=InnoDB;
