// MainMenuAssets.h
#pragma once
#include <string>

struct MenuAsset {
    std::string name;
    std::string imagePath;
};

class MainMenuAssets {
public:
    static MenuAsset GetAsset(const std::string& name);
};
