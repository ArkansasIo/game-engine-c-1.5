CREATE TABLE IF NOT EXISTS players (
  player_id BIGINT PRIMARY KEY,
  display_name VARCHAR(32) NOT NULL UNIQUE,
  balance BIGINT NOT NULL DEFAULT 0,
  reputation INT NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS skills (
  player_id BIGINT NOT NULL,
  skill_id VARCHAR(64) NOT NULL,
  basic_level INT NOT NULL DEFAULT 0,
  experience_level INT NOT NULL DEFAULT 0,
  experience BIGINT NOT NULL DEFAULT 0,
  PRIMARY KEY (player_id, skill_id)
);

CREATE TABLE IF NOT EXISTS items (
  item_id BIGINT PRIMARY KEY,
  definition_id VARCHAR(64) NOT NULL,
  owner_player_id BIGINT NULL,
  quality VARCHAR(24) NOT NULL DEFAULT 'Common',
  enchant_level INT NOT NULL DEFAULT 0,
  durability INT NOT NULL DEFAULT 100
);

CREATE TABLE IF NOT EXISTS market_listings (
  listing_id BIGINT PRIMARY KEY,
  seller_player_id BIGINT NOT NULL,
  town_id INT NOT NULL,
  definition_id VARCHAR(64) NOT NULL,
  unit_price BIGINT NOT NULL,
  quantity INT NOT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  expires_at TIMESTAMP NULL
);

CREATE TABLE IF NOT EXISTS towns (
  town_id INT PRIMARY KEY,
  name VARCHAR(64) NOT NULL UNIQUE,
  leader_player_id BIGINT NULL,
  treasury BIGINT NOT NULL DEFAULT 0,
  sales_tax_bps INT NOT NULL DEFAULT 500
);

CREATE TABLE IF NOT EXISTS town_shares (
  town_id INT NOT NULL,
  player_id BIGINT NOT NULL,
  shares BIGINT NOT NULL DEFAULT 0,
  PRIMARY KEY (town_id, player_id)
);

CREATE TABLE IF NOT EXISTS elections (
  election_id BIGINT PRIMARY KEY,
  office VARCHAR(32) NOT NULL,
  scope_id INT NULL,
  opens_at TIMESTAMP NOT NULL,
  closes_at TIMESTAMP NOT NULL,
  winner_player_id BIGINT NULL
);

CREATE TABLE IF NOT EXISTS election_votes (
  election_id BIGINT NOT NULL,
  voter_player_id BIGINT NOT NULL,
  candidate_player_id BIGINT NOT NULL,
  vote_weight BIGINT NOT NULL DEFAULT 1,
  PRIMARY KEY (election_id, voter_player_id)
);

CREATE TABLE IF NOT EXISTS recipes (
  recipe_id VARCHAR(64) PRIMARY KEY,
  output_definition_id VARCHAR(64) NOT NULL,
  skill_id VARCHAR(64) NOT NULL,
  difficulty INT NOT NULL
);

CREATE TABLE IF NOT EXISTS recipe_materials (
  recipe_id VARCHAR(64) NOT NULL,
  material_definition_id VARCHAR(64) NOT NULL,
  quantity INT NOT NULL,
  PRIMARY KEY (recipe_id, material_definition_id)
);

CREATE TABLE IF NOT EXISTS guilds (
  guild_id BIGINT PRIMARY KEY,
  name VARCHAR(64) NOT NULL UNIQUE,
  leader_player_id BIGINT NOT NULL,
  level INT NOT NULL DEFAULT 1,
  treasury BIGINT NOT NULL DEFAULT 0
);

CREATE TABLE IF NOT EXISTS mentor_links (
  mentor_player_id BIGINT NOT NULL,
  mentee_player_id BIGINT NOT NULL,
  created_at TIMESTAMP NOT NULL DEFAULT CURRENT_TIMESTAMP,
  PRIMARY KEY (mentor_player_id, mentee_player_id)
);
