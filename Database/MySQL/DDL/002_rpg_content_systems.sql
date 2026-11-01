-- Original RPG content catalogs and persistent quest/instance progression.
-- Apply once after 001_initial_schema.sql.

USE game_engine;

ALTER TABLE quests
    ADD COLUMN quest_chain_key VARCHAR(128) NULL,
    ADD COLUMN quest_order INT UNSIGNED NOT NULL DEFAULT 0,
    ADD COLUMN zone_key VARCHAR(128) NULL,
    ADD COLUMN min_level INT UNSIGNED NOT NULL DEFAULT 1,
    ADD COLUMN max_level INT UNSIGNED NOT NULL DEFAULT 80,
    ADD COLUMN is_repeatable TINYINT(1) NOT NULL DEFAULT 0,
    ADD COLUMN reset_period VARCHAR(16) NULL,
    ADD COLUMN is_enabled TINYINT(1) NOT NULL DEFAULT 1,
    ADD COLUMN source_tag VARCHAR(32) NOT NULL DEFAULT 'Original',
    ADD KEY ix_quests_type_level (quest_type, min_level, max_level),
    ADD KEY ix_quests_chain_order (quest_chain_key, quest_order),
    ADD KEY ix_quests_zone (zone_key);

CREATE TABLE quest_objectives (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    quest_id BIGINT UNSIGNED NOT NULL,
    objective_order SMALLINT UNSIGNED NOT NULL,
    objective_type VARCHAR(32) NOT NULL,
    target_key VARCHAR(128) NOT NULL,
    required_count INT UNSIGNED NOT NULL DEFAULT 1,
    objective_text VARCHAR(512) NOT NULL,
    is_optional TINYINT(1) NOT NULL DEFAULT 0,
    metadata JSON NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_quest_objective_order (quest_id, objective_order),
    UNIQUE KEY uq_quest_objective_id_quest (id, quest_id),
    CONSTRAINT fk_quest_objectives_quest FOREIGN KEY (quest_id) REFERENCES quests(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE quest_rewards (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    quest_id BIGINT UNSIGNED NOT NULL,
    reward_order SMALLINT UNSIGNED NOT NULL,
    reward_type VARCHAR(32) NOT NULL,
    item_id BIGINT UNSIGNED NULL,
    amount BIGINT UNSIGNED NOT NULL DEFAULT 0,
    metadata JSON NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_quest_reward_order (quest_id, reward_order),
    KEY ix_quest_rewards_quest (quest_id),
    CONSTRAINT fk_quest_rewards_quest FOREIGN KEY (quest_id) REFERENCES quests(id) ON DELETE CASCADE,
    CONSTRAINT fk_quest_rewards_item FOREIGN KEY (item_id) REFERENCES items(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

ALTER TABLE loot_entries
    ADD UNIQUE KEY uq_loot_entry_item (loot_table_id, item_id);

CREATE TABLE quest_prerequisites (
    quest_id BIGINT UNSIGNED NOT NULL,
    prerequisite_quest_id BIGINT UNSIGNED NOT NULL,
    PRIMARY KEY (quest_id, prerequisite_quest_id),
    CONSTRAINT fk_quest_prereq_quest FOREIGN KEY (quest_id) REFERENCES quests(id) ON DELETE CASCADE,
    CONSTRAINT fk_quest_prereq_required FOREIGN KEY (prerequisite_quest_id) REFERENCES quests(id) ON DELETE CASCADE,
    CONSTRAINT ck_quest_prereq_not_self CHECK (quest_id <> prerequisite_quest_id)
) ENGINE=InnoDB;

CREATE TABLE quest_objective_progress (
    character_id BIGINT UNSIGNED NOT NULL,
    quest_id BIGINT UNSIGNED NOT NULL,
    objective_id BIGINT UNSIGNED NOT NULL,
    progress_count INT UNSIGNED NOT NULL DEFAULT 0,
    completed_at DATETIME NULL,
    PRIMARY KEY (character_id, objective_id),
    KEY ix_objective_progress_quest (character_id, quest_id),
    CONSTRAINT fk_objective_progress_character FOREIGN KEY (character_id) REFERENCES characters(id) ON DELETE CASCADE,
    CONSTRAINT fk_objective_progress_quest FOREIGN KEY (character_id, quest_id) REFERENCES quest_progress(character_id, quest_id) ON DELETE CASCADE,
    CONSTRAINT fk_objective_progress_objective FOREIGN KEY (objective_id, quest_id) REFERENCES quest_objectives(id, quest_id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE instance_catalog (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    instance_key VARCHAR(128) NOT NULL,
    display_name VARCHAR(255) NOT NULL,
    instance_type VARCHAR(16) NOT NULL,
    zone_key VARCHAR(128) NOT NULL,
    min_level INT UNSIGNED NOT NULL DEFAULT 1,
    max_level INT UNSIGNED NOT NULL DEFAULT 80,
    recommended_party_size TINYINT UNSIGNED NOT NULL DEFAULT 5,
    max_party_size TINYINT UNSIGNED NOT NULL DEFAULT 5,
    description TEXT NOT NULL,
    is_enabled TINYINT(1) NOT NULL DEFAULT 1,
    metadata JSON NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_instance_catalog_key (instance_key),
    KEY ix_instance_type_level (instance_type, min_level, max_level),
    CONSTRAINT ck_instance_catalog_type CHECK (instance_type IN ('Dungeon', 'Raid', 'Trial')),
    CONSTRAINT ck_instance_catalog_party CHECK (recommended_party_size > 0 AND max_party_size >= recommended_party_size)
) ENGINE=InnoDB;

CREATE TABLE instance_difficulties (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    instance_id BIGINT UNSIGNED NOT NULL,
    difficulty_key VARCHAR(32) NOT NULL,
    display_name VARCHAR(64) NOT NULL,
    min_level INT UNSIGNED NOT NULL,
    recommended_party_size TINYINT UNSIGNED NOT NULL,
    max_party_size TINYINT UNSIGNED NOT NULL,
    loot_multiplier DECIMAL(6,3) NOT NULL DEFAULT 1.000,
    lockout_hours INT UNSIGNED NOT NULL DEFAULT 0,
    metadata JSON NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_instance_difficulty (instance_id, difficulty_key),
    UNIQUE KEY uq_instance_difficulty_id_instance (id, instance_id),
    CONSTRAINT fk_instance_difficulty_instance FOREIGN KEY (instance_id) REFERENCES instance_catalog(id) ON DELETE CASCADE,
    CONSTRAINT ck_instance_difficulty_party CHECK (recommended_party_size > 0 AND max_party_size >= recommended_party_size)
) ENGINE=InnoDB;

CREATE TABLE creature_catalog (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    creature_key VARCHAR(128) NOT NULL,
    display_name VARCHAR(255) NOT NULL,
    creature_type VARCHAR(32) NOT NULL,
    level INT UNSIGNED NOT NULL DEFAULT 1,
    health BIGINT UNSIGNED NOT NULL DEFAULT 1,
    attack INT UNSIGNED NOT NULL DEFAULT 1,
    defense INT UNSIGNED NOT NULL DEFAULT 0,
    experience_reward BIGINT UNSIGNED NOT NULL DEFAULT 0,
    loot_table_id BIGINT UNSIGNED NULL,
    metadata JSON NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_creature_catalog_key (creature_key),
    KEY ix_creature_level_type (level, creature_type),
    CONSTRAINT fk_creature_loot_table FOREIGN KEY (loot_table_id) REFERENCES loot_tables(id) ON DELETE SET NULL
) ENGINE=InnoDB;

CREATE TABLE creature_abilities (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    creature_id BIGINT UNSIGNED NOT NULL,
    ability_key VARCHAR(128) NOT NULL,
    display_name VARCHAR(128) NOT NULL,
    ability_type VARCHAR(32) NOT NULL,
    power INT UNSIGNED NOT NULL DEFAULT 0,
    cooldown_seconds DECIMAL(6,2) NOT NULL DEFAULT 0,
    metadata JSON NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_creature_ability (creature_id, ability_key),
    CONSTRAINT fk_creature_ability_creature FOREIGN KEY (creature_id) REFERENCES creature_catalog(id) ON DELETE CASCADE
) ENGINE=InnoDB;

CREATE TABLE instance_encounters (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    instance_id BIGINT UNSIGNED NOT NULL,
    encounter_order SMALLINT UNSIGNED NOT NULL,
    creature_id BIGINT UNSIGNED NOT NULL,
    encounter_name VARCHAR(255) NOT NULL,
    is_final_encounter TINYINT(1) NOT NULL DEFAULT 0,
    metadata JSON NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_instance_encounter_order (instance_id, encounter_order),
    UNIQUE KEY uq_instance_encounter_id_instance (id, instance_id),
    CONSTRAINT fk_instance_encounter_instance FOREIGN KEY (instance_id) REFERENCES instance_catalog(id) ON DELETE CASCADE,
    CONSTRAINT fk_instance_encounter_creature FOREIGN KEY (creature_id) REFERENCES creature_catalog(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE instance_loot_tables (
    instance_id BIGINT UNSIGNED NOT NULL,
    difficulty_id BIGINT UNSIGNED NOT NULL,
    loot_table_id BIGINT UNSIGNED NOT NULL,
    PRIMARY KEY (instance_id, difficulty_id),
    CONSTRAINT fk_instance_loot_instance FOREIGN KEY (instance_id) REFERENCES instance_catalog(id) ON DELETE CASCADE,
    CONSTRAINT fk_instance_loot_difficulty FOREIGN KEY (difficulty_id, instance_id) REFERENCES instance_difficulties(id, instance_id) ON DELETE CASCADE,
    CONSTRAINT fk_instance_loot_table FOREIGN KEY (loot_table_id) REFERENCES loot_tables(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE instance_runs (
    id BIGINT UNSIGNED NOT NULL AUTO_INCREMENT,
    run_key BINARY(16) NOT NULL,
    instance_id BIGINT UNSIGNED NOT NULL,
    difficulty_id BIGINT UNSIGNED NOT NULL,
    leader_character_id BIGINT UNSIGNED NOT NULL,
    run_state VARCHAR(24) NOT NULL DEFAULT 'InProgress',
    started_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    completed_at TIMESTAMP NULL,
    state_json JSON NULL,
    PRIMARY KEY (id),
    UNIQUE KEY uq_instance_run_key (run_key),
    UNIQUE KEY uq_instance_run_id_instance (id, instance_id),
    KEY ix_instance_run_leader_state (leader_character_id, run_state),
    CONSTRAINT fk_instance_run_instance FOREIGN KEY (instance_id) REFERENCES instance_catalog(id) ON DELETE RESTRICT,
    CONSTRAINT fk_instance_run_difficulty FOREIGN KEY (difficulty_id) REFERENCES instance_difficulties(id) ON DELETE RESTRICT,
    CONSTRAINT fk_instance_run_leader FOREIGN KEY (leader_character_id) REFERENCES characters(id) ON DELETE RESTRICT,
    CONSTRAINT ck_instance_run_state CHECK (run_state IN ('InProgress', 'Completed', 'Failed', 'Abandoned'))
) ENGINE=InnoDB;

CREATE TABLE instance_run_members (
    run_id BIGINT UNSIGNED NOT NULL,
    character_id BIGINT UNSIGNED NOT NULL,
    joined_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
    left_at TIMESTAMP NULL,
    PRIMARY KEY (run_id, character_id),
    CONSTRAINT fk_instance_run_member_run FOREIGN KEY (run_id) REFERENCES instance_runs(id) ON DELETE CASCADE,
    CONSTRAINT fk_instance_run_member_character FOREIGN KEY (character_id) REFERENCES characters(id) ON DELETE RESTRICT
) ENGINE=InnoDB;

CREATE TABLE instance_run_encounters (
    run_id BIGINT UNSIGNED NOT NULL,
    instance_id BIGINT UNSIGNED NOT NULL,
    encounter_id BIGINT UNSIGNED NOT NULL,
    encounter_state VARCHAR(24) NOT NULL DEFAULT 'Locked',
    started_at DATETIME NULL,
    completed_at DATETIME NULL,
    state_json JSON NULL,
    PRIMARY KEY (run_id, encounter_id),
    CONSTRAINT fk_instance_run_encounter_run FOREIGN KEY (run_id, instance_id) REFERENCES instance_runs(id, instance_id) ON DELETE CASCADE,
    CONSTRAINT fk_instance_run_encounter_definition FOREIGN KEY (encounter_id, instance_id) REFERENCES instance_encounters(id, instance_id) ON DELETE RESTRICT,
    CONSTRAINT ck_instance_encounter_state CHECK (encounter_state IN ('Locked', 'Available', 'InProgress', 'Completed', 'Failed'))
) ENGINE=InnoDB;

CREATE TABLE instance_character_lockouts (
    character_id BIGINT UNSIGNED NOT NULL,
    difficulty_id BIGINT UNSIGNED NOT NULL,
    locked_until DATETIME NOT NULL,
    completed_at DATETIME NOT NULL,
    PRIMARY KEY (character_id, difficulty_id),
    CONSTRAINT fk_instance_lockout_character FOREIGN KEY (character_id) REFERENCES characters(id) ON DELETE CASCADE,
    CONSTRAINT fk_instance_lockout_difficulty FOREIGN KEY (difficulty_id) REFERENCES instance_difficulties(id) ON DELETE CASCADE
) ENGINE=InnoDB;
