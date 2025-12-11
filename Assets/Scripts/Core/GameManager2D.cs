using UnityEngine;

public class GameManager2D : MonoBehaviour
{
    public static GameManager2D Instance { get; private set; }

    public enum GameState
    {
        MainMenu,
        Playing,
        Paused,
        GameOver
    }

    public GameState currentState = GameState.MainMenu;
    public Player2D player;
    public MapGenerator mapGenerator;
    public CombatManager combatManager;
    public SaveSystem saveSystem;
    public ScoreSystem scoreSystem;
    public HostageManager hostageManager;
    public ItemManager itemManager;
    public ExperienceSystem experienceSystem;
    public int currentSeed;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        // UI inicializálás - ha MainMenu állapotban vagyunk, mutassuk a főmenüt
        if (currentState == GameState.MainMenu)
        {
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowMainMenu();
            }
        }
    }

    public void StartGame()
    {
        // Random seed-del indít
        StartGameWithSeed(Random.Range(0, int.MaxValue));
    }
    
    public void StartGameWithSeed(int seed)
    {
        if (mapGenerator == null)
        {
            Debug.LogError("[GameManager2D] HIBA: MapGenerator NULL! Nem lehet elindítani a játékot.");
            return;
        }
        
        currentState = GameState.Playing;
        currentSeed = seed;
        
        // Pontszám resetelése
        if (scoreSystem != null)
        {
            scoreSystem.ResetScore();
        }
        
        // XP rendszer resetelése
        if (experienceSystem != null)
        {
            experienceSystem.Reset();
        }
        
        mapGenerator.GenerateMap(currentSeed);
        
        SetupPlayerPosition();
        
        // UI frissítése
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnGameStateChanged(currentState);
        }
        
        Debug.Log($"[GameManager2D] Játék elindítva seed-del: {currentSeed}");
    }
    
    public void StartGameWithMap(string mapFileName)
    {
        if (mapGenerator == null)
        {
            Debug.LogError("[GameManager2D] HIBA: MapGenerator NULL! Nem lehet elindítani a játékot.");
            return;
        }
        
        if (string.IsNullOrEmpty(mapFileName))
        {
            Debug.LogError("[GameManager2D] HIBA: Üres pálya fájlnév!");
            return;
        }
        
        currentState = GameState.Playing;
        
        // Pontszám resetelése
        if (scoreSystem != null)
        {
            scoreSystem.ResetScore();
        }
        
        // XP rendszer resetelése
        if (experienceSystem != null)
        {
            experienceSystem.Reset();
        }
        
        // Pálya importálása JSON-ból
        mapGenerator.ImportMapFromJSON(mapFileName);
        
        // Seed beállítása az importált pályából
        MapExportData mapData = mapGenerator.ExportMap();
        if (mapData != null)
        {
            currentSeed = mapData.seed;
        }
        
        SetupPlayerPosition();
        
        // UI frissítése
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnGameStateChanged(currentState);
        }
        
        Debug.Log($"[GameManager2D] Játék elindítva pályával: {mapFileName}");
    }
    
    private void SetupPlayerPosition()
    {
        if (player != null && mapGenerator != null)
        {
            var rooms = mapGenerator.GetRooms();
            if (rooms != null && rooms.Count > 0)
            {
                Vector2Int startPos = rooms[0].GetCenter();
                player.position = startPos;
                player.transform.position = new Vector3(startPos.x, startPos.y, 0);
            }
        }
    }

    public void PauseGame()
    {
        currentState = GameState.Paused;
        Time.timeScale = 0f;
        
        // UI frissítése
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnGameStateChanged(currentState);
        }
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        
        // UI frissítése
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnGameStateChanged(currentState);
        }
    }

    public void EndGame()
    {
        EndGame(false); // Vereség
    }
    
    public void EndGame(bool isVictory)
    {
        currentState = GameState.GameOver;
        Time.timeScale = 0f;
        
        // UI frissítése
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnGameStateChanged(currentState, isVictory);
        }
    }
    
    public void WinGame()
    {
        if (scoreSystem != null)
        {
            scoreSystem.AddScore(500);
        }
        
        EndGame(true);
        Debug.Log("[GameManager2D] Győzelem! Minden túsz megmentve és a boss legyőzve!");
    }
    
    public void SaveGame()
    {
        if (saveSystem == null || player == null || mapGenerator == null)
        {
            Debug.LogWarning("Nem lehet menteni a játékot: hiányzó komponensek");
            return;
        }
        
        // Pálya exportálása
        MapExportData fullMapData = mapGenerator.ExportMap();
        
        GameData2D data = new GameData2D
        {
            playerData = new PlayerData2D
            {
                position = player.position,
                health = player.stats.currentHealth,
                maxHealth = player.stats.maxHealth,
                stats = new StatsData
                {
                    attack = player.stats.attack,
                    defense = player.stats.defense,
                    speed = player.stats.speed,
                    accuracy = player.stats.accuracy
                },
                level = experienceSystem != null ? experienceSystem.currentLevel : 1,
                currentXP = experienceSystem != null ? experienceSystem.currentXP : 0
            },
            mapData = new MapData2D
            {
                seed = currentSeed,
                width = mapGenerator.mapWidth,
                height = mapGenerator.mapHeight,
                fullMapData = fullMapData
            },
            gameData = new GameData
            {
                score = scoreSystem != null ? scoreSystem.currentScore : 0,
                playTime = Time.time,
                saveTime = ""
            }
        };
        
        saveSystem.SaveGame(data);
    }
    
    public void LoadGame()
    {
        if (saveSystem == null)
        {
            Debug.LogWarning("Nem lehet betölteni a játékot: SaveSystem nem található");
            return;
        }
        
        GameData2D data = saveSystem.LoadGame();
        if (data == null)
        {
            Debug.LogWarning("Nem található mentési adat");
            return;
        }
        
        // Játékos adatok visszaállítása
        if (player != null && data.playerData != null)
        {
            player.position = data.playerData.position;
            player.transform.position = new Vector3(data.playerData.position.x, data.playerData.position.y, 0);
            
            if (player.stats != null)
            {
                player.stats.currentHealth = data.playerData.health;
                player.stats.maxHealth = data.playerData.maxHealth;
                
                if (data.playerData.stats != null)
                {
                    player.stats.attack = data.playerData.stats.attack;
                    player.stats.defense = data.playerData.stats.defense;
                    player.stats.speed = data.playerData.stats.speed;
                    player.stats.accuracy = data.playerData.stats.accuracy;
                }
            }
            
            // XP és szint visszaállítása
            if (experienceSystem != null)
            {
                experienceSystem.currentLevel = data.playerData.level;
                experienceSystem.currentXP = data.playerData.currentXP;
                experienceSystem.CalculateXPToNextLevel();
            }
        }
        
        // Térkép visszaállítása
        if (mapGenerator != null && data.mapData != null)
        {
            currentSeed = data.mapData.seed;
            
            // Ha van teljes pálya adat, importáljuk, különben generáljuk seed alapján
            if (data.mapData.fullMapData != null)
            {
                mapGenerator.ImportMapFromJSON("", data.mapData.fullMapData);
            }
            else
            {
                // Seed alapú generálás
                mapGenerator.GenerateMap(currentSeed);
            }
        }
        
        // Pontszám visszaállítása
        if (scoreSystem != null && data.gameData != null)
        {
            scoreSystem.currentScore = data.gameData.score;
        }
        
        currentState = GameState.Playing;
        Time.timeScale = 1f;
        
        // UI frissítése
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnGameStateChanged(currentState);
        }
        
        Debug.Log("Játék betöltve sikeresen");
    }
    
    public void GenerateNewMap()
    {
        if (mapGenerator == null)
        {
            Debug.LogWarning("Nem lehet generálni a térképet: MapGenerator nem található");
            return;
        }
        
        currentSeed = Random.Range(0, int.MaxValue);
        mapGenerator.GenerateMap(currentSeed);
        
        if (player != null && mapGenerator != null)
        {
            var rooms = mapGenerator.GetRooms();
            if (rooms != null && rooms.Count > 0)
            {
                Vector2Int startPos = rooms[0].GetCenter();
                player.position = startPos;
                player.transform.position = new Vector3(startPos.x, startPos.y, 0);
            }
        }
        
        Debug.Log($"Új térkép generálva seed-del: {currentSeed}");
    }

    private void Update()
    {
        // ESC gombbal pause/unpause
        #if ENABLE_INPUT_SYSTEM
        if (UnityEngine.InputSystem.Keyboard.current != null && 
            UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            HandleEscapeKey();
        }
        #else
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            HandleEscapeKey();
        }
        #endif
    }
    
    private void HandleEscapeKey()
    {
        if (currentState == GameState.Playing)
        {
            PauseGame();
        }
        else if (currentState == GameState.Paused)
        {
            ResumeGame();
        }
    }
}

