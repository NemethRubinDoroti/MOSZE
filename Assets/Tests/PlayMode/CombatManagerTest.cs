using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Logging;

public class CombatManagerTest
{
    private GameObject combatManagerObject;
    private CombatManager combatManager;
    private GameObject playerObject;
    private Player2D player;
    private Stats playerStats;

    [SetUp]
    public void SetUp()
    {
        // CombatManager létrehozása
        combatManagerObject = new GameObject("TestCombatManager");
        combatManager = combatManagerObject.AddComponent<CombatManager>();
        
        // Player létrehozása
        playerObject = new GameObject("TestPlayer");
        player = playerObject.AddComponent<Player2D>();
        playerStats = playerObject.AddComponent<Stats>();
        player.stats = playerStats;
        playerStats.maxHealth = 100;
        playerStats.currentHealth = 100;
        playerStats.attack = 15;
        playerStats.defense = 10;
    }

    [TearDown]
    public void TearDown()
    {
        if (combatManagerObject != null) Object.DestroyImmediate(combatManagerObject);
        if (playerObject != null) Object.DestroyImmediate(playerObject);
    }

    [UnityTest]
    public IEnumerator CombatManager_StartCombat_InitializesCombatState()
    {
        Combatant playerCombatant = player.GetCombatant();
        List<Combatant> enemies = new List<Combatant>();
        
        // Mock ellenség létrehozása
        GameObject enemyObject = new GameObject("TestEnemy");
        Enemy2D enemy = enemyObject.AddComponent<Enemy2D>();
        Stats enemyStats = enemyObject.AddComponent<Stats>();
        enemy.stats = enemyStats;
        enemyStats.maxHealth = 50;
        enemyStats.currentHealth = 50;
        
        Combatant enemyCombatant = new Combatant(Vector2Int.zero, enemyStats, false, "enemy");
        enemies.Add(enemyCombatant);
        
        // Elvárjuk, hogy a CombatUI hiánya miatt error log jöjjön létre
        LogAssert.Expect(LogType.Error, "[CombatManager] CombatUI is NULL! Cannot show combat UI. Please assign it in the Inspector or ensure UIManager has a CombatUI reference.");
        
        combatManager.StartCombat(playerCombatant, enemies);
        
        yield return null;
        
        Assert.AreEqual(CombatManager.CombatState.PlayerTurn, combatManager.currentState, "A harc PlayerTurn állapotban van");
        Assert.IsNotNull(combatManager.combatants, "A combatants lista inicializálva");
        Assert.Greater(combatManager.combatants.Count, 0, "Van legalább egy combatant");
        
        Object.DestroyImmediate(enemyObject);
    }

    [UnityTest]
    public IEnumerator CombatManager_CheckWinCondition_ReturnsTrue_WhenAllEnemiesDead()
    {
        Combatant playerCombatant = player.GetCombatant();
        List<Combatant> enemies = new List<Combatant>();
        
        // Halott ellenség létrehozása
        GameObject enemyObject = new GameObject("TestEnemy");
        Enemy2D enemy = enemyObject.AddComponent<Enemy2D>();
        Stats enemyStats = enemyObject.AddComponent<Stats>();
        enemy.stats = enemyStats;
        enemyStats.maxHealth = 50;
        enemyStats.currentHealth = 0; // Halott
        
        Combatant enemyCombatant = new Combatant(Vector2Int.zero, enemyStats, false, "enemy");
        enemyCombatant.isAlive = false;
        enemies.Add(enemyCombatant);
        
        // Elvárjuk, hogy a CombatUI hiánya miatt error log jöjjön létre
        LogAssert.Expect(LogType.Error, "[CombatManager] CombatUI is NULL! Cannot show combat UI. Please assign it in the Inspector or ensure UIManager has a CombatUI reference.");
        
        combatManager.StartCombat(playerCombatant, enemies);
        
        yield return null;
        
        bool winCondition = combatManager.CheckWinCondition();
        Assert.IsTrue(winCondition, "A győzelem feltétele teljesül, ha minden ellenség halott");
        
        Object.DestroyImmediate(enemyObject);
    }

    [UnityTest]
    public IEnumerator CombatManager_CheckWinCondition_ReturnsFalse_WhenEnemiesAlive()
    {
        Combatant playerCombatant = player.GetCombatant();
        List<Combatant> enemies = new List<Combatant>();
        
        // Élő ellenség létrehozása
        GameObject enemyObject = new GameObject("TestEnemy");
        Enemy2D enemy = enemyObject.AddComponent<Enemy2D>();
        Stats enemyStats = enemyObject.AddComponent<Stats>();
        enemy.stats = enemyStats;
        enemyStats.maxHealth = 50;
        enemyStats.currentHealth = 50; // Élő
        
        Combatant enemyCombatant = new Combatant(Vector2Int.zero, enemyStats, false, "enemy");
        enemies.Add(enemyCombatant);
        
        // Elvárjuk, hogy a CombatUI hiánya miatt error log jöjjön létre
        LogAssert.Expect(LogType.Error, "[CombatManager] CombatUI is NULL! Cannot show combat UI. Please assign it in the Inspector or ensure UIManager has a CombatUI reference.");
        
        combatManager.StartCombat(playerCombatant, enemies);
        
        yield return null;
        
        bool winCondition = combatManager.CheckWinCondition();
        Assert.IsFalse(winCondition, "A győzelem feltétele nem teljesül, ha van élő ellenség");
        
        Object.DestroyImmediate(enemyObject);
    }
}

