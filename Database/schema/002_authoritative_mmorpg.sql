CREATE TABLE IF NOT EXISTS accounts (
    id CHAR(36) NOT NULL,
    username VARCHAR(64) NOT NULL,
    password_hash VARCHAR(255) NOT NULL,
    created_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    last_login_at DATETIME(6) NULL,
    status ENUM('active','locked','disabled') NOT NULL DEFAULT 'active',
    PRIMARY KEY (id),
    UNIQUE KEY uq_accounts_username (username)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS sessions (
    id CHAR(36) NOT NULL,
    account_id CHAR(36) NOT NULL,
    token_hash CHAR(64) NOT NULL,
    created_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    last_activity_at DATETIME(6) NOT NULL DEFAULT CURRENT_TIMESTAMP(6),
    expires_at DATETIME(6) NOT NULL,
    revoked_at DATETIME(6) NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_sessions_token_hash (token_hash),
    KEY ix_sessions_account (account_id),
    CONSTRAINT fk_sessions_account FOREIGN KEY (account_id) REFERENCES accounts(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS character_classes (
    id VARCHAR(64) NOT NULL,
    name VARCHAR(100) NOT NULL,
    starting_ability_bonuses JSON NOT NULL,
    base_health INT NOT NULL DEFAULT 10,
    PRIMARY KEY (id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS character_subclasses (
    id VARCHAR(64) NOT NULL,
    class_id VARCHAR(64) NOT NULL,
    name VARCHAR(100) NOT NULL,
    granted_skills JSON NOT NULL,
    PRIMARY KEY (id),
    KEY ix_subclasses_class (class_id),
    CONSTRAINT fk_subclasses_class FOREIGN KEY (class_id) REFERENCES character_classes(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS item_definitions (
    id VARCHAR(100) NOT NULL,
    name VARCHAR(150) NOT NULL,
    rarity VARCHAR(32) NOT NULL,
    max_stack INT NOT NULL DEFAULT 1,
    weight DECIMAL(12,3) NOT NULL DEFAULT 0,
    PRIMARY KEY (id)
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS wallets (
    account_id CHAR(36) NOT NULL,
    currency VARCHAR(32) NOT NULL,
    amount BIGINT NOT NULL DEFAULT 0,
    PRIMARY KEY (account_id, currency),
    CONSTRAINT fk_wallet_account FOREIGN KEY (account_id) REFERENCES accounts(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE IF NOT EXISTS crafting_recipes (
    id VARCHAR(100) NOT NULL,
    name VARCHAR(150) NOT NULL,
    output_item_id VARCHAR(100) NOT NULL,
    output_quantity INT NOT NULL DEFAULT 1,
    duration_seconds INT NOT NULL DEFAULT 1,
    ingredients JSON NOT NULL,
    PRIMARY KEY (id),
    KEY ix_recipe_output (output_item_id),
    CONSTRAINT fk_recipe_output FOREIGN KEY (output_item_id) REFERENCES item_definitions(id)
) ENGINE=InnoDB;
