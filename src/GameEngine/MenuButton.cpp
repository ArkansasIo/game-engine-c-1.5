// MenuButton.cpp
#include "MenuButton.h"
#include <iostream>

MenuButton::MenuButton(const std::string& label, const std::string& assetPath)
    : label(label), assetPath(assetPath) {}

void MenuButton::Render() {
    std::cout << "Button: " << label << " (Asset: " << assetPath << ")" << std::endl;
}
