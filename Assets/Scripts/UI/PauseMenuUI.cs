using UnityEngine;
using UnityEngine.UI;


public class PauseMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button resumeButton;
    public Button mainMenuButton;

    private void Start()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (resumeButton != null)
            resumeButton.onClick.AddListener(OnResumeClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    public void OnShow()
    {
        // Pause menü megjelenítésekor aktiváljuk
        gameObject.SetActive(true);
    }

    private void OnResumeClicked()
    {
        Debug.Log("[PauseMenuUI] Folytatás gomb kattintva");
        if (GameManager2D.Instance != null)
        {
            GameManager2D.Instance.ResumeGame();
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HidePauseMenu();
            }
        }
    }

    private void OnMainMenuClicked()
    {
        Debug.Log("[PauseMenuUI] Főmenü gomb kattintva");
        if (GameManager2D.Instance != null)
        {
            GameManager2D.Instance.ResumeGame(); // Visszaállítjuk a time scale-t
            GameManager2D.Instance.currentState = GameManager2D.GameState.MainMenu;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowMainMenu();
                UIManager.Instance.HideInGameHUD();
            }
        }
    }

    private void Update()
    {
        // ESC gombbal is bezárhatjuk a pause menüt (csak ha a pause menü aktív)
        if (gameObject.activeSelf)
        {
            #if ENABLE_INPUT_SYSTEM
            if (UnityEngine.InputSystem.Keyboard.current != null && 
                UnityEngine.InputSystem.Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                OnResumeClicked();
            }
            #else
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                OnResumeClicked();
            }
            #endif
        }
    }
}

