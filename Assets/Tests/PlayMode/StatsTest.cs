using System.Collections;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;

public class StatsTest
{
    private GameObject statsObject;
    private Stats stats;

    [SetUp]
    public void SetUp()
    {
        statsObject = new GameObject("TestStats");
        stats = statsObject.AddComponent<Stats>();
        stats.maxHealth = 100;
        stats.currentHealth = 100;
        stats.defense = 10;
    }

    [TearDown]
    public void TearDown()
    {
        if (statsObject != null)
        {
            Object.DestroyImmediate(statsObject);
        }
    }

    [UnityTest]
    public IEnumerator Stats_TakeDamage_ReducesHealth()
    {
        int initialHealth = stats.currentHealth;
        int damage = 20;
        
        stats.TakeDamage(damage);
        
        yield return null;
        
        Assert.Less(stats.currentHealth, initialHealth, "A HP csökkent");
        int expectedHealth = initialHealth - Mathf.Max(1, damage - stats.defense);
        Assert.AreEqual(expectedHealth, stats.currentHealth, "A HP helyesen csökkent");
    }

    [UnityTest]
    public IEnumerator Stats_TakeDamage_WithDefense_CalculatesCorrectly()
    {
        stats.currentHealth = 100;
        stats.defense = 15;
        int damage = 20;
        
        stats.TakeDamage(damage);
        
        yield return null;
        
        int expectedHealth = 100 - Mathf.Max(1, damage - stats.defense);
        Assert.AreEqual(expectedHealth, stats.currentHealth, "A védelem figyelembe vétele helyes");
    }

    [UnityTest]
    public IEnumerator Stats_TakeDamage_MinimumDamageIsOne()
    {
        stats.currentHealth = 100;
        stats.defense = 100;
        int damage = 5;
        
        stats.TakeDamage(damage);
        
        yield return null;
        
        Assert.AreEqual(99, stats.currentHealth, "Minimum 1 sebzés mindig van");
    }

    [UnityTest]
    public IEnumerator Stats_Heal_IncreasesHealth()
    {
        stats.currentHealth = 50;
        int healAmount = 30;
        
        stats.Heal(healAmount);
        
        yield return null;
        
        Assert.AreEqual(80, stats.currentHealth, "A HP növekedett");
    }

    [UnityTest]
    public IEnumerator Stats_Heal_DoesNotExceedMaxHealth()
    {
        stats.currentHealth = 90;
        stats.maxHealth = 100;
        int healAmount = 30;
        
        stats.Heal(healAmount);
        
        yield return null;
        
        Assert.AreEqual(stats.maxHealth, stats.currentHealth, "A HP nem haladja meg a maxHealth-t");
    }

    [UnityTest]
    public IEnumerator Stats_IsAlive_ReturnsTrue_WhenHealthAboveZero()
    {
        stats.currentHealth = 50;
        
        yield return null;
        
        Assert.IsTrue(stats.IsAlive(), "A játékos él, ha HP > 0");
    }

    [UnityTest]
    public IEnumerator Stats_IsAlive_ReturnsFalse_WhenHealthIsZero()
    {
        stats.currentHealth = 0;
        
        yield return null;
        
        Assert.IsFalse(stats.IsAlive(), "A játékos halott, ha HP = 0");
    }

    [UnityTest]
    public IEnumerator Stats_TakeDamage_HealthCannotGoBelowZero()
    {
        stats.currentHealth = 5;
        int damage = 100;
        
        stats.TakeDamage(damage);
        
        yield return null;
        
        Assert.AreEqual(0, stats.currentHealth, "A HP nem mehet 0 alá");
    }
}

