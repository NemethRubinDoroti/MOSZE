using UnityEngine;
using UnityEngine.UI;


public class InGameHUD : MonoBehaviour
{
    [Header("Health UI")]
    public Slider healthBar;
    public Text healthText;

    [Header("Score UI")]
    public Text scoreText;

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
}

