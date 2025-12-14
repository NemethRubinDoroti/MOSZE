using UnityEngine;
using UnityEngine.UI;


public class PauseMenuUI : MonoBehaviour
{
    [Header("UI Elements")]
    public Button resumeButton;
    public Button saveButton;
    public Button loadButton;
    public Button exportMapButton;
    public Button settingsButton;
    public Button mainMenuButton;

    [Header("Sub Panels")]
    public GameObject settingsPanel;

    [Header("Settings UI")]
    public Slider masterVolumeSlider;
    public Slider musicVolumeSlider;
    public Slider sfxVolumeSlider;
    public Button settingsBackButton;

    private void Start()
    {
        SetupButtons();
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
    }

    private void SetupButtons()
    {
        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(OnResumeClicked);
            resumeButton.onClick.AddListener(() => PlayButtonSound());
        }

        if (saveButton != null)
        {
            saveButton.onClick.AddListener(OnSaveClicked);
            saveButton.onClick.AddListener(() => PlayButtonSound());
        }

        if (loadButton != null)
        {
            loadButton.onClick.AddListener(OnLoadClicked);
            loadButton.onClick.AddListener(() => PlayButtonSound());
        }

        if (exportMapButton != null)
        {
            exportMapButton.onClick.AddListener(OnExportMapClicked);
            exportMapButton.onClick.AddListener(() => PlayButtonSound());
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.AddListener(OnSettingsClicked);
            settingsButton.onClick.AddListener(() => PlayButtonSound());
        }

        if (mainMenuButton != null)
        {
            mainMenuButton.onClick.AddListener(OnMainMenuClicked);
            mainMenuButton.onClick.AddListener(() => PlayButtonSound());
        }

        if (settingsBackButton != null)
        {
            settingsBackButton.onClick.AddListener(OnSettingsBackClicked);
            settingsBackButton.onClick.AddListener(() => PlayButtonSound());
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
        // Pause menü megjelenítésekor elrejtjük az almenüket
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(false);
        }
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

    private void OnSaveClicked()
    {
        Debug.Log("[PauseMenuUI] Save clicked");
        if (GameManager2D.Instance != null)
        {
            GameManager2D.Instance.SaveGame();
            // TODO: Megjeleníthetünk egy "Game Saved!" üzenetet
        }
    }

    private void OnLoadClicked()
    {
        Debug.Log("[PauseMenuUI] Load clicked");
        if (GameManager2D.Instance != null)
        {
            GameManager2D.Instance.LoadGame();
            if (UIManager.Instance != null)
            {
                UIManager.Instance.HidePauseMenu();
                UIManager.Instance.OnGameStateChanged(GameManager2D.GameState.Playing);
            }
        }
    }

    private void OnExportMapClicked()
    {
        Debug.Log("[PauseMenuUI] Export Map clicked");
        
        if (GameManager2D.Instance == null)
        {
            Debug.LogError("[PauseMenuUI] GameManager2D.Instance == NULL!");
            return;
        }
        
        if (GameManager2D.Instance.mapGenerator == null)
        {
            Debug.LogError("[PauseMenuUI] GameManager2D.Instance.mapGenerator == NULL!");
            return;
        }
        
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("[PauseMenuUI] SaveSystem.Instance == NULL!");
            return;
        }
        
        // Fájlnév generálása seed alapján
        string fileName = $"map_seed_{GameManager2D.Instance.currentSeed}";
        Debug.Log($"[PauseMenuUI] Exportálás indítása: {fileName}");
        GameManager2D.Instance.mapGenerator.ExportMapToJSON(fileName);
    }

    private void OnSettingsClicked()
    {
        Debug.Log("[PauseMenuUI] Settings clicked");
        if (settingsPanel != null)
        {
            settingsPanel.SetActive(true);
            LoadSettings();
        }
    }

    private void OnSettingsBackClicked()
    {
        if (settingsPanel != null)
        {
            SaveSettings();
            settingsPanel.SetActive(false);
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
                UIManager.Instance.OnGameStateChanged(GameManager2D.GameState.MainMenu);
            }
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
                masterVolumeSlider.onValueChanged.RemoveAllListeners();
                masterVolumeSlider.onValueChanged.AddListener(OnMasterVolumeChanged);
            }
            if (musicVolumeSlider != null)
            {
                musicVolumeSlider.value = AudioManager.Instance.musicVolume;
                musicVolumeSlider.onValueChanged.RemoveAllListeners();
                musicVolumeSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
            }
            if (sfxVolumeSlider != null)
            {
                sfxVolumeSlider.value = AudioManager.Instance.sfxVolume;
                sfxVolumeSlider.onValueChanged.RemoveAllListeners();
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

    private void SaveSettings()
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
