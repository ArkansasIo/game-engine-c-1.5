# Ethereal Veni RPG/MMORPG Stats

`[CORE][GAME]` This folder defines the shared stat contract for **Ethereo Pandora**.

## Stat layers

1. **Core attributes** — Strength, Dexterity, Constitution, Intelligence, Wisdom, Vitality, Spirit, Luck, Willpower, Charisma.
2. **Derived combat stats** — health, mana, stamina, attack power, magic power, defense, armor, accuracy, evasion, critical chance/damage, attack/cast speed, movement speed, healing power, threat, and regeneration.
3. **Resources** — HP, MP, stamina, rage, energy, experience, and progression thresholds.
4. **Resistances** — physical, elemental, poison, arcane, shadow, holy, and control resistance.
5. **Skills** — skill level, experience, and rank.
6. **Modifiers** — flat and percentage changes from equipment, buffs, debuffs, talents, classes, races, zones, abilities, and temporary effects.

## RPG/MMORPG integration

The framework is intentionally data-driven. Characters, NPCs, monsters, bosses, companions, and player-controlled entities can all own an `RpgStatBlock`. Combat, crafting, quests, AI, equipment, and UI should consume the same stat API rather than maintaining separate stat formulas.

## Progression

The default experience helper uses the project's established quadratic/exponential-style curve:

`RequiredXP(level) = floor(level^2.5 * 500)`

The engine may override progression curves through game configuration without changing the stat model.

## Buffs and debuffs

`StatEffectController` manages timed effects. Effects identify their source, target stat, modifier kind, value, polarity, duration, and stack count. This supports persistent equipment modifiers as well as temporary combat effects.

## Compatibility

Existing GoonZu-oriented stat classes remain historical/reference material. Their useful concepts are represented here under the Ethereal Veni namespace rather than allowing legacy namespaces to define the active engine API. Stable serialized IDs should be migrated explicitly instead of being renamed blindly.
