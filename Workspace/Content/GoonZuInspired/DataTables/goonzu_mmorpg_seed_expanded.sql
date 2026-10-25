-- Example seed data for expanded MMORPG schema

-- Crafting Recipes
INSERT INTO crafting_recipes (name, output_item_id, output_quantity, required_level) VALUES ('Iron Sword Recipe', 1, 1, 1);
INSERT INTO crafting_ingredients (recipe_id, item_id, quantity) VALUES (1, 3, 2); -- 2x Health Potion as ingredient

-- Trading
INSERT INTO trades (seller_id, status) VALUES (1, 'open');
INSERT INTO trade_items (trade_id, item_id, quantity) VALUES (1, 1, 1);

-- Market Listings
INSERT INTO market_listings (seller_id, item_id, price, quantity, status) VALUES (1, 1, 120, 1, 'active');
INSERT INTO market_price_history (item_id, price, quantity) VALUES (1, 120, 1);

-- Guilds
INSERT INTO guilds (name, leader_id) VALUES ('Knights of Dawn', 1);
INSERT INTO guild_members (guild_id, player_id, role) VALUES (1, 1, 'leader');
INSERT INTO guild_members (guild_id, player_id, role) VALUES (1, 2, 'member');

-- Parties
INSERT INTO parties (leader_id) VALUES (2);
INSERT INTO party_members (party_id, player_id) VALUES (1, 2);
INSERT INTO party_members (party_id, player_id) VALUES (1, 1);
