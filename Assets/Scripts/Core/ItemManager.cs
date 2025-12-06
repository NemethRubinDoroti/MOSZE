using System.Collections.Generic;
using UnityEngine;

public class ItemManager : MonoBehaviour
{
    public static ItemManager Instance { get; private set; }
    
    private Dictionary<Item2D.ItemType, int> inventory = new Dictionary<Item2D.ItemType, int>();
    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
        
        InitializeInventory();
    }
    
    private void InitializeInventory()
    {
        inventory.Clear();
        inventory[Item2D.ItemType.Heal] = 0;
        inventory[Item2D.ItemType.Treasure] = 0;
    }
    
    public void CollectItem(Item2D item)
    {
        if (item == null) return;
        
        if (item.itemType == Item2D.ItemType.Treasure)
        {
            UseTreasure();
            return;
        }
        
        if (item.itemType == Item2D.ItemType.Weapon)
        {
            UseWeaponBoost();
            return;
        }
        
        if (item.itemType == Item2D.ItemType.Defense)
        {
            UseDefenseBoost();
            return;
        }
        
        if (item.itemType == Item2D.ItemType.Speed)
        {
            UseSpeedBoost();
            return;
        }
        
        if (item.itemType == Item2D.ItemType.Accuracy)
        {
            UseAccuracyBoost();
            return;
        }
        
        if (inventory.ContainsKey(item.itemType))
        {
            inventory[item.itemType]++;
        }
        else
        {
            inventory[item.itemType] = 1;
        }
        
        // UI frissítése ha a harcban vagyunk
        if (GameManager2D.Instance != null && GameManager2D.Instance.combatManager != null && 
            GameManager2D.Instance.combatManager.currentState != CombatManager.CombatState.None)
        {
            if (UIManager.Instance != null && UIManager.Instance.combatUI != null)
            {
                UIManager.Instance.combatUI.UpdateItemButtons();
            }
        }
    }
    
    private void UseTreasure()
    {
        if (GameManager2D.Instance != null && GameManager2D.Instance.scoreSystem != null)
        {
            GameManager2D.Instance.scoreSystem.AddScore(200);
        }
    }
    
    public bool UseHeal()
    {
        if (!HasItem(Item2D.ItemType.Heal))
        {
            return false;
        }
        
        if (GameManager2D.Instance != null && GameManager2D.Instance.player != null)
        {
            Stats playerStats = GameManager2D.Instance.player.stats;
            if (playerStats != null)
            {
                playerStats.currentHealth = Mathf.Min(playerStats.currentHealth + 50, playerStats.maxHealth);
                inventory[Item2D.ItemType.Heal]--;
                
                // UI frissítése
                if (UIManager.Instance != null && UIManager.Instance.combatUI != null)
                {
                    UIManager.Instance.combatUI.UpdateUI();
                    UIManager.Instance.combatUI.UpdateItemButtons();
                    UIManager.Instance.combatUI.AddCombatLog($"Játékos healelt! +50 HP! Jelenlegi HP: {playerStats.currentHealth}/{playerStats.maxHealth}");
                }
                
                return true;
            }
        }
        
        return false;
    }
    

    private void UseWeaponBoost()
    {
        if (GameManager2D.Instance != null && GameManager2D.Instance.player != null)
        {
            Stats playerStats = GameManager2D.Instance.player.stats;
            if (playerStats != null)
            {
                int boost = Mathf.CeilToInt(playerStats.attack * 0.1f);
                playerStats.attack += boost;
                
                ShowBoostMessage("Weapon", "Attack", boost, playerStats.attack);
            }
        }
    }
    

    private void UseDefenseBoost()
    {
        if (GameManager2D.Instance != null && GameManager2D.Instance.player != null)
        {
            Stats playerStats = GameManager2D.Instance.player.stats;
            if (playerStats != null)
            {
                int boost = Mathf.CeilToInt(playerStats.defense * 0.1f);
                playerStats.defense += boost;
                
                ShowBoostMessage("Defense", "Defense", boost, playerStats.defense);
            }
        }
    }

    private void UseSpeedBoost()
    {
        if (GameManager2D.Instance != null && GameManager2D.Instance.player != null)
        {
            Stats playerStats = GameManager2D.Instance.player.stats;
            if (playerStats != null)
            {
                int boost = Mathf.CeilToInt(playerStats.speed * 0.1f);
                playerStats.speed += boost;
                
                ShowBoostMessage("Speed", "Speed", boost, playerStats.speed);
            }
        }
    }
    
    private void UseAccuracyBoost()
    {
        if (GameManager2D.Instance != null && GameManager2D.Instance.player != null)
        {
            Stats playerStats = GameManager2D.Instance.player.stats;
            if (playerStats != null)
            {
                int boost = Mathf.CeilToInt(playerStats.accuracy * 0.1f);
                playerStats.accuracy += boost;
                
                ShowBoostMessage("Accuracy", "Accuracy", boost, playerStats.accuracy);
            }
        }
    }

    private void ShowBoostMessage(string itemName, string statName, int boost, int newValue)
    {
        string message = $"{itemName} összegyűjtve! {statName} növelve {boost} (10% boost). Új {statName}: {newValue}";
        
        if (UIManager.Instance != null && UIManager.Instance.combatUI != null)
        {
            UIManager.Instance.combatUI.AddCombatLog(message);
        }
        else
        {
            Debug.Log($"[ItemManager] {message}");
        }
    }
    
    public bool HasItem(Item2D.ItemType type)
    {
        return inventory.ContainsKey(type) && inventory[type] > 0;
    }
    
    public int GetItemCount(Item2D.ItemType type)
    {
        return inventory.ContainsKey(type) ? inventory[type] : 0;
    }
}

