// MenuButton.h
#pragma once
#include <string>

class MenuButton {
public:
    MenuButton(const std::string& label, const std::string& assetPath);
    void Render();
private:
    std::string label;
    std::string assetPath;
};
