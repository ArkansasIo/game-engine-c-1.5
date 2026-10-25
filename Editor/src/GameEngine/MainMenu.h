// MainMenu.h
#pragma once
#include <string>
#include <vector>

class MainMenu {
public:
    MainMenu();
    void Render();
    void HandleInput(int input);
    std::vector<std::string> GetMenuOptions() const;
private:
    std::vector<std::string> menuOptions;
};
