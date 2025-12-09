using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using UnityEngine.TestTools.Logging;

public class Player2DTest
{
    private GameObject playerObject;
    private Player2D player;
    private Stats stats;

    [SetUp]
    public void SetUp()
    {
        // Player GameObject létrehozása
        playerObject = new GameObject("TestPlayer");
        player = playerObject.AddComponent<Player2D>();
        stats = playerObject.AddComponent<Stats>();
        player.stats = stats;
        
        // Alapértelmezett statok beállítása
        stats.maxHealth = 100;
        stats.currentHealth = 100;
        stats.defense = 10;
    }

    [TearDown]
    public void TearDown()
    {
        if (playerObject != null)
        {
            Object.DestroyImmediate(playerObject);
        }
    }

    [UnityTest]
    public IEnumerator PlayerTakeDamage_ReducesHealth()
    {
        int initialHealth = stats.currentHealth;
        int damage = 20;
        
        player.TakeDamage(damage);
        
        yield return null;
        
        Assert.Less(stats.currentHealth, initialHealth, "A játékos HP-ja csökkent kell legyen");
        Assert.AreEqual(initialHealth - Mathf.Max(1, damage - stats.defense), stats.currentHealth, 
            "A HP pontosan annyival csökkent, amennyi a tényleges sebzés");
    }

    [UnityTest]
    public IEnumerator PlayerTakeDamage_WithDefense_CalculatesCorrectly()
    {
        stats.currentHealth = 100;
        stats.defense = 10;
        int damage = 15;
        
        player.TakeDamage(damage);
        
        yield return null;
        
        int expectedHealth = 100 - Mathf.Max(1, damage - stats.defense);
        Assert.AreEqual(expectedHealth, stats.currentHealth, "A védelem figyelembe vétele helyes");
    }

    [UnityTest]
    public IEnumerator PlayerDies_WhenHealthReachesZero()
    {
        stats.currentHealth = 10;
        stats.defense = 0;
        
        // GameManager2D mock létrehozása
        // Az Awake() automatikusan beállítja az Instance-t
        GameObject gameManagerObject = new GameObject("TestGameManager");
        GameManager2D gameManager = gameManagerObject.AddComponent<GameManager2D>();
        
        // Várunk egy frame-et, hogy az Awake() meghívódjon
        yield return null;
        
        // Elvárjuk, hogy az UIManager hiánya miatt error log jöjjön létre
        LogAssert.Expect(LogType.Error, "[GameManager2D] UIManager.Instance == NULL! Nem lehet megjeleníteni a Game Over UI-t!");
        
        player.TakeDamage(10);
        
        yield return null;
        
        Assert.IsFalse(stats.IsAlive(), "A játékos halott kell legyen");
        Assert.AreEqual(GameManager2D.GameState.GameOver, GameManager2D.Instance.currentState, 
            "A játék GameOver állapotban kell legyen");
        
        Object.DestroyImmediate(gameManagerObject);
    }

    [UnityTest]
    public IEnumerator PlayerMove_UpdatesPosition()
    {
        Vector2Int initialPosition = player.position;
        Vector2Int direction = new Vector2Int(1, 0);
        
        player.Move(direction);
        
        yield return null;
        
        Assert.AreEqual(initialPosition + direction, player.position, "A pozíció frissült");
    }
}
