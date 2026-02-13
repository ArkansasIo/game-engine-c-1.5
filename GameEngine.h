        // Controller controls
        struct ControllerMapping {
            // Common actions
            int moveUp = 0;    // D-pad up
            int moveDown = 1;  // D-pad down
            int moveLeft = 2;  // D-pad left
            int moveRight = 3; // D-pad right
            int interact = 4;  // A/X (Xbox/PS5)
            int openMenu = 5;  // Start/Options
            int jump = 6;      // B/Circle
            int attack = 7;    // X/Square
            int special = 8;   // Y/Triangle
            int pause = 9;     // Menu/Options

            // PS5 specific
            int ps5Cross = 4;     // Cross (X)
            int ps5Circle = 6;    // Circle
            int ps5Square = 7;    // Square
            int ps5Triangle = 8;  // Triangle
            int ps5Options = 5;   // Options

            // Xbox specific
            int xboxA = 4;        // A
            int xboxB = 6;        // B
            int xboxX = 7;        // X
            int xboxY = 8;        // Y
            int xboxMenu = 5;     // Menu
        } controllerMapping;
        // Controller functions
        void SetControllerMapping(const std::string& action, int button);
        ControllerMapping GetControllerMapping() const;
    // Keybinds
    struct Keybinds {
        char moveUp = 'W';
        char moveDown = 'S';
        char moveLeft = 'A';
        char moveRight = 'D';
        char interact = 'E';
        char openMenu = 'M';
        // Add more as needed
    } keybinds;

    // Mouse controls
    struct MouseControls {
        bool leftClickMove = true;
        bool rightClickInteract = true;
        // Add more as needed
    } mouseControls;
    // Keybinds functions
    void SetKeybind(const std::string& action, char key);
    Keybinds GetKeybinds() const;

    // Mouse controls functions
    void SetMouseControl(const std::string& action, bool enabled);
    MouseControls GetMouseControls() const;
// Example header for C++ game engine
#ifndef GAME_ENGINE_H
#define GAME_ENGINE_H

#include <string>
#include <vector>
#include <memory>

// Forward declarations
class Scene;
class Entity;
class Component;
class Renderer;
class InputManager;
class PhysicsManager;
class AudioManager;

// Game Engine class
class GameEngine {
private:
    bool isRunning;
    std::unique_ptr<Renderer> renderer;
    std::unique_ptr<InputManager> inputManager;
    std::unique_ptr<PhysicsManager> physicsManager;
    std::unique_ptr<AudioManager> audioManager;
    std::vector<std::unique_ptr<Scene>> scenes;
    Scene* currentScene;

    // Save slots
    std::vector<std::string> saveSlots; // file paths or slot names
    int currentSaveSlot = -1;

    // Options/settings
    struct GameSettings {
        int volume = 100;
        int brightness = 100;
        bool fullscreen = true;
        // Add more settings as needed
    } settings;

public:
    GameEngine();
    ~GameEngine();

    void Initialize();
    void Run();
    void Update(float deltaTime);
    void Render();
    void Shutdown();

    void LoadScene(const std::string& sceneName);
    void AddScene(std::unique_ptr<Scene> scene);

    // Save/load slot functions
    void SaveGame(int slot);
    void LoadGame(int slot);
    int GetCurrentSaveSlot() const;
    void ListSaveSlots() const;

    // Options/settings functions
    void SetVolume(int volume);
    void SetBrightness(int brightness);
    void SetFullscreen(bool fullscreen);
    GameSettings GetSettings() const;
};

// Scene class
class Scene {
private:
    std::string name;
    std::vector<std::unique_ptr<Entity>> entities;

public:
    Scene(const std::string& name);
    ~Scene();

    void AddEntity(std::unique_ptr<Entity> entity);
    void Update(float deltaTime);
    void Render();
    const std::string& GetName() const;
};

// Entity class
class Entity {
private:
    std::string name;
    std::vector<std::unique_ptr<Component>> components;

public:
    Entity(const std::string& name);
    ~Entity();

    void AddComponent(std::unique_ptr<Component> component);
    void Update(float deltaTime);
    void Render();
    const std::string& GetName() const;
};

// Component base class
class Component {
public:
    virtual ~Component() = default;
    virtual void Update(float deltaTime) = 0;
    virtual void Render() = 0;
};

// Renderer class
class Renderer {
public:
    void RenderScene(const Scene& scene);
};

// InputManager class
class InputManager {
public:
    void ProcessInput(const GameEngine::Keybinds& keybinds, const GameEngine::MouseControls& mouseControls, const GameEngine::ControllerMapping& controllerMapping);
    bool IsKeyPressed(char key);
    bool IsMouseButtonPressed(int button); // 0: left, 1: right
    bool IsControllerButtonPressed(int button);
};

// PhysicsManager class
class PhysicsManager {
public:
    void UpdatePhysics(float deltaTime);
};

// AudioManager class
class AudioManager {
public:
    void PlaySound(const std::string& soundName);
};

void StartGame();

#endif // GAME_ENGINE_H
