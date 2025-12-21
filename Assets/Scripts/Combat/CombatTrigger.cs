using System.Collections.Generic;
using UnityEngine;

public class CombatTrigger : MonoBehaviour
{
    [Header("Combat Settings")]
    public float triggerDistance = 2f; // Távolság a harc indításához
    public bool triggerOnce = true; // Csak egyszer indítható
    
    private bool hasTriggered = false;
    private Enemy2D enemy;
    private Player2D player;
    
    private void Start()
    {
        enemy = GetComponent<Enemy2D>();
        
        if (GameManager2D.Instance != null)
        {
            player = GameManager2D.Instance.player;
        }
    }
    
    private void Update()
    {
        if (hasTriggered && triggerOnce) return;
        if (enemy == null || player == null) return;
        if (GameManager2D.Instance == null || GameManager2D.Instance.combatManager == null) return;
        
        // Ellenőrizzük, hogy a harc már aktív-e
        if (GameManager2D.Instance.combatManager.currentState != CombatManager.CombatState.None)
        {
            return;
        }
        
    
        float distance = GridUtils.CalculateDistanceFloat(enemy.position, player.position);
        
        if (distance <= triggerDistance)
        {
            TriggerCombat();
        }
    }
    
    private void TriggerCombat()
    {
        if (hasTriggered && triggerOnce) return;
        if (enemy == null || player == null) return;
        if (GameManager2D.Instance == null || GameManager2D.Instance.combatManager == null) return;
        
        hasTriggered = true;
        
        // Keressük meg az összes közelben lévő ellenséget
        List<Enemy2D> nearbyEnemies = FindNearbyEnemies(enemy.position, triggerDistance * 2f);
        
        // Harc résztvevőivé tesszük őket
        List<Combatant> enemyCombatants = new List<Combatant>();
        List<Enemy2D> enemy2DList = new List<Enemy2D>();
        
        foreach (Enemy2D nearbyEnemy in nearbyEnemies)
        {
            if (nearbyEnemy != null && nearbyEnemy.stats != null && nearbyEnemy.stats.IsAlive())
            {
                enemyCombatants.Add(nearbyEnemy.GetCombatant());
                enemy2DList.Add(nearbyEnemy);
            }
        }
        
        if (enemyCombatants.Count == 0)
        {
            hasTriggered = false; // Ha nincs ellenség, akkor a trigger-t visszaállítjuk
            return;
        }
        
        Combatant playerCombatant = player.GetCombatant();
        
        // Harc indítása
        GameManager2D.Instance.combatManager.StartCombat(playerCombatant, enemyCombatants, enemy2DList);
        
        Canvas.ForceUpdateCanvases();
        
    }
    
    // Közelben lévő ellenségek keresése
    private List<Enemy2D> FindNearbyEnemies(Vector2Int center, float radius)
    {
        List<Enemy2D> nearbyEnemies = new List<Enemy2D>();
        
        if (GameManager2D.Instance == null || GameManager2D.Instance.mapGenerator == null)
        {
            return nearbyEnemies;
        }
        
        EnemySpawner spawner = GameManager2D.Instance.mapGenerator.enemySpawner;
        if (spawner == null)
        {
            return nearbyEnemies;
        }
        
        List<Enemy2D> allEnemies = spawner.GetSpawnedEnemies();
        
        foreach (Enemy2D enemy in allEnemies)
        {
            if (enemy == null || enemy.stats == null || !enemy.stats.IsAlive())
            {
                continue;
            }
            
            float distance = GridUtils.CalculateDistanceFloat(center, enemy.position);
            
            if (distance <= radius)
            {
                nearbyEnemies.Add(enemy);
            }
        }
        
        return nearbyEnemies;
    }
    
    public void ResetTrigger()
    {
        hasTriggered = false;
    }
}

