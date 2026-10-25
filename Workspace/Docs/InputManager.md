# Input Manager

This document describes the InputManager class and its role in handling input for the game engine.

## Responsibilities
- Process keyboard, mouse, and controller input
- Support customizable keybinds and controller mappings

## Methods
- `ProcessInput(keybinds, mouseControls, controllerMapping)`
- `IsKeyPressed(char key)`
- `IsMouseButtonPressed(int button)`
- `IsControllerButtonPressed(int button)`

## Customization
Input handling can be extended for advanced features (e.g., analog sticks, vibration).