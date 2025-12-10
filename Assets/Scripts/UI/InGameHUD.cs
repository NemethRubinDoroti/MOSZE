using UnityEngine;
using UnityEngine.UI;


public class InGameHUD : MonoBehaviour
{
    [Header("Health UI")]
    public Slider healthBar;
    public Text healthText;

    [Header("Score UI")]
    public Text scoreText;

    [Header("XP & Level UI")]
    public Text levelText;
    public Text xpText;
    public Slider xpBar;

    private void OnEnable()
    {
        if (scoreText != null)
        {
            if (!scoreText.gameObject.activeSelf)
            {
                scoreText.gameObject.SetActive(true);
            }
            
            Transform parent = scoreText.transform.parent;
            while (parent != null)
            {
                if (!parent.gameObject.activeSelf)
                {
                    parent.gameObject.SetActive(true);
                }
                parent = parent.parent;
            }
        }
        
        Canvas.ForceUpdateCanvases();
    }

    private void Update()
    {
        UpdateHealth();
        UpdateScore();
        UpdateXPAndLevel();
    }

    private void UpdateHealth()
    {
        if (GameManager2D.Instance != null && GameManager2D.Instance.player != null)
        {
            if (GameManager2D.Instance.player.stats != null)
            {
                var stats = GameManager2D.Instance.player.stats;

                if (healthBar != null)
                {
                    healthBar.maxValue = stats.maxHealth;
                    healthBar.value = stats.currentHealth;
                }

                if (healthText != null)
                {
                    healthText.text = $"HP: {stats.currentHealth}/{stats.maxHealth}";
                }
            }
        }
    }

    private void UpdateScore()
    {
        if (scoreText == null)
        {
            return;
        }

        if (!scoreText.gameObject.activeSelf)
        {
            scoreText.gameObject.SetActive(true);
        }

        if (GameManager2D.Instance != null && GameManager2D.Instance.scoreSystem != null)
        {
            scoreText.text = $"Score: {GameManager2D.Instance.scoreSystem.currentScore}";
        }
    }
    
    private void UpdateXPAndLevel()
    {
        if (GameManager2D.Instance != null && GameManager2D.Instance.experienceSystem != null)
        {
            ExperienceSystem xpSystem = GameManager2D.Instance.experienceSystem;
            
            // Level text
            if (levelText != null)
            {
                levelText.text = $"Level: {xpSystem.currentLevel}";
            }
            
            // XP text
            if (xpText != null)
            {
                xpText.text = $"XP: {xpSystem.currentXP}/{xpSystem.xpToNextLevel}";
            }
            
            // XP bar
            if (xpBar != null)
            {
                xpBar.maxValue = xpSystem.xpToNextLevel;
                xpBar.value = xpSystem.currentXP;
            }
        }
        else
        {
            // Ha nincs XP rendszer, alapértelmezett értékek
            if (levelText != null)
            {
                levelText.text = "Level: 1";
            }
            if (xpText != null)
            {
                xpText.text = "XP: 0/100";
            }
            if (xpBar != null)
            {
                xpBar.value = 0;
                xpBar.maxValue = 100;
            }
        }
    }
    
    public void OnLevelUp(int newLevel)
    {
        // Frissítjük az UI-t
        UpdateXPAndLevel();
        UpdateHealth(); // HP is változhat
        
        Debug.Log($"[InGameHUD] Szintlépés! Új szint: {newLevel}!");
    }
}

