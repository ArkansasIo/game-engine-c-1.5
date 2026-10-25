# UML UI Module Overview

## Main UI Components
- MainWindow
- InventoryWindow
- CharacterWindow
- QuestWindow
- MarketWindow
- PartyWindow
- OptionsWindow
- SkillsWindow
- CraftingWindow
- ChatWindow
- HUDWindow

## Example Diagram (Mermaid)

```
classDiagram
    MainWindow <|-- InventoryWindow
    MainWindow <|-- CharacterWindow
    MainWindow <|-- QuestWindow
    MainWindow <|-- MarketWindow
    MainWindow <|-- PartyWindow
    MainWindow <|-- OptionsWindow
    MainWindow <|-- SkillsWindow
    MainWindow <|-- CraftingWindow
    MainWindow <|-- ChatWindow
    MainWindow <|-- HUDWindow
```
