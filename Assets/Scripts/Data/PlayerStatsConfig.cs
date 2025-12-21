using UnityEngine;

[CreateAssetMenu(fileName = "Player Stats Config", menuName = "Game/Player Stats Config")]
public class PlayerStatsConfig : ScriptableObject
{
    [Header("Starting Stats")]
    public int startingMaxHealth = 200;
    public int startingAttack = 20;
    public int startingDefense = 10;
    public int startingSpeed = 12;
    [Range(0, 100)]
    public int startingAccuracy = 85;
    
    [Header("Level Progression (Future Use)")]
    public int healthPerLevel = 20;
    public int attackPerLevel = 3;
    public int defensePerLevel = 2;
    public int speedPerLevel = 2;
    public int accuracyPerLevel = 2;
    

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

