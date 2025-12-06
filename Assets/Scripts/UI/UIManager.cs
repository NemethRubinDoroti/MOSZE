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


    public void ShowGameOverMenu(int finalScore)
    {
        HideAllMenus();
        if (gameOverUI != null)
        {
            gameOverUI.gameObject.SetActive(true);
            gameOverUI.OnShow(finalScore);
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


    public void OnGameStateChanged(GameManager2D.GameState newState)
    {
        switch (newState)
        {
            case GameManager2D.GameState.MainMenu:
                ShowMainMenu();
                break;
            case GameManager2D.GameState.Playing:
                HideAllMenus();
                ShowInGameHUD();
                break;
            case GameManager2D.GameState.Paused:
                ShowPauseMenu();
                break;
            case GameManager2D.GameState.GameOver:
                HideInGameHUD();
                if (GameManager2D.Instance != null && GameManager2D.Instance.scoreSystem != null)
                {
                    ShowGameOverMenu(GameManager2D.Instance.scoreSystem.currentScore);
                }
                break;
        }
    }
}

