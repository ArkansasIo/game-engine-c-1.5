# Game Logic

This document describes the core game logic implemented in the engine.

## Overview
- Handles main gameplay loop
- Updates entities and components
- Manages scenes and transitions

## Main Functions
- `Update(float deltaTime)`: Updates game state
- `Render()`: Renders current scene
- `LoadScene(sceneName)`: Loads a new scene

## Customization
Game logic can be extended for quests, combat, AI, and more.