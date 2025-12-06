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
        if (mapGenerator == null)
        {
            Debug.LogError("[GameManager2D] HIBA: MapGenerator NULL! Nem lehet elindítani a játékot.");
            return;
        }
        
        currentState = GameState.Playing;
        currentSeed = Random.Range(0, int.MaxValue);
        
        // Pontszám resetelése
        if (scoreSystem != null)
        {
            scoreSystem.ResetScore();
        }
        
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
        
        // UI frissítése
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnGameStateChanged(currentState);
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
        currentState = GameState.GameOver;
        Time.timeScale = 0f;
        
        // UI frissítése
        if (UIManager.Instance != null)
        {
            UIManager.Instance.OnGameStateChanged(currentState);
        }
    }
    
    public void SaveGame()
    {
        if (saveSystem == null || player == null || mapGenerator == null)
        {
            Debug.LogWarning("Nem lehet menteni a játékot: hiányzó komponensek");
            return;
        }
        
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
                }
            },
            mapData = new MapData2D
            {
                seed = currentSeed,
                width = mapGenerator.mapWidth,
                height = mapGenerator.mapHeight
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
        }
        
        // Térkép visszaállítása
        if (mapGenerator != null && data.mapData != null)
        {
            currentSeed = data.mapData.seed;
            mapGenerator.GenerateMap(currentSeed);
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
        
        // Reset player position to first room center
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

