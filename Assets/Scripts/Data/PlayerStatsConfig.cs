using UnityEngine;

[CreateAssetMenu(fileName = "Player Stats Config", menuName = "Game/Player Stats Config")]
public class PlayerStatsConfig : ScriptableObject
{
    [Header("Starting Stats")]
    public int startingMaxHealth = 100;
    public int startingAttack = 15;
    public int startingDefense = 8;
    public int startingSpeed = 12;
    [Range(0, 100)]
    public int startingAccuracy = 85;
    
    [Header("Level Progression (Future Use)")]
    public int healthPerLevel = 10;
    public int attackPerLevel = 2;
    public int defensePerLevel = 1;
    public int speedPerLevel = 1;
    public int accuracyPerLevel = 1;
    

    public void ApplyToStats(Stats stats)
    {
        if (stats == null) return;
        
        stats.maxHealth = startingMaxHealth;
        stats.currentHealth = startingMaxHealth;
        stats.attack = startingAttack;
        stats.defense = startingDefense;
        stats.speed = startingSpeed;
        stats.accuracy = startingAccuracy;
    }
    

    public Stats CreateStats()
    {
        Stats stats = new Stats();
        ApplyToStats(stats);
        return stats;
    }
}

