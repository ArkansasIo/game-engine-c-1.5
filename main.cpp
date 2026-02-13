#include <iostream>
#include "src/GameEngine/MainMenu.h"
#include "src/GameEngine/MainMenuAssets.h"

int main() {
    MainMenu menu;
    menu.Render();
    int input;
    std::cout << "Select an option: ";
    std::cin >> input;
    menu.HandleInput(input);
    // Example: Display asset path for selected option
    auto options = menu.GetMenuOptions();
    if (input >= 1 && input <= options.size()) {
        MenuAsset asset = MainMenuAssets::GetAsset(options[input-1]);
        std::cout << "Asset path: " << asset.imagePath << std::endl;
    }

    std::cout << "Game Engine Starting..." << std::endl;

    GameEngine engine;
    engine.Initialize();

    // Demo: List save slots
    engine.ListSaveSlots();
    // Demo: Save to slot 1
    engine.SaveGame(0);
    // Demo: Load from slot 1
    engine.LoadGame(0);

    // Demo: Change settings
    engine.SetVolume(80);
    engine.SetBrightness(90);
    engine.SetFullscreen(false);

    // Demo: Show settings
    auto settings = engine.GetSettings();
    std::cout << "Current Settings:\nVolume: " << settings.volume << "\nBrightness: " << settings.brightness << "\nFullscreen: " << (settings.fullscreen ? "ON" : "OFF") << std::endl;

    // Demo: Keybinds
    engine.SetKeybind("moveUp", 'W');
    engine.SetKeybind("moveDown", 'S');
    engine.SetKeybind("moveLeft", 'A');
    engine.SetKeybind("moveRight", 'D');
    engine.SetKeybind("interact", 'E');
    engine.SetKeybind("openMenu", 'M');
    auto keybinds = engine.GetKeybinds();
    std::cout << "Keybinds: Up(" << keybinds.moveUp << "), Down(" << keybinds.moveDown << "), Left(" << keybinds.moveLeft << "), Right(" << keybinds.moveRight << "), Interact(" << keybinds.interact << "), Menu(" << keybinds.openMenu << ")" << std::endl;

    // Demo: Mouse controls
    engine.SetMouseControl("leftClickMove", true);
    engine.SetMouseControl("rightClickInteract", true);
    auto mouseControls = engine.GetMouseControls();
    std::cout << "MouseControls: LeftClickMove(" << (mouseControls.leftClickMove ? "ON" : "OFF") << "), RightClickInteract(" << (mouseControls.rightClickInteract ? "ON" : "OFF") << ")" << std::endl;

    // Demo: Controller mapping
    engine.SetControllerMapping("moveUp", 0);
    engine.SetControllerMapping("moveDown", 1);
    engine.SetControllerMapping("moveLeft", 2);
    engine.SetControllerMapping("moveRight", 3);
    engine.SetControllerMapping("interact", 4);
    engine.SetControllerMapping("openMenu", 5);
    auto controllerMapping = engine.GetControllerMapping();
    std::cout << "ControllerMapping: Up(" << controllerMapping.moveUp << "), Down(" << controllerMapping.moveDown << "), Left(" << controllerMapping.moveLeft << "), Right(" << controllerMapping.moveRight << "), Interact(" << controllerMapping.interact << "), Menu(" << controllerMapping.openMenu << ")" << std::endl;
    std::cout << "PS5: Cross(" << controllerMapping.ps5Cross << "), Circle(" << controllerMapping.ps5Circle << "), Square(" << controllerMapping.ps5Square << "), Triangle(" << controllerMapping.ps5Triangle << "), Options(" << controllerMapping.ps5Options << ")" << std::endl;
    std::cout << "Xbox: A(" << controllerMapping.xboxA << "), B(" << controllerMapping.xboxB << "), X(" << controllerMapping.xboxX << "), Y(" << controllerMapping.xboxY << "), Menu(" << controllerMapping.xboxMenu << ")" << std::endl;

    StartGame();
    std::cout << "Game Engine Exited." << std::endl;
    return 0;
}
