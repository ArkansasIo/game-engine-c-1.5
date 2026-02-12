// Main entry point for the C++ game engine
#include <iostream>

int main() {
    std::cout << "Game Engine Started!" << std::endl;
    bool running = true;
    int tick = 0;
    while (running && tick < 10) {
        std::cout << "Tick: " << tick << std::endl;
        // TODO: Add input, update, render, etc.
        tick++;
    }
    std::cout << "Game Engine Shutting Down." << std::endl;
    return 0;
}
