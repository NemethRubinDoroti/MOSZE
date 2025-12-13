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
    public GameObject newGamePanel;

    [Header("Highscore UI")]
    public Transform highscoreContent;
    public Button highscoreBackButton;

    [Header("Settings UI")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Button settingsBackButton;

    [Header("New Game UI")]
    public Button randomGameButton;
    public Button seedGameButton;
    public Button jsonGameButton;
    public InputField seedInputField;
    public Dropdown mapDropdown;
    public Button newGameBackButton;
    public Text seedLabel;
    public Text mapLabel;

    private void Start()
    {
        SetupButtons();
        HideSubPanels();
    }

    private void SetupButtons()
    {
        if (newGameButton != null)
        {
            newGameButton.onClick.AddListener(OnNewGameClicked);
            newGameButton.onClick.AddListener(() => PlayButtonSound());
        }

        if (loadGameButton != null)
        {
            loadGameButton.onClick.AddListener(OnLoadGameClicked);
            loadGameButton.onClick.AddListener(() => PlayButtonSound());
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsClicked);
            settingsButton.onClick.AddListener(() => PlayButtonSound());
        }

        if (highscoreButton != null)
        {
            highscoreButton.onClick.AddListener(OnHighscoreClicked);
            highscoreButton.onClick.AddListener(() => PlayButtonSound());
        }

        if (quitButton != null)
        {
            quitButton.onClick.AddListener(OnQuitClicked);
            quitButton.onClick.AddListener(() => PlayButtonSound());
        }

        if (highscoreBackButton != null)
        {
            highscoreBackButton.onClick.AddListener(HideSubPanels);
            highscoreBackButton.onClick.AddListener(() => PlayButtonSound());
        }

        if (settingsBackButton != null)
        {
            settingsBackButton.onClick.AddListener(HideSubPanels);
            settingsBackButton.onClick.AddListener(() => PlayButtonSound());
        }

        if (randomGameButton != null)
        {
            randomGameButton.onClick.AddListener(OnRandomGameClicked);
            randomGameButton.onClick.AddListener(() => PlayButtonSound());
        }

        if (seedGameButton != null)
        {
            seedGameButton.onClick.AddListener(OnSeedGameClicked);
            seedGameButton.onClick.AddListener(() => PlayButtonSound());
        }

        if (jsonGameButton != null)
        {
            jsonGameButton.onClick.AddListener(OnJsonGameClicked);
            jsonGameButton.onClick.AddListener(() => PlayButtonSound());
        }

        if (newGameBackButton != null)
        {
            newGameBackButton.onClick.AddListener(HideSubPanels);
            newGameBackButton.onClick.AddListener(() => PlayButtonSound());
        }
    }

    private void PlayButtonSound()
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayButtonClick();
        }
    }

    public void OnShow()
    {
        HideSubPanels();
    }

    private void OnNewGameClicked()
    {
        Debug.Log("[MainMenuUI] Új játék kattintva");
        if (newGamePanel != null)
        {
            newGamePanel.SetActive(true);
            LoadAvailableMaps();
        }

        HideMainMenuButtons();
    }

    private void OnRandomGameClicked()
    {
        Debug.Log("[MainMenuUI] Véletlenszerű játék kattintva");
        if (GameManager2D.Instance != null)
        {
            GameManager2D.Instance.StartGame(); // Random seed
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OnGameStateChanged(GameManager2D.GameState.Playing);
            }
        }
    }

    private void OnSeedGameClicked()
    {
        Debug.Log("[MainMenuUI] Seedelt játék kattintva");
        if (seedInputField == null || string.IsNullOrEmpty(seedInputField.text))
        {
            Debug.LogWarning("[MainMenuUI] Nincs megadva seed!");
            return;
        }

        if (int.TryParse(seedInputField.text, out int seed))
        {
            if (GameManager2D.Instance != null)
            {
                GameManager2D.Instance.StartGameWithSeed(seed);
                if (UIManager.Instance != null)
                {
                    UIManager.Instance.OnGameStateChanged(GameManager2D.GameState.Playing);
                }
            }
        }
        else
        {
            Debug.LogWarning("[MainMenuUI] Érvénytelen seed érték!");
        }
    }

    private void OnJsonGameClicked()
    {
        Debug.Log("[MainMenuUI] JSON játék kattintva");
        if (mapDropdown == null)
        {
            Debug.LogWarning("[MainMenuUI] Map dropdown nincs beállítva!");
            return;
        }

        string selectedMap = mapDropdown.options[mapDropdown.value].text;
        if (string.IsNullOrEmpty(selectedMap))
        {
            Debug.LogWarning("[MainMenuUI] Nincs kiválasztva pálya!");
            return;
        }

        if (GameManager2D.Instance != null)
        {
            GameManager2D.Instance.StartGameWithMap(selectedMap);
            if (UIManager.Instance != null)
            {
                UIManager.Instance.OnGameStateChanged(GameManager2D.GameState.Playing);
            }
        }
    }

    private void LoadAvailableMaps()
    {
        if (mapDropdown == null) return;

        mapDropdown.ClearOptions();

        if (SaveSystem.Instance != null)
        {
            string[] maps = SaveSystem.Instance.GetMapList();
            if (maps.Length == 0)
            {
                mapDropdown.AddOptions(new System.Collections.Generic.List<string> { "Nincs elérhető pálya" });
                if (jsonGameButton != null)
                {
                    jsonGameButton.interactable = false;
                }
            }
            else
            {
                mapDropdown.AddOptions(new System.Collections.Generic.List<string>(maps));
                if (jsonGameButton != null)
                {
                    jsonGameButton.interactable = true;
                }
            }
        }
        else
        {
            mapDropdown.AddOptions(new System.Collections.Generic.List<string> { "SaveSystem nem elérhető" });
            if (jsonGameButton != null)
            {
                jsonGameButton.interactable = false;
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
        Debug.Log("[MainMenuUI] DisplayHighscores() meghívva");

        if (highscoreContent == null)
        {
            Debug.LogWarning("[MainMenuUI] highscoreContent == NULL! Nem lehet megjeleníteni a highscore-okat.");
            return;
        }

        UnityEngine.UI.VerticalLayoutGroup layoutGroup = highscoreContent.GetComponent<UnityEngine.UI.VerticalLayoutGroup>();
        if (layoutGroup == null)
        {
            layoutGroup = highscoreContent.gameObject.AddComponent<UnityEngine.UI.VerticalLayoutGroup>();
            layoutGroup.spacing = 5f;
            layoutGroup.childAlignment = TextAnchor.UpperLeft;
            layoutGroup.childControlWidth = true;
            layoutGroup.childControlHeight = false;
            layoutGroup.childForceExpandWidth = true;
            layoutGroup.childForceExpandHeight = false;
        }

        // Töröljük a meglévő bejegyzéseket
        foreach (Transform child in highscoreContent)
        {
            Destroy(child.gameObject);
        }

        // Betöltjük a highscore-okat
        if (GameManager2D.Instance != null && GameManager2D.Instance.scoreSystem != null)
        {
            var highscores = GameManager2D.Instance.scoreSystem.GetHighScores();
            Debug.Log($"[MainMenuUI] Betöltött highscore-ok száma: {highscores.Count}");

            if (highscores.Count == 0)
            {
                // Ha nincs highscore, jelenítsünk meg egy üzenetet
                CreateHighscoreEntry("Nincs mentett highscore!");
            }
            else
            {
                // Megjelenítjük a highscore-okat
                for (int i = 0; i < highscores.Count; i++)
                {
                    var score = highscores[i];
                    string entryText = $"{i + 1}. {score.playerName} - {score.score} ({score.date})";
                    CreateHighscoreEntry(entryText);
                }
            }
        }
    }

    private void CreateHighscoreEntry(string text)
    {
        // Dinamikusan létrehozunk egy egyszerű Text objektumot
        GameObject entry = new GameObject("HighscoreEntry");
        entry.transform.SetParent(highscoreContent, false);

        Text textComponent = entry.AddComponent<Text>();
        textComponent.text = text;
        textComponent.font = Resources.GetBuiltinResource<Font>("LegacyRuntime.ttf");
        textComponent.fontSize = 20;
        textComponent.color = Color.white;
        textComponent.alignment = TextAnchor.MiddleLeft;

        RectTransform rectTransform = entry.GetComponent<RectTransform>();
        if (rectTransform != null)
        {
            // Beállítjuk az anchor-t és a pozíciót, hogy a VerticalLayoutGroup megfelelően működjön
            rectTransform.anchorMin = new Vector2(0, 1);
            rectTransform.anchorMax = new Vector2(1, 1);
            rectTransform.pivot = new Vector2(0.5f, 1);
            rectTransform.anchoredPosition = Vector2.zero;

            rectTransform.sizeDelta = new Vector2(0, 30);
        }
    }

    private void LoadSettings()
    {
        // Betöltjük a beállításokat AudioManager-ből
        if (AudioManager.Instance != null)
        {
            if (masterVolumeSlider != null)
            {
                masterVolumeSlider.value = AudioManager.Instance.masterVolume;
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            }
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = AudioManager.Instance.musicVolume;
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = AudioManager.Instance.sfxVolume;
                sfxVolumeSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
            }
        }
        else
        {
            // Fallback PlayerPrefs-re ha nincs AudioManager
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
    }

    private void OnMasterVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMasterVolume(value);
        }
    }

    private void OnMusicVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetMusicVolume(value);
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.SetSFXVolume(value);
        }
    }

    public void SaveSettings()
    {
        // Beállítások mentése AudioManager-en keresztül (automatikusan menti)
        if (AudioManager.Instance != null)
        {
            if (masterVolumeSlider != null)
            {
                AudioManager.Instance.SetMasterVolume(masterVolumeSlider.value);
            }
            if (musicVolumeSlider != null)
            {
                AudioManager.Instance.SetMusicVolume(musicVolumeSlider.value);
            }
            if (sfxVolumeSlider != null)
            {
                AudioManager.Instance.SetSFXVolume(sfxVolumeSlider.value);
            }
        }
        else
        {
            // Fallback PlayerPrefs-re
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
}

