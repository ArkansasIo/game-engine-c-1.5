using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace GoonzuGame
{
    public enum GameState
    {
        MainMenu,
        CharacterCreation,
        Loading,
        Playing,
        Paused,
        GameOver
    }

    public class GoonzuGameManager : MonoBehaviour
    {
        public static GoonzuGameManager Instance { get; private set; }

        [Header("Game Settings")]
        public GameState currentGameState = GameState.MainMenu;
        public bool isGamePaused = false;

        [Header("Player")]
        public GoonzuCharacter playerCharacter;
        public GameObject playerPrefab;

        [Header("Managers")]
        public GoonzuAssetManager assetManager;
        public GoonzuUIManager uiManager;
        public GoonzuCombatManager combatManager;
        public GoonzuQuestManager questManager;
        public GoonzuWorldManager worldManager;
        public GoonzuSaveLoadManager saveLoadManager;

        [Header("Game World")]
        public Camera mainCamera;
        public Transform playerSpawnPoint;

        [Header("Game Settings")]
        public float gameSpeed = 1f;
        public bool enableDebugMode = false;

        // Game time
        private float gameTime = 0f;
        private int dayCount = 1;
        private float timeOfDay = 12f; // 12:00 PM start

        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject);
            }
            else
            {
                Destroy(gameObject);
                return;
            }

            InitializeManagers();
        }

        void Start()
        {
            SetGameState(GameState.MainMenu);
        }

        void Update()
        {
            UpdateGameTime();

            switch (currentGameState)
            {
                case GameState.Playing:
                    HandleGameplayInput();
                    break;
                case GameState.Paused:
                    HandlePauseInput();
                    break;
            }

            if (enableDebugMode)
            {
                HandleDebugInput();
            }
        }

        void InitializeManagers()
        {
            // Create managers if they don't exist
            if (assetManager == null)
            {
                assetManager = gameObject.AddComponent<GoonzuAssetManager>();
            }
            if (uiManager == null)
            {
                uiManager = gameObject.AddComponent<GoonzuUIManager>();
            }
            if (combatManager == null)
            {
                combatManager = gameObject.AddComponent<GoonzuCombatManager>();
            }
            if (questManager == null)
            {
                questManager = gameObject.AddComponent<GoonzuQuestManager>();
            }
            if (worldManager == null)
            {
                worldManager = gameObject.AddComponent<GoonzuWorldManager>();
            }
            if (saveLoadManager == null)
            {
                saveLoadManager = gameObject.AddComponent<GoonzuSaveLoadManager>();
            }
        }

        void UpdateGameTime()
        {
            if (currentGameState == GameState.Playing && !isGamePaused)
            {
                gameTime += Time.deltaTime * gameSpeed;
                timeOfDay += (Time.deltaTime * gameSpeed) / 60f; // 1 real second = 1 game minute

                if (timeOfDay >= 24f)
                {
                    timeOfDay = 0f;
                    dayCount++;
                    OnNewDay();
                }
            }
        }

        void OnNewDay()
        {
            Debug.Log($"New day: {dayCount}");

            // Reset daily quests, refresh shops, etc.
            if (questManager != null)
            {
                // Generate new daily quests
            }

            // Heal player slightly overnight
            if (playerCharacter != null)
            {
                playerCharacter.Heal(playerCharacter.stats.maxHealth / 10);
            }
        }

        public void SetGameState(GameState newState)
        {
            GameState oldState = currentGameState;
            currentGameState = newState;

            Debug.Log($"Game state changed: {oldState} -> {newState}");

            switch (newState)
            {
                case GameState.MainMenu:
                    ShowMainMenu();
                    break;
                case GameState.CharacterCreation:
                    ShowCharacterCreation();
                    break;
                case GameState.Loading:
                    ShowLoadingScreen();
                    break;
                case GameState.Playing:
                    StartGameplay();
                    break;
                case GameState.Paused:
                    PauseGame();
                    break;
                case GameState.GameOver:
                    ShowGameOver();
                    break;
            }
        }

        void ShowMainMenu()
        {
            if (uiManager != null)
            {
                uiManager.ShowPanel("main_menu");
            }
        }

        void ShowCharacterCreation()
        {
            if (uiManager != null)
            {
                uiManager.ShowPanel("character_creation");
            }
        }

        void ShowLoadingScreen()
        {
            if (uiManager != null)
            {
                uiManager.ShowPanel("loading");
            }
        }

        void StartGameplay()
        {
            isGamePaused = false;
            Time.timeScale = gameSpeed;

            if (uiManager != null)
            {
                uiManager.ShowPanel("game_ui");
            }
        }

        void PauseGame()
        {
            isGamePaused = true;
            Time.timeScale = 0f;

            if (uiManager != null)
            {
                uiManager.ShowPanel("pause_menu");
            }
        }

        void ShowGameOver()
        {
            isGamePaused = true;
            Time.timeScale = 0f;

            if (uiManager != null)
            {
                uiManager.ShowPanel("game_over");
            }
        }

        void HandleGameplayInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetGameState(GameState.Paused);
            }

            if (Input.GetKeyDown(KeyCode.F5))
            {
                saveLoadManager.QuickSave();
            }

            if (Input.GetKeyDown(KeyCode.F9))
            {
                saveLoadManager.QuickLoad();
            }

            if (Input.GetKeyDown(KeyCode.Tab))
            {
                if (uiManager != null)
                {
                    uiManager.TogglePanel("character_sheet");
                }
            }

            if (Input.GetKeyDown(KeyCode.I))
            {
                if (uiManager != null)
                {
                    uiManager.TogglePanel("inventory");
                }
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                if (uiManager != null)
                {
                    uiManager.TogglePanel("quest_log");
                }
            }

            if (Input.GetKeyDown(KeyCode.M))
            {
                if (uiManager != null)
                {
                    uiManager.TogglePanel("world_map");
                }
            }
        }

        void HandlePauseInput()
        {
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                SetGameState(GameState.Playing);
            }
        }

        void HandleDebugInput()
        {
            if (Input.GetKeyDown(KeyCode.F1))
            {
                // Toggle debug mode
                enableDebugMode = !enableDebugMode;
                Debug.Log($"Debug mode: {enableDebugMode}");
            }

            if (Input.GetKeyDown(KeyCode.F2))
            {
                // Give player random item
                if (playerCharacter != null)
                {
                    GoonzuItem randomItem = GoonzuItemManager.Instance.GenerateRandomItem();
                    playerCharacter.AddItemToInventory(randomItem);
                    Debug.Log($"Added random item: {randomItem.itemName}");
                }
            }

            if (Input.GetKeyDown(KeyCode.F3))
            {
                // Level up player
                if (playerCharacter != null)
                {
                    playerCharacter.GainExperience(1000);
                    Debug.Log("Player leveled up");
                }
            }

            if (Input.GetKeyDown(KeyCode.F4))
            {
                // Teleport to random location
                if (playerCharacter != null)
                {
                    Vector3 randomPos = new Vector3(
                        Random.Range(-50f, 50f),
                        Random.Range(-50f, 50f),
                        0f
                    );
                    playerCharacter.transform.position = randomPos;
                    Debug.Log($"Teleported to: {randomPos}");
                }
            }
        }

        public void StartNewGame()
        {
            SetGameState(GameState.CharacterCreation);
        }

        public void LoadGame(string saveName)
        {
            SetGameState(GameState.Loading);

            // Load game data
            bool success = saveLoadManager.LoadGame(saveName);

            if (success)
            {
                SetGameState(GameState.Playing);
            }
            else
            {
                SetGameState(GameState.MainMenu);
                Debug.LogError("Failed to load game");
            }
        }

        public void CreateCharacter(string name, GoonzuCharacter.CharacterRace race, GoonzuCharacter.CharacterClass charClass)
        {
            SetGameState(GameState.Loading);

            // Create player character
            GameObject playerObj = Instantiate(playerPrefab, playerSpawnPoint.position, Quaternion.identity);
            playerCharacter = playerObj.GetComponent<GoonzuCharacter>();

            if (playerCharacter != null)
            {
                playerCharacter.InitializeCharacter(name, race, charClass);

                // Set camera to follow player
                if (mainCamera != null)
                {
                    mainCamera.transform.SetParent(playerObj.transform);
                    mainCamera.transform.localPosition = new Vector3(0, 0, -10);
                }

                SetGameState(GameState.Playing);
                Debug.Log($"Created character: {name} ({race} {charClass})");
            }
            else
            {
                Debug.LogError("Failed to create player character");
                SetGameState(GameState.MainMenu);
            }
        }

        public void ExitToMainMenu()
        {
            // Save game before exiting
            saveLoadManager.AutoSave();

            // Destroy player
            if (playerCharacter != null)
            {
                Destroy(playerCharacter.gameObject);
                playerCharacter = null;
            }

            // Reset camera
            if (mainCamera != null)
            {
                mainCamera.transform.SetParent(null);
                mainCamera.transform.position = new Vector3(0, 0, -10);
            }

            SetGameState(GameState.MainMenu);
        }

        public void QuitGame()
        {
            // Save game before quitting
            saveLoadManager.AutoSave();

            #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
            #else
                Application.Quit();
            #endif
        }

        public GoonzuCharacter GetPlayer()
        {
            return playerCharacter;
        }

        public string GetCurrentTimeString()
        {
            int hours = (int)timeOfDay;
            int minutes = (int)((timeOfDay - hours) * 60);
            string ampm = hours >= 12 ? "PM" : "AM";

            if (hours > 12) hours -= 12;
            if (hours == 0) hours = 12;

            return $"{hours:D2}:{minutes:D2} {ampm}";
        }

        public int GetDayCount()
        {
            return dayCount;
        }

        public float GetGameTime()
        {
            return gameTime;
        }

        public void SetGameSpeed(float speed)
        {
            gameSpeed = Mathf.Max(0.1f, speed);
            if (currentGameState == GameState.Playing && !isGamePaused)
            {
                Time.timeScale = gameSpeed;
            }
        }

        public void PauseGameplay(bool pause)
        {
            isGamePaused = pause;
            Time.timeScale = pause ? 0f : gameSpeed;
        }

        public void OnPlayerDeath()
        {
            SetGameState(GameState.GameOver);
        }

        public void RespawnPlayer()
        {
            if (playerCharacter != null)
            {
                playerCharacter.transform.position = playerSpawnPoint.position;
                playerCharacter.FullHeal();
                SetGameState(GameState.Playing);
            }
        }

        // Utility methods
        public Vector3 GetMouseWorldPosition()
        {
            Vector3 mousePos = Input.mousePosition;
            mousePos.z = -mainCamera.transform.position.z;
            return mainCamera.ScreenToWorldPoint(mousePos);
        }

        public GameObject FindNearestGameObject(Vector3 position, string tag, float maxDistance = float.MaxValue)
        {
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);
            GameObject nearest = null;
            float nearestDistance = maxDistance;

            foreach (GameObject obj in objects)
            {
                float distance = Vector3.Distance(position, obj.transform.position);
                if (distance < nearestDistance)
                {
                    nearest = obj;
                    nearestDistance = distance;
                }
            }

            return nearest;
        }

        public List<GameObject> FindGameObjectsInRange(Vector3 position, string tag, float range)
        {
            List<GameObject> objectsInRange = new List<GameObject>();
            GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

            foreach (GameObject obj in objects)
            {
                if (Vector3.Distance(position, obj.transform.position) <= range)
                {
                    objectsInRange.Add(obj);
                }
            }

            return objectsInRange;
        }

        // Debug methods
        public void LogGameState()
        {
            Debug.Log($"Game State: {currentGameState}");
            Debug.Log($"Game Time: {GetCurrentTimeString()} (Day {dayCount})");
            Debug.Log($"Game Speed: {gameSpeed}");
            Debug.Log($"Paused: {isGamePaused}");

            if (playerCharacter != null)
            {
                Debug.Log($"Player: {playerCharacter.characterName} (Level {playerCharacter.stats.level})");
                Debug.Log($"Player Position: {playerCharacter.transform.position}");
            }
        }

        public void ResetGame()
        {
            // Reset all game data
            gameTime = 0f;
            dayCount = 1;
            timeOfDay = 12f;

            ExitToMainMenu();
        }
    }
}