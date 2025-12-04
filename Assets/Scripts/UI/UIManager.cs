using UnityEngine;


public class UIManager : MonoBehaviour
{
    public static UIManager Instance { get; private set; }

    [Header("Menu References")]
    public MainMenuUI mainMenuUI;
    public PauseMenuUI pauseMenuUI;
    public InGameHUD inGameHUD;

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

    private void HideAllMenus()
    {
        if (mainMenuUI != null) mainMenuUI.gameObject.SetActive(false);
        if (pauseMenuUI != null) pauseMenuUI.gameObject.SetActive(false);
        if (inGameHUD != null) inGameHUD.gameObject.SetActive(false);
    }
}

