using UnityEngine;
using UnityEngine.UI;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button newGameButton;
    public Button quitButton;

    private void Start()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGameClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);
    }

    public void OnShow()
    {
        // Főmenü megjelenítésekor aktiváljuk
        gameObject.SetActive(true);
    }

    private void OnNewGameClicked()
    {
        Debug.Log("[MainMenuUI] Új játék kattintva");
        if (GameManager2D.Instance != null)
        {
            GameManager2D.Instance.StartGame();
            if (UIManager.Instance != null)
            {
                UIManager.Instance.ShowInGameHUD();
                gameObject.SetActive(false);
            }
        }
    }

    private void OnQuitClicked()
    {
        Debug.Log("[MainMenuUI] Kilépés kattintva");
        #if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
        #else
            Application.Quit();
        #endif
    }
}

