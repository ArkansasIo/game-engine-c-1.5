// MainMenuAssets.cpp
#include "MainMenuAssets.h"
#include <map>

static std::map<std::string, std::string> assetMap = {
    {"Start Game", "/Assets/MainMenu/start_game.png"},
    {"My Character", "/Assets/MainMenu/my_character.png"},
    {"Inventory", "/Assets/MainMenu/inventory.png"},
    {"Quests", "/Assets/MainMenu/quests.png"},
    {"Community", "/Assets/MainMenu/community.png"},
    {"Market", "/Assets/MainMenu/market.png"},
    {"Settings", "/Assets/MainMenu/settings.png"},
    {"Exit Game", "/Assets/MainMenu/exit_game.png"}
};

MenuAsset MainMenuAssets::GetAsset(const std::string& name) {
    MenuAsset asset;
    asset.name = name;
    asset.imagePath = assetMap[name];
    return asset;
}
