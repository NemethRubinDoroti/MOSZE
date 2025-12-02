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
        // UI inicializálás
        if (currentState == GameState.MainMenu)
        {
            // TODO: Főmenü UI
        }
    }

    public void StartGame()
    {
        currentState = GameState.Playing;
        currentSeed = Random.Range(0, int.MaxValue);

        if (mapGenerator != null)
        {
            mapGenerator.GenerateMap(currentSeed);
        }

        // Játékost lerakjuk az első szoba közepére
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
    }

    public void ResumeGame()
    {
        currentState = GameState.Playing;
        Time.timeScale = 1f;
    }

    public void EndGame()
    {
        currentState = GameState.GameOver;
        Time.timeScale = 0f;
    }

    private void Update()
    {
        // ESC gombbal pause/unpause
        if (Input.GetKeyDown(KeyCode.Escape))
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
}

