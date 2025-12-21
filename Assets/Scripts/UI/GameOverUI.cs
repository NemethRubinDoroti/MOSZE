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
        Debug.Log($"[GameOverUI] OnShow hívva: finalScore={finalScore}, isVictory={isVictory}");
        currentFinalScore = finalScore;
        
        RectTransform rectTransform = GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            rectTransform.anchorMin = Vector2.zero;
            rectTransform.anchorMax = Vector2.one;
            rectTransform.anchoredPosition = Vector2.zero;
            rectTransform.sizeDelta = Vector2.zero;
            rectTransform.pivot = new Vector2(0.5f, 0.5f);
            
            // Biztosítjuk, hogy a Canvas-en legyen
            Canvas canvas = GetComponentInParent<Canvas>();
            if (canvas == null)
            {
                Debug.LogWarning("[GameOverUI] Nincs Canvas a hierarchiában! A GameOverPanel-nek a Canvas alatt kell lennie.");
            }
        }

        // Megjelenítjük a végső pontszámot
        if (finalScoreText != null)
        {
            finalScoreText.text = $"Final Score: {finalScore}";
            Debug.Log($"[GameOverUI] Final score text beállítva: {finalScoreText.text}");
        }
        else
        {
            Debug.LogWarning("[GameOverUI] finalScoreText == NULL!");
        }

        // Különböző szöveg győzelem/vereség esetén
        if (gameOverText != null)
        {
            if (isVictory)
            {
                gameOverText.text = "G Y Ő Z T É L";
                gameOverText.color = Color.green; // Zöld szín győzelem esetén
                Debug.Log("[GameOverUI] Győzelem szöveg beállítva: 'Győztél!' (zöld)");
            }
            else
            {
                gameOverText.text = "E L B U K T Á L";
                gameOverText.color = Color.red; // Piros szín vereség esetén
                Debug.Log("[GameOverUI] Vereség szöveg beállítva: 'Elbuktál!' (piros)");
            }
        }
        else
        {
            Debug.LogWarning("[GameOverUI] gameOverText == NULL!");
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

