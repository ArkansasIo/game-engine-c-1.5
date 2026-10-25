// MainMenu.cpp
#include "MainMenu.h"
#include <iostream>

MainMenu::MainMenu() {
    menuOptions = {
        "Start Game",
        "My Character",
        "Inventory",
        "Quests",
        "Community",
        "Market",
        "Settings",
        "Exit Game"
    };
}

void MainMenu::Render() {
    std::cout << "=== GOONZU MAIN MENU ===" << std::endl;
    for (size_t i = 0; i < menuOptions.size(); ++i) {
        std::cout << i + 1 << ". " << menuOptions[i] << std::endl;
    }
}

void MainMenu::HandleInput(int input) {
    if (input < 1 || input > menuOptions.size()) {
        std::cout << "Invalid option." << std::endl;
        return;
    }
    std::cout << "Selected: " << menuOptions[input - 1] << std::endl;
    // Add logic for each menu option
}

std::vector<std::string> MainMenu::GetMenuOptions() const {
    return menuOptions;
}
