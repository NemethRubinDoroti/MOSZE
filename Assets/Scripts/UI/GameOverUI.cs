using UnityEngine;
using UnityEngine.UI;

public class GameOverUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Text gameOverText;
    public Text finalScoreText;
    public InputField playerNameInput;
    public Button saveHighscoreButton;
    public Button newGameButton;
    public Button mainMenuButton;

    private int currentFinalScore = 0;

    private void Start()
    {
        SetupButtons();
    }

    private void SetupButtons()
    {
        if (saveHighscoreButton != null)
            saveHighscoreButton.onClick.AddListener(OnSaveHighscoreClicked);

        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGameClicked);

        if (mainMenuButton != null)
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
    }

    public void OnShow(int finalScore, bool isVictory = false)
    {
        currentFinalScore = finalScore;

        // Megjelenítjük a végső pontszámot
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Végső pontszám: {finalScore}";
        }

        // Különböző szöveg győzelem/vereség esetén
        if (gameOverText != null)
        {
            if (isVictory)
            {
                gameOverText.text = "Győztél!";
                gameOverText.color = Color.green;
            }
            else
            {
                gameOverText.text = "Elbuktál!";
                gameOverText.color = Color.red;
            }
        }

        // Alapértelmezett játékosnév
        if (playerNameInput != null)
        {
            playerNameInput.text = "Player";
        }

        // Ha nincs pontszám, elrejtjük a save gombot
        if (saveHighscoreButton != null)
        {
            saveHighscoreButton.interactable = finalScore > 0;
        }
    }

    private void OnSaveHighscoreClicked()
    {
        string playerName = "Player";
        if (playerNameInput != null && !string.IsNullOrEmpty(playerNameInput.text))
        {
            playerName = playerNameInput.text;
        }

        Debug.Log($"[GameOverUI] Highscore mentése: {playerName} - {currentFinalScore}");

        if (GameManager2D.Instance != null && GameManager2D.Instance.scoreSystem != null)
        {
            GameManager2D.Instance.scoreSystem.SaveHighScore(playerName);
        }

        // Elrejtjük a save gombot, mert már mentettük
        if (saveHighscoreButton != null)
        {
            saveHighscoreButton.interactable = false;
            saveHighscoreButton.GetComponentInChildren<Text>().text = "Mentve!";
        }
    }

    private void OnNewGameClicked()
    {
        Debug.Log("[GameOverUI] Új játék gomb kattintva");
        if (GameManager2D.Instance != null)
        {
            GameManager2D.Instance.ResumeGame(); // Visszaállítjuk a time scale-t
            GameManager2D.Instance.StartGame();
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OnGameStateChanged(GameManager2D.GameState.Playing);
            }
        }
    }

    private void OnMainMenuClicked()
    {
        Debug.Log("[GameOverUI] Főmenü gomb kattintva");
        if (GameManager2D.Instance != null)
        {
            GameManager2D.Instance.ResumeGame(); // Visszaállítjuk a time scale-t
            GameManager2D.Instance.currentState = GameManager2D.GameState.MainMenu;
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OnGameStateChanged(GameManager2D.GameState.MainMenu);
            }
        }
    }
}

