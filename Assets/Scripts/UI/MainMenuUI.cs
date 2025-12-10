using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class MainMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button newGameButton;
    public Button loadGameButton;
    public Button settingsButton;
    public Button highscoreButton;
    public Button quitButton;

    [Header("Sub Panels")]
    public GameObject settingsPanel;
    public GameObject highscorePanel;

    [Header("Highscore UI")]
    public Transform highscoreContent;
    public GameObject highscoreEntryPrefab;
    public Button highscoreBackButton;

    [Header("Settings UI")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Button settingsBackButton;

    private void Start()
    {
        SetupButtons();
        HideSubPanels();
    }

    private void SetupButtons()
    {
        if (newGameButton != null)
            newGameButton.onClick.AddListener(OnNewGameClicked);

        if (loadGameButton != null)
            loadGameButton.onClick.AddListener(OnLoadGameClicked);

        if (settingsButton != null)
            settingsButton.onClick.AddListener(OnSettingsClicked);

        if (highscoreButton != null)
            highscoreButton.onClick.AddListener(OnHighscoreClicked);

        if (quitButton != null)
            quitButton.onClick.AddListener(OnQuitClicked);

        if (highscoreBackButton != null)
            highscoreBackButton.onClick.AddListener(HideSubPanels);

        if (settingsBackButton != null)
            settingsBackButton.onClick.AddListener(HideSubPanels);
    }

    public void OnShow()
    {
        // Főmenü megjelenítésekor elrejtjük az almenüket
        HideSubPanels();
    }

    private void OnNewGameClicked()
    {
        Debug.Log("[MainMenuUI] Új játék kattintva");
        if (GameManager2D.Instance != null)
        {
            GameManager2D.Instance.StartGame();
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OnGameStateChanged(GameManager2D.GameState.Playing);
            }
        }
    }

    private void OnLoadGameClicked()
    {
        Debug.Log("[MainMenuUI] Betöltés kattintva");
        if (GameManager2D.Instance != null)
        {
            GameManager2D.Instance.LoadGame();
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OnGameStateChanged(GameManager2D.GameState.Playing);
            }
        }
    }

    private void OnSettingsClicked()
    {
        Debug.Log("[MainMenuUI] Beállítások kattintva");
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            LoadSettings();
        }

        // Elrejtjük a főmenü gombokat
        HideMainMenuButtons();
    }

    private void OnHighscoreClicked()
    {
        Debug.Log("[MainMenuUI] Highscore kattintva");
        if (highscorePanel != null)
        {
            highscorePanel.SetActive(true);
            DisplayHighscores();
        }

        // Elrejtjük a főmenü gombokat
        HideMainMenuButtons();
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

    private void HideSubPanels()
    {
        if (settingsPanel != null) settingsPanel.SetActive(false);
        if (highscorePanel != null) highscorePanel.SetActive(false);
        if (newGamePanel != null) newGamePanel.SetActive(false);

        // Visszaállítjuk a főmenü gombokat
        ShowMainMenuButtons();
    }

    private void HideMainMenuButtons()
    {
        if (newGameButton != null && newGameButton.gameObject != null)
            newGameButton.gameObject.SetActive(false);
        if (loadGameButton != null && loadGameButton.gameObject != null)
            loadGameButton.gameObject.SetActive(false);
        if (settingsButton != null && settingsButton.gameObject != null)
            settingsButton.gameObject.SetActive(false);
        if (highscoreButton != null && highscoreButton.gameObject != null)
            highscoreButton.gameObject.SetActive(false);
        if (quitButton != null && quitButton.gameObject != null)
            quitButton.gameObject.SetActive(false);
    }

    private void ShowMainMenuButtons()
    {
        if (newGameButton != null && newGameButton.gameObject != null)
            newGameButton.gameObject.SetActive(true);
        if (loadGameButton != null && loadGameButton.gameObject != null)
            loadGameButton.gameObject.SetActive(true);
        if (settingsButton != null && settingsButton.gameObject != null)
            settingsButton.gameObject.SetActive(true);
        if (highscoreButton != null && highscoreButton.gameObject != null)
            highscoreButton.gameObject.SetActive(true);
        if (quitButton != null && quitButton.gameObject != null)
            quitButton.gameObject.SetActive(true);
    }

    private void DisplayHighscores()
    {
        if (highscoreContent == null) return;

        // Töröljük a meglévő bejegyzéseket
        foreach (Transform child in highscoreContent)
        {
            Destroy(child.gameObject);
        }

        // Betöltjük a highscore-okat
        if (GameManager2D.Instance != null && GameManager2D.Instance.scoreSystem != null)
        {
            var highscores = GameManager2D.Instance.scoreSystem.GetHighScores();

            if (highscores.Count == 0)
            {
                // Ha nincs highscore, jelenítsünk meg egy üzenetet
                if (highscoreEntryPrefab != null)
                {
                    GameObject entry = Instantiate(highscoreEntryPrefab, highscoreContent);
                    var text = entry.GetComponentInChildren<Text>();
                    if (text != null)
                    {
                        text.text = "No high scores yet!";
                    }
                }
            }
            else
            {
                // Megjelenítjük a highscore-okat
                for (int i = 0; i < highscores.Count; i++)
                {
                    if (highscoreEntryPrefab != null)
                    {
                        GameObject entry = Instantiate(highscoreEntryPrefab, highscoreContent);
                        var text = entry.GetComponentInChildren<Text>();
                        if (text != null)
                        {
                            var score = highscores[i];
                            text.text = $"{i + 1}. {score.playerName} - {score.score} ({score.date})";
                        }
                    }
                }
            }
        }
    }

    private void LoadSettings()
    {
        // TODO: Betöltjük a beállításokat (pl. PlayerPrefs-ből)
        if (masterVolumeSlider != null)
        {
            masterVolumeSlider.value = PlayerPrefs.GetFloat("MasterVolume", 1f);
        }
        if (musicVolumeSlider != null)
        {
            musicVolumeSlider.value = PlayerPrefs.GetFloat("MusicVolume", 1f);
        }
        if (sfxVolumeSlider != null)
        {
            sfxVolumeSlider.value = PlayerPrefs.GetFloat("SFXVolume", 1f);
        }
    }

    public void SaveSettings()
    {
        // TODO: Mentjük a beállításokat
        if (masterVolumeSlider != null)
        {
            PlayerPrefs.SetFloat("MasterVolume", masterVolumeSlider.value);
        }
        if (musicVolumeSlider != null)
        {
            PlayerPrefs.SetFloat("MusicVolume", musicVolumeSlider.value);
        }
        if (sfxVolumeSlider != null)
        {
            PlayerPrefs.SetFloat("SFXVolume", sfxVolumeSlider.value);
        }
        PlayerPrefs.Save();
    }
}
