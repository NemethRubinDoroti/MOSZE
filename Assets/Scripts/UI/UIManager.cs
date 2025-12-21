using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Menu References")]
    public MainMenuUI mainMenuUI;
    public PauseMenuUI pauseMenuUI;
    public GameOverUI gameOverUI;
    public InGameHUD inGameHUD;
    public CombatUI combatUI;

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
            return;
        }
    }

    private void Start()
    {
        // Kezdetben csak a főmenü látszik, ha a GameManager MainMenu állapotban van
        if (GameManager2D.Instance != null && GameManager2D.Instance.currentState == GameManager2D.GameState.MainMenu)
        {
            ShowMainMenu();
        }
        else
        {
            // Ha már Playing állapotban vagyunk, mutassuk a HUD-ot
            if (GameManager2D.Instance != null && GameManager2D.Instance.currentState == GameManager2D.GameState.Playing)
            {
                HideAllMenus();
                ShowInGameHUD();
            }
        }
    }

    public void ShowMainMenu()
    {
        HideAllMenus();
        if (mainMenuUI != null)
        {
            mainMenuUI.gameObject.SetActive(true);
            mainMenuUI.OnShow();
        }
    }

    public void ShowPauseMenu()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.gameObject.SetActive(true);
            pauseMenuUI.OnShow();
        }
    }

    public void HidePauseMenu()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.gameObject.SetActive(false);
        }
    }


    public void ShowGameOverMenu(int finalScore, bool isVictory = false)
    {
        Debug.Log($"[UIManager] ShowGameOverMenu hívva: finalScore={finalScore}, isVictory={isVictory}");
        HideAllMenus();
        if (gameOverUI != null)
        {
            gameOverUI.gameObject.SetActive(true);
            
            // Biztosítjuk, hogy a GameOverUI megfelelően pozícionálva legyen
            Transform current = gameOverUI.transform;
            while (current != null)
            {
                if (!current.gameObject.activeSelf)
                {
                    current.gameObject.SetActive(true);
                }
                current = current.parent;
            }
            
            Canvas.ForceUpdateCanvases();
            Debug.Log("[UIManager] GameOverUI aktiválva");
            gameOverUI.OnShow(finalScore, isVictory);
        }
        else
        {
            Debug.LogError("[UIManager] gameOverUI == NULL! Nem lehet megjeleníteni a Game Over menüt!");
        }
    }


    public void ShowInGameHUD()
    {
        if (inGameHUD != null)
        {
            inGameHUD.gameObject.SetActive(true);
            
            Transform current = inGameHUD.transform;
            while (current != null)
            {
                if (!current.gameObject.activeSelf)
                {
                    current.gameObject.SetActive(true);
                }
                current = current.parent;
            }
            
            Canvas.ForceUpdateCanvases();
        }
    }

    public void HideInGameHUD()
    {
        if (inGameHUD != null)
        {
            inGameHUD.gameObject.SetActive(false);
        }
    }


    public void ShowCombatUI()
    {
        if (combatUI != null)
        {
            combatUI.ShowCombatUI();
        }
    }


    public void HideCombatUI()
    {
        if (combatUI != null)
        {
            combatUI.HideCombatUI();
        }
    }

    
    private void HideAllMenus()
    {
        if (mainMenuUI != null) mainMenuUI.gameObject.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.gameObject.SetActive(false);
        if (gameOverUI != null) gameOverUI.gameObject.SetActive(false);
        if (inGameHUD != null) inGameHUD.gameObject.SetActive(false);
        if (combatUI != null) combatUI.HideCombatUI();
    }


    public void OnGameStateChanged(GameManager2D.GameState newState, bool isVictory = false)
    {
        Debug.Log($"[UIManager] OnGameStateChanged: newState={newState}, isVictory={isVictory}");
        
        switch (newState)
        {
            case GameManager2D.GameState.MainMenu:
                Debug.Log("[UIManager] MainMenu állapot");
                ShowMainMenu();
                break;
            case GameManager2D.GameState.Playing:
                Debug.Log("[UIManager] Playing állapot");
                HideAllMenus();
                ShowInGameHUD();
                break;
            case GameManager2D.GameState.Paused:
                Debug.Log("[UIManager] Paused állapot");
                ShowPauseMenu();
                break;
            case GameManager2D.GameState.GameOver:
                Debug.Log($"[UIManager] GameOver állapot (isVictory={isVictory})");
                HideInGameHUD();
                if (GameManager2D.Instance != null && GameManager2D.Instance.scoreSystem != null)
                {
                    int finalScore = GameManager2D.Instance.scoreSystem.currentScore;
                    Debug.Log($"[UIManager] Végső pontszám: {finalScore}");
                    ShowGameOverMenu(finalScore, isVictory);
                }
                else
                {
                    Debug.LogError("[UIManager] GameManager2D.Instance vagy scoreSystem == NULL!");
                }
                break;
        }
    }
}

