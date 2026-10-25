# UML High-Level Overview

## Main Components
- GameEngine
- Character
- Inventory
- Equipment
- Combat
- World
- Zone
- GroupFinder
- Event

## Relationships
- Character owns Inventory and Equipment
- Equipment and Items are managed by Inventory
- Combat interacts with Character, Equipment, and Items
- World contains Zones, Cities, Towns, Dungeons, Trials, Raids
- GroupFinder manages player groups for events

## Example Diagram (Mermaid)

```
classDiagram
    Character <|-- Inventory
    Character <|-- Equipment
    Inventory <|-- Item
    Equipment <|-- Item
    Combat <|-- Character
    Combat <|-- Equipment
    Combat <|-- Item
    World <|-- Zone
    Zone <|-- City
    Zone <|-- Town
    Zone <|-- Dungeon
    Zone <|-- Trial
    Zone <|-- Raid
    GroupFinder <|-- Event
```
