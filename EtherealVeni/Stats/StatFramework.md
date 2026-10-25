# RPG/MMORPG Stat Framework — Technical Reference

## Attribute model

The engine supports ten core attributes: Strength, Dexterity, Constitution, Intelligence, Wisdom, Vitality, Spirit, Luck, Willpower, and Charisma.

## Stat pipeline

```text
Base attributes -> race/class/level -> equipment -> talents/skills
-> buffs/debuffs/auras -> zone/encounter modifiers -> derived stats
-> combat / AI / UI / progression / persistence
```

## D&D compatibility

The six traditional D&D-style ability scores remain represented by Strength, Dexterity, Constitution, Intelligence, Wisdom, and Charisma. `RpgProgression.AbilityModifier` uses the conventional `(score - 10) / 2` floor calculation. The additional attributes provide MMO-oriented progression without forcing every system to depend on tabletop rules.

## Buffs and debuffs

Effects can be permanent or timed and can originate from items, abilities, talents, consumables, quests, zones, world events, party auras, guild systems, and scripted gameplay. Stacking is explicit so the server can enforce deterministic rules.

## Server authority

For multiplayer gameplay, the authoritative server owns canonical stat state. Clients may calculate predicted/display values, but accepted changes to progression, inventory-derived modifiers, buffs, debuffs, resources, and combat outcomes must be validated server-side.
