using UnityEngine;
using System;

public class ExperienceSystem : MonoBehaviour
{
    public static ExperienceSystem Instance { get; private set; }
    
    [Header("Level Settings")]
    public int baseXPRequired = 100; // Ennyi XP szükséges az 1. szinthez
    public float xpMultiplier = 1.5f; // Szorzó minden szinthez
    
    public int currentLevel = 1;
    public int currentXP = 0;
    public int xpToNextLevel = 100;
    
    public event Action<int> OnLevelUp; // int = új szint
    public event Action<int> OnXPGained; // int = kapott XP
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
        
        CalculateXPToNextLevel();
    }
    
    public void AddXP(int amount)
    {
        if (amount <= 0) return;
        
        currentXP += amount;
        OnXPGained?.Invoke(amount);
        
        // Ellenőrizzük, hogy szintet lépett-e
        while (currentXP >= xpToNextLevel)
        {
            LevelUp();
        }
    }
    
    private void LevelUp()
    {
        currentXP -= xpToNextLevel;
        currentLevel++;
        
        CalculateXPToNextLevel();
        
        OnLevelUp?.Invoke(currentLevel);
        
        // Stat növelés a PlayerStatsConfig alapján
        ApplyLevelUpStats();
    }
    
    public void CalculateXPToNextLevel()
    {
        // Exponenciális XP görbe: baseXP * (multiplier ^ (level - 1))
        xpToNextLevel = Mathf.RoundToInt(baseXPRequired * Mathf.Pow(xpMultiplier, currentLevel - 1));
    }
    
    private void ApplyLevelUpStats()
    {
        if (GameManager2D.Instance == null || GameManager2D.Instance.player == null)
        {
            return;
        }
        
        Player2D player = GameManager2D.Instance.player;
        if (player.stats == null || player.statsConfig == null)
        {
            return;
        }
        
        PlayerStatsConfig config = player.statsConfig;
        Stats stats = player.stats;
        
        // Stat növelés
        stats.maxHealth += config.healthPerLevel;
        stats.currentHealth += config.healthPerLevel; // Full HP szintlépéskor
        stats.attack += config.attackPerLevel;
        stats.defense += config.defensePerLevel;
        stats.speed += config.speedPerLevel;
        stats.accuracy += config.accuracyPerLevel;
        
        // UI frissítése
        if (UIManager.Instance != null && UIManager.Instance.inGameHUD != null)
        {
            UIManager.Instance.inGameHUD.OnLevelUp(currentLevel);
        }
    }
    
    public void Reset()
    {
        currentLevel = 1;
        currentXP = 0;
        CalculateXPToNextLevel();
    }
    
    public int GetXPRequiredForLevel(int level)
    {
        if (level <= 1) return 0;
        return Mathf.RoundToInt(baseXPRequired * Mathf.Pow(xpMultiplier, level - 2));
    }
    
    public float GetXPProgress()
    {
        if (xpToNextLevel == 0) return 0f;
        return (float)currentXP / xpToNextLevel;
    }
}

