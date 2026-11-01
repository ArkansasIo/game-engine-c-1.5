#!/usr/bin/env python3
"""Generate deterministic, original RPG catalog seed SQL for MySQL 8."""

import argparse
import json
from collections import Counter
from pathlib import Path


ROOT = Path(__file__).resolve().parents[3]
DEFAULT_OUTPUT = ROOT / "Database" / "MySQL" / "DDL" / "003_original_rpg_catalog_seed.sql"

REGIONS = [
    ("frostmere", "Frostmere", "snowfield"),
    ("emberfall", "Emberfall", "volcanic"),
    ("greenhollow", "Greenhollow", "forest"),
    ("sunreach", "Sunreach", "desert"),
    ("stormwatch", "Stormwatch", "coastal"),
    ("moonfen", "Moonfen", "marsh"),
    ("skyreach", "Skyreach", "highland"),
    ("deepdelve", "Deepdelve", "underground"),
    ("thornvale", "Thornvale", "wilderness"),
    ("starfall", "Starfall", "astral"),
]

QUEST_COUNTS = {
    "MainStory": 500,
    "Side": 26000,
    "Daily": 2000,
    "Weekly": 500,
    "Profession": 500,
    "Instance": 500,
}

MATERIALS = [
    ("ore", "Vein Ore", 8), ("herb", "Wild Herb", 6), ("hide", "Beast Hide", 9),
    ("wood", "Hardwood", 7), ("crystal", "Resonant Crystal", 18), ("scale", "Drake Scale", 24),
    ("essence", "Bound Essence", 20), ("salt", "Mineral Salt", 3), ("fiber", "Moon Fiber", 8),
    ("relic", "Weathered Relic", 30),
]
WEAPON_KINDS = [
    ("sword", "Longsword", "Sword"), ("greatblade", "Greatblade", "Sword"),
    ("axe", "War Axe", "Axe"), ("bow", "Hunting Bow", "Bow"),
    ("staff", "Runestaff", "Staff"), ("dagger", "Wayfinder Dagger", "Dagger"),
]
ARMOR_SLOTS = [
    ("head", "Helm", "Head"), ("chest", "Cuirass", "Chest"),
    ("hands", "Gauntlets", "Hands"), ("legs", "Greaves", "Legs"),
    ("feet", "Sabatons", "Feet"),
]
TIERS = [
    ("Worn", "Common", 1), ("Field", "Common", 8), ("Tempered", "Uncommon", 16),
    ("Runed", "Rare", 28), ("Stormforged", "Epic", 42), ("Starforged", "Epic", 58),
    ("Eclipse", "Legendary", 72), ("Mythic", "Mythic", 80),
]

CREATURE_FAMILIES = [
    ("wolf", "Warg", "Beast"), ("ooze", "Mire Ooze", "Aberration"),
    ("bandit", "Outland Reaver", "Humanoid"), ("golem", "Shard Golem", "Construct"),
    ("spider", "Cave Weaver", "Beast"), ("wisp", "Dusk Wisp", "Elemental"),
    ("drake", "Cinder Drake", "Dragon"), ("wraith", "Hollow Wraith", "Undead"),
    ("boar", "Ironback Boar", "Beast"), ("sentinel", "Ruin Sentinel", "Construct"),
]
CREATURE_PREFIXES = [
    "Ashen", "Bitter", "Clouded", "Drowned", "Elder", "Feral", "Gloam",
    "Hollow", "Ivory", "Jade", "Keen", "Luminous", "Mournful", "Nightbound",
    "Oaken", "Pale", "Quiet", "Rime", "Shattered", "Thorned", "Umbral",
    "Verdant", "Wild", "Gale-touched", "Sable", "Dawnmarked", "Ember-veined",
    "Frostbitten", "Gravemoss", "Starwrought",
]
INSTANCE_THEMES = [
    ("Hollow", "Cistern"), ("Cinder", "Foundry"), ("Gloam", "Sanctum"),
    ("Sunken", "Archive"), ("Thorn", "Bastion"), ("Storm", "Observatory"),
    ("Glass", "Catacomb"), ("Frost", "Monastery"), ("Star", "Vault"),
    ("Ash", "Citadel"),
]
OBJECTIVE_KINDS = ["Kill", "Gather", "Craft", "VisitZone", "CompleteInstance", "CompleteTrial"]


def sql(value):
    if value is None:
        return "NULL"
    if isinstance(value, bool):
        return "1" if value else "0"
    if isinstance(value, (int, float)):
        return str(value)
    if isinstance(value, (dict, list)):
        value = json.dumps(value, separators=(",", ":"), ensure_ascii=True)
    return "'" + str(value).replace("\\", "\\\\").replace("'", "''") + "'"


def insert_statements(table, columns, rows, chunk_size=250, upsert=None):
    if not rows:
        return []
    prefix = f"INSERT INTO {table} ({', '.join(columns)}) VALUES\n"
    statements = []
    for offset in range(0, len(rows), chunk_size):
        values = ",\n".join("    (" + ", ".join(map(str, row)) + ")" for row in rows[offset:offset + chunk_size])
        statement = prefix + values
        if upsert:
            statement += "\nON DUPLICATE KEY UPDATE " + ", ".join(
                f"{column}=VALUES({column})" for column in upsert
            )
        statements.append(statement + ";")
    return statements


def item_catalog():
    rows = []

    def add(key, name, kind, rarity, level, stack, metadata):
        rows.append((key, name, rarity, kind, level, stack, metadata))

    for category, (prefix, title, weapon_type) in enumerate(WEAPON_KINDS):
        for tier_index, (tier, rarity, level) in enumerate(TIERS):
            key = f"weapon_{prefix}_{tier_index + 1:02d}"
            add(key, f"{tier} {title}", "Weapon", rarity, level, 1,
                {"slot": "Weapon", "weapon_type": weapon_type, "attack": 8 + level * 2, "category_index": category})

    for slot_key, slot_name, slot in ARMOR_SLOTS:
        for tier_index, (tier, rarity, level) in enumerate(TIERS[:6]):
            key = f"armor_{slot_key}_{tier_index + 1:02d}"
            add(key, f"{tier} {slot_name}", "Armor", rarity, level, 1,
                {"slot": slot, "defense": 3 + level, "magic_defense": 1 + level // 3})

    for tier_index, (tier, rarity, level) in enumerate(TIERS[:5]):
        for style_index, style in enumerate(["Bulwark", "Wayfarer", "Spellguard", "Sentinel", "Warden", "Aegis"]):
            key = f"shield_{style.lower()}_{tier_index + 1:02d}"
            add(key, f"{tier} {style} Shield", "Shield", rarity, level, 1,
                {"slot": "OffHand", "defense": 5 + level + style_index, "block": 0.08 + tier_index * 0.015})

    for index in range(45):
        tier, rarity, level = TIERS[(index // 6) % len(TIERS)]
        effect = ["healing", "mana", "stamina", "resistance", "haste"][index % 5]
        name = f"{tier} {effect.title()} Tonic {index + 1:02d}"
        add(f"consumable_tonic_{index + 1:03d}", name, "Consumable", rarity, level, 20,
            {"effect": effect, "power": 20 + level * 3, "duration_seconds": 0 if effect in ("healing", "mana") else 30})

    for index in range(60):
        material_key, material_name, price = MATERIALS[index % len(MATERIALS)]
        tier_index = (index // len(MATERIALS)) % len(TIERS)
        tier, rarity, level = TIERS[tier_index]
        add(f"material_{material_key}_{index + 1:03d}", f"{tier} {material_name} {index + 1:02d}",
            "Material", rarity, level, 99, {"gathering_source": material_key, "base_value": price + level * 2})

    for index in range(25):
        tier, rarity, level = TIERS[(index // 4) % len(TIERS)]
        slot = ["Ring", "Amulet", "Charm", "Cloak", "Relic"][index % 5]
        add(f"accessory_{slot.lower()}_{index + 1:03d}", f"{tier} {slot} of the {CREATURE_PREFIXES[index]} Star",
            "Accessory", rarity, level, 1, {"slot": slot, "stat_bonus": 1 + level // 8})

    for index in range(20):
        region_id, region_name, _ = REGIONS[index % len(REGIONS)]
        add(f"quest_token_{index + 1:03d}", f"{region_name} Oath Token {index + 1:02d}",
            "QuestItem", "Uncommon", 1 + index * 3, 10, {"quest_region": region_id, "tradeable": False})

    for index in range(20):
        tier, rarity, level = TIERS[(index // 3) % len(TIERS)]
        add(f"gem_{index + 1:03d}", f"{tier} {CREATURE_PREFIXES[index]} Starstone", "Gem", rarity, level, 20,
            {"socketable": True, "stat_bonus": 1 + level // 10})

    for index in range(22):
        name, skill = [
            ("Prospector's Pick", "Mining"), ("Forager's Sickle", "Gathering"),
            ("Woodwright's Hatchet", "Woodworking"), ("Wayfarer's Fishing Rod", "Fishing"),
            ("Field Cook's Pot", "Cooking"), ("Runesmith's Hammer", "Blacksmithing"),
        ][index % 6]
        tier, rarity, level = TIERS[(index // 6) % len(TIERS)]
        add(f"tool_{index + 1:03d}", f"{tier} {name} {index + 1:02d}", "Tool", rarity, level, 1,
            {"profession": skill, "gather_bonus": 1 + level // 20})

    if len(rows) != 300 or len({row[0] for row in rows}) != 300:
        raise ValueError(f"Expected 300 unique items, got {len(rows)}")
    return rows


def creature_catalog():
    rows = []
    for index in range(300):
        family_key, species, creature_type = CREATURE_FAMILIES[index % len(CREATURE_FAMILIES)]
        prefix = CREATURE_PREFIXES[(index // len(CREATURE_FAMILIES)) % len(CREATURE_PREFIXES)]
        region_id, region_name, _ = REGIONS[(index // 30) % len(REGIONS)]
        level = 1 + index % 80
        key = f"creature_{family_key}_{index + 1:03d}"
        name = f"{prefix} {species} {index + 1:03d}"
        metadata = {"region": region_id, "region_name": region_name, "is_elite": index % 10 == 0,
                    "is_boss": index % 25 == 0}
        rows.append((key, name, creature_type, level, 50 + level * level * 12,
                     4 + level * 3, level, 10 + level * 20,
                     f"loot_{key}", metadata))
    return rows


def instance_catalog():
    instances = []
    for kind, count, party_size, max_party, difficulty_names, lockout in [
        ("Dungeon", 100, 5, 5, ["Normal", "Veteran", "Mythic"], 0),
        ("Raid", 50, 10, 20, ["Normal", "Heroic", "Mythic"], 168),
        ("Trial", 100, 1, 5, ["Bronze", "Silver", "Gold"], 0),
    ]:
        for index in range(count):
            theme, place = INSTANCE_THEMES[index % len(INSTANCE_THEMES)]
            region_id, region_name, _ = REGIONS[index % len(REGIONS)]
            number = index + 1
            key = f"{kind.lower()}_{number:03d}"
            name = f"{theme} {place} of {region_name} {number:03d}"
            level = 1 + (index * 79 // max(1, count - 1))
            instances.append({
                "key": key, "name": name, "kind": kind, "zone": region_id, "level": level,
                "party": party_size, "max_party": max_party, "difficulties": difficulty_names,
                "lockout": lockout, "number": number,
            })
    return instances


def quest_catalog(instances, item_rows, creatures):
    material_keys = [row[0] for row in item_rows if row[3] == "Material"]
    item_names = {row[0]: row[1] for row in item_rows}
    quest_rows = []
    objective_rows = []
    reward_rows = []
    prerequisite_rows = []
    quest_key_by_type = {kind: [] for kind in QUEST_COUNTS}
    item_keys = [row[0] for row in item_rows]

    sequence = 0
    for quest_type, count in QUEST_COUNTS.items():
        for index in range(count):
            sequence += 1
            region_index = index % len(REGIONS)
            if quest_type in ("MainStory", "Side"):
                arc_length = 50 if quest_type == "MainStory" else 25
                region_index = (index // arc_length) % len(REGIONS)
            region_id, region_name, _ = REGIONS[region_index]
            level = 1 + (index * 79 // max(1, count - 1))
            quest_key = f"quest_{quest_type.lower()}_{sequence:05d}"
            repeatable = quest_type in ("Daily", "Weekly")
            reset = "Daily" if quest_type == "Daily" else "Weekly" if quest_type == "Weekly" else None
            if quest_type in ("MainStory", "Side"):
                arc_length = 50 if quest_type == "MainStory" else 25
                arc = index // (arc_length * len(REGIONS))
                order = index % arc_length + 1
                chain_key = f"{quest_type.lower()}_{region_id}_{arc:04d}"
            else:
                order = 0
                chain_key = None

            family_key, species, _ = CREATURE_FAMILIES[index % len(CREATURE_FAMILIES)]
            creature_key = creatures[index % len(creatures)][0]
            material_key = material_keys[index % len(material_keys)]
            instance = instances[index % len(instances)]
            objective_type = OBJECTIVE_KINDS[(sequence + index) % len(OBJECTIVE_KINDS)]
            quantity = 1 + (index % 12)

            if quest_type == "Profession":
                objective_type = "Gather" if index % 2 == 0 else "Craft"
            elif quest_type == "Instance":
                objective_type = "CompleteTrial" if instance["kind"] == "Trial" else "CompleteInstance"
            if objective_type == "Kill":
                target_key = creature_key
                objective_text = f"Defeat {quantity} {species} in {region_name}."
            elif objective_type == "Gather":
                target_key = material_key
                objective_text = f"Gather {quantity} units of {item_names[material_key]} near {region_name}."
            elif objective_type == "Craft":
                target_key = item_keys[(index * 7) % len(item_keys)]
                objective_text = f"Craft {quantity} items using a local workbench in {region_name}."
            elif objective_type == "VisitZone":
                target_key = region_id
                quantity = 1
                objective_text = f"Travel to {region_name} and report to its waystone."
            elif objective_type == "CompleteTrial":
                trial = next((candidate for candidate in instances if candidate["kind"] == "Trial"), instance)
                target_key = trial["key"]
                quantity = 1
                objective_text = f"Complete {trial['name']} on any available difficulty."
            else:
                target_key = instance["key"]
                quantity = 1
                objective_text = f"Complete an encounter in {instance['name']}."

            title_prefix = {
                "MainStory": "Chronicle", "Side": "Wayfarer's Errand", "Daily": "Daily Writ",
                "Weekly": "Weekly Expedition", "Profession": "Artisan's Commission",
                "Instance": "Instance Challenge",
            }[quest_type]
            title = f"{title_prefix}: {region_name} {sequence:05d}"
            description = f"{objective_text} Return to the local quartermaster for your reward."
            quest_rows.append((quest_key, title, quest_type, description, chain_key, order, region_id,
                               level, min(80, level + 8), repeatable, reset, True, "Original"))
            objective_rows.append((quest_key, 1, objective_type, target_key, quantity, objective_text, False, {}))
            reward_rows.append((quest_key, 1, "Experience", None, 25 + level * 15, {"grant": "experience"}))
            reward_rows.append((quest_key, 2, "Currency", None, 10 + level * 8, {"currency": "Luma"}))
            if sequence % 10 == 0:
                reward_rows.append((quest_key, 3, "Item", item_keys[(sequence * 13) % len(item_keys)], 1,
                                    {"selection": "fixed"}))
            quest_key_by_type[quest_type].append((quest_key, chain_key, order))

    # Resolve chain predecessors against generated quest keys without relying on auto-increment IDs.
    by_chain_order = {(chain, order): key for key, chain, order in
                      (entry for entries in quest_key_by_type.values() for entry in entries) if chain}
    prerequisite_rows = [
        (quest_key, by_chain_order[(chain, order - 1)])
        for quest_key, chain, order in
        (entry for entries in quest_key_by_type.values() for entry in entries)
        if chain and order > 1
    ]
    return quest_rows, objective_rows, reward_rows, prerequisite_rows


def generate(output):
    items = item_catalog()
    creatures = creature_catalog()
    instances = instance_catalog()
    quests, objectives, rewards, prerequisites = quest_catalog(instances, items, creatures)
    instance_counts = Counter(instance["kind"] for instance in instances)
    expected_instances = {"Dungeon": 100, "Raid": 50, "Trial": 100}
    quest_keys = {quest[0] for quest in quests}
    item_keys = {item[0] for item in items}
    if len(creatures) != 300 or instance_counts != expected_instances:
        raise ValueError(f"Unexpected creature/instance totals: {len(creatures)}, {dict(instance_counts)}")
    if len(quests) != sum(QUEST_COUNTS.values()) or len(quest_keys) != len(quests):
        raise ValueError(f"Expected {sum(QUEST_COUNTS.values())} unique quests, got {len(quests)}")
    if len(objectives) != len(quests):
        raise ValueError("Every generated quest must have exactly one objective")
    if any(quest_id not in quest_keys or prerequisite_id not in quest_keys
           for quest_id, prerequisite_id in prerequisites):
        raise ValueError("Generated quest prerequisite references a missing quest")
    if any(item_key is not None and item_key not in item_keys
           for _, _, _, item_key, _, _ in rewards):
        raise ValueError("Generated quest reward references a missing item")
    lines = [
        "-- Generated by Tools/generate_original_rpg_catalog.py; do not edit by hand.",
        "-- Original procedural catalog; deterministic output, MySQL 8.0+.",
        "USE game_engine;",
        "START TRANSACTION;",
        "",
    ]
    lines += insert_statements("items",
        ["item_key", "display_name", "rarity", "item_type", "level_requirement", "stack_size", "metadata"],
        [(sql(k), sql(n), sql(r), sql(t), lvl, stack, sql(meta)) for k, n, r, t, lvl, stack, meta in items],
        upsert=["display_name", "rarity", "item_type", "level_requirement", "stack_size", "metadata"])

    loot_tables = []
    for creature in creatures:
        loot_tables.append((creature[8], creature[1] + " Loot", 1, {"source": creature[0]}))
    for instance in instances:
        loot_tables.append((f"loot_{instance['key']}", instance["name"] + " Rewards", 1,
                            {"source": instance["key"], "instance_type": instance["kind"]}))
    lines += insert_statements("loot_tables", ["loot_key", "display_name", "rolls", "metadata"],
        [(sql(k), sql(n), rolls, sql(meta)) for k, n, rolls, meta in loot_tables],
        upsert=["display_name", "rolls", "metadata"])

    creature_values = []
    for key, name, kind, level, hp, attack, defense, xp, loot_key, metadata in creatures:
        loot_id = f"(SELECT id FROM loot_tables WHERE loot_key={sql(loot_key)})"
        creature_values.append((sql(key), sql(name), sql(kind), level, hp, attack, defense, xp, loot_id, sql(metadata)))
    lines += insert_statements("creature_catalog",
        ["creature_key", "display_name", "creature_type", "level", "health", "attack", "defense",
         "experience_reward", "loot_table_id", "metadata"],
        creature_values, upsert=["display_name", "creature_type", "level", "health", "attack", "defense",
                                 "experience_reward", "loot_table_id", "metadata"])

    loot_entries = []
    for index, creature in enumerate(creatures):
        table_id = f"(SELECT id FROM loot_tables WHERE loot_key={sql(creature[8])})"
        for item_offset, weight in ((0, "1.000000"), (1, "0.250000")):
            item_key = items[(index * 7 + item_offset) % len(items)][0]
            item_id = f"(SELECT id FROM items WHERE item_key={sql(item_key)})"
            loot_entries.append((table_id, item_id, weight, 1, 2 if item_offset else 1, 1 if item_offset == 0 else 0))
    for index, instance in enumerate(instances):
        table_key = f"loot_{instance['key']}"
        table_id = f"(SELECT id FROM loot_tables WHERE loot_key={sql(table_key)})"
        for item_offset, weight in ((0, "1.000000"), (1, "0.500000"), (2, "0.100000")):
            item_key = items[(index * 11 + item_offset * 23) % len(items)][0]
            item_id = f"(SELECT id FROM items WHERE item_key={sql(item_key)})"
            loot_entries.append((table_id, item_id, weight, 1, 1, 1 if item_offset == 0 else 0))
    lines += insert_statements("loot_entries",
        ["loot_table_id", "item_id", "weight", "min_quantity", "max_quantity", "guaranteed"],
        loot_entries, chunk_size=250, upsert=["weight", "min_quantity", "max_quantity", "guaranteed"])

    ability_rows = []
    for index, creature in enumerate(creatures):
        creature_id = f"(SELECT id FROM creature_catalog WHERE creature_key={sql(creature[0])})"
        ability_rows.append((creature_id, sql(f"ability_{index + 1:03}"), sql(f"{CREATURE_PREFIXES[index % 30]} Strike"),
                             sql("Signature"), 5 + creature[3] * 2, 8.0, sql({"original": True})))
    lines += insert_statements("creature_abilities",
        ["creature_id", "ability_key", "display_name", "ability_type", "power", "cooldown_seconds", "metadata"],
        ability_rows, chunk_size=250, upsert=["display_name", "ability_type", "power", "cooldown_seconds", "metadata"])

    instance_rows = []
    difficulty_rows = []
    difficulty_links = []
    encounter_rows = []
    for instance_index, instance in enumerate(instances):
        instance_id = f"(SELECT id FROM instance_catalog WHERE instance_key={sql(instance['key'])})"
        instance_rows.append((sql(instance["key"]), sql(instance["name"]), sql(instance["kind"]), sql(instance["zone"]),
                              instance["level"], min(80, instance["level"] + 8), instance["party"],
                              instance["max_party"], sql(f"An original {instance['kind'].lower()} set in {REGIONS[instance_index % 10][1]}."),
                              1, sql({"catalog_version": 1})))
        for difficulty_index, difficulty in enumerate(instance["difficulties"]):
            difficulty_key = difficulty.lower()
            difficulty_id = (
                f"(SELECT d.id FROM instance_difficulties d JOIN instance_catalog i ON i.id=d.instance_id "
                f"WHERE i.instance_key={sql(instance['key'])} AND d.difficulty_key={sql(difficulty_key)})"
            )
            group_size = instance["party"] if instance["kind"] != "Trial" else min(5, 1 + difficulty_index * 2)
            difficulty_rows.append((instance_id, sql(difficulty_key), sql(difficulty), instance["level"],
                                    group_size, instance["max_party"], 1 + difficulty_index * 0.5,
                                    instance["lockout"], sql({"tier": difficulty_index + 1})))
            instance_loot_key = "loot_" + instance["key"]
            loot_id = f"(SELECT id FROM loot_tables WHERE loot_key={sql(instance_loot_key)})"
            difficulty_links.append((instance_id, difficulty_id, loot_id))

        encounter_count = 5 if instance["kind"] == "Raid" else 3 if instance["kind"] == "Dungeon" else 2
        for encounter_order in range(1, encounter_count + 1):
            creature = creatures[(instance_index * 3 + encounter_order - 1) % len(creatures)]
            creature_id = f"(SELECT id FROM creature_catalog WHERE creature_key={sql(creature[0])})"
            encounter_rows.append((instance_id, encounter_order, creature_id,
                                   sql(f"{creature[1]} - {instance['name']} Encounter {encounter_order}"),
                                   1 if encounter_order == encounter_count else 0,
                                   sql({"original_encounter": True})))
    lines += insert_statements("instance_catalog",
        ["instance_key", "display_name", "instance_type", "zone_key", "min_level", "max_level",
         "recommended_party_size", "max_party_size", "description", "is_enabled", "metadata"],
        instance_rows, upsert=["display_name", "instance_type", "zone_key", "min_level", "max_level",
                               "recommended_party_size", "max_party_size", "description", "is_enabled", "metadata"])
    lines += insert_statements("instance_difficulties",
        ["instance_id", "difficulty_key", "display_name", "min_level", "recommended_party_size",
         "max_party_size", "loot_multiplier", "lockout_hours", "metadata"],
        difficulty_rows, upsert=["display_name", "min_level", "recommended_party_size", "max_party_size",
                                 "loot_multiplier", "lockout_hours", "metadata"])
    lines += insert_statements("instance_encounters",
        ["instance_id", "encounter_order", "creature_id", "encounter_name", "is_final_encounter", "metadata"],
        encounter_rows, upsert=["creature_id", "encounter_name", "is_final_encounter", "metadata"])
    lines += insert_statements("instance_loot_tables",
        ["instance_id", "difficulty_id", "loot_table_id"], difficulty_links, upsert=["loot_table_id"])

    quest_values = []
    for row in quests:
        quest_values.append(tuple(sql(value) for value in row))
    lines += insert_statements("quests",
        ["quest_key", "title", "quest_type", "description", "quest_chain_key", "quest_order", "zone_key",
         "min_level", "max_level", "is_repeatable", "reset_period", "is_enabled", "source_tag"],
        quest_values, chunk_size=200,
        upsert=["title", "quest_type", "description", "quest_chain_key", "quest_order", "zone_key",
                "min_level", "max_level", "is_repeatable", "reset_period", "is_enabled", "source_tag"])

    objective_values = []
    for quest_key, order, kind, target, count, text, optional, metadata in objectives:
        quest_id = f"(SELECT id FROM quests WHERE quest_key={sql(quest_key)})"
        objective_values.append((quest_id, order, sql(kind), sql(target), count, sql(text), sql(optional), sql(metadata)))
    lines += insert_statements("quest_objectives",
        ["quest_id", "objective_order", "objective_type", "target_key", "required_count",
         "objective_text", "is_optional", "metadata"], objective_values, chunk_size=200,
        upsert=["objective_type", "target_key", "required_count", "objective_text", "is_optional", "metadata"])

    reward_values = []
    for quest_key, reward_order, reward_type, item_key, amount, metadata in rewards:
        quest_id = f"(SELECT id FROM quests WHERE quest_key={sql(quest_key)})"
        item_id = "NULL" if item_key is None else f"(SELECT id FROM items WHERE item_key={sql(item_key)})"
        reward_values.append((quest_id, reward_order, sql(reward_type), item_id, amount, sql(metadata)))
    lines += insert_statements("quest_rewards",
        ["quest_id", "reward_order", "reward_type", "item_id", "amount", "metadata"], reward_values,
        chunk_size=200, upsert=["reward_type", "item_id", "amount", "metadata"])

    prerequisite_values = []
    for quest_key, prerequisite_key in prerequisites:
        quest_id = f"(SELECT id FROM quests WHERE quest_key={sql(quest_key)})"
        prerequisite_id = f"(SELECT id FROM quests WHERE quest_key={sql(prerequisite_key)})"
        prerequisite_values.append((quest_id, prerequisite_id))
    lines += insert_statements("quest_prerequisites",
        ["quest_id", "prerequisite_quest_id"], prerequisite_values, chunk_size=200,
        upsert=["prerequisite_quest_id"])

    lines += [
        "COMMIT;",
        "",
        "-- Catalog counts: 300 items, 300 creatures, 30,000 quests,",
        "-- 100 dungeons, 50 raids, and 100 trials.",
    ]
    output.parent.mkdir(parents=True, exist_ok=True)
    output.write_text("\n\n".join(lines), encoding="utf-8", newline="\n")
    print(f"Generated {output}")
    print(f"items={len(items)} creatures={len(creatures)} quests={len(quests)} "
          f"objectives={len(objectives)} rewards={len(rewards)} prerequisites={len(prerequisites)} "
          f"dungeons=100 raids=50 trials=100")


def main():
    parser = argparse.ArgumentParser(description=__doc__)
    parser.add_argument("--output", type=Path, default=DEFAULT_OUTPUT, help="SQL file to generate")
    args = parser.parse_args()
    generate(args.output.resolve())


if __name__ == "__main__":
    main()
