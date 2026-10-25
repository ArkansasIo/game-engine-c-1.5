USE game_engine;
INSERT INTO items(item_key,name,category,rarity,stack_limit,base_value) VALUES
('coin','Gold Coin','Currency','Common',999999,1),
('iron_ore','Iron Ore','Material','Common',999,5),
('health_potion','Health Potion','Consumable','Common',99,25),
('training_sword','Training Sword','Weapon','Common',1,50)
ON DUPLICATE KEY UPDATE name=VALUES(name), base_value=VALUES(base_value);

INSERT INTO items(item_key,name,category,rarity,stack_limit,base_value) VALUES
('mana_potion','Mana Potion','Consumable','Common',99,30),
('steel_sword','Steel Sword','Weapon','Uncommon',1,250),
('adventurer_mail','Adventurer Mail','Armor','Common',1,180)
ON DUPLICATE KEY UPDATE name=VALUES(name), base_value=VALUES(base_value);