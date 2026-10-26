CREATE TABLE IF NOT EXISTS players (
    player_id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    account_id VARCHAR(128) NOT NULL,
    display_name VARCHAR(64) NOT NULL,
    level INT NOT NULL DEFAULT 1,
    experience BIGINT NOT NULL DEFAULT 0,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (player_id),
    UNIQUE KEY uq_players_account (account_id),
    UNIQUE KEY uq_players_name (display_name)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS characters (
    character_id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    player_id BIGINT UNSIGNED NOT NULL,
    character_name VARCHAR(64) NOT NULL,
    class_id VARCHAR(64) NULL,
    level INT NOT NULL DEFAULT 1,
    experience BIGINT NOT NULL DEFAULT 0,
    health INT NOT NULL DEFAULT 10,
    max_health INT NOT NULL DEFAULT 10,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (character_id),
    UNIQUE KEY uq_character_name (character_name),
    KEY ix_characters_player (player_id),
    CONSTRAINT fk_characters_player FOREIGN KEY (player_id) REFERENCES players(player_id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS inventory_items (
    item_instance_id CHAR(36) NOT NULL,
    character_id BIGINT UNSIGNED NOT NULL,
    definition_id VARCHAR(128) NOT NULL,
    quantity INT NOT NULL DEFAULT 1,
    created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    PRIMARY KEY (item_instance_id),
    KEY ix_inventory_character (character_id),
    CONSTRAINT fk_inventory_character FOREIGN KEY (character_id) REFERENCES characters(character_id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS quests (
    character_id BIGINT UNSIGNED NOT NULL,
    quest_id VARCHAR(128) NOT NULL,
    status VARCHAR(32) NOT NULL,
    progress_json JSON NOT NULL,
    updated_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP ON UPDATE CURRENT_TIMESTAMP,
    PRIMARY KEY (character_id, quest_id),
    CONSTRAINT fk_quests_character FOREIGN KEY (character_id) REFERENCES characters(character_id) ON DELETE CASCADE
) ENGINE=InnoDB;
